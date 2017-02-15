using HLG.Objects;
using Microsoft.Xna.Framework;

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
    class Skeleton : IA_Characters
    {

        //-//-// VARIABLES //-//-//
        private Color[] default_colors = new Color[pieces_skeleton.Length];
        static string[] pieces_skeleton = new string[] { "gauntletback", "greaveback", "breastplate", "helm", "tasset", "greavetop", "gauntlettop" };

        //-//-// METHODS //-//-//
        public override void Initialize(Vector2 posicion)
        {
            animation_pieces = new Animation[pieces_skeleton.Length];
            object_textures = Global.skeleton_textures;

            position = posicion;
            playerMoveSpeed = 1.5f;
            currentAction = Global.Actions.STAND;
            oldAction = currentAction;
            frameTime = 50;

            frameWidth = 400;
            frameHeight = 300;

            // Alcance del ataque
            hitRangeX = 50;
            hitRangeY = 2;

            // La maxima vida que puede tener el personaje
            max_health = 10;
            current_health = max_health;

            // Establezco las banderas de dañados
            ResetInjured();

            pieces_armor.Initialize(pieces_skeleton); // Inicializo partes de armadura actual
            for (int i = 0; i < pieces_skeleton.Length; i++) // Inicializo las piezas de animacion
            {
                animation_pieces[i] = new Animation();
                animation_pieces[i].Initialize(pieces_skeleton[i]);
            }
            UpdateArmor(pieces_armor_new); // Piezas de la armadura al comenzar
            animations = animation_pieces;

            // Seteo condicion de busqueda de objetivo para atacar
            GetCondition();

            // Genero colores al azar en cada pieza del personaje
            // Mas tarde usaremos DefaultRandomPiecesColors() para repintar con estos colores obtenidos luego de alguna modificacion
            GenerateRandomPiecesColors();
            
            // Ralentizar los cuadros por segundo del personaje
            // TiempoFrameEjecucion(1);

            //this.ActivatePlayer(true);
        }
        
        public override void UpdatePlayer(GameTime gameTime, int AltoNivel, int AnchoNivel)
        {

            AnimationFramePositionUpdate(gameTime);

            CapsMaxHealth();

            ActionLogicAutomatic();
            CollisionLogic();
            EffectLogic();

            TextureRegularLoad();
            FrameNumberActionReset();

        }
        
        /// <summary>
        /// Establece el tiempo de frame en ejecucion
        /// </summary>
        /// <param name="Tiempo">El tiempo que va a durar el frame en pantalla de las distintas animaciones del personaje</param>
        private void FrameSpeed(int Tiempo)
        {
            foreach (Animation piezaAnimada in animation_pieces)
            {
                piezaAnimada.frameTime = Tiempo;
            }
        }

        /// <summary>
        /// Logica de todas las acciones, los movimientos, los golpes, etc.
        /// </summary>
        private void ActionLogicAutomatic()
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

                for (int i = 0; i < Global.total_quant; i++)
                {
                    // Ver sumamry
                    if (Global.players[i].index != -1 &&
                        !injuredByMe[i] &&
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

        private void GenerateRandomPiecesColors()
        {
            for (int i = 0; i < pieces_skeleton.Length; i++)
            {
                default_colors[i] = Global.skeleton_random_colors[Global.randomly.Next(0, Global.skeleton_random_colors.Length)];
                ColorPieceChange(default_colors[i], i);
            }
        }
        private void DefaultRandomPiecesColors()
        {
            for (int i = 0; i < pieces_skeleton.Length; i++)
                ColorPieceChange(default_colors[i], i);
        }
        
    }
}
