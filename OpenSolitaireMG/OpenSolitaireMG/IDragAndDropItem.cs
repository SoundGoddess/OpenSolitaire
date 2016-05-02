using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace OpenSolitaireMG {
    /// <summary>
    /// Interface describing necessary implementation for working with the DragAndDropController.
    /// </summary>
    public interface IDragAndDropItem {
        Vector2 Position { get; set; }
        bool IsSelected { set; }
        bool IsMouseOver { set; }
        bool Contains(Vector2 pointToCheck);
        Rectangle Border { get; }
    }


    // original code saved below
    /*
        public interface IDragAndDropItem {
        Vector2 Position { get; set; }
        bool IsSelected { set; }
        bool IsMouseOver { set; }
        bool Contains(Vector2 pointToCheck);
        Rectangle Border { get; }
    }
    */

}
 