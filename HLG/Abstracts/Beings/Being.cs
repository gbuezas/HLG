using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace HLG.Abstracts.Beings
{
    public abstract class Being
    {
        // GAB - chequear que esta bien asignado ese -1, fijarse cuando se crea si se crea con el -1 - BORRAR ESTE COMENTARIO TRAS VERIFICACION
        public int index { get; internal set; } = -1;

        public bool ghost_mode { get; internal set; }
        public int injured_value { get; internal set; }
        public int current_health { get; internal set; }

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

        private Vector2 Position;
        public Vector2 position
        {
            get { return Position; }
            set { Position = value; }
        }

        public Keys[] Controls = new Keys[Enum.GetNames(typeof(Global.Controls)).Length];

        public Keys[] controls
        {
            get { return Controls; }
            set { Controls = value; }
        }

        public abstract void Initialize(Vector2 posicion);

        public abstract Rectangle GetPositionRec();
        public abstract Vector2 GetPositionVec();

        public abstract void DrawWithParallax(SpriteBatch spriteBatch);

        public abstract void DrawWithoutParallax(SpriteBatch spriteBatch);
        public abstract void UpdatePlayer(GameTime gameTime, int var_AltoNivel, int var_AnchoNivel);
    }
}
