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

        public static SpriteBatch spriteBatch; // Variable utilizada para todo el dibujado
        
        public static SpriteFont checkStatusFont; // Fuente de los mensajes de chequeo
        public static SpriteFont checkStatusFont2;
        public static float mensaje1;
        public static float mensaje2;
        public static float mensaje3;
        public static float mensaje4;
        public static float mensaje5;

        public static Camera camara; // Camara - Se setea en el Load() de cada nivel

        public static Color colorGhost = new Color(255, 255, 255, 30);
        public static Color[] skeletonRandomColors = new Color[] { Color.AntiqueWhite, Color.Wheat, Color.WhiteSmoke, Color.SeaShell, Color.OldLace, Color.LightYellow, Color.Gainsboro, Color.Cornsilk };
        
        public static States currentGameState;

        public static KeyboardState currentKeyboardState = new KeyboardState(); // Determina lo que se presiono en el teclado
        public static KeyboardState previousKeyboardState = new KeyboardState();
        //public static GamePadState[] currentGamePadState = new GamePadState[4]; // Determina lo que se presiono en el gamepad
        //public static GamePadState[] previousGamePadState = new GamePadState[4];

        public static int viewportHeight;
        public static int viewportWidth;

        //public static int FrameHeight = 240;
        //public static int FrameWidth = 320;
        public static float scalar = 6; // Mas grande es el numero mas chico es el personaje
        
        // Crea lista del objeto Being, el cual alberga a los personajes y a los enemigos
        public static List<Being> players = new List<Being>();
        public static int playersQuant = 4;
        public static int enemiesQuant = 8;
        public static int totalQuant = playersQuant + enemiesQuant;

        /// Este nombre corresponde al de los archivos, asi que si cambio estos tambien tengo que ir a cambiar las carpetas de los mismos
        public static string[] characters = new string[7] { "Paladin", "Paladina", "Barbaro", "Barbara", "Arquero", "Arquera", "Skeleton" }; 
        public static string[] scenes = new string[] { "Avance", "Versus", "Titulo" };

        // Las distintas armaduras o skins que puede llevar y las piezas de animacion
        public static List<Textures> paladinTextures = new List<Textures>();
        public static List<Textures> skeletonTextures = new List<Textures>();

        // Los distintos niveles de avance y los tamaños de sus tiles
        public static List<Textures> level1Textures = new List<Textures>();

        // Los distintos niveles de versus
        public static List<Textures> versusTextures = new List<Textures>();

        // Las distintas capas de parallax
        public static List<Parallax> backgroundLayers = new List<Parallax>();
        public static List<Parallax> frontLayers = new List<Parallax>();

        // Textures de titulos
        public static Texture2D titleScreen;
        // Textura de Seleccion
        public static Texture2D selectionScreen;
        public static Texture2D selector;

        // Rectangulos de colisiones para chequear y su textura, 
        // solo se implementa en el modo de creación, no se utiliza para el producto final.
        public static Rectangle rectangleCollision;
        public static Rectangle rectangleCollision2;
        public static Texture2D whiteDot;
        public static Boolean enableRectangles = false;

        // Texturas de UI e InGameInv
        public static List<Textures> uiTextures = new List<Textures>();

        // UI de vida
        public static int uiWidht = 450; //100;
        public static int uiHeight = 550; //150;
        public static int uiY = 78;
        public static float maxBarLength = 49;
        
        // InGameInv
        public static int invWidth = 41;
        public static int invHeight = 41;
        public static int invBarWidth = 328;
        public static int invBarHeight = 41;

        // Para llevar la cuenta de los frames por segundo
        public static TimeSpan elapsedTime = TimeSpan.Zero;
        public static int frameRate = 0;
        public static int frameCounter = 0;
        
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

            Global.spriteBatch.Draw(tex, rec, col);
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
            if (previousKeyboardState.IsKeyDown(Tecla) && currentKeyboardState.IsKeyUp(Tecla))
                return true;
            else
                return false;
        }

    }
}
