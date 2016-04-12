using HLG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace HLG.Abstracts.Beings
{
    public abstract class Being
    {
        #region VARIABLES

        #region CONTROLES

        /// <summary>
        /// Esta bandera es para que no se vuelva a dibujar varias veces el mismo objeto, 
        /// en caso de que el eje Y del mismo se repita con otro objeto
        /// </summary>
        private bool Drawn = false;

        /// <summary>
        /// Si el personaje es manejado por la maquina o por un humano
        /// </summary>
        private bool Machine = false;

        /// <summary>
        /// Posicion del jugador relativa a la parte superior izquierda de la pantalla.
        /// Esta posicion marca donde se encuentra el jugador en la pantalla y no en el mapa donde se esta moviendo,
        /// y es a esta posicion a la que se le aplican los limites de la pantalla.  
        /// </summary>
        private Vector2 Position;

        /// <summary>
        /// Para que lado esta mirando el personaje
        /// </summary>
        private Global.Mirada Direction;

        /// <summary>
        /// Controles del jugador
        /// </summary>
        private Keys[] Controls = new Keys[Enum.GetNames(typeof(Global.Controls)).Length];

        /// <summary>
        /// Esta es una copia de las animaciones que va a usar el hijo de esta clase, en donde se decide que texturas va a utilizar.
        /// En esta instancia la clase no sabe que texturas se van a utilizar. 
        /// </summary>
        private Animation[] Animations = null;

        #endregion

        #region JUGABILIDAD

        /// <summary>
        /// Cuando daño a un personaje lo marco en esta lista.
        /// La resta se hace inmediatamente en vuelta logica, no de dibujado, del damnificado.
        /// Para evitar que se vuelva a generar daño en un plazo corto se utilizara esta variable que tendra en cuenta a quien se daño y 
        /// sera interna de cada atacante, la misma se reseteara cuando acabe la animacion del golpe correspondiente.
        /// Siempre tiene que englobar al total de personajes que estan en el juego (tanto jugables como IA).
        /// </summary>
        private bool[] Injured = new bool[Global.totalQuant];

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

        #endregion

        #endregion

        #region METODOS

        #region GET-SET

        #region CONTROLES

        public bool drawn
        {
            get { return Drawn; }
            set { Drawn = value; }
        }

        public bool machine
        {
            get { return Machine; }
            set { Machine = value; }
        }

        public Vector2 position
        {
            get { return Position; }
            set { Position = value; }
        }

        public float positionX
        {
            get { return Position.X; }
            set { Position.X = value; }
        }

        public float positionY
        {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        public Global.Mirada direction
        {
            get { return Direction; }
            set { Direction = value; }
        }

        public Keys[] controls
        {
            get { return Controls; }
            set { Controls = value; }
        }
        
        internal Animation[] animations
        {
            get { return Animations; }
            set { Animations = value; }
        }

        #endregion

        #region JUGABILIDAD

        protected bool[] injured
        {
            get { return Injured; }
            set { Injured = value; }
        }

        public int injured_value
        {
            get { return Injured_Value; }
            set { Injured_Value = value; }
        }

        public int max_health
        {
            get { return MaxHealth; }
            set { MaxHealth = value; }
        }

        public int current_health
        {
            get { return CurrentHealth; }
            set { CurrentHealth = value; }
        }

        public bool ghost_mode
        {
            get { return Ghost_Mode; }
            set { Ghost_Mode = value; }
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

        #endregion

        #endregion

        #region ABSTRACTAS

        // Inicializar al jugador
        public abstract void Initialize(Vector2 posicion);

        // Actualizar animacion
        public abstract void Update(GameTime gameTime);

        // Dibujar Jugador
        public abstract void Draw(SpriteBatch spriteBatch);

        // Actualizar cosas del jugador - GAB retocar
        public abstract void UpdatePlayer(GameTime gameTime, Rectangle LimitesJugador, int AltoNivel, int AnchoNivel);

        // Carga los set de armadura que corresponden a cada pieza del cuerpo.
        public abstract void UpdateArmor(List<Piece_Set> set_pieces);

        // Obtiene posicion del jugador en pantalla con vector
        public abstract Vector2 GetPositionVec();

        // Obtiene posicion de pieza de animacion en rectangulo
        public abstract Rectangle GetPositionRec();

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

        #endregion

        #region PROPIAS

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

        #endregion

        #endregion
    }
}
