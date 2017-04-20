using HLG.Abstracts.GUI;
using HLG.Objects;
using Microsoft.Xna.Framework;

namespace HLG.Abstracts.Beings
{
    public abstract class Playable_Characters : Being
    {

        //-//-// VARIABLES //-//-//
        public User_Interface gui = new User_Interface();

        //-//-// METHODS //-//-//
        public override void DrawWithParallax()
        {
            foreach (Animation animationPiecesItem in animationPieces)
            {
                animationPiecesItem.facing = facing;
                animationPiecesItem.Draw();
            }

            // Si no separo este proceso de dibujo desconcha las posiciones de las capas del jugador
            // +++ Me parece que esto se soluciono cuando cambie el parametro de dibujo en el draw general +++
            Global.spriteBatch.DrawString(Global.checkStatusFont2,
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
            if (Global.enableRectangles)
            {
                Global.DrawRectangle(GetPositionRec(), Global.whiteDot);
                Global.DrawRectangle(Global.rectangleCollision, Global.whiteDot);
                Global.DrawRectangle(Global.rectangleCollision2, Global.whiteDot);
            }
        }
        public override void DrawWithoutParallax()
        {
            gui.DrawUI(currentHealth, maxHealth);
        }
        public void StayInScreen(int AltoNivel)
        {
            if (Global.camara.screenLimits.Right != 0)
            {
                Rectangle FrameEscalado = GetPositionRec();
                positionX = MathHelper.Clamp(position.X, Global.camara.screenLimits.Left + FrameEscalado.Width / 2, Global.camara.screenLimits.Width - FrameEscalado.Width / 2);
                positionY = MathHelper.Clamp(position.Y, AltoNivel - AltoNivel / 2, AltoNivel - FrameEscalado.Height / 2);
            }
        }

    }
}
