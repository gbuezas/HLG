using Microsoft.Xna.Framework;
using System;

namespace HLG.Objects
{
    public class Parallax
    {

        private string[] CapaParallax;
        public string[] capa_parallax
        {
            get { return CapaParallax; }
            set { CapaParallax = value; }
        }

        // Rectangulo adaptado al parallax para no poner tiles de mas
        // y que se adapte segun la velocidad de la capa correspondiente al limite de la pantalla
        public Rectangle RectanguloParallax;

        // Velocidad del parallax en el eje X
        private float Parallax_X;
        public float parallax_x
        {
            get { return Parallax_X; }
            set { Parallax_X = value; }
        }

        // Velocidad del parallax en el eje Y
        private float Parallax_Y;
        public float parallax_y
        {
            get { return Parallax_Y; }
            set { Parallax_Y = value; }
        }

        public Parallax(String[] capa_parallax, float parallax_x, float parallax_y)
        {
            CapaParallax = capa_parallax;
            Parallax_X = parallax_x;
            Parallax_Y = parallax_y;
        }
    }
}
