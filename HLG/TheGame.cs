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
    public class TheGame : Game
    {
        
        //-//-// VARIABLES //-//-//
        GraphicsDeviceManager graphics;
        
        Vector2 check_status = new Vector2(50, 550); // Donde se va a alojar el mensaje de chequeo de status
        Global.EstadosJuego state_check; // Check de estado de juego

        public static List<Being> players = new List<Being>();

        static List<string> armors = new List<string>();

        //-//-// METHODS //-//-//
        public TheGame()
        {
            graphics = new GraphicsDeviceManager(this);

            // Establezco la resolucion maxima adecuada para el dispositivo
            // Supuestamente con esta resolucion autoescala a menores
            // Hay que probarlo en algun lado

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

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Agrego los personajes a la lista asi se pueden utilizar mas tarde, no importa la clase en esta instancia
            for (int i = 0; i < Global.players_quant; i++)
            {
                Global.players.Add(new Paladin());
                Global.players[i].index = i;
            }

            // Agrego los enemigos a la lista, no importa la clase en esta instancia
            for (int i = 0; i < Global.enemies_quant; i++)
            {
                Global.players.Add(new Skeleton());
                //Global.players[i].indexPlayer = i;
            }

            Global.current_game_state = new State_Level_1();
            Global.current_game_state.ongoing_state = Global.EstadosJuego.TITULO;

            // Ponemos este estado por defecto en un modo que no es nada, asi cuando va al case detecta incongruencia
            // y acomoda al que corresponde, que seria el que dice arriba en ejecutandose.
            state_check = Global.EstadosJuego.GAMEOVER;

            Global.current_game_state.Initialize();

            // Ralentizar los cuadros por segundo de todo el juego
            //this.TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 5);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            Global.sprite_batch = new SpriteBatch(GraphicsDevice);

            Global.viewport_width = GraphicsDevice.Viewport.Width;
            Global.viewport_height = GraphicsDevice.Viewport.Height;

            Global.current_game_state.Load(GraphicsDevice.Viewport);

            // Cargo todas las Textures de los personas y sus movimientos de cada carpeta.
            // Acordarse que los png tienen que estar en la carpeta DEBUG para el modo DEBUG, y asi con cada modo.
            // Si no hay nada va al catch asi que no pasa nada
            # region TEXTURA_HEROES
            foreach (string heroe in Global.characters)
            {
                try
                {
                    // Hago la lista solamente de los archivos PNG (animaciones de las piezas de los heroes) que estan en el content 
                    // para crear las Textures de cada uno.
                    string[] archivos_personajes = Directory.GetFiles("Content/" + heroe, "*.png");

                    foreach (string nombre_personajes in archivos_personajes)
                    {
                        string Nombre = Path.GetFileNameWithoutExtension(nombre_personajes);
                        Textures textura = new Textures(Content.Load<Texture2D>(heroe + "/" + Nombre), Nombre);

                        switch (heroe)
                        {

                            case "Paladin":
                                {
                                    Global.paladin_textures.Add(textura);
                                    break;
                                }

                            case "IA_1":
                                {
                                    Global.skeleton_textures.Add(textura);
                                    break;
                                }

                            default:
                                {
                                    break;
                                }

                        }

                        if (!armors.Contains(Nombre.Split('_')[0]))
                        {
                            armors.Add(Nombre.Split('_')[0]);
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
            #endregion

            // Cargo los niveles
            #region TEXTURA_NIVELES

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
                                    Global.level_1textures.Add(textura);
                                    break;
                                }

                            case "Versus":
                                {
                                    Global.versus_textures.Add(textura);
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

            #endregion

            // Cargo las UI
            #region TEXTURA_UI

            try
            {

                string[] archivos_UI = Directory.GetFiles("Content/UI", "*.png");

                foreach (string nombre_UI in archivos_UI)
                {
                    string nombre = Path.GetFileNameWithoutExtension(nombre_UI);
                    Textures textura = new Textures(Content.Load<Texture2D>("UI/" + nombre), nombre);

                    Global.UITextures.Add(textura);
                }
            }
            catch(Exception ex)
            {
                // Para que aunque no encuentre carpeta ni nada siga igual, 
                // porque no hace diferencia al juego que no pueda encontrar nada.
                // Mas tarde si va a importar que no cargue todas las Textures de todos cuando las tengamos.
                // Pero ahora en esta etapa de prueba donde me faltan un monton de cosas no es necesario.
                throw ex;
            }
            
            #endregion

            // Cargo punto blanco
            Global.white_dot = Content.Load<Texture2D>("Seleccion/puntoblanco");

            // Cargo titulos y pantallas de presentacion
            Global.title_screen = Content.Load<Texture2D>("Titulo/TitleScreen");

            // Cargo pantalla de seleccion y selectores
            Global.selection_screen = Content.Load<Texture2D>("Seleccion/fondo");
            //Variables_Generales.Selector = Content.Load<Texture2D>("Seleccion/Selector");
            
            // Cargo fuentes
            Global.check_status_font = Content.Load<SpriteFont>("Fuente_Prueba");
            Global.check_status_font_2 = Content.Load<SpriteFont>("Fuente_Prueba_2");

            // Asigno posiciones iniciales de los personajes, se chequea antes del Initialize
            foreach (Being Jugador in Global.players)
            {
                if (Jugador.index == -1)
                {
                    Jugador.Initialize(new Vector2(Global.randomly.Next(-100, Global.viewport_width + 100),
                                                   Global.randomly.Next(Global.viewport_height / 2 + 50, Global.viewport_height + 100)));
                }
                else
                {
                    Jugador.Initialize(new Vector2(Global.randomly.Next(200, Global.viewport_width - 200),
                                                   Global.randomly.Next(Global.viewport_height / 2 + 50, Global.viewport_height - 50)));
                }
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Funciones del juego
            QuitGame();
            GiveLife();
            EnableColRec();

            // Acomoda los estados correspondientes del juego, las distintas pantallas
            StateSwitch();

            Global.current_game_state.Update(gameTime);

            base.Update(gameTime);

            Global.elapsed_time += gameTime.ElapsedGameTime;

            if (Global.elapsed_time > TimeSpan.FromSeconds(1))
            {
                Global.elapsed_time -= TimeSpan.FromSeconds(1);
                Global.frame_rate = Global.frame_counter;
                Global.frame_counter = 0;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Global.frame_counter++;

            GraphicsDevice.Clear(Color.White);

            // Dibuja el estado actual
            Global.current_game_state.Draw();

            # region MENSAJES DE ERROR
            Global.sprite_batch.Begin();

            Global.sprite_batch.DrawString(Global.check_status_font,
            "altoViewport = " + Global.mensaje1.ToString() + Environment.NewLine +
            "anchoViewport = " + Global.mensaje2.ToString() + Environment.NewLine +
            "limitePantallaX = " + Global.mensaje3.ToString() + Environment.NewLine +
            "limitePantallaAncho = " + Global.mensaje4.ToString() + Environment.NewLine +
            "Zoom = " + Global.mensaje5.ToString() + Environment.NewLine +
            "FPS = " + Global.frame_rate + Environment.NewLine,
            check_status, Color.DarkRed);

            Global.sprite_batch.End();
            #endregion

            base.Draw(gameTime);
        }

        /// <summary>
        /// Añade vida a todos los Being
        /// </summary>
        private static void GiveLife()
        {
            // Da vida a los Being
            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                foreach (Being jugador in Global.players)
                {
                    jugador.current_health += 1;
                }
            }
        }

        /// <summary>
        /// Permite salir del juego desde el joystick o teclado apretando select o escape.
        /// </summary>
        private void QuitGame()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
        }

        /// <summary>
        /// Habilita rectangulo de colisiones.
        /// </summary>
        private static void EnableColRec()
        {
            if (Global.OnePulseKey(Keys.D2))
            {
                if (Global.enable_rectangles)
                {
                    Global.enable_rectangles = false;
                }
                else
                {
                    Global.enable_rectangles = true;
                }
            }
        }

        /// <summary>
        /// Chequea en que estado tiene que estar el juego y lo gestiona.
        /// </summary>
        private void StateSwitch()
        {

            if (state_check != Global.current_game_state.ongoing_state)
            {
                switch (Global.current_game_state.ongoing_state)
                {

                    case Global.EstadosJuego.TITULO:
                        {
                            state_check = Global.EstadosJuego.TITULO;
                            Global.current_game_state = new State_Title();
                            break;
                        }

                    case Global.EstadosJuego.SELECCION:
                        {
                            state_check = Global.EstadosJuego.SELECCION;
                            Global.current_game_state = new State_Selection();
                            break;
                        }

                    case Global.EstadosJuego.AVANCE:
                        {
                            state_check = Global.EstadosJuego.AVANCE;
                            Global.current_game_state = new State_Level_1();
                            break;
                        }

                    default: break;

                }
            }
        }
                
    }
}
