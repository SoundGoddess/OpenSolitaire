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
using MonoGame.Extended.ViewportAdapters;
using DragonDrop;
using System.Linq;

namespace OpenSolitaireMG {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class OpenSolitaireGame : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        BoxingViewportAdapter viewport;
        DragAndDropHandler<Card> dragonDrop;

        const int WindowWidth = 1000;
        const int WindowHeight = 750;

        const int spacer = 10;
        const int cardWidth = 125; 
        const int cardHeight = 182;

        bool initialSetup = false;
        bool doOnce = true;

        private MouseState prevMouseState;

        Texture2D cardSlotTex, cardSlotTex2;
        Texture2D cardBackTex;

        Texture2D refreshMe, newGame;
        Rectangle refreshRect, newGameRect;
        

        List<Deck> tablePile = new List<Deck>();
        List<Deck> scorePile = new List<Deck>();
        List<Vector2> animationQueue = new List<Vector2>();
        List<Rectangle> cardSlot = new List<Rectangle>();
        List<Rectangle> scoreSlot = new List<Rectangle>();


        Deck drawPile, discardPile;
        Rectangle drawPileRect, discardPileRect;




        public OpenSolitaireGame() {
            graphics = new GraphicsDeviceManager(this);


            // set the screen resolution
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;

            this.Window.Title = "Open Solitaire";

            //disable for now
            this.Window.AllowUserResizing = true;
            IsMouseVisible = true;

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

            dragonDrop = new DragAndDropHandler<Card>(this, spriteBatch, viewport);
            
            Components.Add(dragonDrop);

            
            cardSlotTex = Content.Load<Texture2D>("assets/small/cardslot");
            cardSlotTex2 = Content.Load<Texture2D>("assets/small/cardslot2");
            cardBackTex = Content.Load<Texture2D>("assets/small/green_back");
            refreshMe = Content.Load<Texture2D>("assets/refresh");

            newGame = Content.Load<Texture2D>("assets/new_game");
            newGameRect = new Rectangle(300, 30, newGame.Width, newGame.Height);
            
            SetupTable();


        }

        private void ClearTable() {

            animationQueue.Clear();
            drawPile.Clear();
            discardPile.Clear();
            tablePile.Clear();
            cardSlot.Clear();
            scoreSlot.Clear();
            dragonDrop.Clear();
            SetupTable();
            initialSetup = false;
            doOnce = true;

        }

        private void SetupTable() {

            drawPile = new Deck(cardBackTex, spriteBatch);
            drawPile.freshDeck();
            drawPile.shuffle();

            discardPile = new Deck(cardBackTex, spriteBatch);

            for (int i = 0; i < 7; i++) {
                tablePile.Add(new Deck(cardBackTex, spriteBatch));
            }

            foreach (Card loadTexture in drawPile.cards) {

                loadTexture.SetTexture(Content.Load<Texture2D>(loadTexture.asset));
                dragonDrop.Add(loadTexture);

            }


            for (int i = 0; i < 4; i++) scoreSlot.Add(new Rectangle());
            for (int i = 0; i < 7; i++) cardSlot.Add(new Rectangle());

            int x, y;

            x = spacer * 3;
            y = spacer * 3;

            drawPileRect = new Rectangle(x, y, cardWidth, cardHeight);

            foreach (Card card in drawPile.cards) card.Position = new Vector2(x, y);

            x += cardWidth + spacer;

            discardPileRect = new Rectangle(x, y, cardWidth, cardHeight);

            x += cardWidth * 2 - spacer;

            int newspacer = x;

            for (int i = 0; i < 4; i++) {

                x = (i * (cardWidth + spacer)) + (spacer * 3);
                y = spacer * 3;

                scoreSlot[i] = new Rectangle(x + newspacer, y, cardWidth, cardHeight);
            }

            refreshRect = new Rectangle(drawPileRect.X + cardWidth / 2 - refreshMe.Width,
                drawPileRect.Y + cardHeight / 2 - refreshMe.Height, refreshMe.Width * 2, refreshMe.Height * 2);


            animationQueue.Clear();

            for (int i = 0; i < 7; i++) {

                x = (i * (cardWidth + spacer)) + (spacer * 3);
                y = cardHeight + spacer * 7;

                cardSlot[i] = new Rectangle(x, y, cardWidth, cardHeight);

                animationQueue.Add(new Vector2(x, y));

                int z = 25;

                for (int j = 1; j < i + 1; j++) {

                    animationQueue.Add(new Vector2(x, y + (z * j)));

                }

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
            


            if (initialSetup) {

                MouseState mouseState = Mouse.GetState();

                Point point = viewport.PointToScreen(mouseState.X, mouseState.Y);

                if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released) {

                    if (drawPileRect.Contains(point)) {

                        if (drawPile.Count > 0) {
                            if (discardPile.Count > 1) {
                                Card lastCard = discardPile.cards[discardPile.Count - 1];
                                lastCard.IsDraggable = false;
                                lastCard.ZIndex = -100;
                            }
                            Card card = drawPile.drawCard();
                            card.Position = new Vector2(discardPileRect.X, discardPileRect.Y);
                            card.flipCard();
                            discardPile.addCard(card);
                            card.origin = card.Position;
                            card.IsDraggable = true;
                            card.isDiscarded = true;
                        }
                        else {

                            // drew all the cards from the deck; time to move the cards back to the draw pile
                            for (int i = 0; i < discardPile.Count; i++) {

                                Card card = discardPile.drawCard();
                                card.Position = new Vector2(drawPileRect.X, drawPileRect.Y);
                                card.flipCard();
                                drawPile.addCard(card);
                                card.origin = card.Position;
                                card.IsDraggable = false;


                            }

                        }

                    }

                    if (newGameRect.Contains(point)) ClearTable();

                }

                prevMouseState = mouseState;

                // once the deck has finished dealing into the piles, flip the top card
                if (doOnce) {

                    for (int i = 0; i < 7; i++) {

                        Card card = drawPile.drawCard();
                        card.ZIndex = -50;

                        tablePile[i].addCard(card);

                        if (i == 0) {
                            card.flipCard();
                            card.IsDraggable = true;
                            card.ZIndex = 2000;
                        }

                        for (int j = 1; j < i + 1; j++) {

                            card = drawPile.drawCard();

                            if (j == i) {
                                card.flipCard();
                                card.IsDraggable = true;
                                card.ZIndex = 2000;
                            }

                            tablePile[i].addCard(card);
                        }

                    }

                    doOnce = false;

                }

            }

            int animationCount = 0;

            // animates dealing from the draw pile to set up the board
            if (animationQueue.Count > 0) {
                for (int i = 1; i < animationQueue.Count + 1; i++) {

                    Card sprite = drawPile.cards[drawPile.cards.Count - i];

                    bool hasArrived = AnimateSprite(ref sprite, animationQueue[i - 1]);

                    if (hasArrived) {
                        animationCount++;
                    }
                    
                }
            }

            if (!initialSetup && (animationCount == animationQueue.Count)) {

                foreach (Card card in dragonDrop.Items) card.origin = card.Position;
                animationQueue.Clear();
                initialSetup = true;

            }


            foreach (Deck deck in tablePile) {

                if (deck.cards.Count > 0) {

                    if (!deck.cards[deck.cards.Count - 1].faceUp) deck.cards[deck.cards.Count - 1].flipCard();

                }

            }


            foreach (Card item in dragonDrop.Items) item.Update(gameTime);
            base.Update(gameTime);
        }

        
        bool AnimateSprite(ref Card sprite, Vector2 destination) {

            bool hasArrived = false;

            if (sprite.Position == destination) {
                hasArrived = true;
                sprite.ZIndex = -50;
            }
            else {
                
                var pos = sprite.Position;
                float speed = 5.0f;

                float distance = (float)Math.Sqrt(Math.Pow(destination.X - pos.X, 2) + (float)Math.Pow(destination.Y - pos.Y, 2));
                float directionX = (destination.X - pos.X) / distance;
                float directionY = (destination.Y - pos.Y) / distance;

                pos.X += directionX * speed;
                pos.Y += directionY * speed;


                if (Math.Sqrt(Math.Pow(pos.X - sprite.Position.X, 2) + Math.Pow(pos.Y - sprite.Position.Y, 2)) >= distance) {

                    sprite.Position = destination;

                    hasArrived = true;

                    sprite.ZIndex = -50;

                }
                else sprite.Position = pos;
            }

            return hasArrived;

        }



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.SandyBrown);

            spriteBatch.Begin(transformMatrix: viewport.GetScaleMatrix());

            spriteBatch.Draw(cardSlotTex, drawPileRect, Color.Black);
            spriteBatch.Draw(cardSlotTex, discardPileRect, Color.Black);

            foreach (Rectangle slot in scoreSlot) {

                spriteBatch.Draw(cardSlotTex2, slot, Color.Black);

            }

            foreach (Rectangle slot in cardSlot) {

                spriteBatch.Draw(cardSlotTex, slot, Color.Black);

            }

            spriteBatch.Draw(refreshMe, refreshRect, Color.White);
     //       spriteBatch.Draw(newGame, newGameRect, Color.White);



            foreach (Deck deck in tablePile) {

                deck.cards = deck.cards.OrderBy(z => z.ZIndex).ToList();
                foreach (Card card in deck.cards) {

                    //spriteBatch.Draw(card.texture, card.rect, Color.White);
                    card.Draw(gameTime);

                }

            }

            for (int i = drawPile.cards.Count - 1; i >= 0; i--) {

                //spriteBatch.Draw(drawPile.cards[i].texture, drawPile.cards[i].rect, Color.White);
                drawPile.cards[i].Draw(gameTime);

            }

            //foreach (Card card in discardPile.cards) spriteBatch.Draw(card.texture, card.rect, Color.White);
            foreach (Card card in discardPile.cards) card.Draw(gameTime);

            foreach (Card item in dragonDrop.Items) {

                //if (!item.faceUp) item.flipCard();
                if (item.ZIndex > 1000) item.Draw(gameTime);

            }


            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
