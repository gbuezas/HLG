using Microsoft.Xna.Framework.Graphics;

namespace HLG.Objects
{
    public class Textures
    {
        #region VARIABLES

        // La textura a utilizar
        private Texture2D Textura;
        public Texture2D textura
        {
            get { return Textura; }
            set { Textura = value; }
        }

        // Nombre completo de la textura
        private string Nombre_textura;
        public string nombre_textura
        {
            get { return Nombre_textura; }
            set { Nombre_textura = value; }
        }

        // Set de armadura a la que pertenece esta pieza
        private string Set;
        public string set
        {
            get { return Set; }
        }

        // Pieza que representa la textura
        private string Piece;
        public string piece
        {
            get { return Piece; }
        }

        // Accion que realiza esta pieza
        private string Action;
        public string action
        {
            get { return Action; }
        }

        // Frames de la animacion
        private string Frame;
        public string frame
        {
            get { return Frame; }
            set { Frame = value; }
        }

        #endregion

        #region METODOS

        /// <summary>
        /// Cargamos los sprites a utilizar y los datos necesarios para poder utilizarlos.
        /// Mas tarde se van a contrastar con los datos del jugador para utilizarlos correctamente.
        /// Ejemplo: blue_gauntlettop_walk_10.png
        /// </summary>
        /// <param name="textura">Los sprites que se van a utilizar</param>
        /// <param name="nombre">El nombre del archivo con sus datos pertinentes</param>
        public Textures(Texture2D textura, string nombre)
        {
            Textura = textura;
            Nombre_textura = nombre;

            string[] separador = nombre.Split('_');

            Set = separador[0];
            Piece = separador[1];
            Action = separador[2];
            Frame = separador[3];
        }

        #endregion
    }
}
