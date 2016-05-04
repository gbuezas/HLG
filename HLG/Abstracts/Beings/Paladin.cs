using System.Collections.Generic;
using HLG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HLG.Abstracts.Beings
{
    class Paladin : Being
    {
        #region VARIABLES

        #region CONTROLES

        /// <summary>
        /// Accion que se realiza 
        /// </summary>
        private Global.Actions CurrentAction;

        /// <summary>
        /// Accion realizada anteriormente
        /// </summary>
        private Global.Actions OldAction;

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

        // GAB - borrar despues
        List<Piece_Set> pieces_armor_recambio = new List<Piece_Set>();

        /// <summary>
        /// Ancho y alto de un cuadro del sprite
        /// </summary>
        protected int FrameWidth = Global.FrameWidth;
        protected int FrameHeight = Global.FrameHeight;

        #endregion

        #region JUGABILIDAD

        /// <summary>
        /// Tipo de cada pieza de armadura:
        /// Shield, gauntletback, greaveback, helm, breastplate, tasset, greavetop, sword, gauntlettop. 
        /// </summary>
        protected Pieces_Sets pieces_armor = new Pieces_Sets();
        protected List<Piece_Set> pieces_armor_new = new List<Piece_Set>();

        /// <summary>
        /// Velocidad de movimiento del jugador 
        /// </summary>
        protected float PlayerSpeed;

        /// <summary>
        /// Establece tiempo de frame inicial cuando llama al UpdateArmor
        /// El UpdateArmor no ocurre en el loop se pide explicitamente 
        /// </summary>
        protected int FrameTime;

        #endregion

        #region DATOS

        // Mensajes de datos
        protected float mensaje1;
        protected float mensaje2;
        protected Global.Mirada mensaje3;
        protected Global.Actions mensaje4;
        protected float mensaje5;
        protected float mensaje6;
        protected float mensaje7;
        protected float mensaje8;
        protected float mensaje9;

        // Donde se va a alojar el mensaje de chequeo de status
        Vector2 mensaje;

        #endregion

        #endregion

        #region METODOS

        #region GET-SET

        public Global.Actions currentAction
        {
            get { return CurrentAction; }
            set { CurrentAction = value; }
        }

        public Global.Actions oldAction
        {
            get { return OldAction; }
            set { OldAction = value; }
        }

        public Animation[] pieces_anim
        {
            get { return Pieces_Anim; }
            set { Pieces_Anim = value; }
        }
        
        #endregion

        #region ABSTRACTAS

        /// <summary>
        /// Inicializar al jugador
        /// </summary>
        /// <param name="posicion"></param>
        public override void Initialize(Vector2 posicion)
        {

            // Establezco variables por default para comenzar
            position = posicion;
            mensaje = position;
            PlayerSpeed = 3.0f;
            direction = Global.Mirada.RIGHT;
            currentAction = Global.Actions.STAND;
            oldAction = currentAction;
            FrameTime = 50;

            // La maxima vida que puede tener el personaje
            max_health = 200;
            current_health = max_health;

            // Alcance del ataque
            hitrangeX = 95;
            hitrangeY = 7;

            // Establezco las banderas de dañados
            ResetInjured();

            // Inicializo partes de armadura actual
            pieces_armor.Initialize();

            // Inicializo las piezas de animacion
            for (int i = 0; i < Global.PiecesPaladin.Length; i++)
            {
                Pieces_Anim[i] = new Animation();
                Pieces_Anim[i].Initialize(Global.PiecesPaladin[i]);
            }
            
            // Piezas de la armadura al comenzar
            UpdateArmor(pieces_armor_new);

            animations = Pieces_Anim;

            // Asigno control por default al jugador
            controls[(int)Global.Controls.UP] = Keys.W;
            controls[(int)Global.Controls.DOWN] = Keys.S;
            controls[(int)Global.Controls.LEFT] = Keys.A;
            controls[(int)Global.Controls.RIGHT] = Keys.D;
            controls[(int)Global.Controls.BUTTON_1] = Keys.T;
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
            recambio = new Piece_Set();
            recambio.Initialize("lifebar", "set1");
            pieces_armor_recambio.Add(recambio);
            #endregion
        }

        /// <summary>
        /// Actualizar animacion
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            #region CAMBIO_ARMADURA_MANUAL

            // Para cambiar armadura, solo para probar cosas
            // funciona pero cambia muy rapido al apretar
            if ((Keyboard.GetState().IsKeyDown(Keys.D9)))
            {
                if (pieces_armor_recambio[9].set == "set1")
                    pieces_armor_recambio[9].set = "set2";
                else
                    pieces_armor_recambio[9].set = "set1";

                UpdateArmor(pieces_armor_recambio);
            }
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

            #endregion

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
        }

        /// <summary>
        /// Dibujar Jugador
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
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
        /// Actualizar cosas del jugador antes del update general. Aca va todo lo que es la logica del mismo, saltar pegar, etc.
        /// </summary>
        /// <param name="gameTime">El gametime del juego.</param>
        /// <param name="LimitesPantalla">Los limites que puso la camara con respecto a la pantalla que vemos.</param>
        /// <param name="AltoNivel">La altura total del escenario.</param>
        /// <param name="AnchoNivel">El ancho total del escenario.</param>
        public override void UpdatePlayer(GameTime gameTime, int AltoNivel, int AnchoNivel)
        {

            Update(gameTime);

            // Obtengo teclas presionadas
            Global.currentKeyboardState = Keyboard.GetState();

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
            // Hago este if porque al principio apenas empieza esta todo en 0 y no deja poner posiciones randoms
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
                        textura.action == currentAction.ToString().ToLower())
                    {
                        piezaAnimation.LoadTexture(textura);
                    }
                }
            }

            // Vuelve a 0 el frame de la animacion si cambio de accion
            if (oldAction != currentAction)
            {
                foreach (Animation Animation in Pieces_Anim)
                {
                    Animation.CurrentFrame = 0;
                }

                oldAction = currentAction;
            }

            #endregion

            // Status del personaje
            mensaje1 = GetCurrentFrame();
            mensaje2 = GetTotalFrames();
            mensaje3 = direction;
            mensaje4 = currentAction;
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
                        textura.action == currentAction.ToString().ToLower())
                    {
                        piezaAnimation.LoadTexture(textura, position, FrameWidth, FrameHeight, FrameTime, Color.White, true);
                    }
                }
            }

        }

        /// <summary>
        /// Obtiene la posicion del jugador relativa a la parte superior izquierda de la pantalla
        /// </summary>
        /// <returns> Posicion del jugador </returns>
        public override Vector2 GetPositionVec()
        {
            return position;
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

        #endregion

        #region PROPIOS

        public Paladin()
        {
            machine = false;
        }

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
        /// Logica de todas las acciones, los movimientos, los golpes, etc.
        /// </summary>
        private void ActionLogic()
        {
            if (currentAction != Global.Actions.HIT1 &&
                currentAction != Global.Actions.HIT2 &&
                currentAction != Global.Actions.HIT3)
            {

                #region MOVIMIENTO

                // Si no se toca nada quedara por default que esta parado
                currentAction = Global.Actions.STAND;

                // Si se presiona alguna tecla de movimiento
                if (Global.currentKeyboardState.IsKeyDown(controls[(int)Global.Controls.LEFT]))
                {
                    positionX -= PlayerSpeed;
                    direction = Global.Mirada.LEFT;
                    currentAction = Global.Actions.WALK;
                }
                else if (Global.currentKeyboardState.IsKeyDown(controls[(int)Global.Controls.RIGHT]))
                {
                    positionX += PlayerSpeed;
                    direction = Global.Mirada.RIGHT;
                    currentAction = Global.Actions.WALK;
                }

                if (Global.currentKeyboardState.IsKeyDown(controls[(int)Global.Controls.UP]))
                {
                    positionY -= PlayerSpeed;
                    currentAction = Global.Actions.WALK;
                }
                else if (Global.currentKeyboardState.IsKeyDown(controls[(int)Global.Controls.DOWN]))
                {
                    positionY += PlayerSpeed;
                    currentAction = Global.Actions.WALK;
                }

                #endregion


                #region GOLPE

                // Si presiono golpear cancela todas las demas acciones hasta que esta termine su ciclo
                // Tambien genera un rango de los 3 diferentes tipos de golpes (algo netamente visual sin impacto en el juego)
                if (Global.currentKeyboardState.IsKeyDown(controls[(int)Global.Controls.BUTTON_1]))
                {
                    // La aleatoriedad en los golpes depende de como estan almacenados en las variables Global, la primer variable es incluyente y la segunda excluyente.
                    currentAction = (Global.Actions)Global.randomly.Next(2, 5);

                }
                #endregion
            }
            else
            {
                // Si esta pegando tiene que terminar su animacion y despues desbloquear otra vez la gama de movimientos
                // Para esto comparamos el frame actual de la animacion con su frame total
                if (GetCurrentFrame() == GetTotalFrames())
                {
                    currentAction = Global.Actions.STAND;

                    // Cuando termine la animacion de pegar puede generar daño de vuelta a alguien que ya haya atacado
                    ResetInjured();
                }
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
                            Global.players[i].ColorAnimationChange(Color.Red);
                            Global.players[i].injured_value = 10;
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
        
        #endregion

        #endregion
    }
}
