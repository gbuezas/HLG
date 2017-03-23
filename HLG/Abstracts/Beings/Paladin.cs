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
        static string[] pieces_inventory = new string[] { "greaveback", "gauntletback", "breastplate", "helm", "shield", "sword", "potion", "throw" }; // Este numero tendría que sacarse de los frames que tiene la skin del inventorio correspondiente al personaje
        List<string> sets = new List<string>(); 

        //-//-// METHODS //-//-//
        public override void Initialize(Vector2 posicion)
        {
            animationPieces = new Animation[pieces_paladin.Length];
            objectTextures = Global.paladin_textures;

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
            
            Reset_Injured();

            piecesArmor.Initialize(pieces_paladin); // Inicializo partes de armadura actual
            for (int i = 0; i < pieces_paladin.Length; i++) // Inicializo las piezas de animacion
            {
                animationPieces[i] = new Animation();
                animationPieces[i].Initialize(pieces_paladin[i]);
            }
            Update_Armor(piecesArmorNew); // Piezas de la armadura al comenzar
            animations = animationPieces;

            string nombresettextura = string.Empty;
            foreach ( Textures texture in Global.paladin_textures )
            {
                if (texture.texture_set_name != nombresettextura)
                {
                    sets.Add(texture.texture_set_name);
                    nombresettextura = texture.texture_set_name;
                }
            }

            gui.initialize(index, pieces_inventory, sets);
            
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
        
        public override void Update_Player(GameTime gameTime, int AltoNivel, int AnchoNivel)
        {
            ManualArmorChange();

            Animation_Frame_Position_Update(gameTime);

            Caps_Max_Health();

            // Para los stats de cada personaje (borrar mas tarde) GAB
            mensaje = position;
            
            gui.UpdateGUI(gameTime, currentHealth, maxHealth);
            
            Action_Logic_Manual();
            CollisionLogic();
            EffectLogic();

            /// Hace que el jugador no salga de la pantalla reacomodandolo dentro de la misma.
            /// Tomamos como pantalla el rectangulo que genera la camara para acomodar al jugador y limitamos de acuerdo a estas medidas.
            /// El FrameEscalado es para acomodar al personaje de acuerdo a la nueva escala adquirida dependiendo 
            /// de la pantalla fisica donde se ejecuta el juego.
            StayInScreen(AltoNivel);

            /// Carga texturas y acomoda los frames al cambiar de accion
            Texture_Regular_Load();
            Frame_Number_Action_Reset();
            
            // Status del personaje
            mensaje1 = Get_Current_Frame();
            mensaje2 = Get_Total_Frames();
            mensaje3 = facing;
            mensaje4 = currentAction;
            //mensaje5 = Global.FrameHeight;
            //mensaje6 = Global.FrameWidth;
            mensaje7 = Get_Position_Vec().X;
            mensaje8 = Get_Position_Vec().Y;
        }
        
        /// <summary>
        /// Establece el tiempo de frame en ejecucion
        /// </summary>
        /// <param name="Tiempo">El tiempo que va a durar el frame en pantalla de las distintas animaciones del personaje</param>
        void FrameSpeed(int Tiempo)
        {
            foreach (Animation piezaAnimada in animationPieces)
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

                Update_Armor(pieces_armor_recambio);
            }

            if ((Keyboard.GetState().IsKeyDown(Keys.D7)))
            {
                if (pieces_armor_recambio[0].set == "set1")
                    pieces_armor_recambio[0].set = "set2";
                else
                    pieces_armor_recambio[0].set = "set1";

                Update_Armor(pieces_armor_recambio);
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

                Update_Armor(pieces_armor_recambio);
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

                Update_Armor(pieces_armor_recambio);
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

                Update_Armor(pieces_armor_recambio);
            }

            if ((Keyboard.GetState().IsKeyDown(Keys.D3)))
            {
                if (pieces_armor_recambio[4].set == "set1")
                    pieces_armor_recambio[4].set = "set2";
                else
                    pieces_armor_recambio[4].set = "set1";

                Update_Armor(pieces_armor_recambio);
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
                 Get_Current_Frame() == 5)
            {

                for (int i = 0; i < Global.total_quant; i++)
                {
                    // Ver summary
                    if (!injuredByMe[i] &&
                        Global.players[i] != this &&
                        !Global.players[i].ghostMode)
                    {
                        
                        // Si esta dentro del radio del golpe
                        if (Collision_Verifier(Global.players[i].Get_Position_Rec()))
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
                    Color_Animation_Change(Color.White);
                else
                    Color_Animation_Change(Color.Red);

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
                Color_Animation_Change(Global.color_ghost);

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
