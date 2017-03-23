using HLG.Objects;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace HLG.Abstracts.Beings
{
    public abstract class IA_Characters : Being
    {

        //-//-// VARIABLES //-//-//
        public enum TargetCondition { MAXHEALTH, MINHEALTH/*, MAXMONEY, MINMONEY */}
        public TargetCondition TargetCond;
         
        //-//-// METHODS //-//-//
        /// <summary>
        /// Obtiene condiciones al azar, se hace en inicializar para que se haga una sola vez en la creacion del personaje
        /// </summary>
        public void GetCondition()
        {
            /// Me muevo en el rango de la cantidad de condiciones que existen en generales
            TargetCond = (TargetCondition)Global.randomly.Next(0, Enum.GetNames(typeof(TargetCondition)).Length);
        }
        /// <summary>
        /// Setea un objetivo segun los criterios de busqueda que se obtuvieron de GetCondition() en Initialize.
        /// Se hace en cada vuelta logica ya que recalcula los parametros por si hay que cambiar de blanco bajo los mismos criterios.
        /// </summary>
        /// <returns></returns>
        public Being GetTarget(TargetCondition Condition)
        {
            /// Si estan todos muertos van por default al jugador 0, no es necesario que devuelva null. El cambio de target se hace solo al
            /// recalcular si la vida del target actual es igual o menor a 0
            switch (Condition)
            {
                /// MAX HEALTH: No mata a nadie pero lastima siempre al mas fuerte, un equilibrador.
                #region MAX HEALTH
                case TargetCondition.MAXHEALTH:
                    {
                        int healthTemp = 0;
                        int playerMaxHealth = 0;

                        for (int i = 0; i < Global.players_quant; i++)
                        {
                            if (Global.players[i].currentHealth >= healthTemp && Global.players[i].currentHealth > 0)
                            {
                                healthTemp = Global.players[i].currentHealth;
                                playerMaxHealth = i;
                            }
                        }

                        return Global.players[playerMaxHealth];
                    }
                #endregion

                /// MIN HEALTH: Es un finisher, va a tratar de matar a los mas débiles.
                #region MIN HEALTH
                case TargetCondition.MINHEALTH:
                    {
                        int healthTemp = 5000;
                        int playerMinHealth = 0;

                        for (int i = 0; i < Global.players_quant; i++)
                        {
                            if (Global.players[i].currentHealth <= healthTemp && Global.players[i].currentHealth > 0)
                            {
                                healthTemp = Global.players[i].currentHealth;
                                playerMinHealth = i;
                            }
                        }

                        return Global.players[playerMinHealth];
                    }
                #endregion

                /*case Global.TargetCondition.MAXMONEY:
                    {
                        return Global.players[2];
                    }

                case Global.TargetCondition.MINMONEY:
                    {
                        return Global.players[3];
                    }*/

                default: return Global.players[0];

            }
        }
        /// <summary>
        /// Logica de movimiento hacia el objetivo adquirido
        /// Dirigirse al blanco, dependiendo de donde esta el eje del blanco vamos a sumarle la velocidad hacia el.
        /// Tambien se toma el punto que va a buscar para atacar a cierto personaja.
        /// Para obtener el lugar antes mencionado usamos la variable de HitRange asi se posiciona optimamente para su ataque.
        /// El HitRangeX tiene que ser mayor para que no hostigue tanto al blanco, sino se pega mucho a el
        /// Uno de los 2 tenia que tener el igual (=) asi no habia un punto en el que se queda quieto el esqueleto, en este caso
        /// es el primer check, el segundo ya no lo tiene
        /// </summary>
        /// <param name="target">Objetivo adquirido</param>
        protected void GoToTarget(Being target)
        {
            // Si no esta en movimiento por default queda parado
            //currentAction = Global.Actions.STAND;

            if (Get_Position_Rec().Center.X <= target.Get_Position_Rec().Center.X)
            {
                // Izquierda
                if (Get_Position_Rec().Center.X >= target.Get_Position_Rec().Center.X - hitRangeX)
                {
                    positionX -= playerMoveSpeed;
                }
                else
                {
                    positionX += playerMoveSpeed;
                }

                facing = Global.Facing.RIGHT;
                //currentAction = Global.Actions.WALK;
            }
            else if (Get_Position_Rec().Center.X > target.Get_Position_Rec().Center.X)
            {
                // Derecha
                if (Get_Position_Rec().Center.X <= target.Get_Position_Rec().Center.X + hitRangeX)
                {
                    positionX += playerMoveSpeed;
                }
                else
                {
                    positionX -= playerMoveSpeed;
                }

                facing = Global.Facing.LEFT;
                //currentAction = Global.Actions.WALK;
            }

            //if (target.GetPositionRec().Center.Y < GetPositionRec().Center.Y - hitrangeY)
            if (target.Get_Position_Rec().Center.Y <= Get_Position_Rec().Center.Y - hitRangeY)
            {
                // Arriba
                positionY -= playerMoveSpeed;
                //currentAction = Global.Actions.WALK;
            }
            else if (target.Get_Position_Rec().Center.Y > Get_Position_Rec().Center.Y + hitRangeY)
            {
                // Abajo
                positionY += playerMoveSpeed;
                //currentAction = Global.Actions.WALK;
            }

            currentAction = Global.Actions.WALK;
        }
        
        public override void Draw_With_Parallax()
        {
            foreach (Animation piezaAnimada in animationPieces)
            {
                piezaAnimada.Draw(facing, piezaAnimada.color);
            }
        }
        public override void Draw_Without_Parallax()
        {
            throw new NotImplementedException();
        }

    }
}
