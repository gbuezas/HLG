﻿using Microsoft.Xna.Framework.Graphics;

namespace HLG.Objects
{
    public class Textures
    {
        
        //-//-// VARIABLES //-//-//
        public Texture2D texture { get; internal set; }
        
        public string texture_set_name { get; }
        public string texture_piece_name { get; }
        public string texture_action { get; }
        public string texture_frames { get; set; }
        
        //-//-// METHODS //-//-//
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
            texture_set_name = separador[0];
            texture_piece_name = separador[1];
            texture_action = separador[2];
            texture_frames = separador[3];
        }
        
    }
}
