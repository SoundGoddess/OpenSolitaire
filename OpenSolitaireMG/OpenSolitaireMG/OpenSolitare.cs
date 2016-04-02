using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace OpenSolitaireMG {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class OpenSolitare : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public OpenSolitare() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here

            Deck deck = new Deck();

            Console.WriteLine("===\ninitial deck");
            deck.testDeck(5);
            deck.debugDeck();

            Console.WriteLine("===\ninitial hand");
            Deck hand = new Deck();
            hand.testDeck(2);
            hand.debugDeck();

            Card moveCard = deck.drawCard();
            hand.addCard(moveCard);

            Console.WriteLine("===\nnew deck");
            deck.debugDeck();

            Console.WriteLine("===\nnew hand");
            hand.debugDeck();

            moveCard = deck.drawCard();
            hand.addCard(moveCard);

            Console.WriteLine("===\nnew deck");
            deck.debugDeck();

            Console.WriteLine("===\nnew hand");
            hand.debugDeck();


            moveCard = deck.drawCard();
            moveCard.flipCard();
            hand.addCard(moveCard);

            Console.WriteLine("===\nnew deck");
            deck.debugDeck();

            Console.WriteLine("===\nnew hand");
            hand.debugDeck();


        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
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

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
