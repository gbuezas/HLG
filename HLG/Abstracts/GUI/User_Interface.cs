using HLG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HLG.Abstracts.GUI
{
    public class User_Interface
    {
        //-//-// VARIABLES //-//-//
        protected Animation uiAnimation;
        protected Animation uiAnimationInventory;
        static protected Animation currentPiece;

        //protected string item_name = string.Empty;
        protected Vector2 itemNameVector;
        protected string itemName = "BLANK";
        public string[] itemsName { get; internal set; } = null;
        //protected List<Animation> UIInventory = new List<Animation>();
        int playerIndex;
        int setsQuantity;
        //int current_set;
        //string chequeo = string.Empty;

        //List<string> losdistintossets;

        protected Vector2 uiLifeNumber;
        protected float actualBarLength;
        protected Color barColor;

        //-//-// METHODS //-//-//
        //public void initialize(int index, string[] inventory_equipment, List<string> sets )
        public void Initialize(int index, string[] inventory_equipment)
        {
            playerIndex = index;
            setsQuantity = Global.players[playerIndex].objectTextures.Count;
            //current_set = 0;


            uiAnimation = new Animation();
            uiAnimation.LoadTexture(
                Global.uiTextures[2], 
                new Vector2((int)(Global.camara.parallax.X + Global.viewportWidth / 5) * (index + 1), Global.uiY),
                Global.uiWidht,
                Global.uiHeight,
                2,
                Color.White,
                true);

            uiAnimationInventory = new Animation();
            uiAnimationInventory.LoadTexture(
                Global.uiTextures[1], 
                new Vector2(uiAnimation.position.X, 20),
                41,
                41,
                2,
                Color.White,
                true);
            uiAnimationInventory.pause = true;

            itemNameVector = new Vector2(uiAnimationInventory.position.X, uiAnimationInventory.position.Y);
            itemsName = inventory_equipment;

            uiLifeNumber.X = ((Global.viewportWidth / 5) * (index + 1)) - 25;
            uiLifeNumber.Y = Global.uiY + 8;
            
            //losdistintossets = sets;
            
        }

        public void UpdateGUI(GameTime gameTime, int currentHealth, int maxHealth)
        {

            uiAnimation.frameTime = 300;
            uiAnimation.Update(gameTime);

            uiAnimationInventory.frameTime = 3000;
            uiAnimationInventory.Update(gameTime);
            uiAnimationInventory.SetScale(20);

            int chequeoframe = uiAnimationInventory.currentFrame;
            if ((Global.OnePulseKey(Keys.Right)) && uiAnimationInventory.currentFrame < uiAnimationInventory.frameCount - 1)
                uiAnimationInventory.currentFrame++;

            if ((Global.OnePulseKey(Keys.Left)) && uiAnimationInventory.currentFrame > 0)
                uiAnimationInventory.currentFrame--;

            if (uiAnimationInventory.currentFrame != chequeoframe)
            {
                string temp = itemsName[uiAnimationInventory.currentFrame];
                foreach (Animation animationPiecesItem in Global.players[playerIndex].animationPieces)
                {
                    if (animationPiecesItem.loadedTexture.texturePieceName == temp)
                    {
                        itemName = animationPiecesItem.loadedTexture.textureSetName;
                        chequeoframe = uiAnimationInventory.currentFrame;
                        currentPiece = animationPiecesItem;
                    }
                }
            }

            int indicedesetmostrado = The_Game.allSetsNames.IndexOf(itemName);

            /// ACA ESTA EL QUILOMBO - el cambio en el current cambia el original
            if ((Global.OnePulseKey(Keys.Up)) && indicedesetmostrado < The_Game.allSetsNames.Count - 1)
            {
                indicedesetmostrado++;
                itemName = The_Game.allSetsNames[indicedesetmostrado];
                currentPiece.loadedTexture.textureSetName = itemName;
            }
                
            if ((Global.OnePulseKey(Keys.Down)) && indicedesetmostrado > 0)
            {
                indicedesetmostrado--;
                itemName = The_Game.allSetsNames[indicedesetmostrado];
                currentPiece.loadedTexture.textureSetName = itemName;
            }
            /// ACA ESTA EL QUILOMBO - el cambio en el current cambia el original

            /// Los calculos del tamaño y el color de la barra de vida estan hechos con regla de 3 simple
            actualBarLength = currentHealth * Global.maxBarLength / maxHealth;
            barColor = new Color(255 - (int)(actualBarLength * 210 / Global.maxBarLength), (int)(actualBarLength * 210 / Global.maxBarLength), 0);
        }

        public void DrawUI(int currentHealth, int maxHealth)
        {
            /// Para obtener los 4 puntos donde tiene que dibujarse cada UI, obtener el punto Y es innecesario, el mismo es siempre 0 ya que se
            /// usan las transparencias de la imagen para obtener el espacio necesario del eje Y.
            //int UIx = (int)(Global.Camara.parallax.X + Global.ViewportWidth / 5);

            // Dibuja UI animada (escuditos)
            uiAnimation.Draw();

            uiAnimationInventory.Draw();

            // Dibujar barra de vida
            Global.DrawStraightLine(new Vector2(uiLifeNumber.X, uiLifeNumber.Y),
                                    new Vector2(uiLifeNumber.X + actualBarLength, uiLifeNumber.Y),
                                    Global.whiteDot,
                                    barColor,
                                    14);

            // Vida en numeros
            Global.spriteBatch.DrawString(Global.checkStatusFont2,
                                   currentHealth.ToString() + " / " + maxHealth.ToString(),
                                   uiLifeNumber,
                                   Color.White);


            Global.spriteBatch.DrawString(Global.checkStatusFont, itemName, itemNameVector, Color.DarkRed);
            
        }

        public Animation GetCurrentItem()
        {
            return currentPiece;
        }
    }
}
