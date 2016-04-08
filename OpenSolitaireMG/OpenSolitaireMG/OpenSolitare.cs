/* ©2016 Hathor Gaia 
 * http://HathorsLove.com
 * 
 * Licensed Under GNU GPL 3:
 * http://www.gnu.org/licenses/gpl-3.0.html
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace OpenSolitaireMG {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class OpenSolitare : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        const int WindowWidth = 1024;
        const int WindowHeight = 768;

        const float cardRatio = 1.452f;

        int spacer, cardWidth, cardHeight;

        Card card0;
        Card card1;

        Texture2D card0tex;
        Texture2D card1tex;
        Texture2D cardSlotTex;
        Texture2D cardBackTex;

        Rectangle card0rect;
        Rectangle card1rect;

        List<Rectangle> cardSlot = new List<Rectangle>();


        private SpriteBatch _spriteBatch;
        private DragAndDropController<Item> _dragDropController;

        public float ScaledWidth(float width) {
            return Window.ClientBounds.Width / WindowWidth;
        }
        public float ScaledHeight(float height) {
            return Window.ClientBounds.Height / WindowHeight;
        }


        public OpenSolitare() {
            graphics = new GraphicsDeviceManager(this);

            // set the screen resolution
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;

            this.Window.AllowUserResizing = true;
            IsMouseVisible = true;

            Content.RootDirectory = "Content";

            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);


            _setSize();
        }

        void Window_ClientSizeChanged(object sender, EventArgs e) { _setSize(); }

        private void _setSize() {
            spacer = Window.ClientBounds.Width / 90;
            cardWidth = (Window.ClientBounds.Width / 7) - spacer;

            float cardHeightF = (float)cardWidth * cardRatio;
            cardHeight = (int)cardHeightF;

            //need some way to call this on resize
            if (_dragDropController != null) {

            SetupDraggableItems();
            }
       }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {


            Deck deck = new Deck();
            deck.freshDeck();
            deck.shuffle();

            card0 = deck.drawCard();
            card1 = deck.drawCard();


                        

            //don't accidentally delete this :O
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteBatch = new SpriteBatch(GraphicsDevice);


            _dragDropController = new DragAndDropController<Item>(this, _spriteBatch);
            Components.Add(_dragDropController);


            cardSlotTex = Content.Load<Texture2D>("assets/cardslot2");
            cardBackTex = Content.Load<Texture2D>("assets/back_purple");
            card0tex = Content.Load<Texture2D>(card0.asset);
            card1tex = Content.Load<Texture2D>(card1.asset);


            for (int i = 0; i < 7; i++) {
                
                cardSlot.Add(new Rectangle());
            }

            SetupDraggableItems();
        }

        private void SetupDraggableItems() {

           // _setSize();
            _dragDropController.Clear();
                        
            for (int i = 0; i < 7; i++) {

                int x = (i * (cardWidth + spacer)) + (spacer / 2);
                int y = 220;

                Item item = new Item(_spriteBatch, cardBackTex, new Vector2(x, y), .4f);
                _dragDropController.Add(item);
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here



            //SetupDraggableItems();

            /*
            _dragDropController.Clear();
            for (int i = 0; i < 7; i++) {

                int x = (i * (cardWidth + spacer)) + (spacer / 2);
                int y = 220;

                Item item = new Item(_spriteBatch, cardBackTex, new Vector2(x, y), .4f);
                _dragDropController.Add(item);
            }
            */

            card0rect = new Rectangle(150, 220, cardWidth, cardHeight);
            card1rect = new Rectangle(300, 220, cardWidth, cardHeight);

            for (int i = 0; i < 7; i++) {

                int x = (i * (cardWidth + spacer)) + (spacer / 2);
                int y = 10;

                cardSlot[i] = new Rectangle(x, y, cardWidth, cardHeight);
            }

            
            MouseState mouse = Mouse.GetState();
            var mousePos = new Point(mouse.X, mouse.Y);

            //drawRectangle.X = mouse.X - currentCharacter.Width / 2;
            //drawRectangle.Y = mouse.Y - currentCharacter.Height / 2;

            if (mouse.LeftButton == ButtonState.Pressed) {

                Console.WriteLine(mouse.X + "," + mouse.Y);

            
                if (card0rect.Contains(mousePos)) {

                    card0rect.X = mouse.X - (card0rect.Width/2);
                    card0rect.Y = mouse.Y - (card0rect.Height/2);

                }
                //int deltaX = card0rect.X - mouse.X;
                int deltaX = mouse.X - card1rect.X - (card1rect.Width / 2);
                int deltaY = mouse.Y - card1rect.Y - (card1rect.Height / 2);

                if (card1rect.Contains(mousePos)) {

                    card1rect.X = deltaX;
                    card1rect.Y = deltaY;

                    Console.WriteLine("mouse:" + mouse.X + "," + mouse.Y);
                    Console.WriteLine("card:" + card1rect.X + "," + card1rect.Y);
                    Console.WriteLine("delta:" + deltaX + "," + deltaY);

                }
                
            }
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            Console.WriteLine(Window.ClientBounds.Width + "," + Window.ClientBounds.Height);



            // z-index is determined by the order which the spriteBatch.Draw methods are called

            spriteBatch.Begin();
            _spriteBatch.Begin();

           // spriteBatch.Draw(card0tex, card0rect, Color.White);
           // spriteBatch.Draw(card1tex, card1rect, Color.White);

            foreach (Rectangle slot in cardSlot) {

                spriteBatch.Draw(cardSlotTex, slot, Color.Black);

            }

            float ratio = .002f * Window.ClientBounds.Width / 7;

            foreach (var item in _dragDropController.Items) { item.Draw(gameTime,ratio); }

            /*spriteBatch.Begin(SpriteSortMode.BackToFront, null);

            spriteBatch.Draw(card0tex, card0rect, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(card1tex, card1rect, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.0f);
            */

            _spriteBatch.End();
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
