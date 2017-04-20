using HLG.Abstracts.Beings;
using HLG.Abstracts.GameStates;
using HLG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace HLG
{
    public class The_Game : Game
    {
        
        //-//-// VARIABLES //-//-//
        GraphicsDeviceManager graphics;
        
        Vector2 checkStatus = new Vector2(50, 350); // Donde se va a alojar el mensaje de chequeo de status
        Global.EstadosJuego stateCheck; // Check de estado de juego

        public static List<Being> players = new List<Being>();
        public static List<string> allSetsNames = new List<string>();

        //-//-// METHODS //-//-//
        public The_Game()
        {
            graphics = new GraphicsDeviceManager(this);

            // Seteo manual de resolucion
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 675;

            // Obtengo lista de modos soportados por el adaptador gráfico actual de la pc
            //DisplayModeCollection SupportModes;
            //SupportModes = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes;

            // Seteo la resolucion grafica que esta actualmente en la pc y hago pantalla completa
            //graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //graphics.ToggleFullScreen();

            // No estoy seguro de si esto me da antialiasing o ya con lo que puse cuando dibujo alcanza
            //graphics.PreferMultiSampling = true;

            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // Agrego los personajes a la lista asi se pueden utilizar mas tarde, no importa la clase en esta instancia
            for (int i = 0; i < Global.playersQuant; i++)
            {
                Global.players.Add(new Paladin());
                Global.players[i].index = i;
            }

            // Agrego los enemigos a la lista, no importa la clase en esta instancia
            for (int i = 0; i < Global.enemiesQuant; i++)
            {
                Global.players.Add(new Skeleton());
            }

            Global.currentGameState = new State_Level_1();
            Global.currentGameState.ongoingState = Global.EstadosJuego.TITULO;
            stateCheck = Global.EstadosJuego.GAMEOVER;
            Global.spriteBatch = new SpriteBatch(GraphicsDevice); // Create a new SpriteBatch, which is used to draw textures.
            Global.viewportWidth = GraphicsDevice.Viewport.Width;
            Global.viewportHeight = GraphicsDevice.Viewport.Height;
            Global.currentGameState.Load(GraphicsDevice.Viewport);
            
            Global.currentGameState.Initialize();
            base.Initialize();
            
        }

        protected override void LoadContent()
        {
            LoadTextures();
            LoadFonts();

            AsignCharactersInitialPosition();
        }
        private void LoadTextures()
        {
            LoadCharactersTextures();
            LoadLevelsTextures();
            LoadGUITextures();
            LoadScatteredTextures();
        }
        private void LoadGUITextures()
        {
            try
            {

                string[] archivosUI = Directory.GetFiles("Content/UI", "*.png");

                foreach (string archivosUIItem in archivosUI)
                {
                    string nombre = Path.GetFileNameWithoutExtension(archivosUIItem);
                    Textures textura = new Textures(Content.Load<Texture2D>("UI/" + nombre), nombre);

                    Global.uiTextures.Add(textura);
                }
            }
            catch (Exception ex)
            {
                // Para que aunque no encuentre carpeta ni nada siga igual, 
                // porque no hace diferencia al juego que no pueda encontrar nada.
                // Mas tarde si va a importar que no cargue todas las Textures de todos cuando las tengamos.
                // Pero ahora en esta etapa de prueba donde me faltan un monton de cosas no es necesario.
                throw ex;
            }
        }
        private void LoadLevelsTextures()
        {
            foreach (string escenario in Global.scenes)
            {
                try
                {
                    string[] archivos_niveles = Directory.GetFiles("Content/" + escenario, "*.png");

                    foreach (string nombre_niveles in archivos_niveles)
                    {
                        string Nombre = Path.GetFileNameWithoutExtension(nombre_niveles);
                        Textures textura = new Textures(Content.Load<Texture2D>(escenario + "/" + Nombre), Nombre);

                        switch (escenario)
                        {

                            case "Avance":
                                {
                                    Global.level1Textures.Add(textura);
                                    break;
                                }

                            case "Versus":
                                {
                                    Global.versusTextures.Add(textura);
                                    break;
                                }

                            default:
                                {
                                    break;
                                }

                        }

                    }
                }
                catch
                {
                    // Para que aunque no encuentre carpeta ni nada siga igual, 
                    // porque no hace diferencia al juego que no pueda encontrar nada.
                    // Mas tarde si va a importar que no cargue todas las Textures de todos cuando las tengamos.
                    // Pero ahora en esta etapa de prueba donde me faltan un monton de cosas no es necesario.
                }
            }
        }
        private void LoadCharactersTextures()
        {
            foreach (string charactersItem in Global.characters)
            {
                try
                {
                    // Hago la lista solamente de los archivos PNG (animaciones de las piezas de los heroes) que estan en el content 
                    // para crear las Textures de cada uno.
                    string[] archivos_personajes = Directory.GetFiles("Content/" + charactersItem, "*.png");

                    foreach (string nombre_personajes in archivos_personajes)
                    {
                        string Nombre = Path.GetFileNameWithoutExtension(nombre_personajes);
                        Textures textura = new Textures(Content.Load<Texture2D>(charactersItem + "/" + Nombre), Nombre);

                        switch (charactersItem)
                        {

                            case "Paladin":
                                {
                                    Global.paladinTextures.Add(textura);
                                    break;
                                }

                            case "Skeleton":
                                {
                                    Global.skeletonTextures.Add(textura);
                                    break;
                                }

                            default:
                                {
                                    break;
                                }

                        }

                        if (!allSetsNames.Contains(Nombre.Split('_')[0]))
                        {
                            allSetsNames.Add(Nombre.Split('_')[0]);
                        }
                    }
                }
                catch
                {
                    // Para que aunque no encuentre carpeta ni nada siga igual, 
                    // porque no hace diferencia al juego que no pueda encontrar nada.
                    // Mas tarde si va a importar que no cargue todas las Textures de todos cuando las tengamos.
                    // Pero ahora en esta etapa de prueba donde me faltan un monton de cosas no es necesario.
                }
            }
        }
        private void LoadScatteredTextures()
        {
            Global.whiteDot = Content.Load<Texture2D>("Seleccion/puntoblanco");
            Global.titleScreen = Content.Load<Texture2D>("Titulo/TitleScreen");
            Global.selectionScreen = Content.Load<Texture2D>("Seleccion/fondo");
        }
        private void LoadFonts()
        {
            Global.checkStatusFont = Content.Load<SpriteFont>("Fuente_Prueba");
            Global.checkStatusFont2 = Content.Load<SpriteFont>("Fuente_Prueba_2");
        }
        private static void AsignCharactersInitialPosition()
        {
            foreach (Being playersItem in Global.players)
            {
                if (playersItem.index == -1)
                {
                    playersItem.Initialize(new Vector2(Global.randomly.Next(-100, Global.viewportWidth + 100),
                                                   Global.randomly.Next(Global.viewportHeight / 2 + 50, Global.viewportHeight + 100)));
                }
                else
                {
                    playersItem.Initialize(new Vector2(Global.randomly.Next(200, Global.viewportWidth - 200),
                                                   Global.randomly.Next(Global.viewportHeight / 2 + 50, Global.viewportHeight - 50)));
                }
            }
        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        
        protected override void Update(GameTime gameTime)
        {
            QuitGame();
            GiveLife();
            EnableCollisionRec();

            StateSwitch(); // Acomoda los estados correspondientes del juego, las distintas pantallas
            SetStateAndTime(gameTime);
        }
        private void SetStateAndTime(GameTime gameTime)
        {
            Global.currentGameState.Update(gameTime);
            base.Update(gameTime);

            Global.elapsedTime += gameTime.ElapsedGameTime;
            if (Global.elapsedTime > TimeSpan.FromSeconds(1))
            {
                Global.elapsedTime -= TimeSpan.FromSeconds(1);
                Global.frameRate = Global.frameCounter;
                Global.frameCounter = 0;
            }
        }
        private void StateSwitch()
        {

            if (stateCheck != Global.currentGameState.ongoingState)
            {
                switch (Global.currentGameState.ongoingState)
                {

                    case Global.EstadosJuego.TITULO:
                        {
                            stateCheck = Global.EstadosJuego.TITULO;
                            Global.currentGameState = new State_Title();
                            break;
                        }

                    case Global.EstadosJuego.SELECCION:
                        {
                            stateCheck = Global.EstadosJuego.SELECCION;
                            Global.currentGameState = new State_Selection();
                            break;
                        }

                    case Global.EstadosJuego.AVANCE:
                        {
                            stateCheck = Global.EstadosJuego.AVANCE;
                            Global.currentGameState = new State_Level_1();
                            break;
                        }

                    default: break;

                }
            }
        }
        private static void GiveLife()
        {
            // Da vida a los Being
            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                foreach (Being playersItem in Global.players)
                {
                    playersItem.currentHealth += 1;
                }
            }
        }
        private void QuitGame()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
        }
        private static void EnableCollisionRec()
        {
            if (Global.OnePulseKey(Keys.D2))
            {
                if (Global.enableRectangles)
                {
                    Global.enableRectangles = false;
                }
                else
                {
                    Global.enableRectangles = true;
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            Global.frameCounter++;

            GraphicsDevice.Clear(Color.White);

            // Dibuja el estado actual
            Global.currentGameState.Draw();

            # region MENSAJES DE ERROR
            Global.spriteBatch.Begin();

            Global.spriteBatch.DrawString(Global.checkStatusFont,
            "altoViewport = " + Global.mensaje1.ToString() + Environment.NewLine +
            "anchoViewport = " + Global.mensaje2.ToString() + Environment.NewLine +
            "limitePantallaX = " + Global.mensaje3.ToString() + Environment.NewLine +
            "limitePantallaAncho = " + Global.mensaje4.ToString() + Environment.NewLine +
            "Zoom = " + Global.mensaje5.ToString() + Environment.NewLine +
            "FPS = " + Global.frameRate + Environment.NewLine,
            checkStatus, Color.DarkRed);

            Global.spriteBatch.End();
            #endregion

            base.Draw(gameTime);
        }
        
        /// <summary>
        /// Ralentizar los cuadros por segundo de todo el juego
        /// </summary>
        /// <param name="speed"></param>
        private void SlowMotion(int speed)
        {
            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / speed);
        }

    }
}
