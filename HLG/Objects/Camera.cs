using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

/*Para usar la camara:

    1) Creo la variable de la camara como estatica en la seccion de las variables del objeto.
    2) Le seteamos el viewport correspondiente en el load con el comando new.
    3) Seteamos el rectangulo delimitador. Usualmente las medidas del nivel.
    4) Si queremos algun parallax seteamos el parametro parallax de la misma.
    4) Usamos su matriz en el begin del draw y listo, el parallax que esta tenga sera aplicado en el draw.*/

namespace HLG.Objects
{
    public class Camera
    {

        private const float MinZoom = 0.01f;
        private Viewport _viewport;
        private readonly Vector2 _origin;
        private Vector2 _position;
        private float _zoom = 1f;
        private Rectangle? _limits;
        private int _altonivel;
        private int _anchonivel;

        // Rectangulo delimitador de la camara para pasarle los parametros a los Being
        public Rectangle LimitesPantalla;

        // Para crear el efecto parallax
        public Vector2 parallax;

        // Lista de los Being en la pantalla
        public List<Vector2> ViewTargets = new List<Vector2>();

        public Camera(Viewport viewport, int AltoNivel, int AnchoNivel)
        {
            _viewport = viewport;
            _origin = new Vector2(_viewport.Width / 2.0f, _viewport.Height / 2.0f);
            _altonivel = AltoNivel;
            _anchonivel = AnchoNivel;
        }

        /// <summary>
        /// Gets or sets the position of the camera.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                ValidatePosition();
            }
        }

        /// <summary>
        /// Gets or sets the zoom of the camera.
        /// </summary>
        public float Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _zoom = MathHelper.Max(value, MinZoom);
                ValidateZoom();
                ValidatePosition();
            }
        }

        /// <summary>
        /// Sets a rectangle that describes which region of the world the camera should
        /// be able to see. Setting it to null removes the limit.
        /// </summary>
        public Rectangle? Limits
        {
            set
            {
                _limits = value;
                ValidateZoom();
                ValidatePosition();
            }
        }

        /// <summary>
        /// Calculates a view matrix for this camera.
        /// </summary>
        public Matrix ViewMatrix
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-_position * parallax, 0f)) *
                       Matrix.CreateTranslation(new Vector3(-_origin, 0f)) *
                       Matrix.CreateScale(_zoom, _zoom, 1f) *
                       Matrix.CreateTranslation(new Vector3(_origin, 0f));
            }
        }

        /// <summary>
        /// When using limiting, makes sure the camera position is valid.
        /// </summary>
        private void ValidatePosition()
        {
            if (_limits.HasValue)
            {
                Vector2 cameraWorldMin = Vector2.Transform(Vector2.Zero, Matrix.Invert(ViewMatrix));
                Vector2 cameraSize = new Vector2(_viewport.Width, _viewport.Height) / _zoom;
                Vector2 limitWorldMin = new Vector2(_limits.Value.Left, _limits.Value.Top);
                Vector2 limitWorldMax = new Vector2(_limits.Value.Right, _limits.Value.Bottom);
                Vector2 positionOffset = _position - cameraWorldMin;
                _position = Vector2.Clamp(cameraWorldMin, limitWorldMin, limitWorldMax - cameraSize) + positionOffset;
            }
        }

        /// <summary>
        /// When using limiting, makes sure the camera zoom is valid.
        /// </summary>
        private void ValidateZoom()
        {
            if (_limits.HasValue)
            {
                float minZoomX = (float)_viewport.Width / _limits.Value.Width;
                float minZoomY = (float)_viewport.Height / _limits.Value.Height;
                _zoom = MathHelper.Clamp(_zoom, MathHelper.Max(minZoomX, minZoomY), 1.3f);
                Global.mensaje5 = _zoom;
            }
        }

        public void CentrarCamara()
        {
            if (ViewTargets.Count > 0)
            {
                Vector2 min = ViewTargets[0];
                Vector2 max = ViewTargets[0];

                for (int i = 1; i < ViewTargets.Count; i++)
                {
                    if (ViewTargets[i].X < min.X) min.X = ViewTargets[i].X;
                    else if (ViewTargets[i].X > max.X) max.X = ViewTargets[i].X;
                    if (ViewTargets[i].Y < min.Y) min.Y = ViewTargets[i].Y;
                    else if (ViewTargets[i].Y > max.Y) max.Y = ViewTargets[i].Y;
                }

                Rectangle rect = new Rectangle((int)min.X, (int)min.Y,
                    (int)(max.X - min.X), (int)(max.Y - min.Y));

                // Centro la camara en el punto intermedio de los Being mas alejados
                _position = new Vector2(rect.Center.X - _viewport.Width / 2, rect.Center.Y - _viewport.Height / 2);

                // Acomodo el zoom de acuerdo a la distancia entre los puntos mas alejados
                float diferencia = max.X - min.X;
                if (diferencia <= 1000)
                {
                    _zoom = -2 * diferencia / 1000 + 2;
                }

                ValidateZoom();
                ValidatePosition();

                // Genero los limites de la pantalla para que los Being se puedan acomodar acorde a los mismos
                // y hago que esten dentro de los limites del nivel cuando se genere el zoom
                LimitesPantalla.X = (int)_position.X;
                LimitesPantalla.Y = (int)_position.Y;
                LimitesPantalla.Width = (int)_position.X + _viewport.Width;
                LimitesPantalla.Height = (int)_position.Y + _viewport.Height;

                // Corroboro que no se pueda estar fuera del nivel
                if (LimitesPantalla.X < 0)
                {
                    LimitesPantalla.X = 0;
                }
                else if (LimitesPantalla.Width > _anchonivel)
                {
                    LimitesPantalla.Width = _anchonivel;
                }

            }
        }

        public bool EnCamara(Rectangle Objeto)
        {
            return LimitesPantalla.Intersects(Objeto);
        }

        public bool EnCamaraAmplificado(Rectangle Objeto)
        {
            Rectangle RecAmplify = LimitesPantalla;
            RecAmplify.X = RecAmplify.X - RecAmplify.Width / 2;
            RecAmplify.Width = RecAmplify.Width + RecAmplify.Width / 2;
            return RecAmplify.Intersects(Objeto);
        }
    }
}
