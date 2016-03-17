using HLG.Abstracts.Beings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

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

        // Dibujar estado
        public abstract void Draw(SpriteBatch spriteBatch);

        // Actualizar comportamiento de estado
        public abstract void UpdateState(GameTime gameTime);

        // Ordenar lista de personajes segun su eje Y
        // Ahora con los enemigos se manejan 2 listas lo que complico las cosas a la hora de dibujar el eje Y ordenadamente.
        public void Reordenar_Personajes(SpriteBatch spriteBatch)
        {
            // Genero una lista para todas las coordenadas de los personajes y las agrego
            List<float> Lista_Coordenadas = new List<float>();

            // Agrego personajes y enemigos
            foreach (Being Jugador in Global.players)
            {
                Lista_Coordenadas.Add(Jugador.GetPositionVec().Y);

                // Reseteo el estado de dibujado
                Jugador.drawn = false;
            }

            // Ordeno la lista y la invierto para obtener el efecto buscado
            Lista_Coordenadas.Sort();

            // Ahora por cada elemento de la lista ordenada comparo quien tiene el eje del primer elemento y lo dibujo
            // despues con el segundo y asi sucesivamente
            foreach (float Coordenada in Lista_Coordenadas)
            {
                foreach (Being Jugador in Global.players)
                {
                    // GAB - Fijarse si se estan dibujando los esqueletos fuera de la pantalla, aca o en la IA
                    if (Jugador.drawn == false && Jugador.GetPositionVec().Y == Coordenada)
                    {
                        Jugador.Draw(spriteBatch);

                        // Lo pongo como dibujado para que no lo repita
                        Jugador.drawn = true;
                    }
                }
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
