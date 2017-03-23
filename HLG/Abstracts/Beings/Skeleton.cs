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
        private Color[] defaultColors = new Color[piecesSkeleton.Length];
        static string[] piecesSkeleton = new string[] { "gauntletback", "greaveback", "breastplate", "helm", "tasset", "greavetop", "gauntlettop" };

        //-//-// METHODS //-//-//
        public override void Initialize(Vector2 posicion)
        {
            animationPieces = new Animation[piecesSkeleton.Length];
            objectTextures = Global.skeleton_textures;

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
            maxHealth = 10;
            currentHealth = maxHealth;

            // Establezco las banderas de dañados
            Reset_Injured();

            piecesArmor.Initialize(piecesSkeleton); // Inicializo partes de armadura actual
            for (int i = 0; i < piecesSkeleton.Length; i++) // Inicializo las piezas de animacion
            {
                animationPieces[i] = new Animation();
                animationPieces[i].Initialize(piecesSkeleton[i]);
            }
            Update_Armor(piecesArmorNew); // Piezas de la armadura al comenzar
            animations = animationPieces;

            // Seteo condicion de busqueda de objetivo para atacar
            GetCondition();

            // Genero colores al azar en cada pieza del personaje
            // Mas tarde usaremos DefaultRandomPiecesColors() para repintar con estos colores obtenidos luego de alguna modificacion
            Generate_Random_Pieces_Colors();
            
            // Ralentizar los cuadros por segundo del personaje
            // TiempoFrameEjecucion(1);

            //this.ActivatePlayer(true);
        }
        public override void Update_Player(GameTime gameTime, int AltoNivel, int AnchoNivel)
        {

            Animation_Frame_Position_Update(gameTime);

            Caps_Max_Health();

            Action_Logic_Automatic();
            Collision_Logic();
            Effect_Logic();

            Texture_Regular_Load();
            Frame_Number_Action_Reset();

        }
        
        /// <summary>
        /// Establece el tiempo de frame en ejecucion
        /// </summary>
        /// <param name="Tiempo">El tiempo que va a durar el frame en pantalla de las distintas animaciones del personaje</param>
        private void Frame_Speed(int Tiempo)
        {
            foreach (Animation piezaAnimada in animationPieces)
            {
                piezaAnimada.frameTime = Tiempo;
            }
        }

        /// <summary>
        /// Logica de todas las acciones, los movimientos, los golpes, etc.
        /// </summary>
        private void Action_Logic_Automatic()
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
                if (Collision_Verifier(target.Get_Position_Rec()))
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
                if (Get_Current_Frame() == Get_Total_Frames())
                {
                    currentAction = Global.Actions.STAND;

                    // Cuando termine la animacion de pegar puede generar daño de vuelta a alguien que ya haya atacado
                    Reset_Injured();
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
        private void Collision_Logic()
        {
            if ((currentAction == Global.Actions.HIT1 ||
                    currentAction == Global.Actions.HIT2 ||
                    currentAction == Global.Actions.HIT3) &&
                    !ghostMode &&
                    Get_Current_Frame() == 5)
            {

                for (int i = 0; i < Global.total_quant; i++)
                {
                    // Ver sumamry
                    if (Global.players[i].index != -1 &&
                        !injuredByMe[i] &&
                        !Global.players[i].ghostMode)
                    {

                        /// Si esta dentro del radio del golpe se calculan los daños, 
                        /// se usa en CollisionVerifierEnhanced porque tiene un arreglo que agranda el rango
                        /// lamentablemente no pude reconocer por que hay un desfasaje de pixeles
                        /// podría ser el tema de la camara y tendría que ver si utilizando el revert matrix de la misma se puede solucionar
                        if (Collision_Verifier_Enhanced(Global.players[i].Get_Position_Rec()))
                        {
                            // Cuando la armadura esta detras del efecto de la espada no se puede ver bien el cambio de color
                            // Global.players[i] ColorAnimationChange(Color.Red);
                            // Se le hace una suma para que sea acumulativo el daño de todos, sino siempre era 10 y no se sumaba
                            Global.players[i].injuredValue += 10;
                            injuredByMe[i] = true;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Lógica de los efectos de las colisiones y movimientos realizados.
        /// </summary>
        private void Effect_Logic()
        {

            if (!ghostMode)
            {
                // Reestablezco su color natural si no va a recibir daño, de esta manera no permito que vuelva a su color 
                // demasiado rapido como para que no se vea que fue dañado
                if (injuredValue == 0)
                {
                    Default_Random_Pieces_Colors();
                }
                else
                {
                    Color_Animation_Change(Color.Red);
                }

                // Hago la resta necesaria a la health
                currentHealth -= injuredValue;

                // Vuelvo el contador de daño a 0 y quito que este dañado
                injuredValue = 0;

                // Si pierde toda su HP se vuelve fantasma
                if (currentHealth <= 0)
                {
                    ghostMode = true;
                }
            }
            else
            {
                // Lo manejo con el ghost a la IA tb asi no tengo que cambiar todo lo que esta hecho con los Being.
                // De esta manera es mas facil porque las corroboraciones del ghost_mode siguen corriendo, 
                // entonces si la IA entra en modo ghost (muere) se desactiva su animacion
                Activate_Player(false);
            }
        }

        private void Generate_Random_Pieces_Colors()
        {
            for (int i = 0; i < piecesSkeleton.Length; i++)
            {
                defaultColors[i] = Global.skeleton_random_colors[Global.randomly.Next(0, Global.skeleton_random_colors.Length)];
                Color_Piece_Change(defaultColors[i], i);
            }
        }
        private void Default_Random_Pieces_Colors()
        {
            for (int i = 0; i < piecesSkeleton.Length; i++)
                Color_Piece_Change(defaultColors[i], i);
        }
        
    }
}
