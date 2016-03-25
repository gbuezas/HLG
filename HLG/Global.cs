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

        // Colores
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

        // Crea lista de Being y enemigos
        public static List<Being> players = new List<Being>();
        public static int playersQuant = 4;
        public static int enemiesQuant = 8;
        public static int totalQuant = playersQuant + enemiesQuant;

        // Para donde mira el personaje
        public enum Mirada { RIGHT, LEFT };

        // Las distintas acciones que puede hacer con sus respectivos frames y los controles
        public enum Actions { WALK, STAND, HIT1, HIT2, HIT3, DEAD }
        public enum Controls { UP, DOWN, LEFT, RIGHT, BUTTON_1, BUTTON_2 }

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
        public static List<string> Armors = new List<string>();

        // Los distintos estilos de escenarios
        public static string[] Scenes = new string[] { "Avance", "Versus", "Titulo" };

        // Las distintas armaduras o skins que puede llevar y las piezas de animacion
        public static List<Textures> PaladinTextures = new List<Textures>();
        public static List<Textures> IA_BasicTextures = new List<Textures>();

        // Los distintos niveles de avance
        public static List<Textures> Level_1Textures = new List<Textures>();

        // Los distintos niveles de versus
        public static List<Textures> VersusTextures = new List<Textures>();

        // Las distintas capas de parallax
        public static List<Parallax> Layers = new List<Parallax>();

        // Textures de titulos
        public static Texture2D Pantalla_Titulo;
        // Textura de Seleccion
        public static Texture2D Pantalla_Seleccion;
        public static Texture2D Selector;

        // Rectangulos de colisiones para chequear y su textura
        public static Rectangle Rectangle_Collision;
        public static Rectangle Rectangle_Collision_2;
        public static Texture2D Punto_Blanco;
        public static Boolean EnableRectangles = false;

        // Para llevar la cuenta de los frames por segundo
        public static TimeSpan elapsedTime = TimeSpan.Zero;
        public static int frameRate = 0;
        public static int frameCounter = 0;
        
        /// <summary>
        /// Variable random
        /// </summary>
        public static Random randomly = new Random();
    }
}
