using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HLG.Objects;
using Microsoft.Xna.Framework.Input;
using HLG.Abstracts.Beings;

namespace HLG.Abstracts.GameStates
{
    class State_Level_1 : States
    {
        #region VARIABLES

        #region MAPA Y PARALLAX

        // Mapa de tiles de las capas
        static string[] Mapa_Piso = new string[] { "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1" };
        static string[] Mapa_Arboles = new string[] { "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1" };
        static string[] Mapa_Nubes = new string[] { "cloud1", "cloud2", "cloud3", "cloud4", "cloud5", "cloud6", "cloud7", "cloud8", "cloud9", "cloud10", "cloud11", "cloud1", "cloud2", "cloud3", "cloud4", "cloud5", };

        // Velocidades del parallax de cada capa
        Parallax Piso = new Parallax(Mapa_Piso, 1f, 1f);
        Parallax Arboles = new Parallax(Mapa_Arboles, 0.8f, 0.5f);
        Parallax Nubes = new Parallax(Mapa_Nubes, 0.5f, 1f);

        #endregion

        // Genero un vector con la cantidad de rectangulos necesarios para pintar todo el mapa
        Rectangle[] sourceRect = new Rectangle[Mapa_Nubes.Length];

        // Alto del nivel
        int Var_AltoNivel = Global.ViewportHeight;

        // Ancho del nivel
        int Var_AnchoNivel = Global.ViewportWidth / 4 * Mapa_Nubes.Length;
        
        #endregion

        #region METODOS

        /// <summary>
        /// Cargo los Being segun su clase seleccionada.
        /// Tambien cargo las capas de parallax.
        /// </summary>
        public override void Initialize()
        {
            // Agrego las diferentes capas de parallax
            Global.Layers.Add(Nubes);
            Global.Layers.Add(Arboles);
            Global.Layers.Add(Piso);
        }

        /// <summary>
        /// Para asignarle el viewport en el load del game.
        /// </summary>
        public override void Load(Viewport _viewport)
        {
            // Seteo el viewport correspondiente a la camara
            Global.Camara = new Camera(_viewport, Var_AltoNivel, Global.ViewportWidth / 4 * Mapa_Nubes.Length);
        }

        public override void Update(GameTime gameTime)
        {
            // Guarda y lee los estados actuales y anteriores del joystick y teclado
            Input_Management();

            // Actualiza jugador
            foreach (Being Jugador in Global.players)
            {
                Jugador.UpdatePlayer(gameTime, Var_AltoNivel, Var_AnchoNivel);
            }

            // Ajusto los limites de la camara para que no pueda mostrar mas de este rectangulo
            Global.Camara.Limits = new Rectangle(0, 0, Global.ViewportWidth / 4 * Mapa_Nubes.Length, Var_AltoNivel);

            // Tomo tiempo transcurrido.
            //float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Para poder controlar al otro personaje por separado
            // Si lo saco de aca no me toma los cambios del control
            Global.players[1].controls[(int)Global.Controls.UP] = Keys.Up;
            Global.players[1].controls[(int)Global.Controls.DOWN] = Keys.Down;
            Global.players[1].controls[(int)Global.Controls.LEFT] = Keys.Left;
            Global.players[1].controls[(int)Global.Controls.RIGHT] = Keys.Right;
            Global.players[1].controls[(int)Global.Controls.BUTTON_1] = Keys.Space;

            Global.players[2].controls[(int)Global.Controls.UP] = Keys.I;
            Global.players[2].controls[(int)Global.Controls.DOWN] = Keys.K;
            Global.players[2].controls[(int)Global.Controls.LEFT] = Keys.J;
            Global.players[2].controls[(int)Global.Controls.RIGHT] = Keys.L;
            Global.players[2].controls[(int)Global.Controls.BUTTON_1] = Keys.Enter;

            Global.players[3].controls[(int)Global.Controls.UP] = Keys.I;
            Global.players[3].controls[(int)Global.Controls.DOWN] = Keys.K;
            Global.players[3].controls[(int)Global.Controls.LEFT] = Keys.J;
            Global.players[3].controls[(int)Global.Controls.RIGHT] = Keys.L;
            Global.players[3].controls[(int)Global.Controls.BUTTON_1] = Keys.Enter;

            // Enemigo
            //Global.players[4].controls = null;
            //Global.players[5].controls = null;

            // Hacer un foreach para todos los personajes que quedan en camara, 
            // solo los controlados por humanos, la maquina no, asi pueden salir y no me desconcha toda la camara y el zoom
            // Aca controlamos donde van a aparecer inicialmente todos los jugadores

            Global.Camara.ViewTargets.Clear();
            foreach (Being Jugador in Global.players)
            {
                if (!Jugador.machine)
                {
                    Global.Camara.ViewTargets.Add(Jugador.GetPositionVec());
                }
            }
            Global.Camara.CentrarCamara();

            Global.mensaje1 = Global.ViewportHeight;
            Global.mensaje2 = Global.ViewportWidth;

            CalculateHealthBar();

        }

        /// <summary>
        /// Aca se hace el calculo de que personaje se dibuja primero
        /// Tambien se hace el calculo de la camara y el parallax
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {

            # region CAPAS

            // El rectangulo contenedor del tile
            int rectangulo;
            // La posicion donde va el siguiente rectangulo
            int posicion;

            foreach (Parallax capa in Global.Layers)
            {

                Global.Camara.parallax = new Vector2(capa.parallax_x, capa.parallax_y);

                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicClamp, null, null, null, Global.Camara.ViewMatrix);

                rectangulo = 0;
                posicion = 0;

                foreach (string seccion in capa.capa_parallax)
                {
                    foreach (Textures avance in Global.Level_1Textures)
                    {
                        if (seccion == avance.piece)
                        {
                            sourceRect[rectangulo] = new Rectangle(posicion, 0,
                                Global.ViewportWidth / 4,
                                Global.ViewportHeight);

                            // Recalculo el rectangulo para que se adapte a la velocidad correspondiente de la capa
                            capa.RectanguloParallax = sourceRect[rectangulo];
                            capa.RectanguloParallax.X += (int)(Global.Camara.LimitesPantalla.X * capa.parallax_x + 0.5f);

                            // Mensajes de chequeo
                            Global.mensaje3 = Global.Camara.LimitesPantalla.X;
                            Global.mensaje4 = Global.Camara.LimitesPantalla.Width;

                            // Si no esta dentro de la camara no lo dibujo
                            if (Global.Camara.EnCamara(capa.RectanguloParallax))
                            {
                                spriteBatch.Draw(avance.textura, sourceRect[rectangulo], Color.White);
                            }

                            posicion += Global.ViewportWidth / 4;
                        }

                    }
                    rectangulo++;
                }

                spriteBatch.End();
            }

            #endregion

            #region PERSONAJES

            // SpriteSortMode.Deferred soluciono el problema de que pegaba las capas como se le cantaba el ojete, estaba en BacktoFront
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicClamp, null, null, null, Global.Camara.ViewMatrix);

            Reordenar_Personajes(spriteBatch);

            spriteBatch.End();

            #endregion

            #region INTERFACE

            // Se usa el dibujado por default
            spriteBatch.Begin();

            //DrawHealthBar(spriteBatch);
            foreach (Animation UI in Global.UIAnimation)
            {
                UI.Draw(spriteBatch, Global.Mirada.RIGHT);
            }

            #region Barra de vida

            Vector2[] mensaje = new Vector2[Global.playersQuant];
            int UIancho = 100;
            int UIalto = 150;
            int UIx = int.Parse((Global.Camara.parallax.X + Global.ViewportWidth / 5).ToString());

            for (int i = 0; i < Global.playersQuant; i++)
            {

                #region loop

                Rectangle UI_Rect = new Rectangle(UIx * (i + 1) - UIancho / 2, 0, UIancho, UIalto);

                mensaje[i].X = UI_Rect.X + UIancho / 4;
                mensaje[i].Y = UI_Rect.Y + UIalto / 2 + 10;

                // Barra de vida - GAB
                // Los calculos del tamaño y el color de la barra estan hechos con regla de 3 simple
                float max_bar_length = 49;
                float actual_bar_length = Global.players[i].current_health * max_bar_length / Global.players[i].max_health;

                // Color
                int new_color = (int)(actual_bar_length * 210 / 49);
                Color bar_color = new Color(255 - new_color, new_color, 0);

                // Dibujar barra de vida
                Global.DrawStraightLine(new Vector2(mensaje[i].X - 1, mensaje[i].Y + 2),
                                        new Vector2(mensaje[i].X + actual_bar_length, mensaje[i].Y),
                                        Global.Punto_Blanco,
                                        bar_color,
                                        spriteBatch,
                                        14);

                // Vida en numeros - GAB
                spriteBatch.DrawString(Global.CheckStatusVar_2,
                                       Global.players[i].current_health.ToString() + " / " + Global.players[i].max_health.ToString(),
                                       mensaje[i],
                                       Color.White);
                #endregion

            }

            #endregion

            spriteBatch.End();
            
            #endregion

        }

        private static void CalculateHealthBar()
        {
            /// Obtengo el eje x a partir del cual van a desplegarse los 4 UI de cada personaje, este eje depende estrictamente de la camara
            /// Como usa el espacio transparente del PNG al eje Y lo ponemos en 0
            //int UIx = int.Parse((Global.Camara.parallax.X + spriteBatch.GraphicsDevice.Viewport.Width / 5).ToString());
            int UIx = int.Parse((Global.Camara.parallax.X + Global.ViewportWidth / 5).ToString());

            // Debemos adaptar a la pantalla como los personajes - GAB
            // Se adapta solo al usar el objeto animacion para crear cualquier cosa animada (GENIAL)
            int UIancho = 100;
            int UIalto = 150;

            //Vector2[] mensaje = new Vector2[Global.playersQuant];

            for (int i = 0; i < Global.playersQuant; i++)
            {

                if (Global.players[i].animations[0].active)
                {
                    Rectangle UI_Rect = new Rectangle(UIx * (i + 1) - UIancho / 2, 0, UIancho, UIalto);

                    Vector2 UI_Vec = new Vector2();
                    UI_Vec.X = UI_Rect.X;
                    UI_Vec.Y = UI_Rect.Y;
                    
                    //mensaje[i].X = UI_Rect.X + UIancho / 4;
                    //mensaje[i].Y = UI_Rect.Y + UIalto / 2 + 10;

                    Global.UIAnimation[i] = new Animation();
                    // Aca en texturas donde dice 0 endria que ir el identificador de que textura se tiene que cargar
                    // Ya sea la UI del Paladin o la del Barbaro, etc
                    Global.UIAnimation[i].LoadTexture(Global.UITextures[0], UI_Vec, UIancho, UIalto, int.Parse(Global.UITextures[0].frame), Color.White, true);

                    //Global.UIAnimation[i].Draw(spriteBatch, Global.Mirada.RIGHT);

                    // UI de vida - GAB
                    //if (Global.players[i].animations[9].loadedTexture != null)
                    //{
                    //    spriteBatch.Draw(Global.players[i].animations[9].loadedTexture.textura, UI_Rect, Color.White);
                    //}
                    //else
                    //{

                    //}
                    
                }   
            }
        }

        public override void UpdateState(GameTime gameTime)
        {
            
        }
        
        #endregion
    }
}
