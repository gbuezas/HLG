using System.Collections.Generic;
using HLG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HLG.Abstracts.Beings
{
    class Paladin : Playable_Characters
    {
        
        //-//-// VARIABLES //-//-//
        List<Piece_Set> pieces_armor_recambio = new List<Piece_Set>(); // GAB - borrar despues, es para mostrar el cambio de armadura
        static string[] pieces_paladin = new string[] { "shield", "gauntletback", "greaveback", "breastplate", "helm", "tasset", "greavetop", "sword", "gauntlettop" };
        
        //-//-// METHODS //-//-//
        public override void Initialize(Vector2 posicion)
        {
            animation_pieces = new Animation[pieces_paladin.Length];
            object_textures = Global.paladin_textures;

            position = posicion;
            mensaje = position;
            playerMoveSpeed = 3.0f;
            currentAction = Global.Actions.STAND;
            oldAction = currentAction;
            frameTime = 50;

            frameHeight = 240;
            frameWidth = 320;

            hitRangeX = 100;
            hitRangeY = 7;

            max_health = 200;
            current_health = max_health;
            
            ResetInjured();

            pieces_armor.Initialize(pieces_paladin); // Inicializo partes de armadura actual
            for (int i = 0; i < pieces_paladin.Length; i++) // Inicializo las piezas de animacion
            {
                animation_pieces[i] = new Animation();
                animation_pieces[i].Initialize(pieces_paladin[i]);
            }
            UpdateArmor(pieces_armor_new); // Piezas de la armadura al comenzar
            animations = animation_pieces;

            // Animacion de escudito animado de vida
            /// Usamos [2] porque es la textura de los escuditos animados de vida, pero tenemos que buscar una buena manera de que localice
            /// y que sean los escuditos, porque ahora tambien estan los iconos de inventorio [0] y [1]
            //UIAnimation = new Animation();
            //UIAnimation.LoadTexture(Global.UITextures[2], new Vector2((int)(Global.camara.parallax.X + Global.viewport_width / 5) * (index + 1), Global.ui_y),
            //                            Global.ui_widht,
            //                            Global.ui_height,
            //                            2,
            //                            Color.White,
            //                            true);

            gui.initialize(index);

            /// Animacion de Iconos del inventario
            /// X - Tenemos que automatizar la locacion de los iconos y los fonditos, esta harcodeada. 
            ///     Se hace a partir de las escalas que se le dan a los iconos y barra
            /// X - Tenemos que hacer que cambie de set desde esa barra de iconos. Ella tendria que tener los colores correspondientes a los set disponibles 
            ///     y cuando aceptamos ese cambio se tiene que cambiar el set en el personaje.
            
            //int moverEje = 0;
            //int iii = 0;
            //foreach (var item in Global.IconBarSlots)
            //{

            //    Animation UIInvAnimation = new Animation();
            //    Animation UIIconAnimation = new Animation();
                
            //    UIInvAnimation.SetScale(25);
            //    UIIconAnimation.SetScale(25);
                
            //    // Fondo del inventario (slots)
            //    UIInvAnimation.LoadTexture(Global.UITextures[0], new Vector2((int)(Global.Camara.parallax.X + Global.ViewportWidth / 5) * (index + 1) - (Global.InvAncho * 2 + (41 / 3)) + moverEje, 20),
            //                                Global.InvAncho,
            //                                Global.InvAlto,
            //                                2,
            //                                Color.White,
            //                                false);

            //    // Aca se elige los iconos del invenario
            //    UIIconAnimation.LoadTexture(Global.UITextures[3], new Vector2((int)(Global.Camara.parallax.X + Global.ViewportWidth / 5) * (index + 1) - (Global.InvAncho*2+(41/3)) + moverEje, 20),
            //                                Global.InvAncho,
            //                                Global.InvAlto,
            //                                2,
            //                                Color.White,
            //                                false);
                
            //    //UIInvAnimation.SetScale(30);
            //    //UIIconAnimation.SetScale(30);

            //    UIInvAnimation.pause = true;
            //    UIIconAnimation.pause = true;
                
            //    UIIconAnimation.CurrentFrame = iii;
            //    iii++;

            //    //UIInventario.Add(UIInvAnimation);
            //    //UIIcon.Add(UIIconAnimation);
                
            //    //moverEje += Global.InvAncho - 20;
            //    moverEje += Global.InvAncho - 15;
            //    //iii++;
            //}
            
            /// Calculo sector de numero de vida
            //Rectangle UI_Rect = new Rectangle(UIx * (i + 1) - Global.UIancho / 2, 0, Global.UIancho, Global.UIalto);
            //Rectangle UI_Rect = new Rectangle((int)UIAnimation.position.X * (indexPlayer + 1), 0, Global.UIancho, Global.UIalto);
            //Rectangle UI_Rect = new Rectangle(0, 0, Global.UIancho, Global.UIalto);
            //UILifeNumber.X = UI_Rect.X + Global.UIancho / 4;
            //UILifeNumber.Y = UI_Rect.Y + Global.UIalto / 2 + 10;
            //UILifeNumber.X = ((Global.viewport_width/5) * (index + 1)) - 25;
            //UILifeNumber.Y = Global.ui_y + 8;

            // Asigno control por default al jugador
            controls[(int)Global.Controls.UP] = Keys.W;
            controls[(int)Global.Controls.DOWN] = Keys.S;
            controls[(int)Global.Controls.LEFT] = Keys.A;
            controls[(int)Global.Controls.RIGHT] = Keys.D;
            controls[(int)Global.Controls.BUTTON_HIT] = Keys.T;
            controls[(int)Global.Controls.BUTTON_SPECIAL] = Keys.Y;

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
        
        public override void UpdatePlayer(GameTime gameTime, int AltoNivel, int AnchoNivel)
        {
            ManualArmorChange();

            AnimationFramePositionUpdate(gameTime);

            CapsMaxHealth();

            // Para los stats de cada personaje (borrar mas tarde) GAB
            mensaje = position;

            #region UI

            gui.UpdateGUI(gameTime, current_health, max_health);

            //UIAnimation.frameTime = 300;
            //UIAnimation.Update(gameTime);

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

            ///// Los calculos del tamaño y el color de la barra de vida estan hechos con regla de 3 simple
            //actual_bar_length = current_health * Global.max_bar_length / max_health;
            //bar_color = new Color(255 - (int)(actual_bar_length * 210 / Global.max_bar_length), (int)(actual_bar_length * 210 / Global.max_bar_length), 0);


            #endregion

            ActionLogicManual();
            CollisionLogic();
            EffectLogic();

            /// Hace que el jugador no salga de la pantalla reacomodandolo dentro de la misma.
            /// Tomamos como pantalla el rectangulo que genera la camara para acomodar al jugador y limitamos de acuerdo a estas medidas.
            /// El FrameEscalado es para acomodar al personaje de acuerdo a la nueva escala adquirida dependiendo 
            /// de la pantalla fisica donde se ejecuta el juego.
            StayInScreen(AltoNivel);

            /// Carga texturas y acomoda los frames al cambiar de accion
            TextureRegularLoad();
            FrameNumberActionReset();
            
            // Status del personaje
            mensaje1 = GetCurrentFrame();
            mensaje2 = GetTotalFrames();
            mensaje3 = facing;
            mensaje4 = currentAction;
            //mensaje5 = Global.FrameHeight;
            //mensaje6 = Global.FrameWidth;
            mensaje7 = GetPositionVec().X;
            mensaje8 = GetPositionVec().Y;
        }
        
        /// <summary>
        /// Establece el tiempo de frame en ejecucion
        /// </summary>
        /// <param name="Tiempo">El tiempo que va a durar el frame en pantalla de las distintas animaciones del personaje</param>
        void FrameSpeed(int Tiempo)
        {
            foreach (Animation piezaAnimada in animation_pieces)
            {
                piezaAnimada.frameTime = Tiempo;
            }
        }
        
        private void ManualArmorChange() // Tendria que eliminarse con la beta cerrada
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
            if ((currentAction == Global.Actions.HIT1 ||
                 currentAction == Global.Actions.HIT2 ||
                 currentAction == Global.Actions.HIT3) &&
                 !ghost_mode &&
                 GetCurrentFrame() == 5)
            {

                for (int i = 0; i < Global.total_quant; i++)
                {
                    // Ver summary
                    if (!injuredByMe[i] &&
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
                            injuredByMe[i] = true;
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
                ColorAnimationChange(Global.color_ghost);

                if (current_health > 0)
                {
                    ghost_mode = false;
                }
            }

            // MENSAJES: Veo la health de los personajes
            mensaje9 = current_health;
        }
        
    }
}
