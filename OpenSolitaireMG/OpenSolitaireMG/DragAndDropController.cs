using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace OpenSolitaireMG {
    /// <summary>
    /// Controllerclass for easy drag and drop in XNA
    /// </summary>
    /// <typeparam name="T">The type of items to interact with using the mouse. Must implement IDragAndDropItem</typeparam>
    public class DragAndDropController<T> : DrawableGameComponent where T : IDragAndDropItem {

        #region Variables and properties

        private KeyboardState _keyboard;
        private MouseState _oldMouse, _currentMouse;
        private SpriteBatch _spriteBatch;
        private Vector2 _mouseDown;
        private readonly List<T> _selectedItems;
        private readonly List<T> _items;
        private bool _isDraggingRectangle;

        public T ItemUnderTheMouseCursor { get; private set; }
        public bool IsThereAnItemUnderTheMouseCursor { get; private set; }

        public IEnumerable<T> Items { get { foreach (var item in _items) { yield return item; } } }
        public IEnumerable<T> SelectedItems { get { foreach (var item in _selectedItems) { yield return item; } } }

        public int Count { get { return _items.Count; } }
        public int SelectedCount { get { return _selectedItems.Count; } }

        private Texture2D _selectionTexture;

        private bool MouseWasJustPressed {
            get {
                return _currentMouse.LeftButton == ButtonState.Pressed && _oldMouse.LeftButton == ButtonState.Released;
            }
        }

        private bool IsGroupAction {
            get {
                return _keyboard.IsKeyDown(Keys.LeftControl) || _keyboard.IsKeyDown(Keys.RightControl);
            }
        }

        private Vector2 CurrentMousePosition {
            get { return new Vector2(_currentMouse.X, _currentMouse.Y); }
        }

        public Vector2 OldMousePosition {
            get { return new Vector2(_oldMouse.X, _oldMouse.Y); }
        }

        public Vector2 MouseMovementSinceLastUpdate {
            get { return CurrentMousePosition - OldMousePosition; }
        }

        public T ItemUnderMouseCursor() {
            for (int i = _items.Count - 1; i >= 0; i--) {
                if (_items[i].Contains(CurrentMousePosition)) {
                    return _items[i];
                }
            }
            return default(T);
        }

        #endregion

        #region Constructor, Draw() and Update()

        public DragAndDropController(Game game, SpriteBatch spriteBatch) : base(game) {
            _selectedItems = new List<T>();
            _items = new List<T>();
            _spriteBatch = spriteBatch;
            _selectionTexture = Game.Content.Load<Texture2D>("assets/white");
        }

        public override void Update(GameTime gameTime) {
            GetCurrentMouseState();
            GetCurrentKeyboardState();
            HandleMouseInput();
            HandleKeyboardInput();
            SaveCurrentMouseState();
        }

        public override void Draw(GameTime gameTime) {
            /*
            if (_isDraggingRectangle) {
                Rectangle selectionRectangle = GetSelectionRectangle();
                _spriteBatch.Draw(_selectionTexture, selectionRectangle, Color.White * .4f);
            }
            */
        }


        #endregion

        #region public interaction methods

        public void Add(T item) { _items.Add(item); }
        public void Remove(T item) { _items.Remove(item); _selectedItems.Remove(item); }

        public void DeselectAll() {
            for (int i = _selectedItems.Count - 1; i >= 0; i--) {
                DeselectItem(_selectedItems[i]);
            }
        }

        public void SelectAll() {
            _items.ForEach(SelectItem);
        }

        public void Clear() {
            _selectedItems.Clear();
            _items.Clear();
        }

        public void RemoveSelected() {
            for (int i = _selectedItems.Count - 1; i >= 0; i--) {
                _items.Remove(_selectedItems[i]);
                _selectedItems.Remove(_selectedItems[i]);
            }
        }

        #endregion

        #region Private helpermethods

        private Rectangle GetSelectionRectangle() {
            int left = (int)MathHelper.Min(_mouseDown.X, CurrentMousePosition.X);
            int right = (int)MathHelper.Max(_mouseDown.X, CurrentMousePosition.X);
            int top = (int)MathHelper.Min(_mouseDown.Y, CurrentMousePosition.Y);
            int bottom = (int)MathHelper.Max(_mouseDown.Y, CurrentMousePosition.Y);

            return new Rectangle(left, top, right - left, bottom - top);
        }

        private void HandleKeyboardInput() {
            if (_keyboard.IsKeyDown(Keys.A) && (_keyboard.IsKeyDown(Keys.LeftControl) || _keyboard.IsKeyDown(Keys.RightControl))) {
                SelectAll();
            }
            if (_keyboard.IsKeyDown(Keys.Delete) && _selectedItems.Count > 0) {
                RemoveSelected();
            }
        }

        private void GetCurrentMouseState() { _currentMouse = Mouse.GetState(); }
        private void GetCurrentKeyboardState() { _keyboard = Keyboard.GetState(); }

        private void HandleMouseInput() {
            SetAllItemsToIsMouseOverFalse();
            ItemUnderTheMouseCursor = ItemUnderMouseCursor();
            if (!Equals(ItemUnderTheMouseCursor, default(T))) {
                UpdateItemUnderMouse();
            }
            else {
                if (MouseWasJustPressed) {
                    DeselectAll();
                    _mouseDown = CurrentMousePosition;
                  //  _isDraggingRectangle = true;
                }
            }

            if (_currentMouse.LeftButton == ButtonState.Released) {
                _isDraggingRectangle = false;
            }
            else {
                if (_isDraggingRectangle) {
                    Rectangle selectionRectangle = GetSelectionRectangle();
                    DeselectAll();
                    _items.Where(item => selectionRectangle.Contains(item.Border)).ToList().ForEach(SelectItem);
                }
                else {
                    MoveSelectedItemsIfMouseButtonIsPressed();
                }
            }
        }

        private void SetAllItemsToIsMouseOverFalse() {
            _items.ForEach(item => item.IsMouseOver = false);
        }

        private void MoveSelectedItemsIfMouseButtonIsPressed() {
            if (_currentMouse.LeftButton == ButtonState.Pressed) {
                _selectedItems.ForEach(item => item.Position += MouseMovementSinceLastUpdate);
            }
        }

        private void UpdateItemUnderMouse() {
            ItemUnderTheMouseCursor.IsMouseOver = true;

            if (MouseWasJustPressed) {
                IfItemUnderMouseIsNotInSelectedGroupThenDeselectAll();
                SelectItem(ItemUnderTheMouseCursor);
            }
        }

        private void IfItemUnderMouseIsNotInSelectedGroupThenDeselectAll() {
            if (!IsGroupAction && !_selectedItems.Contains(ItemUnderTheMouseCursor)) {
                DeselectAll();
            }
        }

        private void SaveCurrentMouseState() { _oldMouse = _currentMouse; }

        private void SelectItem(T itemToSelect) {
            itemToSelect.IsSelected = true;
            if (!_selectedItems.Contains(itemToSelect)) {
                _selectedItems.Add(itemToSelect);

                //in theory this should bring the item to the top
                _items.Remove(itemToSelect);
                _items.Add(itemToSelect);
            }

        }

        private void DeselectItem(T itemToDeselect) {
            itemToDeselect.IsSelected = false;
            _selectedItems.Remove(itemToDeselect);
        }
        #endregion

    }
}