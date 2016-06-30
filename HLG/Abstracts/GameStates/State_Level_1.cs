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

        /*
            Claramente hay un problema con las velocidades y el parallax, me parece que cuanto mas lento va mas grande tiene que ser?
        */
        //Parallax Nubes = new Parallax(Mapa_Nubes, 0.3f, 1f);
        //Parallax Arboles = new Parallax(Mapa_Arboles, 0.5f, 0.8f);
        //Parallax Piso = new Parallax(Mapa_Piso, 1f, 1f);

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
            Global.Background_Layers.Add(Nubes);
            Global.Background_Layers.Add(Arboles);
            Global.Background_Layers.Add(Piso);
            
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
            // En vez de ir de 0 al limite del nivel voy a recortarlo un poco asi no se ven los cortes de las capas del parallax

            /*ORIGINAL*/
            Global.Camara.Limits = new Rectangle(0, 0, Global.ViewportWidth / 4 * (Mapa_Nubes.Length - 1), Var_AltoNivel);
            
            //Global.Camara.Limits = new Rectangle(Global.ViewportWidth / 4, 0, Global.ViewportWidth / 4 * (Mapa_Nubes.Length - 1), Var_AltoNivel);

            // Tomo tiempo transcurrido.
            //float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Para poder controlar al otro personaje por separado
            // Si lo saco de aca no me toma los cambios del control
            #region controles de los personajes - borrar

            Global.players[1].controls[(int)Global.Controls.UP] = Keys.Up;
            Global.players[1].controls[(int)Global.Controls.DOWN] = Keys.Down;
            Global.players[1].controls[(int)Global.Controls.LEFT] = Keys.Left;
            Global.players[1].controls[(int)Global.Controls.RIGHT] = Keys.Right;
            Global.players[1].controls[(int)Global.Controls.BUTTON_1] = Keys.Space;

            //Global.players[2].controls[(int)Global.Controls.UP] = Keys.I;
            //Global.players[2].controls[(int)Global.Controls.DOWN] = Keys.K;
            //Global.players[2].controls[(int)Global.Controls.LEFT] = Keys.J;
            //Global.players[2].controls[(int)Global.Controls.RIGHT] = Keys.L;
            //Global.players[2].controls[(int)Global.Controls.BUTTON_1] = Keys.Enter;

            //Global.players[3].controls[(int)Global.Controls.UP] = Keys.I;
            //Global.players[3].controls[(int)Global.Controls.DOWN] = Keys.K;
            //Global.players[3].controls[(int)Global.Controls.LEFT] = Keys.J;
            //Global.players[3].controls[(int)Global.Controls.RIGHT] = Keys.L;
            //Global.players[3].controls[(int)Global.Controls.BUTTON_1] = Keys.Enter;

            // Enemigo
            //Global.players[4].controls = null;
            //Global.players[5].controls = null;

            #endregion

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
            
            int UIancho = 450;
            int UIalto = 550;

            int UIx = (int)(Global.Camara.parallax.X + Global.ViewportWidth / 5);
            //int UIy = (int)Global.Camara.parallax.Y;
            int UIy = 78;

            for (int i=0; i < Global.playersQuant; i++)
            {
                //Vector2 UI_Vec = new Vector2(UIx * (i + 1) - UIancho / 2, 0);
                Vector2 UI_Vec = new Vector2(UIx * (i + 1), UIy);
                if (Global.UIAnimation[i] == null)
                {
                    Global.UIAnimation[i] = new Animation();
                    Global.UIAnimation[i].LoadTexture(Global.UITextures[0], UI_Vec, UIancho, UIalto, 2, Color.White, true);
                }

                Global.UIAnimation[i].frameTime = 300;
                Global.UIAnimation[i].Update(gameTime);
            }
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

            foreach (Parallax capa in Global.Background_Layers)
            {

                Global.Camara.parallax = new Vector2(capa.parallax_x, capa.parallax_y);

                /*ORIGINAL*/
                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicClamp, null, null, null, Global.Camara.ViewMatrix);
                
                //spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Global.Camara.ViewMatrix);

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

                            // Me parece que sumandole el parallax_x a la multiplicacion del mismo hizo el truco de pegar los tiles bien sin que desaparezcan

                            /*ORIGINAL*/
                            capa.RectanguloParallax.X += (int)(Global.Camara.LimitesPantalla.X * capa.parallax_x + 0.5f);
                            
                            //capa.RectanguloParallax.X += (int)(Global.Camara.LimitesPantalla.X * capa.parallax_x + capa.parallax_x);
                            //capa.RectanguloParallax.X += (int)(Global.Camara.LimitesPantalla.X * capa.parallax_x);

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

            // Vuelvo la camara al default?, porque aca todos estamos usando la misma, cuando en el archivo original recibe el paralllax
            // aca lo toma de adentro de ella misma lo cual infiere en un monton de calculos
            // Me parece que tendría que separarlo
            Global.Camara.parallax = new Vector2(1, 1);

            // SpriteSortMode.Deferred soluciono el problema de que pegaba las capas como se le cantaba el ojete, estaba en BacktoFront
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicClamp, null, null, null, Global.Camara.ViewMatrix);
            //spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Global.Camara.ViewMatrix);

            Reordenar_Personajes(spriteBatch);

            spriteBatch.End();

            #endregion

            #region INTERFACE

            // Se usa el dibujado por default asi queda separado de la camara y esta siempre visible
            // XX LO HAGO COMO REORDENAR PERSONAJES EN LA CLASE PADRE? XX
            spriteBatch.Begin();

            DrawUI(spriteBatch);

            spriteBatch.End();

            #endregion

        }

        private static void DrawUI(SpriteBatch spriteBatch)
        {
            /// Para obtener los 4 puntos donde tiene que dibujarse cada UI, obtener el punto Y es innecesario, el mismo es siempre 0 ya que se
            /// usan las transparencias de la imagen para obtener el espacio necesario del eje Y.
            int UIx = (int)(Global.Camara.parallax.X + Global.ViewportWidth / 5);

            for (int i = 0; i < Global.playersQuant; i++)
            {
                // Dibuja UI animada
                Global.UIAnimation[i].Draw(spriteBatch, Global.Mirada.RIGHT);

                /// Calculo sector de numero de vida
                Rectangle UI_Rect = new Rectangle(UIx * (i + 1) - Global.UIancho / 2, 0, Global.UIancho, Global.UIalto);
                Global.UILifeNumber[i].X = UI_Rect.X + Global.UIancho / 4;
                Global.UILifeNumber[i].Y = UI_Rect.Y + Global.UIalto / 2 + 10;

                /// Los calculos del tamaño y el color de la barra de vida estan hechos con regla de 3 simple

                float actual_bar_length = Global.players[i].current_health * Global.max_bar_length / Global.players[i].max_health;

                /// Color
                int new_color = (int)(actual_bar_length * 210 / Global.max_bar_length);
                Color bar_color = new Color(255 - new_color, new_color, 0);

                // Dibujar barra de vida
                Global.DrawStraightLine(new Vector2(Global.UILifeNumber[i].X - 1, Global.UILifeNumber[i].Y + 2),
                                        new Vector2(Global.UILifeNumber[i].X + actual_bar_length, Global.UILifeNumber[i].Y),
                                        Global.Punto_Blanco,
                                        bar_color,
                                        spriteBatch,
                                        14);

                // Vida en numeros
                spriteBatch.DrawString(Global.CheckStatusVar_2,
                                       Global.players[i].current_health.ToString() + " / " + Global.players[i].max_health.ToString(),
                                       Global.UILifeNumber[i],
                                       Color.White);
            }
        }

        public override void UpdateState(GameTime gameTime)
        {
            
        }

        //public Vector2 WorldToScreen(Vector2 worldPosition)
        //{
        //    return Vector2.Transform(worldPosition, Global.Camara.ViewMatrix(parallax));
        //}

        //public Vector2 ScreenToWorld(Vector2 screenPosition)
        //{
        //    return Vector2.Transform(screenPosition, Matrix.Invert(camera.GetViewMatrix(parallax)));
        //}

        #endregion
    }
}
