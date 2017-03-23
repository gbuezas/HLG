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

        int Var_AltoNivel = Global.viewport_height;
        int Var_AnchoNivel = Global.viewport_width;
        
        Camera CamaraTraida; // Traigo la camara del game

        //-//-// METHODS //-//-//
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
            sourceRect = new Rectangle(0, 0, Global.viewport_width,
                Global.viewport_height);

            // Guarda y lee los estados actuales y anteriores del joystick y teclado
            InputManagement();

            // Actualiza el estado del juego
            Update_State(gameTime);
        }
        public override void Update_State(Microsoft.Xna.Framework.GameTime gameTime)
        {

            //if (Variables_Generales.currentGamePadState[0].Buttons.A == ButtonState.Pressed)
            //{
            //    Variables_Generales.Estado_Actual.Estado_ejecutandose = Variables_Generales.EstadosJuego.SELECCION;
            //}

            if ((Keyboard.GetState().IsKeyDown(Keys.A)))
            {
                Global.current_game_state.ongoing_state = Global.EstadosJuego.SELECCION;
            }

        }
        public override void Draw()
        {
            Global.sprite_batch.Begin();
            Global.sprite_batch.Draw(Global.title_screen, sourceRect, Color.White);
            Global.sprite_batch.End();
        }

        
        
    }
}
