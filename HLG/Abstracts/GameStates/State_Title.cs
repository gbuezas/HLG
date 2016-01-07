using HLG.Objects;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HLG.Abstracts.GameStates
{
    class State_Title : States
    {
        // El area de la linea de sprite que queremos mostrar
        Rectangle sourceRect;

        int Var_AltoNivel = Global.ViewportHeight;
        int Var_AnchoNivel = Global.ViewportWidth;

        // Traigo la camara del game
        Camera CamaraTraida;

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override void Load(Viewport _viewport)
        {
            CamaraTraida = new Camera(_viewport, Var_AltoNivel, Var_AnchoNivel);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // Agarro el cuadro correcto
            sourceRect = new Rectangle(0, 0, Global.ViewportWidth,
                Global.ViewportHeight);

            // Guarda y lee los estados actuales y anteriores del joystick y teclado
            Input_Management();

            // Actualiza el estado del juego
            UpdateState(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Global.Pantalla_Titulo, sourceRect, Color.White);
            spriteBatch.End();
        }

        public override void UpdateState(Microsoft.Xna.Framework.GameTime gameTime)
        {

            //if (Variables_Generales.currentGamePadState[0].Buttons.A == ButtonState.Pressed)
            //{
            //    Variables_Generales.Estado_Actual.Estado_ejecutandose = Variables_Generales.EstadosJuego.SELECCION;
            //}

            if ((Keyboard.GetState().IsKeyDown(Keys.A)))
            {
                Global.CurrentState.Estadoejecutandose = Global.EstadosJuego.SELECCION;
            }

        }
        
    }
}
