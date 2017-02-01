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
        #region VARIABLES

        #region MAPA Y PARALLAX

        // Mapa de tiles de las capas
        static string[] Mapa_Piso = new string[] { "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1", "soil1" };
        static string[] Mapa_Arboles = new string[] { "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1", "wood1" };
        static string[] Mapa_Nubes = new string[] { "cloud1", "cloud2", "cloud3", "cloud4", "cloud5", "cloud6", "cloud7", "cloud8", "cloud9", "cloud10", "cloud11", "cloud1", "cloud2", "cloud3", "cloud4", "cloud5", };
        static string[] Mapa_Frente = new string[] { "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", "front1", };

        // Velocidades del parallax de cada capa
        // Valores de parallax aceptables con esta configuracion y zoom (0.2 a 1.0) 
        Parallax Nubes = new Parallax(Mapa_Nubes, 0.4f, 1f);
        Parallax Arboles = new Parallax(Mapa_Arboles, 0.6f, 0.5f);
        Parallax Piso = new Parallax(Mapa_Piso, 0.85f, 1f);
        Parallax Frente = new Parallax(Mapa_Frente, 1f, 1f);
        
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
            Global.Front_Layers.Add(Frente);
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
            Global.Camara.Limits = new Rectangle(0, 0, Global.ViewportWidth / 4 * Mapa_Nubes.Length, Var_AltoNivel);

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
                if (Jugador.index != -1)
                {
                    Global.Camara.ViewTargets.Add(Jugador.GetPositionVec());
                }
            }
            Global.Camara.CentrarCamara();

            Global.mensaje1 = Global.ViewportHeight;
            Global.mensaje2 = Global.ViewportWidth;

            //#region UI Animation

            //int UIancho = 450;
            //int UIalto = 550;

            //int UIx = (int)(Global.Camara.parallax.X + Global.ViewportWidth / 5);
            ////int UIy = (int)Global.Camara.parallax.Y;
            //int UIy = 78;

            
            //for (int i=0; i < Global.playersQuant; i++)
            //{
            //    ////Vector2 UI_Vec = new Vector2(UIx * (i + 1) - UIancho / 2, 0);
            //    //Vector2 UI_Vec = new Vector2(UIx * (i + 1), UIy);

            //    if (Global.UIAnimation[i] == null)
            //    {
            //        Global.UIAnimation[i] = new Animation();
            //        Global.UIAnimation[i].LoadTexture(Global.UITextures[2], new Vector2(UIx * (i + 1), UIy), UIancho, UIalto, 2, Color.White, true);
            //        //Global.UIAnimation[i].LoadTexture(Global.UITextures[2], UI_Vec, UIancho, UIalto, 2, Color.White, true);
                    
            //        /// Usamos 2 porque es la textura de los escuditos animados de vida, pero tenemos que buscar una buena manera de que localice
            //        /// y que sean los escuditos, porque ahora tambien estan los iconos de inventorio 0 y 1
            //        /// fijarse como armar algo bien de esto y tratar de meterlo que se genere en cada personaje automaticamente.

            //    }

            //    if (Global.InvAnimation[i] == null)
            //    {
                    
            //        Global.InvAnimation[i] = new Animation();
            //        Global.InvAnimation[i].escalaAnimacion = 28;
            //        Global.InvAnimation[i].LoadTexture(Global.UITextures[0], new Vector2(UIx * (i + 1), UIy - 53), Global.InvBarAncho, Global.InvBarAlto, 2, Color.White, true);

            //    }

            //    if (Global.IconAnimation[i] == null)
            //    {
            //        Global.IconAnimation[i] = new Animation();
            //        Global.IconAnimation[i].escalaAnimacion = 28;
            //        Global.IconAnimation[i].LoadTexture(Global.UITextures[1], new Vector2(UIx * (i + 1) - Global.InvBarAncho/8*2, UIy - 53), Global.InvAncho, Global.InvAlto, 2, Color.White, true);
            //        Global.IconAnimation[i].pause = false;
            //        // El true al final de la animacion hace que no haga loop
            //        // La pausa hace que se congele en el cuadro deseado

            //    }

            //    Global.UIAnimation[i].frameTime = 300;
            //    Global.UIAnimation[i].Update(gameTime);

            //    Global.InvAnimation[i].frameTime = 300;
            //    Global.InvAnimation[i].Update(gameTime);

            //    Global.IconAnimation[i].frameTime = 300;
            //    Global.IconAnimation[i].Update(gameTime);
            //}
            //#endregion
        }

        /// <summary>
        /// Aca se hace el calculo de que personaje se dibuja primero.
        /// Tambien se hace el calculo de la camara y el parallax.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {

            # region CAPAS FONDO

            // El rectangulo contenedor del tile
            int rectangulo;
            // La posicion donde va el siguiente rectangulo
            int posicion;

            foreach (Parallax capa in Global.Background_Layers)
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

                            // Me parece que sumandole el parallax_x a la multiplicacion del mismo hizo el truco de pegar los tiles bien sin que desaparezcan
                            capa.RectanguloParallax.X += (int)((Global.Camara.LimitesPantalla.X * capa.parallax_x) + capa.parallax_x);
                            
                            // Mensajes de chequeo
                            Global.mensaje3 = Global.Camara.LimitesPantalla.X;
                            Global.mensaje4 = Global.Camara.LimitesPantalla.Width;

                            // Si no esta dentro de la camara amplificada horizontalmente no lo dibujo
                            if (Global.Camara.EnCamaraAmplificada(capa.RectanguloParallax))
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

            /// Vuelvo la camara al default?, porque aca todos estamos usando la misma, en el archivo original recibe el paralllax
            /// y aca lo toma de adentro de ella misma despues de ser manoseada por varios procesos, como el dibujado de las capas en si.
            /// Me parece que tendría que separarlo, pero por ahora lo reseteo de esta manera
            Global.Camara.parallax = new Vector2(1, 1);

            // SpriteSortMode.Deferred soluciono el problema de que pegaba las capas como se le cantaba el ojete, estaba en BacktoFront
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicClamp, null, null, null, Global.Camara.ViewMatrix);
            
            Reordenar_Personajes(spriteBatch);

            spriteBatch.End();
            
            #endregion

            # region CAPAS FRENTE

            // El rectangulo contenedor del tile
            int rectangulo2, posicion2;

            foreach (Parallax capa in Global.Front_Layers)
            {

                Global.Camara.parallax = new Vector2(capa.parallax_x, capa.parallax_y);
                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicClamp, null, null, null, Global.Camara.ViewMatrix);
                
                rectangulo2 = 0;
                posicion2 = 0;

                foreach (string seccion in capa.capa_parallax)
                {
                    foreach (Textures avance in Global.Level_1Textures)
                    {
                        if (seccion == avance.piece)
                        {
                            sourceRect[rectangulo2] = new Rectangle(posicion2, 0, Global.ViewportWidth / 4, Global.ViewportHeight);

                            capa.RectanguloParallax = sourceRect[rectangulo2];
                            capa.RectanguloParallax.X += (int)((Global.Camara.LimitesPantalla.X * capa.parallax_x) + capa.parallax_x);
                            
                            // Si no esta dentro de la camara amplificada horizontalmente no lo dibujo
                            if (Global.Camara.EnCamaraAmplificada(capa.RectanguloParallax))
                            {
                                spriteBatch.Draw(avance.textura, sourceRect[rectangulo2], Color.White);
                            }

                            posicion2 += Global.ViewportWidth / 4;
                        }

                    }
                    rectangulo2++;
                }

                spriteBatch.End();
            }

            #endregion

            #region INTERFACE

            // Se usa el dibujado por default asi queda separado de la camara y esta siempre visible
            spriteBatch.Begin();

            foreach (var item in Global.players)
            {
                if (item.index != -1)
                {
                    item.DrawWithoutParallax(spriteBatch);
                }
            }
            spriteBatch.End();
            
            #endregion
        }

        //private static void DrawUI(SpriteBatch spriteBatch)
        //{
        //    /// Para obtener los 4 puntos donde tiene que dibujarse cada UI, obtener el punto Y es innecesario, el mismo es siempre 0 ya que se
        //    /// usan las transparencias de la imagen para obtener el espacio necesario del eje Y.
        //    int UIx = (int)(Global.Camara.parallax.X + Global.ViewportWidth / 5);

        //    for (int i = 0; i < Global.playersQuant; i++)
        //    {
        //        // Dibuja UI animada (escuditos)
        //        Global.UIAnimation[i].Draw(spriteBatch, Global.Mirada.RIGHT);

        //        // Dibuja barra del Inventario
        //        Global.InvAnimation[i].Draw(spriteBatch, Global.Mirada.RIGHT);

        //        List<Animation> TempIcon = new List<Animation>();

        //        // Dibuja iconos del inventario, tengo que generar nuevos sino dibuja siempre el mismo, no hace un duplicado, y yo quiero 8 duplicados
        //        for (int f = 1; f <= 8; f++ )
        //        {
        //            TempIcon.Add(Global.IconAnimation[i]);
        //            TempIcon[f-1].CurrentFrame = f-1;
        //            //TempIcon[f-1].position.X = TempIcon[f - 1].position.X*f;
        //            TempIcon[f-1].Draw(spriteBatch, Global.Mirada.RIGHT);
        //        }

        //        TempIcon.Clear();

        //        /// Calculo sector de numero de vida
        //        Rectangle UI_Rect = new Rectangle(UIx * (i + 1) - Global.UIancho / 2, 0, Global.UIancho, Global.UIalto);
        //        Global.UILifeNumber[i].X = UI_Rect.X + Global.UIancho / 4;
        //        Global.UILifeNumber[i].Y = UI_Rect.Y + Global.UIalto / 2 + 10;

        //        /// Los calculos del tamaño y el color de la barra de vida estan hechos con regla de 3 simple
        //        float actual_bar_length = Global.players[i].current_health * Global.max_bar_length / Global.players[i].max_health;

        //        /// Color
        //        int new_color = (int)(actual_bar_length * 210 / Global.max_bar_length);
        //        Color bar_color = new Color(255 - new_color, new_color, 0);

        //        // Dibujar barra de vida
        //        Global.DrawStraightLine(new Vector2(Global.UILifeNumber[i].X - 1, Global.UILifeNumber[i].Y + 2),
        //                                new Vector2(Global.UILifeNumber[i].X + actual_bar_length, Global.UILifeNumber[i].Y),
        //                                Global.Punto_Blanco,
        //                                bar_color,
        //                                spriteBatch,
        //                                14);

        //        // Vida en numeros
        //        spriteBatch.DrawString(Global.CheckStatusVar_2,
        //                               Global.players[i].current_health.ToString() + " / " + Global.players[i].max_health.ToString(),
        //                               Global.UILifeNumber[i],
        //                               Color.White);
        //    }
        //}

        private static void DrawLayers(SpriteBatch spriteBatch)
        {

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
