using HLG.Objects;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HLG.Abstracts.GameStates
{
    class State_Title : States
    {

        //-//-// VARIABLES //-//-//
        Rectangle sourceRect; // El area de la linea de sprite que queremos mostrar
        Camera CamaraTraida; // Traigo la camara del game

        //-//-// METHODS //-//-//
        public override void Initialize()
        {
            throw new NotImplementedException();
        }
        public override void Load(Viewport _viewport)
        {
            CamaraTraida = new Camera(_viewport, Global.viewportHeight, Global.viewportWidth);
        }
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // Agarro el cuadro correcto
            sourceRect = new Rectangle(0, 0, Global.viewportWidth,
                Global.viewportHeight);

            // Guarda y lee los estados actuales y anteriores del joystick y teclado
            InputManagement();

            // Actualiza el estado del juego
            UpdateState(gameTime);
        }
        public override void UpdateState(Microsoft.Xna.Framework.GameTime gameTime)
        {

            //if (Variables_Generales.currentGamePadState[0].Buttons.A == ButtonState.Pressed)
            //{
            //    Variables_Generales.Estado_Actual.Estado_ejecutandose = Variables_Generales.EstadosJuego.SELECCION;
            //}

            if ((Keyboard.GetState().IsKeyDown(Keys.A)))
            {
                Global.currentGameState.ongoingState = Global.EstadosJuego.SELECCION;
            }

        }
        public override void Draw()
        {
            Global.spriteBatch.Begin();
            Global.spriteBatch.Draw(Global.titleScreen, sourceRect, Color.White);
            Global.spriteBatch.End();
        }

        
        
    }
}
