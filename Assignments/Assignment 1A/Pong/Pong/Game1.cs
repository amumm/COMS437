using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;

namespace Pong
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        float windowHeight;
        float windowWidth;

        SoundEffect boop;

        SpriteFont player1ScoreText;
        SpriteFont player2ScoreText;

        int player1Score = 0;
        int player2Score = 0;

        Texture2D paddleTexture;
        Vector2 paddleCenter;

        Vector2 player1Position;
        Vector2 player2Position;
        float playerHeightOffset;
        float playerWidthOffset;
        float playerSpeed;


        Texture2D ballTexture;
        Vector2 ballCenter;
        Vector2 ballPosition;
        Vector2 ballDirection;
        float ballOffset;
        float ballSpeed;

        bool colliding = false;
        bool reset = false;

        Random rand = new Random();
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            windowHeight = graphics.PreferredBackBufferHeight;
            windowWidth = graphics.PreferredBackBufferWidth;
            ballPosition = new Vector2(windowWidth / 2, windowHeight / 2);
            float xDir = -1.2f;
            float yDir = -1.2f;
            if (rand.Next(-1, 2) > 0) {
                xDir = 1.2f;
            }
            if(rand.Next(-1, 2) > 0) {
                yDir = 1.2f;
            }

            ballDirection = new Vector2((float)rand.NextDouble() * xDir, (float)rand.NextDouble() * yDir);
            ballSpeed = 500f;

            player1Position = new Vector2(50, windowHeight / 2);
            player2Position = new Vector2(windowWidth - 50, windowHeight / 2);
            playerSpeed = 300f;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            ballTexture = Content.Load<Texture2D>("ball");
            ballOffset = ballTexture.Width / 2;

            paddleTexture = Content.Load<Texture2D>("paddle");
            playerHeightOffset = paddleTexture.Height / 2;
            playerWidthOffset = paddleTexture.Width / 2;

            player1ScoreText = Content.Load<SpriteFont>("score");
            player2ScoreText = Content.Load<SpriteFont>("score");

            boop = Content.Load<SoundEffect>("boop");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            if (reset)
            {
                reset = false;
                System.Threading.Thread.Sleep(600);

            }

            // Handle Player Input
            var kstate = Keyboard.GetState();

            float playerMovement = playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (kstate.IsKeyDown(Keys.W))
                player1Position.Y -= playerMovement;

            if (kstate.IsKeyDown(Keys.S))
                player1Position.Y += playerMovement;

            if (kstate.IsKeyDown(Keys.Up))
                player2Position.Y -= playerMovement;

            if (kstate.IsKeyDown(Keys.Down))
                player2Position.Y += playerMovement;

            player1Position.Y = Math.Min(Math.Max(paddleTexture.Height / 2, player1Position.Y), graphics.PreferredBackBufferHeight - paddleTexture.Height / 2);
            player2Position.Y = Math.Min(Math.Max(paddleTexture.Height / 2, player2Position.Y), graphics.PreferredBackBufferHeight - paddleTexture.Height / 2);

            // Handle Ball Movement
            if (ballPosition.X - ballOffset <= player1Position.X + playerWidthOffset)
            {
                if (ballPosition.Y >= player1Position.Y - playerHeightOffset && ballPosition.Y <= player1Position.Y + playerHeightOffset)
                {
                    if (!colliding)
                    {
                        ballDirection = Vector2.Reflect(ballDirection, Vector2.UnitX);
                        boop.Play();
                        colliding = true;
                    } 
                }
            } else if (ballPosition.X + ballOffset >= player2Position.X - playerWidthOffset)
            {
                if (ballPosition.Y >= player2Position.Y - playerHeightOffset && ballPosition.Y <= player2Position.Y + playerHeightOffset)
                {
                    if (!colliding)
                    {
                        ballDirection = Vector2.Reflect(ballDirection, Vector2.UnitX);
                        boop.Play();
                        colliding = true;
                    }
                }
            } else
            {
                colliding = false;
            }

            if (ballPosition.Y - ballOffset <= 0.0f)
            {
                ballDirection = Vector2.Reflect(ballDirection, Vector2.UnitY);
            } else if (ballPosition.Y + ballOffset >= windowHeight)
            {
                ballDirection = Vector2.Reflect(ballDirection, Vector2.UnitY);
            } else if (ballPosition.X - ballOffset <= 0.0f)
            {
                player2Score++;
                resetBall();
            }
            else if (ballPosition.X + ballOffset >= windowWidth) {
                player1Score++;
                resetBall();
            }


            ballPosition += ballDirection * ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            ballPosition.X = Math.Min(Math.Max(ballTexture.Width / 2, ballPosition.X), graphics.PreferredBackBufferWidth - ballTexture.Width / 2);
            ballPosition.Y = Math.Min(Math.Max(ballTexture.Height / 2, ballPosition.Y), graphics.PreferredBackBufferHeight - ballTexture.Height / 2);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkCyan);

            // TODO: Add your drawing code here
            ballCenter = new Vector2(ballTexture.Width / 2, ballTexture.Height / 2);
            paddleCenter = new Vector2(paddleTexture.Width / 2, paddleTexture.Height / 2);

            spriteBatch.Begin();
            spriteBatch.Draw(ballTexture, ballPosition, null, Color.White, 0f, ballCenter, Vector2.One, SpriteEffects.None, 0f);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw(paddleTexture, player1Position, null, Color.White, 0f, paddleCenter, Vector2.One, SpriteEffects.None, 0f);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw(paddleTexture, player2Position, null, Color.White, 0f, paddleCenter, Vector2.One, SpriteEffects.None, 0f);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.DrawString(player1ScoreText, "Player 1 Score: " + player1Score, new Vector2((windowWidth / 2) - 200, 50), Color.Black);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.DrawString(player2ScoreText, "Player 2 Score: " + player2Score, new Vector2((windowWidth / 2) + 75 , 50), Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void resetBall()
        {
            ballPosition.X = windowWidth / 2;
            ballPosition.Y = windowHeight / 2;
            float xDir = -1.0f;
            float yDir = -1.0f;
            if (rand.Next(-1, 2) > 0)
            {
                xDir = 1.0f;
            }
            if (rand.Next(-1, 2) > 0)
            {
                yDir = 1.0f;
            }

            ballDirection = new Vector2((float)rand.NextDouble() * xDir, (float)rand.NextDouble() * yDir);
            reset = true;
        }

    }
}
