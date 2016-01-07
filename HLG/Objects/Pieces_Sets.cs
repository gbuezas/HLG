using System.Collections.Generic;

namespace HLG.Objects
{
    class Pieces_Sets
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
