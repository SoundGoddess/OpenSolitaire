using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace OpenSolitaireMG {
    public class Item : IDragAndDropItem {

        #region IDragAndDropItem implementation

        private float ratio;
        public Vector2 Position { get; set; }
        public bool IsSelected { get; set; }
        public bool IsMouseOver { get; set; }
        public bool Contains(Vector2 pointToCheck) {
            Point mouse = new Point((int)pointToCheck.X, (int)pointToCheck.Y);
            return Border.Contains(mouse);
        }
        
        #endregion

        #region Properties and variables

        public Texture2D Texture { get; set; }
        private readonly SpriteBatch _spriteBatch;

        public Rectangle Border {
            get {

                float textureWidthF = Texture.Width * ratio;
                int textureWidth = (int)textureWidthF;
                float textureHeightF = Texture.Height * ratio;
                int textureHeight = (int)textureHeightF;

                return new Rectangle((int)Position.X, (int)Position.Y, textureWidth, textureHeight);

            }
        }

        #endregion

        #region Constructor and Draw

        public Item(SpriteBatch spriteBatch, Texture2D texture, Vector2 position) {
            _spriteBatch = spriteBatch;
            Texture = texture;
            Position = position;
        }

        public void Draw(GameTime gameTime, float resizeMe) {

            ratio = resizeMe;

            Color colorToUse = Color.White;

            /*
            if (IsSelected) {
                colorToUse = Color.Orange;
            }
            else {
                if (IsMouseOver) { colorToUse = Color.Cyan; }
            }*/
           //_spriteBatch.Draw(Texture, Position, new Rectangle((int)Position.X, (int)Position.Y,cardWidth,cardHeight), colorToUse);
            _spriteBatch.Draw(Texture, Position, null, null, null, 0, new Vector2(resizeMe), colorToUse, SpriteEffects.None, 0);
            //_spriteBatch.Draw(Texture, Position, colorToUse);
        }

        #endregion

    }
}
