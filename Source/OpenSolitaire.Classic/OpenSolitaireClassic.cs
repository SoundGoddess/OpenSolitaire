/* ©2016 Hathor Gaia 
* http://HathorsLove.com
* 
* Source code licensed under GPL-3
* Assets licensed seperately (see LICENSE.md)
*/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Ruge.ViewportAdapters;
using MonoGame.Ruge.DragonDrop;

namespace OpenSolitaire.Classic {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class OpenSolitaireClassic : Game {

        private const string VERSION = "v 0.9.5";

        SpriteBatch spriteBatch;

        BoxingViewportAdapter viewport;

        const int WINDOW_WIDTH = 1035;
        const int WINDOW_HEIGHT = 666;

        const int CARD_WIDTH = 125;
        const int CARD_HEIGHT = 156;

        Texture2D cardSlot, cardBack, refreshMe, newGame, metaSmug, debug;
        Rectangle newGameRect, debugRect;
        Color newGameColor, debugColor;

        private TableClassic table;

        DragonDrop<IDragonDropItem> dragonDrop;

        private MouseState prevMouseState;
        private SpriteFont debugFont;

        List<SoundEffect> soundFX;


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

            soundFX = new List<SoundEffect> {
                Content.Load<SoundEffect>("table-animation"),
                Content.Load<SoundEffect>("card-parent"),
                Content.Load<SoundEffect>("card-play"),
                Content.Load<SoundEffect>("card-restack"),
                Content.Load<SoundEffect>("game-win")
            };
            
            dragonDrop = new DragonDrop<IDragonDropItem>(this, viewport);


            // table creates a fresh table.deck
            table = new TableClassic(spriteBatch, dragonDrop, cardBack, cardSlot, 20, 30, soundFX);
            
            // load up the card assets for the new deck
            foreach (var card in table.drawPile.cards) {

                var location = card.suit.ToString() + card.rank.ToString();
                card.SetTexture(Content.Load<Texture2D>(location));

            }

            table.InitializeTable();

            table.SetTable();

            Components.Add(dragonDrop);

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


            if (table.isSetup && !table.isSnapAnimating) {

                if (newGameRect.Contains(point)) {

                    newGameColor = Color.Aqua;

                    if (mouseState.LeftButton == ButtonState.Pressed 
                        && prevMouseState.LeftButton == ButtonState.Released) {

                        table.NewGame();


                        // load up the card assets for the new deck
                        foreach (var card in table.drawPile.cards) {

                            var location = card.suit.ToString() + card.rank.ToString();
                            card.SetTexture(Content.Load<Texture2D>(location));

                        }

                        table.SetTable();
                        
                    }
                }

#if DEBUG
                debugRect = new Rectangle(310, 80, newGame.Width, newGame.Height);
                debugColor = Color.White;


                if (debugRect.Contains(point)) {

                    debugColor = Color.Aqua;

                    if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released) {

                        foreach (var stack in table.stacks) stack.debug();

                    }
                }
#endif
            }

            prevMouseState = mouseState;
            
            table.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {

            GraphicsDevice.Clear(Color.SandyBrown);

            spriteBatch.Begin(transformMatrix: viewport.GetScaleMatrix(), samplerState: SamplerState.LinearWrap);

            var logoVect = new Vector2(10, WINDOW_HEIGHT - metaSmug.Height - 10);

            // todo: please comment out the line below if you're going to distribute the game
            spriteBatch.Draw(metaSmug, logoVect, Color.White);
            
            spriteBatch.Draw(newGame, newGameRect, newGameColor);


            spriteBatch.Draw(refreshMe, new Vector2(35, 50), Color.White);



            var versionSize = debugFont.MeasureString(VERSION);
            var versionPos = new Vector2(WINDOW_WIDTH - versionSize.X - 10, WINDOW_HEIGHT - versionSize.Y - 10);
            spriteBatch.DrawString(debugFont, VERSION, versionPos, Color.Black);
         



#if DEBUG

            foreach (var stack in table.stacks) {

                var slot = stack.slot;
                var textWidth = debugFont.MeasureString(slot.name);
                var textPos = new Vector2(slot.Position.X + slot.Texture.Width / 2 - textWidth.X / 2, slot.Position.Y - 16);

                spriteBatch.DrawString(debugFont, slot.name, textPos, Color.Black);

            }

            spriteBatch.Draw(debug, debugRect, debugColor);
#endif



            table.Draw(gameTime);
            
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
