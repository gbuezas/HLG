using Microsoft.Xna.Framework.Graphics;

namespace HLG.Objects
{
    public class Textures
    {
        
        //-//-// VARIABLES //-//-//
        public Texture2D texture { get; internal set; }
        
        public string textureSetName { get; set; }
        public string texturePieceName { get; set; }
        public string textureAction { get; }
        public string textureFrames { get; set; }

        //-//-// METHODS //-//-//
        /// <summary>
        /// Crea una textura totalmente vacia. 
        /// Esto se utiliza para cuando queremos generar una copia de este objeto, por ejemplo en la User_Interface
        /// </summary>
        public Textures()
        {
            texture = null;

            textureSetName = string.Empty;
            texturePieceName = string.Empty; ;
            textureAction = string.Empty;
            textureFrames = string.Empty;
        }

        /// <summary>
        /// Cargamos los sprites a utilizar y los datos necesarios para poder utilizarlos.
        /// Mas tarde se van a contrastar con los datos del jugador para utilizarlos correctamente.
        /// Ejemplo: set1_gauntlettop_walk_10.png
        /// </summary>
        /// <param name="textura">Los sprites que se van a utilizar</param>
        /// <param name="nombre">El nombre del archivo con sus datos pertinentes</param>
        public Textures(Texture2D textura, string nombre)
        {
            texture = textura;
            
            string[] separador = nombre.Split('_');
            textureSetName = separador[0];
            texturePieceName = separador[1];
            textureAction = separador[2];
            textureFrames = separador[3];
        }
        
    }
}
