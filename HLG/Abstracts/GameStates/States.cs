using HLG.Abstracts.Beings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace HLG.Abstracts.GameStates
{
    public abstract class States
    {

        //-//-// VARIABLES //-//-//
        public Global.EstadosJuego ongoingState { get; internal set; }

        //-//-// METHODS //-//-//
        public abstract void Initialize();

        public abstract void Load(Viewport _viewport);
        public abstract void Update(GameTime gameTime);
        public abstract void UpdateState(GameTime gameTime);
        public abstract void Draw();

        public void SortAndDrawCharacters()
        {
            var AxisList = Global.players.Where(item => Global.camara.InsideCamera(item.GetPositionRec()));
            foreach (var AxisListItem in AxisList.OrderBy(item => item.positionY))
                AxisListItem.DrawWithParallax();
        }
        public void InputManagement()
        {
            // Guarda los estados anteriores del joystick y del teclado
            Global.previousKeyboardState = Global.currentKeyboardState;
            Global.currentKeyboardState = Keyboard.GetState();

            //for (int i = 0; i < 4;i++ )
            //{
            //    Variables_Generales.previousGamePadState[i] = Variables_Generales.currentGamePadState[i];
            //    Variables_Generales.currentGamePadState[i] = GamePad.GetState((PlayerIndex)i);
            //}

        }

    }
}
