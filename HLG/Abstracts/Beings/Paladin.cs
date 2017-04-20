using System.Collections.Generic;
using HLG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HLG.Abstracts.Beings
{
    class Paladin : Playable_Characters
    {
        
        //-//-// VARIABLES //-//-//
        List<Piece_Set> piecesArmorRecambio = new List<Piece_Set>(); // GAB - borrar despues, es para mostrar el cambio de armadura
        static string[] piecesPaladin = new string[] { "shield", "gauntletback", "greaveback", "breastplate", "helm", "tasset", "greavetop", "sword", "gauntlettop" };
        static string[] piecesInventory = new string[] { "greaveback", "gauntletback", "breastplate", "helm", "shield", "sword", "potion", "throw" }; // Este numero tendría que sacarse de los frames que tiene la skin del inventorio correspondiente al personaje
        //List<string> sets = new List<string>(); 

        //-//-// METHODS //-//-//
        public override void Initialize(Vector2 posicion)
        {
            animationPieces = new Animation[piecesPaladin.Length];
            objectTextures = Global.paladinTextures;

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

            maxHealth = 200;
            currentHealth = maxHealth;
            
            ResetInjured();

            piecesArmor.Initialize(piecesPaladin); // Inicializo partes de armadura actual
            for (int i = 0; i < piecesPaladin.Length; i++) // Inicializo las piezas de animacion
            {
                animationPieces[i] = new Animation();
                animationPieces[i].Initialize(piecesPaladin[i]);
            }
            UpdateArmor(piecesArmorNew); // Piezas de la armadura al comenzar
            animations = animationPieces;

            //string nombresettextura = string.Empty;
            //foreach ( Textures texture in Global.paladin_textures )
            //{
            //    if (texture.texture_set_name != nombresettextura)
            //    {
            //        sets.Add(texture.texture_set_name);
            //        nombresettextura = texture.texture_set_name;
            //    }
            //}

            //gui.initialize(index, pieces_inventory, sets);
            //gui.initialize(index, pieces_inventory, TheGame.allSetsNames);
            gui.Initialize(index, piecesInventory);

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
            piecesArmorRecambio.Add(recambio);
            recambio = new Piece_Set();
            recambio.Initialize("gauntletback", "set1");
            piecesArmorRecambio.Add(recambio);
            recambio = new Piece_Set();
            recambio.Initialize("greaveback", "set1");
            piecesArmorRecambio.Add(recambio);
            recambio = new Piece_Set();
            recambio.Initialize("breastplate", "set1");
            piecesArmorRecambio.Add(recambio);
            recambio = new Piece_Set();
            recambio.Initialize("helm", "set1");
            piecesArmorRecambio.Add(recambio);
            recambio = new Piece_Set();
            recambio.Initialize("tasset", "set1");
            piecesArmorRecambio.Add(recambio);
            recambio = new Piece_Set();
            recambio.Initialize("greavetop", "set1");
            piecesArmorRecambio.Add(recambio);
            recambio = new Piece_Set();
            recambio.Initialize("sword", "set1");
            piecesArmorRecambio.Add(recambio);
            recambio = new Piece_Set();
            recambio.Initialize("gauntlettop", "set1");
            piecesArmorRecambio.Add(recambio);
            
            #endregion

        }
        
        public override void UpdatePlayer(GameTime gameTime, int AltoNivel, int AnchoNivel)
        {
            ManualArmorChange();

            AnimationFramePositionUpdate(gameTime);

            CapsMaxHealth();

            // Para los stats de cada personaje (borrar mas tarde) GAB
            mensaje = position;
            
            gui.UpdateGUI(gameTime, currentHealth, maxHealth);
            
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
            foreach (Animation animationPiecesItem in animationPieces)
            {
                animationPiecesItem.frameTime = Tiempo;
            }
        }
        
        private void ManualArmorChange() // Tendria que eliminarse con la beta cerrada
        {
            //if ((Keyboard.GetState().IsKeyDown(Keys.Q)))
            if (Global.OnePulseKey(Keys.Q)) 
            {
                var guitempitem = gui.GetCurrentItem();

                foreach (var temporal in piecesArmorRecambio)
                {
                    /*Algo esta pasando que se genera el cambio con las flechas, si tocar la Q, para mi es que se cambia el current y eso genera un cambio
                     manual aunque no tendria que cambiar sin hacer la llamada al manual, fijarse de poner breakpoint en el manual y ver si 
                     entra cuando tocamos las flechas solamente*/

                    /*No pasa por la parte de la Q con la flecha, lo esta haciendo de afuera*/
                    if (temporal.piece.Substring(0,4) == guitempitem.loadedTexture.texturePieceName.Substring(0,4))
                    {
                        temporal.set = guitempitem.loadedTexture.textureSetName;

                        // si cambia un brazo cambiar los 2 y lo mismo para las piernas
                        //if (temporal.piece == )
                        //{

                        //}

                        UpdateArmor(piecesArmorRecambio);
                    }
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
                 !ghostMode &&
                 GetCurrentFrame() == 5)
            {

                for (int i = 0; i < Global.totalQuant; i++)
                {
                    // Ver summary
                    if (!injuredByMe[i] &&
                        Global.players[i] != this &&
                        !Global.players[i].ghostMode)
                    {
                        
                        // Si esta dentro del radio del golpe
                        if (CollisionVerifier(Global.players[i].GetPositionRec()))
                        {
                            // Cuando la armadura esta detras del efecto de la espada no se puede ver bien el cambio de color
                            // Le sumamos el resultado para que sea acumulativo si varios golpean al mismo objetivo
                            //Global.players[i].ColorAnimationChange(Color.Red);
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
        private void EffectLogic()
        {

            if (!ghostMode)
            {
                // Reestablezco su color natural si no va a recibir daño, de esta manera no permito que vuelva a su color 
                // demasiado rapido como para que no se vea que fue dañado
                if (injuredValue == 0)
                    ColorAnimationChange(Color.White);
                else
                    ColorAnimationChange(Color.Red);

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
                ColorAnimationChange(Global.colorGhost);

                if (currentHealth > 0)
                {
                    ghostMode = false;
                }
            }

            // MENSAJES: Veo la health de los personajes
            mensaje9 = currentHealth;
        }
        
    }
}
