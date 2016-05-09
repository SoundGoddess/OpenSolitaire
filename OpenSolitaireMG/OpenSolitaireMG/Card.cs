using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OpenSolitaireMG {


    enum Suit {

        clubs,
        hearts,
        diamonds,
        spades

    };

    enum CardColor {

        red,
        black

    }

    class Rank {

        public char[] ranks = {

        'A',
        '2',
        '3',
        '4',
        '5',
        '6',
        '7',
        '8',
        '9',
        'J',
        'Q',
        'K'

    };


    }


    class Card : IDragAndDropItem {

        Rank ranks = new Rank();
        public char rank;
        public Suit suit;
        bool isFaceUp = false;
        Texture2D cardback, _texture;
        Rectangle _rect;

        private float ratio;
        public Vector2 Position { get; set; }
        public bool IsSelected { get; set; }
        public bool IsMouseOver { get; set; }
        public bool Contains(Vector2 pointToCheck) {
            Point mouse = new Point((int)pointToCheck.X, (int)pointToCheck.Y);
            return Border.Contains(mouse);
        }


        private readonly SpriteBatch _spriteBatch;

        public Rectangle Border {
            get {

                return _rect;
                /*
                float textureWidthF = _texture.Width * ratio;
                int textureWidth = (int)textureWidthF;
                float textureHeightF = _texture.Height * ratio;
                int textureHeight = (int)textureHeightF;

                return new Rectangle((int)Position.X, (int)Position.Y, textureWidth, textureHeight);
                */

            }
        }



        public Card(char rank, Suit suit, Texture2D cardback) {

            this.rank = rank;
            this.suit = suit;
            this.cardback = cardback;

        }

        #region methods

        public void flipCard() {
            isFaceUp = !isFaceUp;
        }

        public CardColor color {

            get {

                if ((suit == Suit.clubs) || (suit == Suit.spades)) {
                    return CardColor.black;
                }
                else {
                    return CardColor.red;
                }

            }

        }

        public void SetTexture(Texture2D texture) { _texture = texture; }
        public void SetRectangle(Rectangle rect) { _rect = rect; }

        #endregion

        #region properties

        public int index { get { return Array.IndexOf(ranks.ranks, rank); } }
        
        public Rectangle rect { get { return _rect; } }

        public string asset {

            get {

                string location;

                switch (rank) {

                    case 'A':
                        location = "ace";
                        break;
                    case 'K':
                        location = "king";
                        break;
                    case 'Q':
                        location = "queen";
                        break;
                    case 'J':
                        location = "jack";
                        break;
                    default:
                        location = rank.ToString();
                        break;

                }

                // hard coded to the current deck asset set can be improved later
                location = "assets/" + location + "_of_" + suit;

                return location;

            }

        }


        public Texture2D texture {

            get {

                if (isFaceUp) return _texture;
                else return cardback;

            }

        }




        public bool faceUp { get { return isFaceUp; } }

        #endregion


    }
    
}
