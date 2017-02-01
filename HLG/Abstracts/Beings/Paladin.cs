using System.Collections.Generic;
using HLG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace HLG.Abstracts.Beings
{
    class Paladin : Playable_Characters
    {
        /// <summary>
        /// animacion de cada pieza de armadura:
        /// 1.shield
        /// 2.gauntletback
        /// 3.greaveback
        /// 4.helm
        /// 5.breastplate
        /// 6.tasset
        /// 7.greavetop
        /// 8.sword
        /// 9.gauntlettop
        /// </summary>
        private Animation[] Pieces_Anim = new Animation[Global.PiecesPaladin.Length];
        public Animation[] pieces_anim
        {
            get { return Pieces_Anim; }
            set { Pieces_Anim = value; }
        }
        // GAB - borrar despues
        List<Piece_Set> pieces_armor_recambio = new List<Piece_Set>();
        
        public override void Initialize(Vector2 posicion)
        {
            position = posicion;
            mensaje = position;
            PlayerSpeed = 3.0f;
            direction = Global.Mirada.RIGHT;
            CurrentAction = Global.Actions.STAND;
            OldAction = CurrentAction;
            FrameTime = 50;
            max_health = 200;
            current_health = max_health;
            hitrangeX = 100;
            hitrangeY = 7;
            ResetInjured();
            pieces_armor.Initialize(); // Inicializo partes de armadura actual
            for (int i = 0; i < Global.PiecesPaladin.Length; i++) // Inicializo las piezas de animacion
            {
                Pieces_Anim[i] = new Animation();
                Pieces_Anim[i].Initialize(Global.PiecesPaladin[i]);
            }
            UpdateArmor(pieces_armor_new); // Piezas de la armadura al comenzar
            animations = Pieces_Anim;

            // Animacion de escudito animado de vida
            /// Usamos [2] porque es la textura de los escuditos animados de vida, pero tenemos que buscar una buena manera de que localice
            /// y que sean los escuditos, porque ahora tambien estan los iconos de inventorio [0] y [1]
            UIAnimation = new Animation();
            UIAnimation.LoadTexture(Global.UITextures[2], new Vector2((int)(Global.Camara.parallax.X + Global.ViewportWidth / 5) * (index + 1), Global.UIy),
                                        Global.UIancho,
                                        Global.UIalto,
                                        2,
                                        Color.White,
                                        true);

            /// Animacion de Iconos del inventario
            /// X - Tenemos que automatizar la locacion de los iconos y los fonditos, esta harcodeada. 
            ///     Se hace a partir de las escalas que se le dan a los iconos y barra
            /// X - Tenemos que hacer que cambie de set desde esa barra de iconos. Ella tendria que tener los colores correspondientes a los set disponibles 
            ///     y cuando aceptamos ese cambio se tiene que cambiar el set en el personaje.
            
            int moverEje = 0;
            int iii = 0;
            foreach (var item in Global.IconBarSlots)
            {

                Animation UIInvAnimation = new Animation();
                Animation UIIconAnimation = new Animation();
                
                UIInvAnimation.SetScale(25);
                UIIconAnimation.SetScale(25);
                
                // Fondo del inventario (slots)
                UIInvAnimation.LoadTexture(Global.UITextures[0], new Vector2((int)(Global.Camara.parallax.X + Global.ViewportWidth / 5) * (index + 1) - (Global.InvAncho * 2 + (41 / 3)) + moverEje, 20),
                                            Global.InvAncho,
                                            Global.InvAlto,
                                            2,
                                            Color.White,
                                            false);

                // Aca se elige los iconos del invenario
                UIIconAnimation.LoadTexture(Global.UITextures[3], new Vector2((int)(Global.Camara.parallax.X + Global.ViewportWidth / 5) * (index + 1) - (Global.InvAncho*2+(41/3)) + moverEje, 20),
                                            Global.InvAncho,
                                            Global.InvAlto,
                                            2,
                                            Color.White,
                                            false);
                
                //UIInvAnimation.SetScale(30);
                //UIIconAnimation.SetScale(30);

                UIInvAnimation.pause = true;
                UIIconAnimation.pause = true;
                
                UIIconAnimation.CurrentFrame = iii;
                iii++;

                //UIInventario.Add(UIInvAnimation);
                //UIIcon.Add(UIIconAnimation);
                
                //moverEje += Global.InvAncho - 20;
                moverEje += Global.InvAncho - 15;
                //iii++;
            }
            
            /// Calculo sector de numero de vida
            //Rectangle UI_Rect = new Rectangle(UIx * (i + 1) - Global.UIancho / 2, 0, Global.UIancho, Global.UIalto);
            //Rectangle UI_Rect = new Rectangle((int)UIAnimation.position.X * (indexPlayer + 1), 0, Global.UIancho, Global.UIalto);
            //Rectangle UI_Rect = new Rectangle(0, 0, Global.UIancho, Global.UIalto);
            //UILifeNumber.X = UI_Rect.X + Global.UIancho / 4;
            //UILifeNumber.Y = UI_Rect.Y + Global.UIalto / 2 + 10;
            UILifeNumber.X = ((Global.ViewportWidth/5) * (index + 1)) - 25;
            UILifeNumber.Y = Global.UIy + 8;

            // Asigno control por default al jugador
            controls[(int)Global.Controls.UP] = Keys.W;
            controls[(int)Global.Controls.DOWN] = Keys.S;
            controls[(int)Global.Controls.LEFT] = Keys.A;
            controls[(int)Global.Controls.RIGHT] = Keys.D;
            controls[(int)Global.Controls.BUTTON_HIT] = Keys.T;
            controls[(int)Global.Controls.BUTTON_2] = Keys.Y;

            // Ralentizar los cuadros por segundo del personaje
            // TiempoFrameEjecucion(1);

            // GAB - borrar despues
            // esta mal armado por eso las posiciones son distintas a las globales
            #region cambiar armadura (solo para probar, borrar mas tarde)
            // shield, gauntletback, greaveback, breastplate, helm, tasset, greavetop, sword, gauntlettop, lifebar
            // 0, 4, 7 ,9
            // 1-8, 2-6, 3-5
            // Coloco un recambio de armadura, que en el juego orginal esto tiene que pasar al obtener armaduras nuevas
            // por lo tanto se haria chequeando el inventario.

            Piece_Set recambio = new Piece_Set();
            recambio.Initialize("shield", "set1");
            pieces_armor_recambio.Add(recambio);
            recambio = new Piece_Set();
            recambio.Initialize("gauntletback", "set1");
            pieces_armor_recambio.Add(recambio);
            recambio = new Piece_Set();
            recambio.Initialize("greaveback", "set1");
            pieces_armor_recambio.Add(recambio);
            recambio = new Piece_Set();
            recambio.Initialize("breastplate", "set1");
            pieces_armor_recambio.Add(recambio);
            recambio = new Piece_Set();
            recambio.Initialize("helm", "set1");
            pieces_armor_recambio.Add(recambio);
            recambio = new Piece_Set();
            recambio.Initialize("tasset", "set1");
            pieces_armor_recambio.Add(recambio);
            recambio = new Piece_Set();
            recambio.Initialize("greavetop", "set1");
            pieces_armor_recambio.Add(recambio);
            recambio = new Piece_Set();
            recambio.Initialize("sword", "set1");
            pieces_armor_recambio.Add(recambio);
            recambio = new Piece_Set();
            recambio.Initialize("gauntlettop", "set1");
            pieces_armor_recambio.Add(recambio);
            
            #endregion

        }

        /// <summary>
        /// Actualizar animacion
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            ManualArmorChange();

            foreach (Animation piezaAnimada in Pieces_Anim)
            {
                piezaAnimada.position = position;
                piezaAnimada.Update(gameTime);

                // Fijarse donde va bien este chequeo, en este lugar parece funcionar bien - GAB
                if (current_health > max_health)
                {
                    current_health = max_health;
                }

                // Para los stats de cada personaje (borrar mas tarde) GAB
                mensaje = position;
            }

            #region UI

            UIAnimation.frameTime = 300;
            UIAnimation.Update(gameTime);

            // Inventario
            //foreach (var item in UIInventario)
            //{
            //    item.frameTime = 300;
            //    item.Update(gameTime);
            //}
            //// Inventario
            //foreach (var item in UIIcon)
            //{
            //    item.frameTime = 300;
            //    item.Update(gameTime);
            //}

            /// Los calculos del tamaño y el color de la barra de vida estan hechos con regla de 3 simple
            actual_bar_length = current_health * Global.max_bar_length / max_health;
            bar_color = new Color(255 - (int)(actual_bar_length * 210 / Global.max_bar_length), (int)(actual_bar_length * 210 / Global.max_bar_length), 0);


            #endregion

        }
        private void ManualArmorChange()
        {
            if ((Keyboard.GetState().IsKeyDown(Keys.D8)))
            {
                if (pieces_armor_recambio[7].set == "set1")
                    pieces_armor_recambio[7].set = "set2";
                else
                    pieces_armor_recambio[7].set = "set1";

                UpdateArmor(pieces_armor_recambio);
            }

            if ((Keyboard.GetState().IsKeyDown(Keys.D7)))
            {
                if (pieces_armor_recambio[0].set == "set1")
                    pieces_armor_recambio[0].set = "set2";
                else
                    pieces_armor_recambio[0].set = "set1";

                UpdateArmor(pieces_armor_recambio);
            }

            if ((Keyboard.GetState().IsKeyDown(Keys.D6)))
            {
                if (pieces_armor_recambio[3].set == "set1")
                {
                    pieces_armor_recambio[3].set = "set2";
                    pieces_armor_recambio[5].set = "set2";
                }
                else
                {
                    pieces_armor_recambio[3].set = "set1";
                    pieces_armor_recambio[5].set = "set1";
                }

                UpdateArmor(pieces_armor_recambio);
            }

            if ((Keyboard.GetState().IsKeyDown(Keys.D5)))
            {
                if (pieces_armor_recambio[2].set == "set1")
                {
                    pieces_armor_recambio[2].set = "set2";
                    pieces_armor_recambio[6].set = "set2";
                }
                else
                {
                    pieces_armor_recambio[2].set = "set1";
                    pieces_armor_recambio[6].set = "set1";
                }

                UpdateArmor(pieces_armor_recambio);
            }

            if ((Keyboard.GetState().IsKeyDown(Keys.D4)))
            {
                if (pieces_armor_recambio[1].set == "set1")
                {
                    pieces_armor_recambio[1].set = "set2";
                    pieces_armor_recambio[8].set = "set2";
                }
                else
                {
                    pieces_armor_recambio[1].set = "set1";
                    pieces_armor_recambio[8].set = "set1";
                }

                UpdateArmor(pieces_armor_recambio);
            }

            if ((Keyboard.GetState().IsKeyDown(Keys.D3)))
            {
                if (pieces_armor_recambio[4].set == "set1")
                    pieces_armor_recambio[4].set = "set2";
                else
                    pieces_armor_recambio[4].set = "set1";

                UpdateArmor(pieces_armor_recambio);
            }
        }

        /// <summary>
        /// Dibujar dentro de parallax, personajes y objetos interactivos con ellos
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void DrawWithParallax(SpriteBatch spriteBatch)
        {
            foreach (Animation piezaAnimada in Pieces_Anim)
            {
                piezaAnimada.Draw(spriteBatch, direction, piezaAnimada.color);
            }

            // Si no separo este proceso de dibujo desconcha las posiciones de las capas del jugador
            // +++ Me parece que esto se soluciono cuando cambie el parametro de dibujo en el draw general +++
            spriteBatch.DrawString(Global.CheckStatusVar_2,
            "Frame Actual = " + mensaje1.ToString() + System.Environment.NewLine +
            "Frame Total = " + mensaje2.ToString() + System.Environment.NewLine +
            "Direccion Actual = " + mensaje3.ToString() + System.Environment.NewLine +
            "Accion Actual = " + mensaje4.ToString() + System.Environment.NewLine +
            "Alto = " + mensaje5.ToString() + System.Environment.NewLine +
            "Ancho = " + mensaje6.ToString() + System.Environment.NewLine +
            "X = " + mensaje7.ToString() + System.Environment.NewLine +
            "Y = " + mensaje8.ToString() + System.Environment.NewLine +
            "Vida = " + mensaje9.ToString(),
            mensaje, Color.DarkRed);

            // rectangulos de colision para chequear
            if (Global.EnableRectangles)
            {
                Global.DrawRectangle(GetPositionRec(), Global.Punto_Blanco, spriteBatch);
                Global.DrawRectangle(Global.Rectangle_Collision, Global.Punto_Blanco, spriteBatch);
                Global.DrawRectangle(Global.Rectangle_Collision_2, Global.Punto_Blanco, spriteBatch);
            }
        }

        /// <summary>
        /// Dibujo fuera del parallax, mayormente UI
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void DrawWithoutParallax(SpriteBatch spriteBatch)
        {

            #region INTERFACE

            // Se usa el dibujado por default asi queda separado de la camara y esta siempre visible
            DrawUI(spriteBatch);

            #endregion

        }

        /// <summary>
        /// Actualizar cosas del jugador antes del update general. Aca va todo lo que es la logica del mismo, saltar pegar, etc.
        /// </summary>
        /// <param name="gameTime">El gametime del juego.</param>
        /// <param name="LimitesPantalla">Los limites que puso la camara con respecto a la pantalla que vemos.</param>
        /// <param name="AltoNivel">La altura total del escenario.</param>
        /// <param name="AnchoNivel">El ancho total del escenario.</param>
        public override void UpdatePlayer(GameTime gameTime, int AltoNivel, int AnchoNivel)
        {

            Update(gameTime);

            // Obtengo teclas presionadas - GAB - 
            // Solo lo voy a hacer aca, sino se resetea cada vez que se llama, y como toca variables globales me modifica todo.
            
            // Logica de las acciones, moverse, pegar, etc
            ActionLogic();

            // Logica de las colisiones al golpear
            CollisionLogic();

            // Aqui aplicamos los daños y todo lo correspondiente a los efectos de las acciones hechas anteriormente
            EffectLogic();

            // Hace que el jugador no salga de la pantalla reacomodandolo dentro de la misma.
            // Tomamos como pantalla el rectangulo que genera la camara para acomodar al jugador y limitamos de acuerdo a estas medidas.
            // El FrameEscalado es para acomodar al personaje de acuerdo a la nueva escala adquirida dependiendo 
            // de la pantalla fisica donde se ejecuta el juego.
            #region CAMARA

            if (Global.Camara.LimitesPantalla.Right != 0)
            {
                Rectangle FrameEscalado = GetPositionRec();
                // No usar LimitesPantalla.Right porque rompe el trabado de la pantalla con los personajes principales y deja que uno ararstre al resto
                // Lo mismo para el LimitesPantalla.Left 
                positionX = MathHelper.Clamp(position.X, Global.Camara.LimitesPantalla.Left + FrameEscalado.Width / 2, Global.Camara.LimitesPantalla.Width - FrameEscalado.Width / 2);
                positionY = MathHelper.Clamp(position.Y, AltoNivel - AltoNivel / 2, AltoNivel - FrameEscalado.Height / 2);
            }

            #endregion

            // No es necesario mas acomodar la fila ya que todos vienen con fila 0
            // Solo se acomoda la cantidad de frames por animacion y que animacion va en cada pieza segun la accion ejecutandose.
            #region ANIMACION POR PIEZA

            foreach (Animation piezaAnimation in Pieces_Anim)
            {
                foreach (Textures textura in Global.PaladinTextures)
                {
                    if (textura.piece == piezaAnimation.pieceName &&
                        textura.set == pieces_armor.Get_Set(textura.piece) &&
                        textura.action == CurrentAction.ToString().ToLower())
                    {
                        piezaAnimation.LoadTexture(textura);
                    }
                }
            }

            // Vuelve a 0 el frame de la animacion si cambio de accion
            if (OldAction != CurrentAction)
            {
                foreach (Animation Animation in Pieces_Anim)
                {
                    Animation.CurrentFrame = 0;
                }

                OldAction = CurrentAction;
            }

            #endregion

            // Status del personaje
            mensaje1 = GetCurrentFrame();
            mensaje2 = GetTotalFrames();
            mensaje3 = direction;
            mensaje4 = CurrentAction;
            mensaje5 = Global.FrameHeight;
            mensaje6 = Global.FrameWidth;
            mensaje7 = GetPositionVec().X;
            mensaje8 = GetPositionVec().Y;
        }

        /// <summary>
        /// Cargo los set de armadura que corresponden a cada pieza del cuerpo.
        /// </summary>
        /// <param name="set_pieces"> Shield, gauntlets, greaves, helm, breastplate, tasset, sword respectivamente </param> 
        public override void UpdateArmor(List<Piece_Set> set_pieces)
        {
            // shield, gauntlets, greaves, helm, breastplate, tasset, sword
            foreach (Piece_Set set_piece in set_pieces)
            {
                pieces_armor.Set_Set(set_piece);
            }

            foreach (Animation piezaAnimation in Pieces_Anim)
            {
                foreach (Textures textura in Global.PaladinTextures)
                {
                    if (textura.piece == piezaAnimation.pieceName &&
                        textura.set == pieces_armor.Get_Set(textura.piece) &&
                        textura.action == CurrentAction.ToString().ToLower())
                    {
                        piezaAnimation.LoadTexture(textura, position, FrameWidth, FrameHeight, FrameTime, Color.White, true);
                    }
                }
            }

        }
        
        /// <summary>
        /// Obtiene la posicion de una pieza de animacion en rectangulo
        /// </summary>
        /// <returns> Posicion del jugador </returns>
        public override Rectangle GetPositionRec()
        {
            return pieces_anim[0].GetPosition();
        }

        /// <summary>
        /// Cambia el color del personaje entero 
        /// </summary>
        /// <param name="tinte"> Color deseado </param>
        public override void ColorAnimationChange(Color tinte)
        {
            foreach (Animation Animation in animations)
            {
                Animation.ColorChange(tinte);
            }
        }

        /// <summary>
        /// Cambia el color de una pieza del personaje 
        /// </summary>
        /// <param name="tinte"> Color deseado </param>
        /// <param name="pieza"> Pieza que queremos cambiar el color </param>
        public override void ColorPieceChange(Color tinte, int pieza)
        {
            animations[pieza].ColorChange(tinte);
        }

        /// <summary>
        /// Obtiene el frame actual de la animacion parandose en la primera pieza de la misma [0]
        /// </summary>
        /// <returns> Frame actual de la animacion </returns>
        public override int GetCurrentFrame()
        {
            return animations[0].CurrentFrame;
        }

        /// <summary>
        /// Obtiene cantidad total de frames de la animacion parandose en la primera pieza de la misma [0]
        /// Se le resta 1 al valor ya que como es cantidad no cuenta desde 0, como lo hacen los indices,
        /// de esta manera podemos comparar indices con cantidad.
        /// </summary>
        /// <returns> Frames total de la animacion </returns>
        public override int GetTotalFrames()
        {
            return animations[0].FrameCount - 1;
        }

        /// <summary>
        /// Activa o desactiva al jugador (si no esta activo no se dibuja)
        /// </summary>
        public override void ActivatePlayer(bool active)
        {
            foreach (Animation piece in animations)
            {
                piece.active = active;
            }
        }

        /// <summary>
        /// Limpio la lista interna de personajes que daño este objeto, este metodo se usa al terminar una animacion que daña.
        /// </summary>
        public override void ResetInjured()
        {
            for (int i = 0; i < injured.Length; i++)
            {
                injured[i] = false;
            }
        }
        
        #region METODOS PROPIOS

        /// <summary>
        /// Establece el tiempo de frame en ejecucion
        /// </summary>
        /// <param name="Tiempo">El tiempo que va a durar el frame en pantalla de las distintas animaciones del personaje</param>
        void FrameSpeed(int Tiempo)
        {
            foreach (Animation piezaAnimada in Pieces_Anim)
            {
                piezaAnimada.frameTime = Tiempo;
            }
        }

        /// <summary>
        /// Pausa la animacion en el frame actual
        /// </summary>
        /// <param name="desactivar">pone o quita la pausa segun este parametro</param>
        void PauseAnimation(bool desactivar)
        {
            foreach (Animation piezaAnimada in Pieces_Anim)
            {
                piezaAnimada.pause = desactivar;
            }
        }

        

        /// <summary>
        /// Logica de las colisiones de los golpes:
        /// 
        ///     Implementamos un chequeo jugador por jugador a la hora de golpear, que cumpla con las siguientes reglas:
        ///     - Si le toca chequear con el mismo se saltea.
        ///     - Si el frame de la animacion no es justo cuando golpea con la espada se saltea.
        ///     - Si fue golpeado anteriormente se saltea
        ///     - Si es fantasma se saltea
        /// </summary>
        private void CollisionLogic()
        {
            if ((CurrentAction == Global.Actions.HIT1 ||
                 CurrentAction == Global.Actions.HIT2 ||
                 CurrentAction == Global.Actions.HIT3) &&
                 !ghost_mode &&
                 GetCurrentFrame() == 5)
            {

                for (int i = 0; i < Global.totalQuant; i++)
                {
                    // Ver summary
                    if (!injured[i] &&
                        Global.players[i] != this &&
                        !Global.players[i].ghost_mode)
                    {
                        
                        // Si esta dentro del radio del golpe
                        if (CollisionVerifier(Global.players[i].GetPositionRec()))
                        {
                            // Cuando la armadura esta detras del efecto de la espada no se puede ver bien el cambio de color
                            // Le sumamos el resultado para que sea acumulativo si varios golpean al mismo objetivo
                            //Global.players[i].ColorAnimationChange(Color.Red);
                            Global.players[i].injured_value += 10;
                            injured[i] = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Lógica de los efectos de las colisiones y movimientos realizados.
        /// </summary>
        private void EffectLogic()
        {

            if (!ghost_mode)
            {
                // Reestablezco su color natural si no va a recibir daño, de esta manera no permito que vuelva a su color 
                // demasiado rapido como para que no se vea que fue dañado
                if (injured_value == 0)
                    ColorAnimationChange(Color.White);
                else
                    ColorAnimationChange(Color.Red);

                // Hago la resta necesaria a la health
                current_health -= injured_value;

                // Vuelvo el contador de daño a 0 y quito que este dañado
                injured_value = 0;

                // Si pierde toda su HP se vuelve fantasma
                if (current_health <= 0)
                {
                    ghost_mode = true;
                }
            }
            else
            {
                ColorAnimationChange(Global.ColorGhost);

                if (current_health > 0)
                {
                    ghost_mode = false;
                }
            }

            // MENSAJES: Veo la health de los personajes
            mensaje9 = current_health;
        }

        public void DrawUI(SpriteBatch spriteBatch)
        {
            /// Para obtener los 4 puntos donde tiene que dibujarse cada UI, obtener el punto Y es innecesario, el mismo es siempre 0 ya que se
            /// usan las transparencias de la imagen para obtener el espacio necesario del eje Y.
            //int UIx = (int)(Global.Camara.parallax.X + Global.ViewportWidth / 5);

            // Dibuja UI animada (escuditos)
            UIAnimation.Draw(spriteBatch, Global.Mirada.RIGHT);
            
            // Dibujo inventorio
            //foreach (var item in UIInventario)
            //{
            //    item.Draw(spriteBatch, Global.Mirada.RIGHT);
            //}
            //foreach (var item in UIIcon)
            //{
            //    item.Draw(spriteBatch, Global.Mirada.RIGHT);
            //}
            
            // Dibujar barra de vida
            Global.DrawStraightLine(new Vector2(UILifeNumber.X, UILifeNumber.Y),
                                    new Vector2(UILifeNumber.X + actual_bar_length, UILifeNumber.Y),
                                    Global.Punto_Blanco,
                                    bar_color,
                                    spriteBatch,
                                    14);

            // Vida en numeros
            spriteBatch.DrawString(Global.CheckStatusVar_2,
                                   current_health.ToString() + " / " + max_health.ToString(),
                                   UILifeNumber,
                                   Color.White);
       }

        #endregion

    }
}
