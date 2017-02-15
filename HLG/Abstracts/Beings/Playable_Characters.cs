using HLG.Abstracts.GUI;
using HLG.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HLG.Abstracts.Beings
{
    public abstract class Playable_Characters : Being
    {

        //-//-// VARIABLES //-//-//
        public User_Interface gui = new User_Interface();

        //-//-// METHODS //-//-//
        public override void DrawWithParallax()
        {
            foreach (Animation piezaAnimada in animation_pieces)
            {
                piezaAnimada.Draw(facing, piezaAnimada.color);
            }

            // Si no separo este proceso de dibujo desconcha las posiciones de las capas del jugador
            // +++ Me parece que esto se soluciono cuando cambie el parametro de dibujo en el draw general +++
            Global.sprite_batch.DrawString(Global.check_status_font_2,
            "Frame Actual = " + mensaje1.ToString() + System.Environment.NewLine +
            "Frame Total = " + mensaje2.ToString() + System.Environment.NewLine +
            "Direccion Actual = " + mensaje3.ToString() + System.Environment.NewLine +
            "Accion Actual = " + mensaje4.ToString() + System.Environment.NewLine +
            "Alto = " + mensaje5.ToString() + System.Environment.NewLine +
            "Ancho = " + mensaje6.ToString() + System.Environment.NewLine +
            "X = " + mensaje7.ToString() + System.Environment.NewLine +
            "Y = " + mensaje8.ToString() + System.Environment.NewLine +
            "Vida = " + mensaje9.ToString(),
            mensaje, Color.DarkRed);

            // rectangulos de colision para chequear
            if (Global.enable_rectangles)
            {
                Global.DrawRectangle(GetPositionRec(), Global.white_dot);
                Global.DrawRectangle(Global.rectangle_collision, Global.white_dot);
                Global.DrawRectangle(Global.rectangle_collision_2, Global.white_dot);
            }
        }
        public override void DrawWithoutParallax()
        {
            gui.DrawUI(current_health, max_health);
        }
        public void StayInScreen(int AltoNivel)
        {
            if (Global.camara.LimitesPantalla.Right != 0)
            {
                Rectangle FrameEscalado = GetPositionRec();
                positionX = MathHelper.Clamp(position.X, Global.camara.LimitesPantalla.Left + FrameEscalado.Width / 2, Global.camara.LimitesPantalla.Width - FrameEscalado.Width / 2);
                positionY = MathHelper.Clamp(position.Y, AltoNivel - AltoNivel / 2, AltoNivel - FrameEscalado.Height / 2);
            }
        }

    }
}
