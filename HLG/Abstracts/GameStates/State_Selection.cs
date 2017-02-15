using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HLG.Abstracts.GameStates
{
    class State_Selection : States
    {
        // El area de la linea de sprite que queremos mostrar
        Rectangle sourceRect;
        Rectangle[] Fichas = new Rectangle[4];

        int Var_AltoNivel = Global.viewport_height;
        int Var_AnchoNivel = Global.viewport_width;

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override void Load(Viewport _viewport)
        {

        }

        public override void Update(GameTime gameTime)
        {
            // Agarro el cuadro correcto
            sourceRect = new Rectangle(0, 0, Global.viewport_width,
                Global.viewport_height);

            Fichas[0] = new Rectangle(0, 0, 200, 150);
            Fichas[1] = new Rectangle(200, 0, 200, 150);
            Fichas[2] = new Rectangle(400, 0, 200, 150);
            Fichas[3] = new Rectangle(600, 0, 200, 150);

            // Guarda y lee los estados actuales y anteriores del joystick y teclado
            InputManagement();

            // Actualiza el estado del juego
            UpdateState(gameTime);
        }

        public override void Draw()
        {
            Global.sprite_batch.Begin();
            Global.sprite_batch.Draw(Global.selection_screen, sourceRect, Color.White);
            //spriteBatch.Draw(Variables_Generales.Selector, new Vector2(0,0), Fichas[0], Color.White);
            //spriteBatch.Draw(Variables_Generales.Selector, new Vector2(200,0), Fichas[1], Color.White);
            //spriteBatch.Draw(Variables_Generales.Selector, new Vector2(400,0), Fichas[2], Color.White);
            //spriteBatch.Draw(Variables_Generales.Selector, new Vector2(600,0), Fichas[3], Color.White);
            Global.sprite_batch.End();
        }

        public override void UpdateState(GameTime gameTime)
        {
            //if (Variables_Generales.currentGamePadState[0].Buttons.B == ButtonState.Pressed)
            //{
            //    Variables_Generales.Estado_Actual.Estado_ejecutandose = Variables_Generales.EstadosJuego.AVANCE;
            //}

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                Global.current_game_state.ongoing_state = Global.EstadosJuego.AVANCE;
            }
        }
        
    }
}
