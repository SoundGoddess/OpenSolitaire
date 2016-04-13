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
        Deck deck;

        Texture2D card0tex;
        Texture2D card1tex;
        Texture2D cardSlotTex, cardSlotTex2;
        Texture2D cardBackTex;

        List<Card> cards = new List<Card>();
        List<Texture2D> cardTex = new List<Texture2D>();

        Rectangle card0rect;
        Rectangle card1rect;

        Rectangle drawPile, discardPile;


        List<Rectangle> cardSlot = new List<Rectangle>();
        List<Rectangle> scoreSlot = new List<Rectangle>();

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

            this.Window.Title = "Open Solitaire";


            this.Window.AllowUserResizing = true;
            IsMouseVisible = true;

            Content.RootDirectory = "Content";

            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);


            _setSize();
        }

        void Window_ClientSizeChanged(object sender, EventArgs e) { _setSize(); }

        private void _setSize() {

            spacer = Window.ClientBounds.Width / 180;
            cardWidth = (int)(Window.ClientBounds.Width / 7.4);

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


            deck = new Deck();
            deck.freshDeck();
            deck.shuffle();


            for (int i = 0; i < 7; i++) {
                cards.Add(deck.drawCard());
            }                        

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
            


            _dragDropController = new DragAndDropController<Item>(this, spriteBatch);
            Components.Add(_dragDropController);


            cardSlotTex = Content.Load<Texture2D>("assets/cardslot");
            cardSlotTex2 = Content.Load<Texture2D>("assets/cardslot2");
            cardBackTex = Content.Load<Texture2D>("assets/back_purple");
            //card0tex = Content.Load<Texture2D>(card0.asset);
            //card1tex = Content.Load<Texture2D>(card1.asset);

            for (int i = 0; i < 4; i++) {

                scoreSlot.Add(new Rectangle());
            }

            for (int i = 0; i < 7; i++) {
                cardTex.Add(Content.Load<Texture2D>(cards[i].asset));
                cardSlot.Add(new Rectangle());
            }

            SetupDraggableItems();
        }

        private void SetupDraggableItems() {

           // _setSize();
            _dragDropController.Clear();

            int x, y;
                    
            for (int i = 0; i < 7; i++) {
                
                x = (i * (cardWidth + spacer)) + (spacer * 3);
                y = cardHeight + spacer * 7;

                //thisCard = deck.drawCard();
                //cardTex = Content.Load<Texture2D>(thisCard.asset);

                Item item = new Item(spriteBatch, cardTex[i], new Vector2(x, y));
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
            

            card0rect = new Rectangle(150, 220, cardWidth, cardHeight);
            card1rect = new Rectangle(300, 220, cardWidth, cardHeight);

            int x, y;

            x = spacer*3;
            y = spacer*3;

            drawPile = new Rectangle(x, y, cardWidth, cardHeight);

            x += cardWidth + spacer;
            
            discardPile = new Rectangle(x, y, cardWidth, cardHeight);

            x += cardWidth*2 - spacer;

            int newspacer = x;

            for (int i = 0; i < 4; i++) {

                x = (i * (cardWidth + spacer)) + (spacer * 3);
                y = spacer * 3;

                scoreSlot[i] = new Rectangle(x + newspacer, y, cardWidth, cardHeight);
            }




            for (int i = 0; i < 7; i++) {

                x = (i * (cardWidth + spacer)) + (spacer * 3);
                y = cardHeight + spacer * 7;

                cardSlot[i] = new Rectangle(x, y, cardWidth, cardHeight);
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
            

            // z-index is determined by the order which the spriteBatch.Draw methods are called

            spriteBatch.Begin();


            spriteBatch.Draw(cardSlotTex, drawPile, Color.Black);
            spriteBatch.Draw(cardSlotTex, discardPile, Color.Black);

            foreach (Rectangle slot in scoreSlot) {

                spriteBatch.Draw(cardSlotTex2, slot, Color.Black);

            }

            foreach (Rectangle slot in cardSlot) {

                spriteBatch.Draw(cardSlotTex, slot, Color.Black);

            }

            float ratio = .0019f * Window.ClientBounds.Width / 7;
            

            foreach (var item in _dragDropController.Items) { item.Draw(gameTime,ratio); }

            
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
