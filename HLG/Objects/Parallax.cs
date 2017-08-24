using Microsoft.Xna.Framework;

namespace HLG.Objects
{
    public class Parallax
    {
        //-//-// VARIABLES //-//-//
        public string[] parallaxLayer { get; internal set; }
        public Rectangle parallaxRectangle = Rectangle.Empty;
        public float parallaxSpeedX { get; internal set; }
        public float parallaxSpeedY { get; internal set; }

        //-//-// METHODS //-//-//
        /// <summary>
        /// Inicializador de capa parallax
        /// </summary>
        /// <param name="capa_parallax">Nombre del archivo capa</param>
        /// <param name="parallax_x">Movimiento sobre eje X</param>
        /// <param name="parallax_y">Movimiento sobre eje Y</param>
        public Parallax(string[] capa_parallax, float parallax_x, float parallax_y)
        {
            parallaxLayer = capa_parallax;
            parallaxSpeedX = parallax_x;
            parallaxSpeedY = parallax_y;
        }
    }
}
