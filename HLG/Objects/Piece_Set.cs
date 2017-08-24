using Microsoft.Xna.Framework;

namespace HLG.Objects
{
    public class Piece_Set
    {
        //-//-// VARIABLES //-//-//
        public string piece { get; internal set; }
        public string set { get; internal set; }

        //-//-// METHODS //-//-//
        public void Initialize(string newPiece, string newSet)
        {
            piece = newPiece;
            set = newSet;
        }
    }
}
