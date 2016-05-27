/* ©2016 Hathor Gaia 
* http://HathorsLove.com
* 
* Source code licensed under GPL-3
* Assets licensed seperately (see LICENSE.md)
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Ruge.ViewportAdapters;
using MonoGame.Ruge.DragonDrop;
using MonoGame.Ruge.CardEngine;

namespace OpenSolitaire.Classic {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class OpenSolitaireClassic : Game {
        SpriteBatch spriteBatch;

        BoxingViewportAdapter viewport;

        const int WINDOW_WIDTH = 1035;
        const int WINDOW_HEIGHT = 600;

        const int CARD_WIDTH = 125;
        const int CARD_HEIGHT = 156;

        Texture2D cardSlot, cardBack, refreshMe, newGame, metaSmug, debug;
        Rectangle newGameRect, debugRect;
        Color newGameColor, debugColor;

        private TableClassic table;

        DragonDrop<IDragonDropItem> dragonDrop;

        private MouseState prevMouseState;
        private SpriteFont debugFont;


        public OpenSolitaireClassic() {
            var graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

            // set the screen resolution
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;

            this.Window.Title = "Open Solitaire Classic";
            this.Window.AllowUserResizing = true;

            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {

            viewport = new BoxingViewportAdapter(Window, GraphicsDevice, WINDOW_WIDTH, WINDOW_HEIGHT);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            cardSlot = Content.Load<Texture2D>("card_slot");
            cardBack = Content.Load<Texture2D>("card_back_green");
            refreshMe = Content.Load<Texture2D>("refresh");
            newGame = Content.Load<Texture2D>("new_game");
            metaSmug = Content.Load<Texture2D>("smug-logo");
            debug = Content.Load<Texture2D>("debug");
            debugFont = Content.Load<SpriteFont>("Arial");

            dragonDrop = new DragonDrop<IDragonDropItem>(this, spriteBatch, viewport);


            // table creates a fresh table.deck
            table = new TableClassic(dragonDrop, cardBack, cardSlot, 0, 30);

            table.InitializeTable();

            table.SetTable();

            // todo: remove after testing
            table.isSetup = true;

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() { }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var mouseState = Mouse.GetState();
            var point = viewport.PointToScreen(mouseState.X, mouseState.Y);

            newGameRect = new Rectangle(310, 20, newGame.Width, newGame.Height);
            newGameColor = Color.White;


            if (table.isSetup && !table.isAnimating) {

                if (newGameRect.Contains(point)) {

                    newGameColor = Color.Aqua;

                    if (mouseState.LeftButton == ButtonState.Pressed &&
                        prevMouseState.LeftButton == ButtonState.Released) {

                    /*

                        table.Clear();


                        // load up the card assets for the new deck
                        foreach (Card card in table.drawPile.cards) {

                            var location = card.suit.ToString() + card.rank.ToString();
                            card.SetTexture(Content.Load<Texture2D>(location));

                            dragonDrop.Add(card);

                        }

                        table.SetTable();
                        */
                    }
                }

#if DEBUG
                debugRect = new Rectangle(310, 80, newGame.Width, newGame.Height);
                debugColor = Color.White;


                if (debugRect.Contains(point)) {

                    debugColor = Color.Aqua;

                    if (mouseState.LeftButton == ButtonState.Pressed &&
                        prevMouseState.LeftButton == ButtonState.Released) {

                     //   foreach (Slot slot in table.slots) slot.stack.debug();

                    }
                }
#endif
            }

            prevMouseState = mouseState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {

            GraphicsDevice.Clear(Color.SandyBrown);

            spriteBatch.Begin(transformMatrix: viewport.GetScaleMatrix(), samplerState: SamplerState.LinearWrap);

            var logoRect = new Rectangle(10, 540, metaSmug.Width, metaSmug.Height);

            // todo: please comment out the line below if you're going to distribute the game
            spriteBatch.Draw(metaSmug, logoRect, Color.White);



            spriteBatch.Draw(newGame, newGameRect, newGameColor);


#if DEBUG
            /*
            int x = 20;
            int y = cardSlot.Height + 23;

            // show the slot number for easier debugging
            for (int i = 0; i < 7; i++) {
                Vector2 vect = new Vector2(x + x * i + cardSlot.Width * i, y);
                vect.X += cardSlot.Width / 2;
                spriteBatch.DrawString(debugFont, i.ToString(), vect, Color.Black);
            }

            y = 5;

            for (int i = 6; i >= 3; i--) {
                Vector2 vect = new Vector2(x + x * i + cardSlot.Width * i, y);
                vect.X += cardSlot.Width / 2;
                spriteBatch.DrawString(debugFont, i.ToString(), vect, Color.Black);
            }
            */

            spriteBatch.Draw(debug, debugRect, debugColor);
#endif


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
