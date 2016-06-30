using HLG.Abstracts.Beings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace HLG.Abstracts.GameStates
{
    public abstract class States
    {
        private Global.EstadosJuego EstadoEjecutandose;

        internal Global.EstadosJuego Estadoejecutandose
        {
            get
            {
                return EstadoEjecutandose;
            }

            set
            {
                EstadoEjecutandose = value;
            }
        }

        // Inicializar estado
        public abstract void Initialize();

        // Cargar la camara y otras cosas
        public abstract void Load(Viewport _viewport);

        // Actualizar estado
        public abstract void Update(GameTime gameTime);

        // Dibujar estado, puede incluir otros draw como DrawUI
        public abstract void Draw(SpriteBatch spriteBatch);

        // Actualizar comportamiento de estado
        public abstract void UpdateState(GameTime gameTime);

        // Ordenar lista de personajes segun su eje Y
        public void Reordenar_Personajes(SpriteBatch spriteBatch)
        {
            // Ordeno los personajes a dibujar segun su eje Y en una nueva lista usando LinQ
            List<Being> AxisList = Global.players.OrderBy(item => item.positionY).ToList();

            // Si los personajes estan en camara los dibujo en pantalla.
            foreach (Being character in AxisList)
            {
                if (Global.Camara.EnCamara(character.GetPositionRec()))
                    character.Draw(spriteBatch);
            }
        }

        // Obtiene los gamepads y teclado que se tocaron y se estan tocando
        public void Input_Management()
        {
            // Guarda los estados anteriores del joystick y del teclado
            Global.previousKeyboardState = Global.currentKeyboardState;

            //for (int i = 0; i < 4;i++ )
            //{
            //    Variables_Generales.previousGamePadState[i] = Variables_Generales.currentGamePadState[i];
            //    Variables_Generales.currentGamePadState[i] = GamePad.GetState((PlayerIndex)i);
            //}

        }
    }
}
