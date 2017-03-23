using HLG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HLG.Abstracts.Beings
{
    public abstract class Being
    {

        //-//-// VARIABLES //-//-//
        public int index { get; internal set; } = -1;

        public Pieces_Sets piecesArmor = new Pieces_Sets();
        public List<Piece_Set> piecesArmorNew = new List<Piece_Set>();
        public List<Textures> objectTextures = new List<Textures>();
        public Animation[] animations { get; internal set; } = null;
        public Animation[] animationPieces { get; internal set; } = null;
        public int frameTime { get; internal set; }
        public int frameWidth { get; internal set; } = 0;
        public int frameHeight { get; internal set; } = 0;

        private Vector2 Position;
        public Vector2 position
        {
            get { return Position; }
            internal set { Position = value; }
        }
        public float positionX
        {
            get { return Position.X; }
            internal set { Position.X = value; }
        }
        public float positionY
        {
            get { return Position.Y; }
            internal set { Position.Y = value; }
        }

        public float playerMoveSpeed { get; internal set; }

        public Global.Actions currentAction { get; internal set; }
        public Global.Actions oldAction { get; internal set; }
        
        public Global.Facing facing { get; internal set; } = Global.Facing.RIGHT;

        public int currentHealth { get; internal set; }
        public int maxHealth { get; internal set; }
        public bool ghostMode { get; internal set; } = false;

        public float hitRangeX { get; internal set; }
        public float hitRangeY { get; internal set; }
        /// <summary>
        /// Cuando daño a un personaje lo marco en esta lista.
        /// La resta se hace inmediatamente en vuelta logica, no de dibujado, del damnificado.
        /// Para evitar que se vuelva a generar daño en un plazo corto se utilizara esta variable que tendra en cuenta a quien se daño y 
        /// sera interna de cada atacante, la misma se reseteara cuando acabe la animacion del golpe correspondiente.
        /// Siempre tiene que englobar al total de personajes que estan en el juego (tanto jugables como IA).
        /// </summary>
        public bool[] injuredByMe { get; internal set; } = new bool[Global.total_quant];
        public int injuredValue { get; internal set; } = 0;

        /// <summary>
        /// Se coloco en este nivel asi se facilita la habilidad de poseer enemigos y controlarlos
        /// </summary>
        public Keys[] controls { get; internal set; } = new Keys[Enum.GetNames(typeof(Global.Controls)).Length];

        protected float mensaje1;
        protected float mensaje2;
        protected Global.Facing mensaje3;
        protected Global.Actions mensaje4;
        protected float mensaje5;
        protected float mensaje6;
        protected float mensaje7;
        protected float mensaje8;
        protected float mensaje9;
        public Vector2 mensaje;

        //-//-// METHODS //-//-//
        public void Activate_Player(bool active)
        {
            foreach (Animation piece in animations)
            {
                piece.active = active;
            }
        }

        /// <summary>
        /// Logica de todas las acciones, los movimientos, los golpes, etc.
        /// </summary>
        public void Action_Logic_Manual()
        {
            /// Si esta pegando tiene que terminar su animacion y despues desbloquear otra vez la gama de movimientos,
            /// para esto comparamos el frame actual de la animacion con su frame total.
            /// Cuando termine la animacion de pegar puede generar daño de vuelta a alguien que ya haya atacado
            if (new Global.Actions[] { Global.Actions.HIT1, Global.Actions.HIT2, Global.Actions.HIT3 }.Contains(currentAction))
            {
                if (Get_Current_Frame() == Get_Total_Frames())
                {
                    currentAction = Global.Actions.STAND;
                    Reset_Injured();
                }
            }
            else
            {
                Movement_Input();
                Hit_Input();
            }
        }
        private void Hit_Input()
        {
            /// Si presiono golpear cancela todas las demas acciones hasta que esta termine su ciclo,
            /// tambien genera un rango de los 3 diferentes tipos de golpes (algo netamente visual sin impacto en el juego).
            /// La aleatoriedad en los golpes depende de como estan almacenados en las variables Global, 
            /// la primer variable es incluyente y la segunda excluyente.
            if (Global.current_keyboard_state.IsKeyDown(controls[(int)Global.Controls.BUTTON_HIT]))
                currentAction = (Global.Actions)Global.randomly.Next(2, 5);
        }
        private void Movement_Input()
        {
            /// Si no se toca nada quedara por default que esta parado.
            /// Si se presiona alguna tecla de movimiento se asignara el mismo.
            currentAction = Global.Actions.STAND;

            if (Global.current_keyboard_state.IsKeyDown(controls[(int)Global.Controls.LEFT]))
            {
                positionX -= playerMoveSpeed;
                facing = Global.Facing.LEFT;
                currentAction = Global.Actions.WALK;
            }
            else if (Global.current_keyboard_state.IsKeyDown(controls[(int)Global.Controls.RIGHT]))
            {
                positionX += playerMoveSpeed;
                facing = Global.Facing.RIGHT;
                currentAction = Global.Actions.WALK;
            }

            if (Global.current_keyboard_state.IsKeyDown(controls[(int)Global.Controls.UP]))
            {
                positionY -= playerMoveSpeed;
                currentAction = Global.Actions.WALK;
            }
            else if (Global.current_keyboard_state.IsKeyDown(controls[(int)Global.Controls.DOWN]))
            {
                positionY += playerMoveSpeed;
                currentAction = Global.Actions.WALK;
            }
        }
        public void Reset_Injured()
        {
            for (int i = 0; i < injuredByMe.Length; i++)
            {
                injuredByMe[i] = false;
            }
        }

        public abstract void Initialize(Vector2 posicion);

        public void Caps_Max_Health()
        {
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
        }

        /// <summary>
        /// Obtiene la posicion de una pieza de animacion en rectangulo
        /// </summary>
        /// <returns> Posicion del jugador </returns>
        public Rectangle Get_Position_Rec()
        {
            return animationPieces[0].Get_Position();
        }
        /// <summary>
        /// Obtiene la posicion del jugador relativa a la parte superior izquierda de la pantalla
        /// </summary>
        /// <returns> Posicion del jugador </returns>
        public Vector2 Get_Position_Vec()
        {
            return position;
        }

        /// <summary>
        /// Chequea las colisiones.
        /// Si el objeto esta del lado izquierdo chequea del rango izquierdo hasta el centro de la victima y
        /// si esta del lado derecho chequea del rango derecho hasta el centro de la victima.
        /// El rango lo toma del mismo objeto, depende de cada atacante.
        /// </summary>
        /// <param name="victima">Rectangulo de la victima</param>
        /// <returns>Si la victima colisiona o no con el objeto</returns>
        public bool Collision_Verifier(Rectangle victima)
        {
            Rectangle atacante = Get_Position_Rec();

            return (atacante.Center.X >= (victima.Center.X - hitRangeX) &&
                    atacante.Center.X <= victima.Center.X &&
                    atacante.Center.Y >= (victima.Center.Y - hitRangeY) &&
                    atacante.Center.Y <= (victima.Center.Y + hitRangeY) &&
                    facing == Global.Facing.RIGHT)
                    ||
                   (atacante.Center.X <= (victima.Center.X + hitRangeX) &&
                    atacante.Center.X >= victima.Center.X &&
                    atacante.Center.Y >= (victima.Center.Y - hitRangeY) &&
                    atacante.Center.Y <= (victima.Center.Y + hitRangeY) &&
                    facing == Global.Facing.LEFT);
        }
        public bool Collision_Verifier_Enhanced(Rectangle victima)
        {
            Rectangle atacante = Get_Position_Rec();

            return (atacante.Center.X >= (victima.Center.X - hitRangeX - 10) &&
                    atacante.Center.X <= victima.Center.X &&
                    atacante.Center.Y >= (victima.Center.Y - hitRangeY) &&
                    atacante.Center.Y <= (victima.Center.Y + hitRangeY) &&
                    facing == Global.Facing.RIGHT)
                    ||
                   (atacante.Center.X <= (victima.Center.X + hitRangeX + 10) &&
                    atacante.Center.X >= victima.Center.X &&
                    atacante.Center.Y >= (victima.Center.Y - hitRangeY) &&
                    atacante.Center.Y <= (victima.Center.Y + hitRangeY) &&
                    facing == Global.Facing.LEFT);
        }
        
        public int Get_Current_Frame()
        {
            return animations[0].currentFrame;
        }
        public int Get_Total_Frames()
        {
            return animations[0].frameCount - 1;
        }
        public void Pause_Animation(bool desactivar)
        {
            foreach (Animation piezaAnimada in animationPieces)
            {
                piezaAnimada.pause = desactivar;
            }
        }
        public void Frame_Number_Action_Reset()
        {
            if (oldAction != currentAction)
            {
                foreach (Animation Animation in animationPieces)
                    Animation.currentFrame = 0;

                oldAction = currentAction;
            }
        }
        public void Animation_Frame_Position_Update(GameTime gameTime)
        {
            foreach (Animation piezaAnimada in animationPieces)
            {
                piezaAnimada.position = position;
                piezaAnimada.Update(gameTime);
            }
        }

        public abstract void Draw_With_Parallax();
        public abstract void Draw_Without_Parallax();
        public void Color_Animation_Change(Color tinte)
        {
            foreach (Animation Animation in animations)
            {
                Animation.Color_Change(tinte);
            }
        }
        public void Color_Piece_Change(Color tinte, int pieza)
        {
            animations[pieza].Color_Change(tinte);
        }

        public abstract void Update_Player(GameTime gameTime, int var_AltoNivel, int var_AnchoNivel);
        public void Update_Armor(List<Piece_Set> set_pieces)
        {
            foreach (Piece_Set set_piece in set_pieces)
                piecesArmor.Set_Set(set_piece);

            Texture_Force_Load();
        }

        private void Texture_Force_Load()
        {
            foreach (Animation piezaAnimation in animationPieces)
            {
                foreach (Textures textura in objectTextures)
                {
                    if (textura.texture_piece_name == piezaAnimation.pieceName &&
                        textura.texture_set_name == piecesArmor.Get_Set(textura.texture_piece_name) &&
                        textura.texture_action == currentAction.ToString().ToLower())
                    {
                        piezaAnimation.Load_Texture(textura, position, frameWidth, frameHeight, frameTime, Color.White, true);
                    }
                }
            }
        }
        public void Texture_Regular_Load()
        {
            foreach (Animation animation_piece in animationPieces)
            {
                foreach (Textures texture in objectTextures)
                {
                    if (texture.texture_piece_name == animation_piece.pieceName &&
                        texture.texture_set_name == piecesArmor.Get_Set(texture.texture_piece_name) &&
                        texture.texture_action == currentAction.ToString().ToLower())
                    {
                        animation_piece.Load_Texture(texture);
                    }
                }
            }
        }

    }
}
