using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HLG.Objects;
using Microsoft.Xna.Framework.Input;
using HLG.Abstracts.Beings;
using System.Collections.Generic;

namespace HLG.Abstracts.GameStates
{
    class State_Level_1 : States
    {
        //-//-// VARIABLES //-//-//
        // Mapa de tiles de las capas
        static string[] mapaPiso = new string[] { "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1" };
        static string[] mapaArboles = new string[] { "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1" };
        static string[] mapaNubes = new string[] { "cloud1", "cloud2", "cloud3", "cloud4", "cloud5", "cloud6", "cloud7", "cloud8", "cloud9", "cloud10", "cloud11", "cloud1", "cloud2", "cloud3", "cloud4", "cloud5", };
        static string[] mapaFrente = new string[] { "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", };

        /// Velocidades del parallax de cada capa
        /// Valores de parallax aceptables con esta configuracion y zoom (0.2 a 1.0) 
        Parallax nubes = new Parallax(mapaNubes, 0.4f, 1f);
        Parallax arboles = new Parallax(mapaArboles, 0.6f, 0.5f);
        Parallax piso = new Parallax(mapaPiso, 0.85f, 1f);
        Parallax frente = new Parallax(mapaFrente, 1f, 1f);
        
        Rectangle[] sourceRect = new Rectangle[mapaNubes.Length]; // Genero un vector con la cantidad de rectangulos necesarios para pintar todo el mapa
        
        int varAltoNivel = Global.viewport_height;
        int varAnchoNivel = Global.viewport_width / 4 * mapaNubes.Length;

        //-//-// METHODS //-//-//
        /// <summary>
        /// Cargo los Being segun su clase seleccionada.
        /// Tambien cargo las capas de parallax.
        /// </summary>
        public override void Initialize()
        {
            // Agrego las diferentes capas de parallax
            Global.background_layers.Add(nubes);
            Global.background_layers.Add(arboles);
            Global.background_layers.Add(piso);
            Global.front_layers.Add(frente);
        }

        /// <summary>
        /// Para asignarle el viewport en el load del game.
        /// </summary>
        public override void Load(Viewport _viewport)
        {
            // Seteo el viewport correspondiente a la camara
            Global.camara = new Camera(_viewport, varAltoNivel, Global.viewport_width / 4 * mapaNubes.Length);
        }

        public override void Update(GameTime gameTime)
        {
            // Guarda y lee los estados actuales y anteriores del joystick y teclado
            InputManagement();

            // Actualiza jugador
            foreach (Being Jugador in Global.players)
            {
                Jugador.Update_Player(gameTime, varAltoNivel, varAnchoNivel);
            }

            // Ajusto los limites de la camara para que no pueda mostrar mas de este rectangulo
            // En vez de ir de 0 al limite del nivel voy a recortarlo un poco asi no se ven los cortes de las capas del parallax

            /*ORIGINAL*/
            Global.camara.Limits = new Rectangle(0, 0, Global.viewport_width / 4 * mapaNubes.Length, varAltoNivel);

            //Global.Camara.Limits = new Rectangle(Global.ViewportWidth / 4, 0, Global.ViewportWidth / 4 * (Mapa_Nubes.Length - 1), Var_AltoNivel);

            // Tomo tiempo transcurrido.
            //float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Para poder controlar al otro personaje por separado
            // Si lo saco de aca no me toma los cambios del control
            #region controles de los personajes - borrar

            Global.players[1].controls[(int)Global.Controls.UP] = Keys.I;
            Global.players[1].controls[(int)Global.Controls.DOWN] = Keys.K;
            Global.players[1].controls[(int)Global.Controls.LEFT] = Keys.J;
            Global.players[1].controls[(int)Global.Controls.RIGHT] = Keys.L;
            Global.players[1].controls[(int)Global.Controls.BUTTON_HIT] = Keys.Space;

            Global.players[2].controls[(int)Global.Controls.UP] = Keys.I;
            Global.players[2].controls[(int)Global.Controls.DOWN] = Keys.K;
            Global.players[2].controls[(int)Global.Controls.LEFT] = Keys.J;
            Global.players[2].controls[(int)Global.Controls.RIGHT] = Keys.L;
            Global.players[2].controls[(int)Global.Controls.BUTTON_HIT] = Keys.Enter;
            
            #endregion

            // Hacer un foreach para todos los personajes que quedan en camara, 
            // solo los controlados por humanos, la maquina no, asi pueden salir y no me desconcha toda la camara y el zoom
            // Aca controlamos donde van a aparecer inicialmente todos los jugadores
            Global.camara.ViewTargets.Clear();
            foreach (Being Jugador in Global.players)
            {
                if (Jugador.index != -1)
                {
                    Global.camara.ViewTargets.Add(Jugador.Get_Position_Vec());
                }
            }
            Global.camara.CentrarCamara();

            Global.mensaje1 = Global.viewport_height;
            Global.mensaje2 = Global.viewport_width;
            
        }

        /// <summary>
        /// Aca se hace el calculo de que personaje se dibuja primero.
        /// Tambien se hace el calculo de la camara y el parallax.
        /// </summary>
        public override void Draw()
        {

            # region CAPAS FONDO

            // El rectangulo contenedor del tile
            int rectangulo;
            // La posicion donde va el siguiente rectangulo
            int posicion;

            foreach (Parallax capa in Global.background_layers)
            {

                Global.camara.parallax = new Vector2(capa.parallax_x, capa.parallax_y);

                Global.sprite_batch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicClamp, null, null, null, Global.camara.ViewMatrix);
                
                rectangulo = 0;
                posicion = 0;

                foreach (string seccion in capa.capa_parallax)
                {
                    foreach (Textures avance in Global.level_1textures)
                    {
                        if (seccion == avance.texture_piece_name)
                        {
                            sourceRect[rectangulo] = new Rectangle(posicion, 0,
                                Global.viewport_width / 4,
                                Global.viewport_height);

                            // Recalculo el rectangulo para que se adapte a la velocidad correspondiente de la capa
                            capa.RectanguloParallax = sourceRect[rectangulo];

                            // Me parece que sumandole el parallax_x a la multiplicacion del mismo hizo el truco de pegar los tiles bien sin que desaparezcan
                            capa.RectanguloParallax.X += (int)((Global.camara.LimitesPantalla.X * capa.parallax_x) + capa.parallax_x);
                            
                            // Mensajes de chequeo
                            Global.mensaje3 = Global.camara.LimitesPantalla.X;
                            Global.mensaje4 = Global.camara.LimitesPantalla.Width;

                            // Si no esta dentro de la camara amplificada horizontalmente no lo dibujo
                            if (Global.camara.EnCamaraAmplificada(capa.RectanguloParallax))
                            {
                                Global.sprite_batch.Draw(avance.texture, sourceRect[rectangulo], Color.White);
                            }

                            posicion += Global.viewport_width / 4;
                        }

                    }
                    rectangulo++;
                }

                Global.sprite_batch.End();
            }

            #endregion

            #region PERSONAJES

            /// Vuelvo la camara al default?, porque aca todos estamos usando la misma, en el archivo original recibe el paralllax
            /// y aca lo toma de adentro de ella misma despues de ser manoseada por varios procesos, como el dibujado de las capas en si.
            /// Me parece que tendría que separarlo, pero por ahora lo reseteo de esta manera
            Global.camara.parallax = new Vector2(1, 1);

            // SpriteSortMode.Deferred soluciono el problema de que pegaba las capas como se le cantaba el ojete, estaba en BacktoFront
            Global.sprite_batch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicClamp, null, null, null, Global.camara.ViewMatrix);
            
                SortAndDrawCharacters();

            Global.sprite_batch.End();
            
            #endregion

            # region CAPAS FRENTE

            // El rectangulo contenedor del tile
            int rectangulo2, posicion2;

            foreach (Parallax capa in Global.front_layers)
            {

                Global.camara.parallax = new Vector2(capa.parallax_x, capa.parallax_y);
                Global.sprite_batch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicClamp, null, null, null, Global.camara.ViewMatrix);
                
                rectangulo2 = 0;
                posicion2 = 0;

                foreach (string seccion in capa.capa_parallax)
                {
                    foreach (Textures avance in Global.level_1textures)
                    {
                        if (seccion == avance.texture_piece_name)
                        {
                            sourceRect[rectangulo2] = new Rectangle(posicion2, 0, Global.viewport_width / 4, Global.viewport_height);

                            capa.RectanguloParallax = sourceRect[rectangulo2];
                            capa.RectanguloParallax.X += (int)((Global.camara.LimitesPantalla.X * capa.parallax_x) + capa.parallax_x);
                            
                            // Si no esta dentro de la camara amplificada horizontalmente no lo dibujo
                            if (Global.camara.EnCamaraAmplificada(capa.RectanguloParallax))
                            {
                                Global.sprite_batch.Draw(avance.texture, sourceRect[rectangulo2], Color.White);
                            }

                            posicion2 += Global.viewport_width / 4;
                        }

                    }
                    rectangulo2++;
                }

                Global.sprite_batch.End();
            }

            #endregion

            #region INTERFACE

            // Se usa el dibujado por default asi queda separado de la camara y esta siempre visible
            Global.sprite_batch.Begin();

            foreach (var item in Global.players)
            {
                if (item.index != -1)
                {
                    item.Draw_Without_Parallax();
                }
            }
            Global.sprite_batch.End();
            
            #endregion
        }
        
        private static void Draw_Layers()
        {

        }

        public override void UpdateState(GameTime gameTime)
        {
            
        }
        
    }
}
