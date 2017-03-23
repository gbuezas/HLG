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
        public Global.EstadosJuego ongoing_state { get; internal set; }

        //-//-// METHODS //-//-//
        public abstract void Initialize();

        public abstract void Load(Viewport _viewport);
        public abstract void Update(GameTime gameTime);
        public abstract void Update_State(GameTime gameTime);
        public abstract void Draw();

        public void RearrangeCharacters()
        {
            // Ordeno los personajes a dibujar segun su eje Y en una nueva lista usando LinQ
            List<Being> AxisList = Global.players.OrderBy(item => item.positionY).ToList();

            // Si los personajes estan en camara los dibujo en pantalla.
            foreach (Being character in AxisList)
            {
                if (Global.camara.EnCamara(character.Get_Position_Rec()))
                    character.Draw_With_Parallax();
            }
        }
        public void InputManagement()
        {
            // Guarda los estados anteriores del joystick y del teclado
            Global.previous_keyboard_state = Global.current_keyboard_state;
            Global.current_keyboard_state = Keyboard.GetState();

            //for (int i = 0; i < 4;i++ )
            //{
            //    Variables_Generales.previousGamePadState[i] = Variables_Generales.currentGamePadState[i];
            //    Variables_Generales.currentGamePadState[i] = GamePad.GetState((PlayerIndex)i);
            //}

        }

    }
}
