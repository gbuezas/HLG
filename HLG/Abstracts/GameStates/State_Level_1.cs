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
        static string[] floorMap = new string[] { "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1" };
        static string[] treeMap = new string[] { "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1" };
        static string[] cloudMap = new string[] { "cloud1", "cloud2", "cloud3", "cloud4", "cloud5", "cloud6", "cloud7", "cloud8", "cloud9", "cloud10", "cloud11", "cloud1", "cloud2", "cloud3", "cloud4", "cloud5", };
        static string[] frontMap = new string[] { "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", };

        /// Velocidades del parallax de cada capa
        /// Valores de parallax aceptables con esta configuracion y zoom (0.2 a 1.0) 
        Parallax clouds = new Parallax(cloudMap, 0.4f, 1f);
        Parallax trees = new Parallax(treeMap, 0.6f, 0.5f);
        Parallax floor = new Parallax(floorMap, 0.85f, 1f);
        Parallax front = new Parallax(frontMap, 1f, 1f);
        
        Rectangle[] sourceRect = new Rectangle[cloudMap.Length]; // Genero un vector con la cantidad de rectangulos necesarios para pintar todo el mapa
        
        int levelHeight = Global.viewportHeight;
        int levelWidth = Global.viewportWidth / 4 * cloudMap.Length;

        //-//-// METHODS //-//-//
        /// <summary>
        /// Cargo los Being segun su clase seleccionada.
        /// Tambien cargo las capas de parallax.
        /// </summary>
        public override void Initialize()
        {
            // Agrego las diferentes capas de parallax
            Global.backgroundLayers.Add(clouds);
            Global.backgroundLayers.Add(trees);
            Global.backgroundLayers.Add(floor);
            Global.frontLayers.Add(front);
        }

        /// <summary>
        /// Para asignarle el viewport en el load del game.
        /// </summary>
        public override void Load(Viewport _viewport)
        {
            // Seteo el viewport correspondiente a la camara
            Global.camara = new Camera(_viewport, levelHeight, Global.viewportWidth / 4 * cloudMap.Length);
        }

        public override void Update(GameTime gameTime)
        {
            // Guarda y lee los estados actuales y anteriores del joystick y teclado
            InputManagement();

            // Actualiza jugador
            foreach (Being playersItem in Global.players)
            {
                playersItem.UpdatePlayer(gameTime, levelHeight, levelWidth);
            }

            // Ajusto los limites de la camara para que no pueda mostrar mas de este rectangulo
            // En vez de ir de 0 al limite del nivel voy a recortarlo un poco asi no se ven los cortes de las capas del parallax

            /*ORIGINAL*/
            Global.camara.Limits = new Rectangle(0, 0, Global.viewportWidth / 4 * cloudMap.Length, levelHeight);

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
            Global.camara.viewTargets.Clear();
            foreach (Being playersItem in Global.players)
            {
                if (playersItem.index != -1)
                {
                    Global.camara.viewTargets.Add(playersItem.GetPositionVec());
                }
            }
            Global.camara.CenterCamera();

            Global.mensaje1 = Global.viewportHeight;
            Global.mensaje2 = Global.viewportWidth;
            
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

            foreach (Parallax backgroundLayersItem in Global.backgroundLayers)
            {

                Global.camara.parallax = new Vector2(backgroundLayersItem.parallaxSpeedX, backgroundLayersItem.parallaxSpeedY);

                Global.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicClamp, null, null, null, Global.camara.ViewMatrix);
                
                rectangulo = 0;
                posicion = 0;

                foreach (string seccion in backgroundLayersItem.parallaxLayer)
                {
                    foreach (Textures avance in Global.level1Textures)
                    {
                        if (seccion == avance.texturePieceName)
                        {
                            sourceRect[rectangulo] = new Rectangle(posicion, 0,
                                Global.viewportWidth / 4,
                                Global.viewportHeight);

                            // Recalculo el rectangulo para que se adapte a la velocidad correspondiente de la capa
                            backgroundLayersItem.parallaxRectangle = sourceRect[rectangulo];

                            // Me parece que sumandole el parallax_x a la multiplicacion del mismo hizo el truco de pegar los tiles bien sin que desaparezcan
                            backgroundLayersItem.parallaxRectangle.X += (int)((Global.camara.screenLimits.X * backgroundLayersItem.parallaxSpeedX) + backgroundLayersItem.parallaxSpeedX);
                            
                            // Mensajes de chequeo
                            Global.mensaje3 = Global.camara.screenLimits.X;
                            Global.mensaje4 = Global.camara.screenLimits.Width;

                            // Si no esta dentro de la camara amplificada horizontalmente no lo dibujo
                            if (Global.camara.InsideCameraAmplified(backgroundLayersItem.parallaxRectangle))
                            {
                                Global.spriteBatch.Draw(avance.texture, sourceRect[rectangulo], Color.White);
                            }

                            posicion += Global.viewportWidth / 4;
                        }

                    }
                    rectangulo++;
                }

                Global.spriteBatch.End();
            }

            #endregion

            #region PERSONAJES

            /// Vuelvo la camara al default?, porque aca todos estamos usando la misma, en el archivo original recibe el paralllax
            /// y aca lo toma de adentro de ella misma despues de ser manoseada por varios procesos, como el dibujado de las capas en si.
            /// Me parece que tendría que separarlo, pero por ahora lo reseteo de esta manera
            Global.camara.parallax = new Vector2(1, 1);

            // SpriteSortMode.Deferred soluciono el problema de que pegaba las capas como se le cantaba el ojete, estaba en BacktoFront
            Global.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicClamp, null, null, null, Global.camara.ViewMatrix);
            
                SortAndDrawCharacters();

            Global.spriteBatch.End();
            
            #endregion

            # region CAPAS FRENTE

            // El rectangulo contenedor del tile
            int rectangulo2, posicion2;

            foreach (Parallax frontLayersItem in Global.frontLayers)
            {

                Global.camara.parallax = new Vector2(frontLayersItem.parallaxSpeedX, frontLayersItem.parallaxSpeedY);
                Global.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicClamp, null, null, null, Global.camara.ViewMatrix);
                
                rectangulo2 = 0;
                posicion2 = 0;

                foreach (string seccion in frontLayersItem.parallaxLayer)
                {
                    foreach (Textures avance in Global.level1Textures)
                    {
                        if (seccion == avance.texturePieceName)
                        {
                            sourceRect[rectangulo2] = new Rectangle(posicion2, 0, Global.viewportWidth / 4, Global.viewportHeight);

                            frontLayersItem.parallaxRectangle = sourceRect[rectangulo2];
                            frontLayersItem.parallaxRectangle.X += (int)((Global.camara.screenLimits.X * frontLayersItem.parallaxSpeedX) + frontLayersItem.parallaxSpeedX);
                            
                            // Si no esta dentro de la camara amplificada horizontalmente no lo dibujo
                            if (Global.camara.InsideCameraAmplified(frontLayersItem.parallaxRectangle))
                            {
                                Global.spriteBatch.Draw(avance.texture, sourceRect[rectangulo2], Color.White);
                            }

                            posicion2 += Global.viewportWidth / 4;
                        }

                    }
                    rectangulo2++;
                }

                Global.spriteBatch.End();
            }

            #endregion

            #region INTERFACE

            // Se usa el dibujado por default asi queda separado de la camara y esta siempre visible
            Global.spriteBatch.Begin();

            foreach (var playersItem in Global.players)
            {
                if (playersItem.index != -1)
                {
                    playersItem.DrawWithoutParallax();
                }
            }
            Global.spriteBatch.End();
            
            #endregion
        }
        
        private static void DrawLayers()
        {

        }

        public override void UpdateState(GameTime gameTime)
        {
            
        }
        
    }
}
