using System.Collections.Generic;

namespace HLG.Objects
{
    public class Pieces_Sets
    {
        public List<Piece_Set> Pieces = new List<Piece_Set>();

        public void Initialize()
        {
            foreach (string piece in Global.PiecesPaladin)
            {
                Piece_Set newPiece = new Piece_Set();
                newPiece.Initialize(piece, "set1");
                Pieces.Add(newPiece);
            }
        }

        public void Set_Set(Piece_Set pieceChanged)
        {
            foreach (Piece_Set piece in Pieces)
            {
                if (piece.piece == pieceChanged.piece)
                {
                    piece.set = pieceChanged.set;
                }
            }
        }

        /// <summary>
        /// Obtiene el set asignado a esa pieza de armadura, si no la puede encontrar devuelve string.Empty.
        /// </summary>
        /// <param name="pieceSearched">Pieza de armadura que deseamos saber a que set pertenece.</param>
        /// <returns>Set al cual pertenece la pieza buscada.</returns>
        public string Get_Set(string pieceSearched)
        {
            foreach (Piece_Set piece in Pieces)
            {
                if (pieceSearched == piece.piece)
                {
                    return piece.set;
                }
            }
            return string.Empty;
        }
    }
}
