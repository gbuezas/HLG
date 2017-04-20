using System.Collections.Generic;

namespace HLG.Objects
{
    public class Pieces_Sets
    {
        //-//-// VARIABLES //-//-//
        public List<Piece_Set> pieces = new List<Piece_Set>();

        //-//-// METHODS //-//-//
        public void Initialize(string[] pieces)
        {
            foreach (string piecesItem in pieces)
            {
                Piece_Set newPiece = new Piece_Set();
                newPiece.Initialize(piecesItem, "set1");
                this.pieces.Add(newPiece);
            }
        }

        public void SetSet(Piece_Set pieceChanged)
        {
            foreach (Piece_Set piecesItem in pieces)
            {
                if (piecesItem.piece == pieceChanged.piece)
                {
                    piecesItem.set = pieceChanged.set;
                }
            }
        }
        /// <summary>
        /// Obtiene el set asignado a esa pieza de armadura, si no la puede encontrar devuelve string.Empty.
        /// </summary>
        /// <param name="pieceSearched">Pieza de armadura que deseamos saber a que set pertenece.</param>
        /// <returns>Set al cual pertenece la pieza buscada.</returns>
        public string GetSet(string pieceSearched)
        {
            foreach (Piece_Set piece in pieces)
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
