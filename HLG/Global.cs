using HLG.Abstracts.Beings;
using HLG.Abstracts.GameStates;
using HLG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace HLG
{
    public class Global
    {

        //-//-// VARIABLES //-//-//
        public enum Facing { RIGHT, LEFT };
        public enum Actions { WALK, STAND, HIT1, HIT2, HIT3, DEAD } // Las distintas acciones que se pueden hacer
        public enum Controls { UP, DOWN, LEFT, RIGHT, BUTTON_HIT, BUTTON_SPECIAL } // Los controles
        public enum EstadosJuego { INTRO, TITULO, SELECCION, MAPA, VS, AVANCE, PAUSA, GAMEOVER, FINAL } // Los distintos estados del juego

        public static SpriteBatch sprite_batch; // Variable utilizada para todo el dibujado

        public static SpriteFont check_status_font; // Fuente de los mensajes de chequeo
        public static SpriteFont check_status_font_2;
        public static float mensaje1;
        public static float mensaje2;
        public static float mensaje3;
        public static float mensaje4;
        public static float mensaje5;

        public static Camera camara; // Camara - Se setea en el Load() de cada nivel

        public static Color color_ghost = new Color(255, 255, 255, 30);
        public static Color[] skeleton_random_colors = new Color[] { Color.AntiqueWhite, Color.Wheat, Color.WhiteSmoke, Color.SeaShell, Color.OldLace, Color.LightYellow, Color.Gainsboro, Color.Cornsilk };
        
        public static States current_game_state;

        public static KeyboardState current_keyboard_state = new KeyboardState(); // Determina lo que se presiono en el teclado
        public static KeyboardState previous_keyboard_state = new KeyboardState();
        //public static GamePadState[] currentGamePadState = new GamePadState[4]; // Determina lo que se presiono en el gamepad
        //public static GamePadState[] previousGamePadState = new GamePadState[4];

        public static int viewport_height;
        public static int viewport_width;

        //public static int FrameHeight = 240;
        //public static int FrameWidth = 320;
        public static float scalar = 6; // Mas grande es el numero mas chico es el personaje
        
        // Crea lista del objeto Being, el cual alberga a los personajes y a los enemigos
        public static List<Being> players = new List<Being>();
        public static int players_quant = 4;
        public static int enemies_quant = 8;
        public static int total_quant = players_quant + enemies_quant;

        /// Este nombre corresponde al de los archivos, asi que si cambio estos tambien tengo que ir a cambiar las carpetas de los mismos
        public static string[] characters = new string[7] { "Paladin", "Paladina", "Barbaro", "Barbara", "Arquero", "Arquera", "IA_1" }; 
        public static string[] scenes = new string[] { "Avance", "Versus", "Titulo" };

        // Las distintas armaduras o skins que puede llevar y las piezas de animacion
        public static List<Textures> paladin_textures = new List<Textures>();
        public static List<Textures> skeleton_textures = new List<Textures>();

        // Los distintos niveles de avance y los tamaños de sus tiles
        public static List<Textures> level_1textures = new List<Textures>();

        // Los distintos niveles de versus
        public static List<Textures> versus_textures = new List<Textures>();

        // Las distintas capas de parallax
        public static List<Parallax> background_layers = new List<Parallax>();
        public static List<Parallax> front_layers = new List<Parallax>();

        // Textures de titulos
        public static Texture2D title_screen;
        // Textura de Seleccion
        public static Texture2D selection_screen;
        public static Texture2D selector;

        // Rectangulos de colisiones para chequear y su textura, 
        // solo se implementa en el modo de creación, no se utiliza para el producto final.
        public static Rectangle rectangle_collision;
        public static Rectangle rectangle_collision_2;
        public static Texture2D white_dot;
        public static Boolean enable_rectangles = false;

        // Texturas de UI e InGameInv
        public static List<Textures> UITextures = new List<Textures>();

        // UI de vida
        public static int ui_widht = 450; //100;
        public static int ui_height = 550; //150;
        public static int ui_y = 78;
        public static float max_bar_length = 49;
        
        // InGameInv
        public static int inv_width = 41;
        public static int inv_height = 41;
        public static int inv_bar_width = 328;
        public static int inv_bar_height = 41;

        // Para llevar la cuenta de los frames por segundo
        public static TimeSpan elapsed_time = TimeSpan.Zero;
        public static int frame_rate = 0;
        public static int frame_counter = 0;
        
        public static Random randomly = new Random();

        //-//-// METHODS //-//-//
        /// <summary>
        /// Dibuja lineas rectas
        /// </summary>
        /// <param name="A">Punto A</param>
        /// <param name="B">Punto B</param>
        /// <param name="tex">Textura utilizada para dibujar las lineas</param>
        /// <param name="col">Color de las lineas</param>
        /// <param name="spriteBatch">Proceso de dibujado</param>
        /// <param name="thickness">Grosor de la linea</param>
        public static void DrawStraightLine(Vector2 A, Vector2 B, Texture2D tex, Color col, int thickness)
        {
            Rectangle rec;
            if (A.X < B.X)
            {
                rec = new Rectangle((int)A.X, (int)A.Y, (int)(B.X - A.X), thickness);
            }
            else
            {
                rec = new Rectangle((int)A.X, (int)A.Y, thickness, (int)(B.Y - A.Y));
            }

            Global.sprite_batch.Draw(tex, rec, col);
        }

        /// <summary>
        /// Dibuja los rectangulos que delimitan las colisiones del personaje.
        /// Se utiliza en la instancia de creación solamente, se quitan para el producto final.
        /// </summary>
        /// <param name="rec">Rectangulo del personaje</param>
        /// <param name="tex">Textura utilizada para dibujar el rectangulo</param>
        /// <param name="spriteBatch">Proceso de dibujado</param>
        public static void DrawRectangle(Rectangle rec, Texture2D tex)
        {
            int border = 1;
            int borderWidth = rec.Width + (border * 2);
            int borderHeight = rec.Height + (border);
            Global.DrawStraightLine(new Vector2(rec.X, rec.Y), new Vector2(rec.X + rec.Width, rec.Y), tex, Color.White, border);
            Global.DrawStraightLine(new Vector2(rec.X, rec.Y + rec.Height), new Vector2(rec.X + rec.Width, rec.Y + rec.Height), tex, Color.White, border);
            Global.DrawStraightLine(new Vector2(rec.X, rec.Y), new Vector2(rec.X, rec.Y + rec.Height), tex, Color.White, border);
            Global.DrawStraightLine(new Vector2(rec.X + rec.Width, rec.Y), new Vector2(rec.X + rec.Width, rec.Y + rec.Height), tex, Color.White, border);
        }

        /// <summary>
        /// La tecla se activa cuando se suelta despues de haber sido presionada.
        /// </summary>
        /// <param name="Tecla">La tecla que queremos verificar si se presiono</param>
        /// <returns></returns>
        public static bool OnePulseKey(Keys Tecla)
        {
            if (previous_keyboard_state.IsKeyDown(Tecla) && current_keyboard_state.IsKeyUp(Tecla))
                return true;
            else
                return false;
        }
    }
}
