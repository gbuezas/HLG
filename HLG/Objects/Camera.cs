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
        //-//-// VARIABLES //-//-//
        private const float minZoom = 0.01f;
        private Viewport viewport;
        private readonly Vector2 origin;
        private Vector2 position;
        private float zoom = 1f;
        private Rectangle? limits;
        private int levelHeight;
        private int levelWidth;

        // Rectangulo delimitador de la camara para pasarle los parametros a los Being
        public Rectangle screenLimits;

        // Para crear el efecto parallax
        public Vector2 parallax;

        // Lista de los Being en la pantalla
        public List<Vector2> viewTargets = new List<Vector2>();

        //-//-// METHODS //-//-//
        public Camera(Viewport viewport, int AltoNivel, int AnchoNivel)
        {
            this.viewport = viewport;
            origin = new Vector2(this.viewport.Width / 2.0f, this.viewport.Height / 2.0f);
            levelHeight = AltoNivel;
            levelWidth = AnchoNivel;
        }

        /// <summary>
        /// Gets or sets the position of the camera.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
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
                return zoom;
            }
            set
            {
                zoom = MathHelper.Max(value, minZoom);
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
                limits = value;
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
                return Matrix.CreateTranslation(new Vector3(-position * parallax, 0f)) *
                       Matrix.CreateTranslation(new Vector3(-origin, 0f)) *
                       Matrix.CreateScale(zoom, zoom, 1f) *
                       Matrix.CreateTranslation(new Vector3(origin, 0f));
            }
        }

        /// <summary>
        /// When using limiting, makes sure the camera position is valid.
        /// </summary>
        private void ValidatePosition()
        {
            if (limits.HasValue)
            {
                Vector2 cameraWorldMin = Vector2.Transform(Vector2.Zero, Matrix.Invert(ViewMatrix));
                Vector2 cameraSize = new Vector2(viewport.Width, viewport.Height) / zoom;
                Vector2 limitWorldMin = new Vector2(limits.Value.Left, limits.Value.Top);
                Vector2 limitWorldMax = new Vector2(limits.Value.Right, limits.Value.Bottom);
                Vector2 positionOffset = position - cameraWorldMin;
                position = Vector2.Clamp(cameraWorldMin, limitWorldMin, limitWorldMax - cameraSize) + positionOffset;
            }
        }
        /// <summary>
        /// When using limiting, makes sure the camera zoom is valid.
        /// </summary>
        private void ValidateZoom()
        {
            if (limits.HasValue)
            {
                float minZoomX = (float)viewport.Width / limits.Value.Width;
                float minZoomY = (float)viewport.Height / limits.Value.Height;
                zoom = MathHelper.Clamp(zoom, MathHelper.Max(minZoomX, minZoomY), 1.3f);
                Global.mensaje5 = zoom;
            }
        }

        public void CenterCamera()
        {
            if (viewTargets.Count > 0)
            {
                Vector2 min = viewTargets[0];
                Vector2 max = viewTargets[0];

                for (int i = 1; i < viewTargets.Count; i++)
                {
                    if (viewTargets[i].X < min.X) min.X = viewTargets[i].X;
                    else if (viewTargets[i].X > max.X) max.X = viewTargets[i].X;
                    if (viewTargets[i].Y < min.Y) min.Y = viewTargets[i].Y;
                    else if (viewTargets[i].Y > max.Y) max.Y = viewTargets[i].Y;
                }

                Rectangle rect = new Rectangle((int)min.X, (int)min.Y,
                    (int)(max.X - min.X), (int)(max.Y - min.Y));

                // Centro la camara en el punto intermedio de los Being mas alejados
                position = new Vector2(rect.Center.X - viewport.Width / 2, rect.Center.Y - viewport.Height / 2);

                // Acomodo el zoom de acuerdo a la distancia entre los puntos mas alejados
                float diferencia = max.X - min.X;
                if (diferencia <= 1000)
                {
                    zoom = -2 * diferencia / 1000 + 2;
                }

                ValidateZoom();
                ValidatePosition();

                // Genero los limites de la pantalla para que los Being se puedan acomodar acorde a los mismos
                // y hago que esten dentro de los limites del nivel cuando se genere el zoom
                screenLimits.X = (int)position.X;
                screenLimits.Y = (int)position.Y;
                screenLimits.Width = (int)position.X + viewport.Width;
                screenLimits.Height = (int)position.Y + viewport.Height;

                // Corroboro que no se pueda estar fuera del nivel
                if (screenLimits.X < 0)
                {
                    screenLimits.X = 0;
                }
                else if (screenLimits.Width > levelWidth)
                {
                    screenLimits.Width = levelWidth;
                }

            }
        }
        public bool InsideCamera(Rectangle Objeto)
        {
            return screenLimits.Intersects(Objeto);
        }
        public bool InsideCameraAmplified(Rectangle Objeto)
        {
            Rectangle RecAmplify = screenLimits;
            RecAmplify.X = RecAmplify.X - RecAmplify.Width / 2;
            RecAmplify.Width = RecAmplify.Width + RecAmplify.Width / 2;
            return RecAmplify.Intersects(Objeto);
        }

    }
}
