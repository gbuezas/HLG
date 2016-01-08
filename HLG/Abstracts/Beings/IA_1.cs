using System.Collections.Generic;
using HLG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HLG.Abstracts.Beings
{
    class IA_1 : Being
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
        /// Animacion de cada pieza de armadura:
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
        private Animation[] Pieces_Anim = new Animation[Global.PiecesIA_1.Length];

        /// <summary>
        /// Posicion del jugador relativa a la parte superior izquierda de la pantalla.
        /// Esta posicion marca donde se encuentra el jugador en la pantalla y no en el mapa donde se esta moviendo,
        /// y es a esta posicion a la que se le aplican los limites de la pantalla.  
        /// </summary>
        protected Vector2 Position;

        /// <summary>
        /// Ancho y alto de un cuadro del sprite, el tamaño que esta en la carpeta, el fisico.
        /// </summary>
        //protected int FrameWidth = Global.FrameWidth;
        //protected int FrameHeight = Global.FrameHeight;
        protected int FrameWidth = 400;
        protected int FrameHeight = 300;

        #endregion

        #region JUGABILIDAD

        /// <summary>
        /// Tipo de cada pieza de armadura:
        /// Gauntletback, greaveback, helm, breastplate, tasset, greavetop, gauntlettop. 
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

        /// <summary>
        /// Parametro de busqueda de objetivo a atacar
        /// </summary>
        protected Global.TargetCondition TargetCond;

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
            Position = posicion;
            PlayerSpeed = 3.0f;
            direction = Global.Mirada.RIGHT;
            currentAction = Global.Actions.STAND;
            oldAction = currentAction;
            FrameTime = 50;
            health -= 70;

            // Establezco las banderas de dañados
            ResetInjured();

            // Seteo IA
            machine = true;

            // Inicializo partes de armadura actual
            pieces_armor.Initialize();

            // Inicializo las piezas de animacion
            for (int i = 0; i < Global.PiecesIA_1.Length; i++)
            {
                Pieces_Anim[i] = new Animation();
                Pieces_Anim[i].Initialize(Global.PiecesIA_1[i]);
            }

            // Piezas de la armadura al comenzar
            UpdateArmor(pieces_armor_new);

            animations = Pieces_Anim;

            // Seteo condicion de busqueda de objetivo para atacar
            GetCondition();

            // Ralentizar los cuadros por segundo del personaje
            // TiempoFrameEjecucion(1);

            //this.ActivatePlayer(true);
        }

        /// <summary>
        /// Actualizar animacion
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            foreach (Animation piezaAnimada in Pieces_Anim)
            {
                piezaAnimada.position = Position;
                piezaAnimada.Update(gameTime);
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
        }

        /// <summary>
        /// Actualizar cosas del jugador antes del update general. Aca va todo lo que es la logica del mismo, saltar pegar, etc.
        /// </summary>
        /// <param name="gameTime">El gametime del juego.</param>
        /// <param name="currentKeyboardState">Para el teclado.</param>
        /// <param name="currentGamePadState">Para los gamepad.</param>
        /// <param name="LimitesPantalla">Los limites que puso la camara con respecto a la pantalla que vemos.</param>
        /// <param name="AltoNivel">La altura total del escenario.</param>
        /// <param name="AnchoNivel">El ancho total del escenario.</param>
        public override void UpdatePlayer(GameTime gameTime, Rectangle LimitesPantalla, int AltoNivel, int AnchoNivel)
        {

            Update(gameTime);

            // Logica de las acciones, moverse, pegar, etc
            ActionLogic();

            // Logica de las colisiones al golpear
            CollisionLogic();

            // Aqui aplicamos los daños y todo lo correspondiente a los efectos de las acciones hechas anteriormente
            EffectLogic();

            // No es necesario mas acomodar la fila ya que todos vienen con fila 0
            // Solo se acomoda la cantidad de frames por animacion y que animacion va en cada pieza segun la accion ejecutandose.
            #region ANIMACION POR PIEZA

            foreach (Animation piezaAnimation in Pieces_Anim)
            {
                foreach (Textures textura in Global.IA_BasicTextures)
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

        }

        /// <summary>
        /// Cargo los set de armadura que corresponden a cada pieza del cuerpo.
        /// </summary>
        /// <param name="set_pieces">Set de shield, gauntlets, greaves, helm, breastplate, tasset, sword respectivamente</param> 
        public override void UpdateArmor(List<Piece_Set> set_pieces)
        {
            // Gauntletback, greaveback, helm, breastplate, tasset, greavetop, gauntlettop. 
            foreach (Piece_Set set_piece in set_pieces)
            {
                pieces_armor.Set_Set(set_piece);
            }

            foreach (Animation piezaAnimation in Pieces_Anim)
            {
                foreach (Textures textura in Global.IA_BasicTextures)
                {
                    if (textura.piece == piezaAnimation.pieceName &&
                        textura.set == pieces_armor.Get_Set(textura.piece) &&
                        textura.action == currentAction.ToString().ToLower())
                    {
                        piezaAnimation.LoadTexture(textura, Position, FrameWidth, FrameHeight, FrameTime, Color.White, true);
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
            return Position;
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
        /// Devuelve el valor con -1 porque empieza a contar desde 0
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
            // Busca blanco ara golpear segun los criterios dados
            Being target = GetTarget(TargetCond);

            if (currentAction != Global.Actions.HIT1 &&
                currentAction != Global.Actions.HIT2 &&
                currentAction != Global.Actions.HIT3)
            {

                #region MOVIMIENTO

                // Si no esta en movimiento por default queda parado
                currentAction = Global.Actions.STAND;

                // Dirigirse al blanco, dependiendo de donde esta ele eje del blanco vamos a sumarle la velocidad hacia el.
                // Tambien se toma el lugar donde la IA va a detenerse y el punto que va a buscar para atacar a cierto personaja.
                // Para obtener el lugar antes mencionado usamos la variable de HitRange asi se posiciona optimamente para su ataque.
                // El HitRangeX tiene que ser mayor para que no hostigue tanto al blanco, sino se pega mucho a el
                if (target.GetPositionVec().X <= Position.X - Global.IA1HitRangeX)
                {
                    // Izquierda
                    Position.X -= PlayerSpeed;
                    direction = Global.Mirada.LEFT;
                    currentAction = Global.Actions.WALK;
                }
                else if (target.GetPositionVec().X >= Position.X + Global.IA1HitRangeX)
                {
                    // Derecha
                    Position.X += PlayerSpeed;
                    direction = Global.Mirada.RIGHT;
                    currentAction = Global.Actions.WALK;
                }

                if (target.GetPositionVec().Y <= Position.Y - Global.IA1HitRangeY)
                {
                    // Arriba
                    Position.Y -= PlayerSpeed;
                    currentAction = Global.Actions.WALK;
                }
                else if (target.GetPositionVec().Y >= Position.Y + Global.IA1HitRangeY)
                {
                    // Abajo
                    Position.Y += PlayerSpeed;
                    currentAction = Global.Actions.WALK;
                }

                #endregion

                #region GOLPEAR

                // Obtengo las posiciones del blanco y nuestra
                Rectangle temp = GetPositionRec();
                Rectangle temp2 = target.GetPositionRec();

                // Si el blanco esta dentro del rango de golpe se lo ataca
                if (CollisionVerifier(ref temp, ref temp2))
                {
                    // El rango depende de como estan almacenados en las variables Global, la primer variable es incluyente y la segunda excluyente.
                    currentAction = (Global.Actions)Global.randomly.Next(2, 5);
                }

                #endregion
            }
            else
            {
                // Si esta pegando tiene que terminar su animacion y despues desbloquear otra vez la gama de movimientos
                // Para esto comparamos el frame actual de la animacion con su frame
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
        ///     - Si el frame de la animacion no es justo cuando golpea con la espada se saltea.
        ///     - Si fue golpeado anteriormente se saltea
        ///     - Si es fantasma se saltea
        ///     - Si es IA se saltea
        /// </summary>
        private void CollisionLogic()
        {
            if ((currentAction == Global.Actions.HIT1 ||
                    currentAction == Global.Actions.HIT2 ||
                    currentAction == Global.Actions.HIT3) &&
                    !ghost_mode)
            {

                for (int i = 0; i < Global.totalQuant; i++)
                {
                    // Ver sumamry
                    if (!Global.players[i].machine &&
                        GetCurrentFrame() == 5 &&
                        !injured[i] &&
                        !Global.players[i].ghost_mode)
                    {
                        Rectangle temp = GetPositionRec();
                        Rectangle temp2 = Global.players[i].GetPositionRec();

                        // Si esta dentro del radio del golpe
                        if (CollisionVerifier(ref temp, ref temp2))
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
                health -= injured_value;

                // Vuelvo el contador de daño a 0 y quito que este dañado
                injured_value = 0;

                // Si pierde toda su HP se vuelve fantasma
                if (health <= 0)
                {
                    ghost_mode = true;
                }
            }
            else
            {
                // Lo manejo con el ghost a la IA tb asi no tengo que cambiar todo lo que esta hecho con los Being.
                // De esta manera es mas facil porque las corroboraciones del ghost_mode siguen corriendo, 
                // nada mas que no se dibujan las animaciones de la IA porque estan desactivadas
                ActivatePlayer(false);
            }
        }

        /// <summary>
        /// Chequea las colisiones
        /// </summary>
        /// <param name="temp">Rectangulo del primer elemento</param>
        /// <param name="temp2">Rectangulo del segundo elemento</param>
        /// <returns></returns>
        private bool CollisionVerifier(ref Rectangle temp, ref Rectangle temp2)
        {
            return (temp.X + temp.Width >= temp2.Center.X - Global.IA1HitRangeX &&
                    temp.X <= temp2.X &&
                    temp.Y >= temp2.Y - Global.IA1HitRangeY &&
                    temp.Y <= temp2.Y + Global.IA1HitRangeY &&
                    direction == Global.Mirada.RIGHT)
                    ||
                   (temp.X <= temp2.Center.X + Global.IA1HitRangeX &&
                    temp.X + temp.Width >= temp2.X + temp2.Width &&
                    temp.Y >= temp2.Y - Global.IA1HitRangeY &&
                    temp.Y <= temp2.Y + Global.IA1HitRangeY &&
                    direction == Global.Mirada.LEFT);
        }

        /// <summary>
        /// Obtiene condiciones al azar, se hace en inicializar para que se haga una sola vez en la creacion del personaje
        /// </summary>
        private void GetCondition()
        {
            TargetCond = (Global.TargetCondition)Global.randomly.Next(0, 4);
        }

        /// <summary>
        /// Setea un objetivo segun los criterios de busqueda que se obtuvieron de GetCondition() en Initialize.
        /// Se hace en cada vuelta logica ya que recalcula los parametros por si hay que cambiar de blanco bajo los mismos criterios.
        /// </summary>
        /// <returns></returns>
        private Being GetTarget(Global.TargetCondition Condition)
        {
            switch (Condition)
            {

                case Global.TargetCondition.MAXHEALTH:
                    {
                        int vida = 0;
                        // GAB - Tiene que buscar un target nuevo o quedarse quieto cuando mata al que era su target
                        // Si le pongo -1 hace el efecto de querer cambiar de target pero si dejo -1 tira out of index
                        int playerMaxHealth = 0;

                        for (int i = 0; i < Global.playersQuant; i++)
                        {
                            if (Global.players[i].health > vida && Global.players[i].health > 0)
                            {
                                vida = Global.players[i].health;
                                playerMaxHealth = i;
                            }
                        }

                        return Global.players[playerMaxHealth];
                    }

                case Global.TargetCondition.MINHEALTH:
                    {
                        int vida = 1000;
                        int playerMinHealth = 0;

                        for (int i = 0; i < Global.playersQuant; i++)
                        {
                            if (Global.players[i].health < vida && Global.players[i].health > 0)
                            {
                                vida = Global.players[i].health;
                                playerMinHealth = i;
                            }
                        }

                        return Global.players[playerMinHealth];
                    }

                case Global.TargetCondition.MAXMONEY:
                    {
                        return Global.players[2];
                    }

                case Global.TargetCondition.MINMONEY:
                    {
                        return Global.players[3];
                    }

                default: break;

            }


            return Global.players[0];
        }
                
        #endregion

        #endregion
    }
}
