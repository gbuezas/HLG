using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HLG.Objects
{
    public class Animation
    {
        
        //-//-// VARIABLES //-//-//
        public Textures loadedTexture; // La textura con los sprites dentro
        public string pieceName; // Nombre de la pieza a animar
        private float escalaAnimacion = Global.viewport_height / Global.scalar; // Escala de Heroes con respecto al alto de la pantalla

        private int elapsedTime; // El tiempo que actualizamos el cuadro por ultima vez
        public int frameTime; // El tiempo que mostramos el cuadro antes de cambiarlo
        public bool pause; // Para pausarse en un frame especifico

        public int frameCount { get; internal set; }
        public int currentFrame { get; internal set; } // El indice del cuadro que estamos mostrando
        public Color color; // El color del cuadro que estamos mostrando
        public int frameWidth; // Ancho y alto de un cuadro dado
        public int frameHeight;
        private int oldFrameCount; // El numero de cuadros que tiene la animacion
        
        public Vector2 position; // Posicion de un cuadro determinado
        Rectangle sourceRect = new Rectangle(); // El area de la fila de sprite que queremos mostrar
        Rectangle destinationRect = new Rectangle(); // El area donde queremos mostrar el sprite
        
        public bool active; // El estado de la animacion
        public bool looping; // Activa o desactiva el loopeo
        
        //-//-// METHODS //-//-//
        /// <summary>
        /// Asignamos el nombre al principio, cuando esta todo vacio, para poder completarlo gracias a este dato.
        /// </summary>
        /// <param name="nombre"></param>
        public void Initialize(string nombre)
        {
            pieceName = nombre;
        }

        /// <summary>
        /// Carga de textura al cambiar de animacion, es la que se usa durante el juego repetidas veces.
        /// </summary>
        /// <param name="texture"></param>
        public void Load_Texture(Textures texture)
        {
            loadedTexture = texture;
            frameCount = int.Parse(texture.texture_frames);
        }
        /// <summary>
        /// Cargo la textura por primera vez, o cuando cambio el set de alguna pieza.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position">Posición Destino</param>
        /// <param name="frameWidth"></param>
        /// <param name="frameHeight"></param>
        /// <param name="frametime"></param>
        /// <param name="color"></param>
        /// <param name="looping"></param>
        public void Load_Texture(Textures texture, Vector2 position, int frameWidth, int frameHeight, int frametime, Color color, bool looping)
        {
            // Mantiene una copia local de los valores obtenidos
            this.color = color;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            frameCount = int.Parse(texture.texture_frames);
            oldFrameCount = frameCount;
            frameTime = frametime;
            this.looping = looping;
            this.position = position;
            loadedTexture = texture;

            // Pone el tiempo en 0
            elapsedTime = 0;
            currentFrame = 0;

            // Cancela la pausa
            pause = false;

            // Pone la animacion en activa por defecto
            active = true;
        }

        /// <summary>
        /// Rectangulo donde se encuentra esta pieza de animacion en la pantalla
        /// </summary>
        /// <returns></returns>
        public Rectangle Get_Position()
        {
            return destinationRect;
        }
        public void Set_Scale(int Scale)
        {
            escalaAnimacion = Global.viewport_height / Scale;
        }

        public void Color_Change(Color tinte)
        {
            color = tinte;
        }
        
        public void Update(GameTime gameTime)
        {
            // No actualizar la animacion si no esta activa
            if (active == false)
                return;

            // Actualizar el tiempo transcurrido
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Si el tiempo transcurrido es mayor al tiempo del cuadro tenemos que cambiar de cuadro
            // Si esta activo la pausa no se pasara al siguiente frame pero seguira dibujandose
            if (elapsedTime > frameTime && pause == false)
            {
                // Ir al otro cuadro
                currentFrame++;

                // Si el cuadro actual es igual a la cuenta total de cuadros pasamos el cuadro actual a 0
                if (currentFrame >= frameCount || frameCount != oldFrameCount || currentFrame >= oldFrameCount)
                {
                    currentFrame = 0;
                    oldFrameCount = frameCount;
                    // Si no hay loopeo desactivo la animacion
                    if (looping == false)
                        active = false;
                }

                // Pongo el tiempo transcurrido en 0
                elapsedTime = 0;
            }

            // Agarro el cuadro correcto en la linea de strip multiplicando el indice del cuadro actual por el ancho del frame
            sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);

            // Escalo con respecto a la altura que deseo comparando el personaje con la pantalla.
            // Se cambia el valor de la escala desde Globales.Escalar
            float AspectRatio = (float)frameHeight / frameWidth;
            int Height = (int)((escalaAnimacion) + 0.5f);
            int Width = (int)((Height / AspectRatio) + 0.5f);

            // Seteo el rectangulo donde va a ir con las dimensiones ajustadas.
            destinationRect = new Rectangle((int)position.X - Width / 2,
                                            (int)position.Y - Height / 2,
                                            Width,
                                            Height);

        }
        public void Draw(Global.Facing direccion)
        {
            // Solo dibujar la animacion si esta activa
            if (active)
            {
                if (direccion == Global.Facing.LEFT)
                {
                    Global.sprite_batch.Draw(loadedTexture.texture, destinationRect, sourceRect, color,
                        0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                }
                else
                {
                    Global.sprite_batch.Draw(loadedTexture.texture, destinationRect, sourceRect, color);
                }

            }
        }
        public void Draw(Global.Facing direccion, Color tinte)
        {
            // Solo dibujar la animacion si esta activa
            if (active)
            {
                if (direccion == Global.Facing.LEFT)
                {
                    Global.sprite_batch.Draw(loadedTexture.texture, destinationRect, sourceRect, tinte,
                        0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                }
                else
                {
                    Global.sprite_batch.Draw(loadedTexture.texture, destinationRect, sourceRect, tinte);
                }

            }
        }
        
    }
}
