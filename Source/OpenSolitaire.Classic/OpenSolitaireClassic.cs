/* ©2016 Hathor Gaia 
* http://HathorsLove.com
* 
* Source code licensed under NWO-SA
* Assets licensed seperately (see LICENSE.md)
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Ruge.CardEngine;
using MonoGame.Ruge.ViewportAdapters;
using MonoGame.Ruge.DragonDrop;
using MonoGame.Ruge.SillyFun.Confetti;

namespace OpenSolitaire.Classic {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class OpenSolitaireClassic : Game {

        MouseState oldMouse, currentMouse;
        private bool click => currentMouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released;

        private string version = "v ";

        // please only set this to true if you are contributing 
        // your code back to the "official" repository:
        // https://github.com/MetaSmug/RollYourOwnGameEngine
        private bool isOfficial = true;

        SpriteBatch spriteBatch;

        BoxingViewportAdapter viewport;

        const int WINDOW_WIDTH = 1035;
        const int WINDOW_HEIGHT = 666;

        const int CARD_WIDTH = 125;
        const int CARD_HEIGHT = 156;

        Texture2D cardSlot, cardBack, refreshMe, newGame, metaSmug, debug, undo, solve, paused;
        Rectangle newGameRect, debugRect, undoRect, solveRect;
        Color newGameColor, debugColor, undoColor, solveColor;

        private TableClassic table;

        DragonDrop<IDragonDropItem> dragonDrop;
        
        private SpriteFont debugFont;

        private List<SoundEffect> soundFX;

        // yay, confetti!
        private readonly EasyTimer explosionTimer = new EasyTimer(TimeSpan.FromSeconds(0.4f));
        private readonly EasyTimer spawnTimer = new EasyTimer(TimeSpan.FromSeconds(0.05f));
        private readonly EasyTimer transitionTimer = new EasyTimer(TimeSpan.FromSeconds(5));
        private Confetti confetti;
        private readonly int confettiRate = 4;
        private ResetableArray<Vector2> explosionPositions;
        private bool spawningConfetti = true;
        private SoundEffect popFX;

        private SoundEffect gameWinFX;
        private bool gameWinPlaying;

        public OpenSolitaireClassic() {
            var graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

            // set the screen resolution
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;

            Window.Title = "Open Solitaire Classic";
            Window.AllowUserResizing = true;

            IsMouseVisible = true;

            version += FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            var subVersion = version.Substring(version.LastIndexOf(".") + 1);
            if (subVersion == "0") version = version.Substring(0, version.LastIndexOf("."));
            subVersion = version.Substring(version.LastIndexOf(".") + 1);
            if (subVersion == "0") version = version.Substring(0, version.LastIndexOf("."));

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {

            viewport = new BoxingViewportAdapter(Window, GraphicsDevice, WINDOW_WIDTH, WINDOW_HEIGHT);

            // confetti stuff
            const int explosionCount = 5;
            explosionPositions = new ResetableArray<Vector2>(
                Enumerable.Range(1, explosionCount).Select(i => {
                    var c = i / (explosionCount + 1f);
                    return new Vector2(
                        c * GraphicsDevice.Viewport.Width,
                        (1 - c) * GraphicsDevice.Viewport.Height);
                }).ToArray());

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            cardSlot = Content.Load<Texture2D>("ui/card_slot");
            cardBack = Content.Load<Texture2D>("deck/card_back_green");
            refreshMe = Content.Load<Texture2D>("ui/refresh");
            newGame = Content.Load<Texture2D>("ui/new_game");
            metaSmug = Content.Load<Texture2D>("ui/smug-logo");
            debug = Content.Load<Texture2D>("ui/wrench");
            undo = Content.Load<Texture2D>("ui/undo");
            solve = Content.Load<Texture2D>("ui/solve");
            paused = Content.Load<Texture2D>("ui/paused");
            debugFont = Content.Load<SpriteFont>("ui/Arial");

            soundFX = new List<SoundEffect> {
                Content.Load<SoundEffect>("audio/card-kick"),
                Content.Load<SoundEffect>("audio/card-parent"),
                Content.Load<SoundEffect>("audio/card-play"),
                Content.Load<SoundEffect>("audio/card-restack"),
                Content.Load<SoundEffect>("audio/card-undo"),
                Content.Load<SoundEffect>("audio/card-bounce")
            };

            gameWinFX = Content.Load<SoundEffect>("audio/game-win");
            popFX = Content.Load<SoundEffect>("audio/pop");

            dragonDrop = new DragonDrop<IDragonDropItem>(this, viewport);


            // table creates a fresh table.deck
            table = new TableClassic(this, spriteBatch, dragonDrop, cardBack, cardSlot, 20, 30, soundFX);

            table.muteSound = Properties.Settings.Default.mute;

            // load up the card assets for the new deck
            foreach (var card in table.drawPile.cards) {
                var location = "deck/" + card.suit + card.rank;
                card.SetTexture(Content.Load<Texture2D>(location));
            }

            table.InitializeTable();

            table.SetTable();

            Components.Add(dragonDrop);
            
            // more confetti stuff
            var textures = new List<Color> {
                new Color(255, 234, 142),
                new Color(129, 232, 138),
                new Color(239, 133, 189)
            }.Select(c => GeometricTextureFactory.Rectangle(GraphicsDevice, 10, 18, c))
                .ToList().AsReadOnly();

            confetti = new Confetti(textures);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                Keyboard.GetState().IsKeyDown(Keys.Escape) ||
                Keyboard.GetState().IsKeyDown(Keys.Q))
                Exit();

            currentMouse = Mouse.GetState();
            
            var point = viewport.PointToScreen(currentMouse.X, currentMouse.Y);

            newGameRect = new Rectangle(310, 20, newGame.Width, newGame.Height);
            newGameColor = Color.White;

            undoRect = new Rectangle(310, 80, undo.Width, undo.Height);
            undoColor = Color.White;

            solveRect = new Rectangle(310, 75, solve.Width, solve.Height);
            solveColor = Color.White;

            if (IsActive && table.isSetup && !table.isAnimating) {

                if (solveRect.Contains(point)) {
                    solveColor = Color.Aqua;
                    if (table.gameState == GameState.complete && click) table.solve = true;
                }

                if (newGameRect.Contains(point)) {

                    newGameColor = Color.Aqua;

                    if (click) {

                        gameWinPlaying = false;
                        table.NewGame(gameTime);
                        
                        foreach (var card in table.drawPile.cards) {
                            var location = "deck/" + card.suit + card.rank;
                            card.SetTexture(Content.Load<Texture2D>(location));
                        }

                        table.SetTable();
                    }
                }

                if (table.gameState == GameState.active && undoRect.Contains(point)) {
                    undoColor = Color.Aqua;
                    if (click) table.Undo(gameTime);
                }
                if (table.gameState == GameState.won && !gameWinPlaying) {
                    gameWinPlaying = true;
                    gameWinFX.Play();
                }

            }

#if DEBUG
                debugRect = new Rectangle(310, 140, debug.Width, debug.Height);
                debugColor = Color.White;
                if (debugRect.Contains(point)) {
                    debugColor = Color.Aqua;
                    if (click) foreach (var stack in table.stacks) stack.debug();
                }
#endif

            oldMouse = currentMouse;
            if (IsActive) table.Update(gameTime);

            if (table.gameState == GameState.won && !table.isAnimating) {
                // final confetti stuff
                if (transitionTimer.IsFinished(gameTime)) {
                    spawningConfetti = !spawningConfetti;
                    transitionTimer.Reset(gameTime);
                    explosionPositions.Reset();
                    explosionTimer.Reset(gameTime);
                    spawnTimer.Reset(gameTime);
                }

                if (spawningConfetti) {
                    if (spawnTimer.IsFinished(gameTime)) {
                        confetti.Sprinkle(confettiRate, new Rectangle(0, -10, GraphicsDevice.Viewport.Width, 10));
                        spawnTimer.Reset(gameTime);
                    }
                }
                else {
                    if (explosionTimer.IsFinished(gameTime)) {
                        if (!Properties.Settings.Default.mute)
                            explosionPositions.TryDoWithCurrent(p => confetti.Explode(20, p, popFX));
                        else explosionPositions.TryDoWithCurrent(p => confetti.Explode(20, p));
                        explosionTimer.Reset(gameTime);
                    }
                }
                confetti.Update(gameTime);
            }
            else {
                spawningConfetti = false;
                transitionTimer.Reset(gameTime);
                explosionPositions.Reset();
                explosionTimer.Reset(gameTime);
                spawnTimer.Reset(gameTime);
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

            if (!IsActive) {
                var pausedRect = new Rectangle(WINDOW_WIDTH/2-paused.Width/2, WINDOW_HEIGHT/2-paused.Height/2, paused.Width, paused.Height);
                spriteBatch.Draw(paused, pausedRect, Color.White);
            }
            else {
                var logoVect = new Vector2(10, WINDOW_HEIGHT - metaSmug.Height - 10);

                // only add the logo if contributing pull requests back to the "official" repository:
                // https://github.com/SoundGoddess/OpenSolitaire
                if (isOfficial) spriteBatch.Draw(metaSmug, logoVect, Color.White);

                if (table.gameState == GameState.complete) spriteBatch.Draw(solve, solveRect, solveColor);
                else spriteBatch.Draw(undo, undoRect, undoColor);

                spriteBatch.Draw(newGame, newGameRect, newGameColor);
                spriteBatch.Draw(refreshMe, new Vector2(35, 50), Color.White);

                var versionSize = debugFont.MeasureString(version);
                var versionPos = new Vector2(WINDOW_WIDTH - versionSize.X - 10, WINDOW_HEIGHT - versionSize.Y - 10);
                spriteBatch.DrawString(debugFont, version, versionPos, Color.Black);
         
    #if DEBUG
                foreach (var stack in table.stacks) {
                    var slot = stack.slot;
                    var textWidth = debugFont.MeasureString(slot.name);
                    var textPos = new Vector2(slot.Position.X + slot.Texture.Width / 2 - textWidth.X / 2, slot.Position.Y - 16);
                //    spriteBatch.DrawString(debugFont, slot.name, textPos, Color.Black);
                }
                spriteBatch.Draw(debug, debugRect, debugColor);
    #endif
                
                table.Draw(gameTime);
                if (table.gameState == GameState.won) confetti.Draw(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
