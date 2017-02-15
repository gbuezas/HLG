using HLG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HLG.Abstracts.GUI
{
    public class User_Interface
    {
        //-//-// VARIABLES //-//-//
        protected Animation UIAnimation;
        protected Vector2 UILifeNumber;
        protected float actual_bar_length;
        protected Color bar_color;

        //-//-// METHODS //-//-//
        public void initialize(int index )
        {
            UIAnimation = new Animation();
            UIAnimation.LoadTexture(Global.UITextures[2], new Vector2((int)(Global.camara.parallax.X + Global.viewport_width / 5) * (index + 1), Global.ui_y),
                                        Global.ui_widht,
                                        Global.ui_height,
                                        2,
                                        Color.White,
                                        true);

            UILifeNumber.X = ((Global.viewport_width / 5) * (index + 1)) - 25;
            UILifeNumber.Y = Global.ui_y + 8;
        }

        public void UpdateGUI(GameTime gameTime, int currentHealth, int maxHealth)
        {

            UIAnimation.frameTime = 300;
            UIAnimation.Update(gameTime);

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
        }
    }
}
