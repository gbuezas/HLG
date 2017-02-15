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

        public Pieces_Sets pieces_armor = new Pieces_Sets();
        public List<Piece_Set> pieces_armor_new = new List<Piece_Set>();
        public List<Textures> object_textures = new List<Textures>();
        public Animation[] animations { get; internal set; } = null;
        public Animation[] animation_pieces { get; internal set; } = null;
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

        public int current_health { get; internal set; }
        public int max_health { get; internal set; }
        public bool ghost_mode { get; internal set; } = false;

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
        public int injured_value { get; internal set; } = 0;

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
        public void ActivatePlayer(bool active)
        {
            foreach (Animation piece in animations)
            {
                piece.active = active;
            }
        }

        /// <summary>
        /// Logica de todas las acciones, los movimientos, los golpes, etc.
        /// </summary>
        public void ActionLogicManual()
        {
            /// Si esta pegando tiene que terminar su animacion y despues desbloquear otra vez la gama de movimientos,
            /// para esto comparamos el frame actual de la animacion con su frame total.
            /// Cuando termine la animacion de pegar puede generar daño de vuelta a alguien que ya haya atacado
            if (new Global.Actions[] { Global.Actions.HIT1, Global.Actions.HIT2, Global.Actions.HIT3 }.Contains(currentAction))
            {
                if (GetCurrentFrame() == GetTotalFrames())
                {
                    currentAction = Global.Actions.STAND;
                    ResetInjured();
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
        public void ResetInjured()
        {
            for (int i = 0; i < injuredByMe.Length; i++)
            {
                injuredByMe[i] = false;
            }
        }

        public abstract void Initialize(Vector2 posicion);

        public void CapsMaxHealth()
        {
            if (current_health > max_health)
                current_health = max_health;
        }

        /// <summary>
        /// Obtiene la posicion de una pieza de animacion en rectangulo
        /// </summary>
        /// <returns> Posicion del jugador </returns>
        public Rectangle GetPositionRec()
        {
            return animation_pieces[0].GetPosition();
        }
        /// <summary>
        /// Obtiene la posicion del jugador relativa a la parte superior izquierda de la pantalla
        /// </summary>
        /// <returns> Posicion del jugador </returns>
        public Vector2 GetPositionVec()
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
        public bool CollisionVerifier(Rectangle victima)
        {
            Rectangle atacante = GetPositionRec();

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
        public bool CollisionVerifierEnhanced(Rectangle victima)
        {
            Rectangle atacante = GetPositionRec();

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
        
        public int GetCurrentFrame()
        {
            return animations[0].CurrentFrame;
        }
        public int GetTotalFrames()
        {
            return animations[0].FrameCount - 1;
        }
        public void PauseAnimation(bool desactivar)
        {
            foreach (Animation piezaAnimada in animation_pieces)
            {
                piezaAnimada.pause = desactivar;
            }
        }
        public void FrameNumberActionReset()
        {
            if (oldAction != currentAction)
            {
                foreach (Animation Animation in animation_pieces)
                    Animation.CurrentFrame = 0;

                oldAction = currentAction;
            }
        }
        public void AnimationFramePositionUpdate(GameTime gameTime)
        {
            foreach (Animation piezaAnimada in animation_pieces)
            {
                piezaAnimada.position = position;
                piezaAnimada.Update(gameTime);
            }
        }

        public abstract void DrawWithParallax();
        public abstract void DrawWithoutParallax();
        public void ColorAnimationChange(Color tinte)
        {
            foreach (Animation Animation in animations)
            {
                Animation.ColorChange(tinte);
            }
        }
        public void ColorPieceChange(Color tinte, int pieza)
        {
            animations[pieza].ColorChange(tinte);
        }

        public abstract void UpdatePlayer(GameTime gameTime, int var_AltoNivel, int var_AnchoNivel);
        public void UpdateArmor(List<Piece_Set> set_pieces)
        {
            foreach (Piece_Set set_piece in set_pieces)
                pieces_armor.Set_Set(set_piece);

            TextureForceLoad();
        }

        private void TextureForceLoad()
        {
            foreach (Animation piezaAnimation in animation_pieces)
            {
                foreach (Textures textura in object_textures)
                {
                    if (textura.texture_piece_name == piezaAnimation.pieceName &&
                        textura.texture_set_name == pieces_armor.Get_Set(textura.texture_piece_name) &&
                        textura.texture_action == currentAction.ToString().ToLower())
                    {
                        piezaAnimation.LoadTexture(textura, position, frameWidth, frameHeight, frameTime, Color.White, true);
                    }
                }
            }
        }
        public void TextureRegularLoad()
        {
            foreach (Animation animation_piece in animation_pieces)
            {
                foreach (Textures texture in object_textures)
                {
                    if (texture.texture_piece_name == animation_piece.pieceName &&
                        texture.texture_set_name == pieces_armor.Get_Set(texture.texture_piece_name) &&
                        texture.texture_action == currentAction.ToString().ToLower())
                    {
                        animation_piece.LoadTexture(texture);
                    }
                }
            }
        }

    }
}
