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
        protected Animation UIAnimation;
        protected Animation UIAnimation_inventory;

        //protected string item_name = string.Empty;
        protected Vector2 item_name_vector;
        protected string item_name = "BLANK";
        public string[] items_name { get; internal set; } = null;
        //protected List<Animation> UIInventory = new List<Animation>();
        int player_index;
        int sets_quantity;
        //int current_set;
        //string chequeo = string.Empty;

        List<string> losdistintossets;

        protected Vector2 UILifeNumber;
        protected float actual_bar_length;
        protected Color bar_color;

        //-//-// METHODS //-//-//
        public void initialize(int index, string[] inventory_equipment, List<string> sets )
        {
            player_index = index;
            sets_quantity = Global.players[player_index].objectTextures.Count;
            //current_set = 0;


            UIAnimation = new Animation();
            UIAnimation.Load_Texture(
                Global.UITextures[2], 
                new Vector2((int)(Global.camara.parallax.X + Global.viewport_width / 5) * (index + 1), Global.ui_y),
                Global.ui_widht,
                Global.ui_height,
                2,
                Color.White,
                true);

            UIAnimation_inventory = new Animation();
            UIAnimation_inventory.Load_Texture(
                Global.UITextures[1], 
                new Vector2(UIAnimation.position.X, 20),
                41,
                41,
                2,
                Color.White,
                true);
            UIAnimation_inventory.pause = true;

            item_name_vector = new Vector2(UIAnimation_inventory.position.X, UIAnimation_inventory.position.Y);
            items_name = inventory_equipment;

            UILifeNumber.X = ((Global.viewport_width / 5) * (index + 1)) - 25;
            UILifeNumber.Y = Global.ui_y + 8;
            
            losdistintossets = sets;
            
        }

        public void UpdateGUI(GameTime gameTime, int currentHealth, int maxHealth)
        {

            UIAnimation.frameTime = 300;
            UIAnimation.Update(gameTime);

            UIAnimation_inventory.frameTime = 3000;
            UIAnimation_inventory.Update(gameTime);
            UIAnimation_inventory.Set_Scale(20);

            int chequeoframe = UIAnimation_inventory.currentFrame;
            if ((Global.OnePulseKey(Keys.Right)) && UIAnimation_inventory.currentFrame < UIAnimation_inventory.frameCount - 1)
                UIAnimation_inventory.currentFrame++;

            if ((Global.OnePulseKey(Keys.Left)) && UIAnimation_inventory.currentFrame > 0)
                UIAnimation_inventory.currentFrame--;

            if (UIAnimation_inventory.currentFrame != chequeoframe)
            {
                string temp = items_name[UIAnimation_inventory.currentFrame];
                foreach (Animation temporal in Global.players[player_index].animationPieces)
                {
                    if (temporal.loadedTexture.texture_piece_name == temp)
                    {
                        item_name = temporal.loadedTexture.texture_set_name;
                        chequeoframe = UIAnimation_inventory.currentFrame;
                    }
                }
            }

            int indicedesetmostrado = losdistintossets.IndexOf(item_name);
            if ((Global.OnePulseKey(Keys.Up)) && indicedesetmostrado < losdistintossets.Count - 1)
            {
                indicedesetmostrado++;
                item_name = losdistintossets[indicedesetmostrado];
            }
                
            if ((Global.OnePulseKey(Keys.Down)) && indicedesetmostrado > 0)
            {
                indicedesetmostrado--;
                item_name = losdistintossets[indicedesetmostrado];
            }
            
            /// Los calculos del tamaño y el color de la barra de vida estan hechos con regla de 3 simple
            actual_bar_length = currentHealth * Global.max_bar_length / maxHealth;
            bar_color = new Color(255 - (int)(actual_bar_length * 210 / Global.max_bar_length), (int)(actual_bar_length * 210 / Global.max_bar_length), 0);
        }

        public void DrawUI(int currentHealth, int maxHealth)
        {
            /// Para obtener los 4 puntos donde tiene que dibujarse cada UI, obtener el punto Y es innecesario, el mismo es siempre 0 ya que se
            /// usan las transparencias de la imagen para obtener el espacio necesario del eje Y.
            //int UIx = (int)(Global.Camara.parallax.X + Global.ViewportWidth / 5);

            // Dibuja UI animada (escuditos)
            UIAnimation.Draw(Global.Facing.RIGHT);

            UIAnimation_inventory.Draw(Global.Facing.RIGHT);

            // Dibujar barra de vida
            Global.DrawStraightLine(new Vector2(UILifeNumber.X, UILifeNumber.Y),
                                    new Vector2(UILifeNumber.X + actual_bar_length, UILifeNumber.Y),
                                    Global.white_dot,
                                    bar_color,
                                    14);

            // Vida en numeros
            Global.sprite_batch.DrawString(Global.check_status_font_2,
                                   currentHealth.ToString() + " / " + maxHealth.ToString(),
                                   UILifeNumber,
                                   Color.White);


            Global.sprite_batch.DrawString(Global.check_status_font, item_name, item_name_vector, Color.DarkRed);
            
        }

        //public string GetCurrentItem()
        //{
        //    return items_name[UIAnimation_inventory.currentFrame];
        //}
    }
}
