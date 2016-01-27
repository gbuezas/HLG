﻿using HLG.Abstracts.Beings;
using HLG.Abstracts.GameStates;
using HLG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

namespace HLG
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class TheGame : Game
    {
        #region VARIABLES

        // Variables necesarias para dibujar por default
        SpriteBatch spriteBatch;
        GraphicsDeviceManager graphics;

        // Donde se va a alojar el mensaje de chequeo de status
        Vector2 ChkStatVar = new Vector2(50, 550);

        // Check de estado de juego
        Global.EstadosJuego Estado_Check;

        #endregion

        #region METODOS

        public TheGame()
        {
            graphics = new GraphicsDeviceManager(this);

            // Establezco la resolucion maxima adecuada para el dispositivo
            // Supuestamente con esta resolucion autoescala a menores
            // Hay que probarlo en algun lado
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            

            // No estoy seguro de si esto me da antialiasing o ya con lo que puse cuando dibujo alcanza
            graphics.PreferMultiSampling = true;

            /* Chquear bien como arreglo que se vea bien con esto */
            graphics.ToggleFullScreen();

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
            // Agrego los personajes a la lista asi se pueden utilizar mas tarde
            for (int i = 0; i < Global.playersQuant; i++)
            {
                Global.players.Add(new Paladin());
            }

            // Agrego los enemigos a la lista
            for (int i = 0; i < Global.enemiesQuant; i++)
            {
                //Global.players.Add(new IA_1((Global.TargetCondition)azar.Next(0, 4)));
                Global.players.Add(new IA_1());
            }

            Global.CurrentState = new State_Level_1();
            Global.CurrentState.Estadoejecutandose = Global.EstadosJuego.TITULO;

            // Ponemos este estado por defecto en un modo que no es nada, asi cuando va al case detecta incongruencia
            // y acomoda al que corresponde, que seria el que dice arriba en ejecutandose.
            Estado_Check = Global.EstadosJuego.GAMEOVER;

            Global.CurrentState.Initialize();

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
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Global.ViewportWidth = GraphicsDevice.Viewport.Width;
            Global.ViewportHeight = GraphicsDevice.Viewport.Height;

            Global.CurrentState.Load(GraphicsDevice.Viewport);

            // Cargo todas las Textures de los personas y sus movimientos de cada carpeta.
            // Acordarse que los png tienen que estar en la carpeta DEBUG para el modo DEBUG, y asi con cada modo.
            // Si no hay nada va al catch asi que no pasa nada
            # region TEXTURA_HEROES
            foreach (string heroe in Global.Heroes)
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
                                    Global.PaladinTextures.Add(textura);
                                    break;
                                }

                            case "IA_1":
                                {
                                    Global.IA_BasicTextures.Add(textura);
                                    break;
                                }

                            default:
                                {
                                    break;
                                }

                        }

                        if (!Global.Armors.Contains(Nombre.Split('_')[0]))
                        {
                            Global.Armors.Add(Nombre.Split('_')[0]);
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

            foreach (string escenario in Global.Scenes)
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
                                    Global.Level_1Textures.Add(textura);
                                    break;
                                }

                            case "Versus":
                                {
                                    Global.VersusTextures.Add(textura);
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

            // Cargo punto blanco
            Global.Punto_Blanco = Content.Load<Texture2D>("Seleccion/puntoblanco");

            // Cargo titulos y pantallas de presentacion
            Global.Pantalla_Titulo = Content.Load<Texture2D>("Titulo/TitleScreen");

            // Cargo pantalla de seleccion y selectores
            Global.Pantalla_Seleccion = Content.Load<Texture2D>("Seleccion/fondo");
            //Variables_Generales.Selector = Content.Load<Texture2D>("Seleccion/Selector");

            // Cargo fuentes
            Global.CheckStatusVar = Content.Load<SpriteFont>("Fuente_Prueba");
            Global.CheckStatusVar_2 = Content.Load<SpriteFont>("Fuente_Prueba_2");

            // Asigno posiciones iniciales de los personajes, tanto Being como IA
            int ejeX = 0;
            int ejeY = 0;
            foreach (Being Jugador in Global.players)
            {


                Jugador.Initialize(new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + ejeX,
                                                GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2 + ejeY));

                ejeX = ejeX + 50;
                ejeY = ejeY + 50;
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

            // Acomoda los estados correspondientes del jeugo
            StateSwitch();

            Global.CurrentState.Update(gameTime);

            base.Update(gameTime);

            Global.elapsedTime += gameTime.ElapsedGameTime;

            if (Global.elapsedTime > TimeSpan.FromSeconds(1))
            {
                Global.elapsedTime -= TimeSpan.FromSeconds(1);
                Global.frameRate = Global.frameCounter;
                Global.frameCounter = 0;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Global.frameCounter++;

            GraphicsDevice.Clear(Color.White);

            // Dibuja el estado actual
            Global.CurrentState.Draw(spriteBatch);

            # region MENSAJES DE ERROR
            spriteBatch.Begin();

            spriteBatch.DrawString(Global.CheckStatusVar,
            "altoViewport = " + Global.mensaje1.ToString() + Environment.NewLine +
            "anchoViewport = " + Global.mensaje2.ToString() + Environment.NewLine +
            "limitePantallaX = " + Global.mensaje3.ToString() + Environment.NewLine +
            "limitePantallaAncho = " + Global.mensaje4.ToString() + Environment.NewLine +
            "Zoom = " + Global.mensaje5.ToString() + Environment.NewLine +
            "FPS = " + Global.frameRate + Environment.NewLine,
            ChkStatVar, Color.DarkRed);

            spriteBatch.End();
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
                    jugador.health += 1;
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
            Global.previousKeyboardState = Global.currentKeyboardState;
            Global.currentKeyboardState = Keyboard.GetState();

            // Acciones que no se tienen que repetir al mantener la tecla
            if (Global.previousKeyboardState.IsKeyDown(Keys.D2) && Global.currentKeyboardState.IsKeyUp(Keys.D2))
            {
                if (Global.EnableRectangles)
                {
                    Global.EnableRectangles = false;
                }
                else
                {
                    Global.EnableRectangles = true;
                }
            }
        }

        /// <summary>
        /// Chequea en que estado tiene que estar el juego y lo gestiona.
        /// </summary>
        private void StateSwitch()
        {

            if (Estado_Check != Global.CurrentState.Estadoejecutandose)
            {
                switch (Global.CurrentState.Estadoejecutandose)
                {

                    case Global.EstadosJuego.TITULO:
                        {
                            Estado_Check = Global.EstadosJuego.TITULO;
                            Global.CurrentState = new State_Title();
                            break;
                        }

                    case Global.EstadosJuego.SELECCION:
                        {
                            Estado_Check = Global.EstadosJuego.SELECCION;
                            Global.CurrentState = new State_Selection();
                            break;
                        }

                    case Global.EstadosJuego.AVANCE:
                        {
                            Estado_Check = Global.EstadosJuego.AVANCE;
                            Global.CurrentState = new State_Level_1();
                            break;
                        }

                    default: break;

                }
            }
        }

        #endregion
    }
}
