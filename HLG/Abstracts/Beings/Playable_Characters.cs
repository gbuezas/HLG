using HLG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HLG.Abstracts.Beings
{
    public abstract class Playable_Characters : Being
    {

        private int IndexPlayer = -1;
        private Global.Mirada Direction;
        /// <summary>
        /// Esta es una copia de las animaciones que va a usar el hijo de esta clase, en donde se decide que texturas va a utilizar.
        /// En esta instancia la clase no sabe que texturas se van a utilizar. 
        /// </summary>
        private Animation[] Animations = null;
        /// <summary>
        /// Cuando daño a un personaje lo marco en esta lista.
        /// La resta se hace inmediatamente en vuelta logica, no de dibujado, del damnificado.
        /// Para evitar que se vuelva a generar daño en un plazo corto se utilizara esta variable que tendra en cuenta a quien se daño y 
        /// sera interna de cada atacante, la misma se reseteara cuando acabe la animacion del golpe correspondiente.
        /// Siempre tiene que englobar al total de personajes que estan en el juego (tanto jugables como IA).
        /// </summary>
        private bool[] InjuredByMe = new bool[Global.totalQuant];
        /// <summary>
        /// La cantidad de daño recibida, en un futuro sera un objeto o un struct que pueda contener distintos tipos de daño.
        /// </summary>
        private int Injured_Value = 0;
        /// <summary>
        /// La vitalidad maxima y actual del personaje
        /// </summary>
        private int MaxHealth = 500;
        private int CurrentHealth = 100;
        /// <summary>
        /// Si pierde toda su HP pasa a modo fantasma
        /// </summary>
        private bool Ghost_Mode = false;
        /// <summary>
        /// Alcance de golpe basico
        /// </summary>
        private float HitRangeX, HitRangeY;
        public Global.Mirada direction
        {
            get { return Direction; }
            set { Direction = value; }
        }
        internal Animation[] animations
        {
            get { return Animations; }
            set { Animations = value; }
        }
        protected bool[] injured
        {
            get { return InjuredByMe; }
            set { InjuredByMe = value; }
        }
        public int max_health
        {
            get { return MaxHealth; }
            set { MaxHealth = value; }
        }
        public float hitrangeX
        {
            get { return HitRangeX; }
            set { HitRangeX = value; }
        }
        public float hitrangeY
        {
            get { return HitRangeY; }
            set { HitRangeY = value; }
        }
        public Global.Actions CurrentAction { get; internal set; }
        public Global.Actions OldAction { get; internal set; }
        protected int FrameWidth = Global.FrameWidth;
        protected int FrameHeight = Global.FrameHeight;
        /// <summary>
        /// Animaciones de la UI de vida, estaba en estático y por eso no dibujaba varias instancias
        /// </summary>
        protected Animation UIAnimation;
        protected Vector2 UILifeNumber;
        protected float actual_bar_length;
        protected Color bar_color;
        protected float PlayerSpeed;
        /// <summary>
        /// Establece tiempo de frame inicial cuando llama al UpdateArmor
        /// El UpdateArmor no ocurre en el loop se pide explicitamente 
        /// </summary>
        protected int FrameTime;
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
        public Vector2 mensaje;


        /// <summary>
        /// Tipo de cada pieza de armadura:
        /// Shield, gauntletback, greaveback, helm, breastplate, tasset, greavetop, sword, gauntlettop. 
        /// </summary>
        public Pieces_Sets pieces_armor = new Pieces_Sets();
        public List<Piece_Set> pieces_armor_new = new List<Piece_Set>();

        // Actualizar animacion
        public abstract void Update(GameTime gameTime);
        // Carga los set de armadura que corresponden a cada pieza del cuerpo.
        public abstract void UpdateArmor(List<Piece_Set> set_pieces);
        // Cambiar color a la animacion
        public abstract void ColorAnimationChange(Color tinte);
        // Cambiar color a una pieza de la animacion
        public abstract void ColorPieceChange(Color tinte, int pieza);
        // Obtener frame actual de la animacion, se posa en la primer pieza del vector para obtenerla
        public abstract int GetCurrentFrame();
        // Obtener frame totales de la animacion, se posa en la primer pieza del vector para obtenerla
        public abstract int GetTotalFrames();
        // Activa o desactiva al jugador (si no esta activo no se dibuja)
        public abstract void ActivatePlayer(bool active);
        // Limpio la lista interna de personajes que dañe, este metodo se usa al terminar una animacion que daña.
        public abstract void ResetInjured();
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

            return (atacante.Center.X >= (victima.Center.X - HitRangeX) &&
                    atacante.Center.X <= victima.Center.X &&
                    atacante.Center.Y >= (victima.Center.Y - HitRangeY) &&
                    atacante.Center.Y <= (victima.Center.Y + HitRangeY) &&
                    direction == Global.Mirada.RIGHT)
                    ||
                   (atacante.Center.X <= (victima.Center.X + HitRangeX) &&
                    atacante.Center.X >= victima.Center.X &&
                    atacante.Center.Y >= (victima.Center.Y - HitRangeY) &&
                    atacante.Center.Y <= (victima.Center.Y + HitRangeY) &&
                    direction == Global.Mirada.LEFT);
        }
        public bool CollisionVerifierEnhanced(Rectangle victima)
        {
            Rectangle atacante = GetPositionRec();

            return (atacante.Center.X >= (victima.Center.X - HitRangeX - 10) &&
                    atacante.Center.X <= victima.Center.X &&
                    atacante.Center.Y >= (victima.Center.Y - HitRangeY) &&
                    atacante.Center.Y <= (victima.Center.Y + HitRangeY) &&
                    direction == Global.Mirada.RIGHT)
                    ||
                   (atacante.Center.X <= (victima.Center.X + HitRangeX + 10) &&
                    atacante.Center.X >= victima.Center.X &&
                    atacante.Center.Y >= (victima.Center.Y - HitRangeY) &&
                    atacante.Center.Y <= (victima.Center.Y + HitRangeY) &&
                    direction == Global.Mirada.LEFT);
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
        /// Logica de todas las acciones, los movimientos, los golpes, etc.
        /// </summary>
        public void ActionLogic()
        {
            /// Si esta pegando tiene que terminar su animacion y despues desbloquear otra vez la gama de movimientos,
            /// para esto comparamos el frame actual de la animacion con su frame total.
            /// Cuando termine la animacion de pegar puede generar daño de vuelta a alguien que ya haya atacado
            if (new Global.Actions[] { Global.Actions.HIT1, Global.Actions.HIT2, Global.Actions.HIT3 }.Contains(CurrentAction))
            {
                if (GetCurrentFrame() == GetTotalFrames())
                {
                    CurrentAction = Global.Actions.STAND;
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
            if (Global.currentKeyboardState.IsKeyDown(controls[(int)Global.Controls.BUTTON_HIT]))
                CurrentAction = (Global.Actions)Global.randomly.Next(2, 5);
        }
        private void Movement_Input()
        {
            /// Si no se toca nada quedara por default que esta parado.
            /// Si se presiona alguna tecla de movimiento se asignara el mismo.
            CurrentAction = Global.Actions.STAND;

            if (Global.currentKeyboardState.IsKeyDown(controls[(int)Global.Controls.LEFT]))
            {
                positionX -= PlayerSpeed;
                direction = Global.Mirada.LEFT;
                CurrentAction = Global.Actions.WALK;
            }
            else if (Global.currentKeyboardState.IsKeyDown(controls[(int)Global.Controls.RIGHT]))
            {
                positionX += PlayerSpeed;
                direction = Global.Mirada.RIGHT;
                CurrentAction = Global.Actions.WALK;
            }

            if (Global.currentKeyboardState.IsKeyDown(controls[(int)Global.Controls.UP]))
            {
                positionY -= PlayerSpeed;
                CurrentAction = Global.Actions.WALK;
            }
            else if (Global.currentKeyboardState.IsKeyDown(controls[(int)Global.Controls.DOWN]))
            {
                positionY += PlayerSpeed;
                CurrentAction = Global.Actions.WALK;
            }
        }
    }
}
