using HLG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HLG.Abstracts.GUI
{
    public class User_Interface
    {
        //-//-// VARIABLES //-//-//
        protected Animation uiAnimationPortrait;
        protected Animation uiAnimationInventory;

        protected Textures currentInventoryPiece;
        
        protected Vector2 itemNameVector;
        public string[] itemsName { get; internal set; } = null;
        
        protected int playerIndex;

        protected Vector2 uiLifeNumber;
        protected float actualBarLength;
        protected Color barColor;

        //-//-// METHODS //-//-//
        public void Initialize(int index, string[] inventoryEquipment)
        {
            playerIndex = index;
            
            uiAnimationPortrait = new Animation();
            uiAnimationPortrait.LoadTexture(Global.uiTextures[2], new Vector2((int)(Global.camara.parallax.X + Global.viewportWidth / 5) * (index + 1), Global.uiY), 
                                    Global.uiWidht, Global.uiHeight, 2, Color.White, true);

            uiAnimationInventory = new Animation();
            uiAnimationInventory.LoadTexture(Global.uiTextures[1], new Vector2(uiAnimationPortrait.position.X, 20), 41, 41, 2, Color.White, true);
            uiAnimationInventory.pause = true;
            
            itemNameVector = new Vector2(uiAnimationInventory.position.X, uiAnimationInventory.position.Y);
            itemsName = inventoryEquipment;

            //currentInventoryPiece = new Animation();
            currentInventoryPiece = new Textures();
            currentInventoryPiece.texturePieceName = Global.players[playerIndex].animationPieces[2].loadedTexture.texturePieceName;
            currentInventoryPiece.textureSetName = Global.players[playerIndex].animationPieces[0].loadedTexture.textureSetName;
            
            uiLifeNumber.X = ((Global.viewportWidth / 5) * (index + 1)) - 25;
            uiLifeNumber.Y = Global.uiY + 8;
        }

        public void UpdateGUI(GameTime gameTime, int currentHealth, int maxHealth)
        {
            uiAnimationPortrait.frameTime = 300;
            uiAnimationPortrait.Update(gameTime);

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
                foreach (Animation animationPiecesItem in Global.players[playerIndex].animationPieces)
                {
                    if (animationPiecesItem.loadedTexture.texturePieceName == itemsName[uiAnimationInventory.currentFrame])
                        currentInventoryPiece.texturePieceName = animationPiecesItem.loadedTexture.texturePieceName;
                }
            }

            int indicedesetmostrado = The_Game.allSetsNames.IndexOf(currentInventoryPiece.textureSetName);

            if ((Global.OnePulseKey(Keys.Up)) && indicedesetmostrado < The_Game.allSetsNames.Count - 1)
                indicedesetmostrado++;
            
            if ((Global.OnePulseKey(Keys.Down)) && indicedesetmostrado > 0)
                indicedesetmostrado--;

            currentInventoryPiece.textureSetName = The_Game.allSetsNames[indicedesetmostrado];
            
            /// Los calculos del tamaño y el color de la barra de vida estan hechos con regla de 3 simple
            actualBarLength = currentHealth * Global.maxBarLength / maxHealth;
            barColor = new Color(255 - (int)(actualBarLength * 210 / Global.maxBarLength), (int)(actualBarLength * 210 / Global.maxBarLength), 0);
        }

        public void DrawUI(int currentHealth, int maxHealth)
        {
            // Dibuja UI animada (escuditos)
            uiAnimationPortrait.Draw();

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


            Global.spriteBatch.DrawString(Global.checkStatusFont, currentInventoryPiece.textureSetName, itemNameVector, Color.DarkRed);
            
        }

        public Textures GetCurrentPiece()
        {
            return currentInventoryPiece;
        }
        
    }
}
