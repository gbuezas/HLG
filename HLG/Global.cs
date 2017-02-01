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
        // Fuente de los mensajes de chequeo
        public static SpriteFont CheckStatusVar;
        public static SpriteFont CheckStatusVar_2;
        public static float mensaje1;
        public static float mensaje2;
        public static float mensaje3;
        public static float mensaje4;
        public static float mensaje5;

        // Camara - Se setea en el Load() de cada nivel
        public static Camera Camara;

        // Colores, tintes
        public static Color ColorGhost = new Color(255, 255, 255, 30);
        public static Color[] SkeletonRandomColors = new Color[] { Color.AntiqueWhite, Color.Wheat, Color.WhiteSmoke, Color.SeaShell, Color.OldLace, Color.LightYellow, Color.Gainsboro, Color.Cornsilk };
        
        // La clase de los estados del juego
        public static States CurrentState;

        // Determina lo que se presiono en el teclado y gamepad
        public static KeyboardState currentKeyboardState = new KeyboardState();
        public static KeyboardState previousKeyboardState = new KeyboardState();
        //public static GamePadState[] currentGamePadState = new GamePadState[4];
        //public static GamePadState[] previousGamePadState = new GamePadState[4];

        // Dimensiones de la pantalla
        public static int ViewportHeight;
        public static int ViewportWidth;

        // Dimensiones del frame de los personajes y escala de los mismos
        public static int FrameHeight = 240;
        public static int FrameWidth = 320;
        // Mas grande es el numero mas chico es el personaje
        public static float Scalar = 6;

        // Dimensiones de los iconos del inventario
        //public static int IconHeight, IconWidht = 41;

        // Crea lista del objeto Being, el cual alberga a los personajes y a los enemigos
        public static List<Being> players = new List<Being>();
        public static int playersQuant = 4;
        public static int enemiesQuant = 8;
        public static int totalQuant = playersQuant + enemiesQuant;

        // Para donde mira el personaje
        public enum Mirada { RIGHT, LEFT };

        // Las distintas acciones que puede hacer con sus respectivos frames y los controles
        public enum Actions { WALK, STAND, HIT1, HIT2, HIT3, DEAD }
        public enum Controls { UP, DOWN, LEFT, RIGHT, BUTTON_HIT, BUTTON_2 }

        // Los distintos estados del juego
        public enum EstadosJuego { INTRO, TITULO, SELECCION, MAPA, VS, AVANCE, PAUSA, GAMEOVER, FINAL }

        // Los distintos parametros de busqueda de objetivo de la IA
        public enum TargetCondition { MAXHEALTH, MINHEALTH/*, MAXMONEY, MINMONEY */}

        // Los distintos heroes
        public static string[] Heroes = new string[7] { "Paladin", "Paladina", "Barbaro", "Barbara", "Arquero", "Arquera", "IA_1" };

        // El orden de los items influye directamente en el orden en el que se dibujan las piezas.
        // Cada clase tiene su set de items (paladin, barbaro, etc)
        public static string[] PiecesPaladin = new string[] { "shield", "gauntletback", "greaveback", "breastplate", "helm", "tasset", "greavetop", "sword", "gauntlettop" };
        public static string[] PiecesBarbaro = new string[] { "gauntletback", "greaveback", "breastplate", "helm", "tasset", "greavetop", "sword", "gauntlettop" };
        public static string[] PiecesIA_1 = new string[] { "gauntletback", "greaveback", "breastplate", "helm", "tasset", "greavetop", "gauntlettop" };
        public static string[] IconBarSlots = new string[] { "greaves", "gauntlets", "breastplates", "helms", "shields", "swords", "weapons", "items" };
        public static List<string> Armors = new List<string>();

        // Los distintos estilos de escenarios
        public static string[] Scenes = new string[] { "Avance", "Versus", "Titulo" };

        // Las distintas armaduras o skins que puede llevar y las piezas de animacion
        public static List<Textures> PaladinTextures = new List<Textures>();
        public static List<Textures> IA_BasicTextures = new List<Textures>();

        // Los distintos niveles de avance y los tamaños de sus tiles
        public static List<Textures> Level_1Textures = new List<Textures>();

        // Los distintos niveles de versus
        public static List<Textures> VersusTextures = new List<Textures>();

        // Las distintas capas de parallax
        public static List<Parallax> Background_Layers = new List<Parallax>();
        public static List<Parallax> Front_Layers = new List<Parallax>();

        // Textures de titulos
        public static Texture2D Pantalla_Titulo;
        // Textura de Seleccion
        public static Texture2D Pantalla_Seleccion;
        public static Texture2D Selector;

        // Rectangulos de colisiones para chequear y su textura, 
        // solo se implementa en el modo de creación, no se utiliza para el producto final.
        public static Rectangle Rectangle_Collision;
        public static Rectangle Rectangle_Collision_2;
        public static Texture2D Punto_Blanco;
        public static Boolean EnableRectangles = false;

        // Texturas de UI e InGameInv
        public static List<Textures> UITextures = new List<Textures>();

        // UI de vida
        public static int UIancho = 450; //100;
        public static int UIalto = 550; //150;
        public static int UIy = 78;
        public static float max_bar_length = 49;
        
        // InGameInv
        public static int InvAncho = 41;
        public static int InvAlto = 41;
        public static int InvBarAncho = 328;
        public static int InvBarAlto = 41;

        // Para llevar la cuenta de los frames por segundo
        public static TimeSpan elapsedTime = TimeSpan.Zero;
        public static int frameRate = 0;
        public static int frameCounter = 0;
        
        /// <summary>
        /// Variable random
        /// </summary>
        public static Random randomly = new Random();
        
        /// <summary>
        /// Dibuja lineas rectas
        /// </summary>
        /// <param name="A">Punto A</param>
        /// <param name="B">Punto B</param>
        /// <param name="tex">Textura utilizada para dibujar las lineas</param>
        /// <param name="col">Color de las lineas</param>
        /// <param name="spriteBatch">Proceso de dibujado</param>
        /// <param name="thickness">Grosor de la linea</param>
        public static void DrawStraightLine(Vector2 A, Vector2 B, Texture2D tex, Color col, SpriteBatch spriteBatch, int thickness)
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

            spriteBatch.Draw(tex, rec, col);
        }

        /// <summary>
        /// Dibuja los rectangulos que delimitan las colisiones del personaje.
        /// Se utiliza en la instancia de creación solamente, se quitan para el producto final.
        /// </summary>
        /// <param name="rec">Rectangulo del personaje</param>
        /// <param name="tex">Textura utilizada para dibujar el rectangulo</param>
        /// <param name="spriteBatch">Proceso de dibujado</param>
        public static void DrawRectangle(Rectangle rec, Texture2D tex, SpriteBatch spriteBatch)
        {
            int border = 1;
            int borderWidth = rec.Width + (border * 2);
            int borderHeight = rec.Height + (border);
            Global.DrawStraightLine(new Vector2(rec.X, rec.Y), new Vector2(rec.X + rec.Width, rec.Y), tex, Color.White, spriteBatch, border);
            Global.DrawStraightLine(new Vector2(rec.X, rec.Y + rec.Height), new Vector2(rec.X + rec.Width, rec.Y + rec.Height), tex, Color.White, spriteBatch, border);
            Global.DrawStraightLine(new Vector2(rec.X, rec.Y), new Vector2(rec.X, rec.Y + rec.Height), tex, Color.White, spriteBatch, border);
            Global.DrawStraightLine(new Vector2(rec.X + rec.Width, rec.Y), new Vector2(rec.X + rec.Width, rec.Y + rec.Height), tex, Color.White, spriteBatch, border);
        }

        /// <summary>
        /// La tecla se activa cuando se suelta despues de haber sido presionada.
        /// </summary>
        /// <param name="Tecla">La tecla que queremos verificar si se presiono</param>
        /// <returns></returns>
        public static bool OnePulseKey(Keys Tecla)
        {
            //previousKeyboardState = currentKeyboardState;
            //currentKeyboardState = Keyboard.GetState();

            if (previousKeyboardState.IsKeyDown(Tecla) && currentKeyboardState.IsKeyUp(Tecla))
                return true;
            else
                return false;
        }
    }
}
