/* 
© 2016 The Ruge Project (http://ruge.metasmug.com/) 

Licensed under MIT (see License.txt)
 
 */
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Ruge.DragonDrop {
    /// <summary>
    /// Interface describing necessary implementation for working with the DragonDrop Handler.
    /// </summary>
    public interface IDragonDropItem {
        Vector2 Position { get; set; }
        bool IsSelected { get; set; }
        bool IsMouseOver { get; set; }
        Rectangle Border { get; }
        bool IsDraggable { get; set; }
        int ZIndex { get; set; }
        Texture2D Texture { get; }

        void OnSelected();
        void OnDeselected();
        bool Contains(Vector2 pointToCheck);
        void OnCollusion(IDragonDropItem item);
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}