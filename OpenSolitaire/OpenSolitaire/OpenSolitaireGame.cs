/* ©2016 Hathor Gaia 
 * http://HathorsLove.com
 * 
 * Licensed Under GNU GPL 3:
 * http://www.gnu.org/licenses/gpl-3.0.html
 */
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Ruge.ViewportAdapters;
using MonoGame.Ruge.CardEngine;
using MonoGame.Ruge.DragonDrop;

namespace OpenSolitaire {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class OpenSolitaireGame : Game {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        BoxingViewportAdapter viewport;

        const int WindowWidth = 1035;
        const int WindowHeight = 750;

        const int spacer = 10;
        const int cardWidth = 125;
        const int cardHeight = 156;

        Texture2D cardSlot, cardBack, refreshMe;
        
        CardTable table;

        DragonDrop<IDragonDropItem> dragonDrop;

        public OpenSolitaireGame() {

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set the screen resolution
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;

            this.Window.Title = "Open Solitaire";
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

            viewport = new BoxingViewportAdapter(Window, GraphicsDevice, WindowWidth, WindowHeight);

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

            dragonDrop = new DragonDrop<IDragonDropItem>(this, spriteBatch, viewport);

            // table creates a fresh table.deck
            table = new CardTable(dragonDrop, cardBack, cardSlot, 10, 10);

            // load up the card assets for the new deck
            foreach (Card card in table.deck.cards) {

                string location = card.suit.ToString() + card.rank.ToString();
                card.SetTexture(Content.Load<Texture2D>(location));

            }

            table.SetTable();

            
            Components.Add(dragonDrop);

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

            foreach (Slot slot in table.slots) slot.Update(gameTime);

            foreach (Stack stack in table.stacks) {

                foreach (Card card in stack.cards) card.Update(gameTime);

            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {

            GraphicsDevice.Clear(Color.SandyBrown);

            spriteBatch.Begin(transformMatrix: viewport.GetScaleMatrix(), samplerState: SamplerState.LinearWrap);
            
            foreach (Slot slot in table.slots) slot.Draw(gameTime);
            
            //all this does is figure out where to center the refresh icon in relation to the draw slot
            Rectangle refreshRect = new Rectangle((int)table.slots[0].Position.X + cardWidth / 2 - refreshMe.Width / 2,
                (int)table.slots[0].Position.Y + cardHeight / 2 - refreshMe.Height / 2, refreshMe.Width, refreshMe.Height);

            spriteBatch.Draw(refreshMe, refreshRect, Color.White);


            foreach (Stack stack in table.stacks) {

                foreach (Card card in stack.cards) card.Draw(gameTime);

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
