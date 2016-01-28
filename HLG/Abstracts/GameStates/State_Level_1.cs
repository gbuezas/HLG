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

        // Creo la variable de la camara en estatica
        static Camera Camara;

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
            Camara = new Camera(_viewport, Var_AltoNivel, Global.ViewportWidth / 4 * Mapa_Nubes.Length);
        }

        public override void Update(GameTime gameTime)
        {
            // Guarda y lee los estados actuales y anteriores del joystick y teclado
            Input_Management();

            // Actualiza jugador
            foreach (Being Jugador in Global.players)
            {
                Jugador.UpdatePlayer(gameTime, Camara.LimitesPantalla, Var_AltoNivel, Var_AnchoNivel);
            }

            // Ajusto los limites de la camara para que no pueda mostrar mas de este rectangulo
            Camara.Limits = new Rectangle(0, 0, Global.ViewportWidth / 4 * Mapa_Nubes.Length, Var_AltoNivel);

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
            Camara.ViewTargets.Clear();
            foreach (Being Jugador in Global.players)
            {
                if (!Jugador.machine)
                {
                    Camara.ViewTargets.Add(Jugador.GetPositionVec());
                }
            }
            Camara.CentrarCamara();

            Global.mensaje1 = Global.ViewportHeight;
            Global.mensaje2 = Global.ViewportWidth;
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

                Camara.parallax = new Vector2(capa.parallax_x, capa.parallax_y);

                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicClamp, null, null, null, Camara.ViewMatrix);

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
                            capa.RectanguloParallax.X += (int)(Camara.LimitesPantalla.X * capa.parallax_x + 0.5f);

                            // Mensajes de chequeo
                            Global.mensaje3 = Camara.LimitesPantalla.X;
                            Global.mensaje4 = Camara.LimitesPantalla.Width;

                            // Si no esta dentro de la camara no lo dibujo
                            if (Camara.EnCamara(capa.RectanguloParallax))
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
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicClamp, null, null, null, Camara.ViewMatrix);

            Reordenar_Personajes(spriteBatch);

            spriteBatch.End();

            #endregion

        }

        public override void UpdateState(GameTime gameTime)
        {
            
        }
        
        #endregion
    }
}
