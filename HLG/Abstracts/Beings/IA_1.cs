using System.Collections.Generic;
using HLG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace HLG.Abstracts.Beings
{
    /// <summary>
    /// Primer IA:
    /// ----------
    /// 
    /// - Los sprites son esqueletos
    /// - No esta implementado para que obtenga su comportamiento de archivos externos, tipo XML
    /// - El movimiento de busqueda de targets es correcto pero tiene un pequeño flickering cuando 
    ///   llega a destino (posible solución utilizar float en vez de int para todos los calculos de movimientos)
    ///   Esto ocurre si sacamos la habilidad de golpear solamente, ya que se la pasa buscando el lugar donde estar.
    /// - Hacerla menos agresiva, ataca muy rapido
    /// - No estan pegando los 8 al mismo tiempo, sino que hace de a 5 como mucho
    /// </summary>
    class IA_1 : IA_Characters
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
        /// Ancho y alto de un cuadro del sprite, el tamaño que esta en la carpeta, el fisico.
        /// </summary>
        //protected int FrameWidth = Global.FrameWidth;
        //protected int FrameHeight = Global.FrameHeight;
        protected int FrameWidth = 400;
        protected int FrameHeight = 300;

        // Colores de las piezas por default
        private Color[] defaultColors = new Color[Global.PiecesIA_1.Length];
        
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
            position = posicion;
            PlayerSpeed = 1.5f;
            direction = Global.Mirada.RIGHT;
            currentAction = Global.Actions.STAND;
            oldAction = currentAction;
            FrameTime = 50;

            // Alcance del ataque
            hitrangeX = 50;
            hitrangeY = 2;

            // La maxima vida que puede tener el personaje
            max_health = 10;
            current_health = max_health;

            // Establezco las banderas de dañados
            ResetInjured();

            // Seteo IA
            // machine = true;

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

            // Genero colores al azar en cada pieza del personaje
            // Mas tarde usaremos DefaultRandomPiecesColors() para repintar con estos colores obtenidos luego de alguna modificacion
            GenerateRandomPiecesColors();
            
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
                piezaAnimada.position = position;
                piezaAnimada.Update(gameTime);
            }

            // Fijarse donde va bien este chequeo, en este lugar parece funcionar bien - GAB
            if (current_health > max_health)
            {
                current_health = max_health;
            }
        }

        /// <summary>
        /// Dibujar Jugador
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void DrawWithParallax(SpriteBatch spriteBatch)
        {
            foreach (Animation piezaAnimada in Pieces_Anim)
            {
                piezaAnimada.Draw(spriteBatch, direction, piezaAnimada.color);
            }
        }

        public override void DrawWithoutParallax(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
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
        public override void UpdatePlayer(GameTime gameTime, int AltoNivel, int AnchoNivel)
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
        /// Constructor de la clase, antes de llamar al Initialize
        /// </summary>
        public IA_1()
        {
            // machine = true;
        }

        /// <summary>
        /// Establece el tiempo de frame en ejecucion
        /// </summary>
        /// <param name="Tiempo">El tiempo que va a durar el frame en pantalla de las distintas animaciones del personaje</param>
        private void FrameSpeed(int Tiempo)
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
        private void PauseAnimation(bool desactivar)
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
            // Busca blanco para golpear segun los criterios dados
            Being target = GetTarget(TargetCond);

            if (currentAction != Global.Actions.HIT1 &&
                currentAction != Global.Actions.HIT2 &&
                currentAction != Global.Actions.HIT3)
            {
                #region MOVIMIENTO

                GoToTarget(target);

                #endregion

                #region GOLPEAR

                // Si el blanco esta dentro del rango de golpe se lo ataca
                if (CollisionVerifier(target.GetPositionRec()))
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
        /// Logica de movimiento hacia el objetivo adquirido
        /// Dirigirse al blanco, dependiendo de donde esta el eje del blanco vamos a sumarle la velocidad hacia el.
        /// Tambien se toma el punto que va a buscar para atacar a cierto personaja.
        /// Para obtener el lugar antes mencionado usamos la variable de HitRange asi se posiciona optimamente para su ataque.
        /// El HitRangeX tiene que ser mayor para que no hostigue tanto al blanco, sino se pega mucho a el
        /// Uno de los 2 tenia que tener el igual (=) asi no habia un punto en el que se queda quieto el esqueleto, en este caso
        /// es el primer check, el segundo ya no lo tiene
        /// </summary>
        /// <param name="target">Objetivo adquirido</param>
        private void GoToTarget(Being target)
        {
            // Si no esta en movimiento por default queda parado
            //currentAction = Global.Actions.STAND;
            
            if (GetPositionRec().Center.X <= target.GetPositionRec().Center.X)
            {
                // Izquierda
                if (GetPositionRec().Center.X >= target.GetPositionRec().Center.X - hitrangeX)
                {
                    positionX -= PlayerSpeed;
                }
                else
                {
                    positionX += PlayerSpeed;
                }

                direction = Global.Mirada.RIGHT;
                //currentAction = Global.Actions.WALK;
            }
            else if (GetPositionRec().Center.X > target.GetPositionRec().Center.X)
            {
                // Derecha
                if (GetPositionRec().Center.X <= target.GetPositionRec().Center.X + hitrangeX)
                {
                    positionX += PlayerSpeed;
                }
                else
                {
                    positionX -= PlayerSpeed;
                }

                direction = Global.Mirada.LEFT;
                //currentAction = Global.Actions.WALK;
            }

            //if (target.GetPositionRec().Center.Y < GetPositionRec().Center.Y - hitrangeY)
            if (target.GetPositionRec().Center.Y <= GetPositionRec().Center.Y - hitrangeY)
            {
                // Arriba
                positionY -= PlayerSpeed;
                //currentAction = Global.Actions.WALK;
            }
            else if (target.GetPositionRec().Center.Y > GetPositionRec().Center.Y + hitrangeY)
            {
                // Abajo
                positionY += PlayerSpeed;
                //currentAction = Global.Actions.WALK;
            }

            currentAction = Global.Actions.WALK;
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
                    !ghost_mode &&
                    GetCurrentFrame() == 5)
            {

                for (int i = 0; i < Global.totalQuant; i++)
                {
                    // Ver sumamry
                    if (Global.players[i].index != -1 &&
                        !injured[i] &&
                        !Global.players[i].ghost_mode)
                    {

                        /// Si esta dentro del radio del golpe se calculan los daños, 
                        /// se usa en CollisionVerifierEnhanced porque tiene un arreglo que agranda el rango
                        /// lamentablemente no pude reconocer por que hay un desfasaje de pixeles
                        /// podría ser el tema de la camara y tendría que ver si utilizando el revert matrix de la misma se puede solucionar
                        if (CollisionVerifierEnhanced(Global.players[i].GetPositionRec()))
                        {
                            // Cuando la armadura esta detras del efecto de la espada no se puede ver bien el cambio de color
                            // Global.players[i] ColorAnimationChange(Color.Red);
                            // Se le hace una suma para que sea acumulativo el daño de todos, sino siempre era 10 y no se sumaba
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
                {
                    DefaultRandomPiecesColors();
                }
                else
                {
                    ColorAnimationChange(Color.Red);
                }

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
                // Lo manejo con el ghost a la IA tb asi no tengo que cambiar todo lo que esta hecho con los Being.
                // De esta manera es mas facil porque las corroboraciones del ghost_mode siguen corriendo, 
                // entonces si la IA entra en modo ghost (muere) se desactiva su animacion
                ActivatePlayer(false);
            }
        }

        /// <summary>
        /// Genero colores al azar en cada pieza del personaje
        /// </summary>
        private void GenerateRandomPiecesColors()
        {
            for (int i = 0; i < Global.PiecesIA_1.Length; i++)
            {
                defaultColors[i] = Global.SkeletonRandomColors[Global.randomly.Next(0, Global.SkeletonRandomColors.Length)];
                ColorPieceChange(defaultColors[i], i);
            }
        }

        /// <summary>
        /// Vuelvo a pintar del color que se genero en un principio con GenerateRandomPiecesColors()
        /// </summary>
        private void DefaultRandomPiecesColors()
        {
            for (int i = 0; i < Global.PiecesIA_1.Length; i++)
            {
                ColorPieceChange(defaultColors[i], i);
            }
        }

        /// <summary>
        /// Obtiene condiciones al azar, se hace en inicializar para que se haga una sola vez en la creacion del personaje
        /// </summary>
        private void GetCondition()
        {
            /// Me muevo en el rango de la cantidad de condiciones que existen en generales
            TargetCond = (Global.TargetCondition)Global.randomly.Next(0, Enum.GetNames(typeof(Global.TargetCondition)).Length);
        }

        /// <summary>
        /// Setea un objetivo segun los criterios de busqueda que se obtuvieron de GetCondition() en Initialize.
        /// Se hace en cada vuelta logica ya que recalcula los parametros por si hay que cambiar de blanco bajo los mismos criterios.
        /// </summary>
        /// <returns></returns>
        private Being GetTarget(Global.TargetCondition Condition)
        {
            /// Si estan todos muertos van por default al jugador 0, no es necesario que devuelva null. El cambio de target se hace solo al
            /// recalcular si la vida del target actual es igual o menor a 0
            switch (Condition)
            {
                /// MAX HEALTH: No mata a nadie pero lastima siempre al mas fuerte, un equilibrador.
                #region MAX HEALTH
                case Global.TargetCondition.MAXHEALTH:
                    {
                        int healthTemp = 0;
                        int playerMaxHealth = 0;

                        for (int i = 0; i < Global.playersQuant; i++)
                        {
                            if (Global.players[i].current_health >= healthTemp && Global.players[i].current_health > 0)
                            {
                                healthTemp = Global.players[i].current_health;
                                playerMaxHealth = i;
                            }
                        }

                        return Global.players[playerMaxHealth];
                    }
                #endregion

                /// MIN HEALTH: Es un finisher, va a tratar de matar a los mas débiles.
                #region MIN HEALTH
                case Global.TargetCondition.MINHEALTH:
                    {
                        int healthTemp = 5000;
                        int playerMinHealth = 0;

                        for (int i = 0; i < Global.playersQuant; i++)
                        {
                            if (Global.players[i].current_health <= healthTemp && Global.players[i].current_health > 0)
                            {
                                healthTemp = Global.players[i].current_health;
                                playerMinHealth = i;
                            }
                        }
                        
                        return Global.players[playerMinHealth];
                    }
                #endregion

                /*case Global.TargetCondition.MAXMONEY:
                    {
                        return Global.players[2];
                    }

                case Global.TargetCondition.MINMONEY:
                    {
                        return Global.players[3];
                    }*/

                default: return Global.players[0];

            }
        }
                
        #endregion

        #endregion
    }
}
