using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonDrop {
    /// <summary>
    /// Interface describing necessary implementation for working with the DragonDrop Handler.
    /// </summary>
    public interface IDragAndDropItem {
        Vector2 Position { get; set; }
        bool IsSelected { get; set; }
        bool IsMouseOver { set; }
        bool Contains(Vector2 pointToCheck);
        Rectangle Border { get; }
        bool IsDraggable { get; set; }
        int ZIndex { get; set; }
        Texture2D Texture { get; }
        
        void OnSelected();
        void OnDeselected();
        void HandleCollusion(IDragAndDropItem item);
    }
}