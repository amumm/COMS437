using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Model5
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Model5 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model myModel;
        float aspectRatio;
        // Set the position of the model in world space, and set the rotation.
        Vector3 modelPosition = Vector3.Zero;
        float modelRotation = MathHelper.PiOver2 + MathHelper.PiOver4;

        // Set the position of the camera in world space, for our view matrix.
        Vector3 cameraPosition = new Vector3(0.0f, 50.0f, 5000.0f);
        private ScrollingBackground myBackground;
        Vector3 modelVelocity = Vector3.Zero;

        SoundEffect soundEngine;
        SoundEffect soundHyperspaceActivation;


        SoundEffectInstance soundEngineInstance;



        public Model5()
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
            spriteBatch = new SpriteBatch(GraphicsDevice);
            myModel = Content.Load<Model>("Models\\p1_wedge");
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            myBackground = new ScrollingBackground();
            Texture2D background = Content.Load<Texture2D>("background");
            myBackground.Load(GraphicsDevice, background);

            soundEngine = Content.Load<SoundEffect>("Audio\\engine_2");
            soundEngineInstance = soundEngine.CreateInstance();
            soundHyperspaceActivation =
                 Content.Load<SoundEffect>("Audio\\hyperspace_activate");

            // TODO: use this.Content to load your game content here
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Get some input.
            UpdateInput();

            // Add velocity to the current position.
            modelPosition += modelVelocity;

            // Bleed off velocity over time.
            modelVelocity *= 0.95f;

            // The time since Update was called last.
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            myBackground.Update(elapsed * 100);

            base.Update(gameTime);
        }

        protected void UpdateInput()
        {


            KeyboardState currentKeyState = Keyboard.GetState();
            GamePadState currentGamePadState = GamePad.GetState(PlayerIndex.One);

            bool engineon = false;

            if (currentGamePadState.IsConnected)
            {
                // Rotate the model using the left thumbstick, and scale it down

                modelRotation -= currentGamePadState.ThumbSticks.Left.X * 0.10f;

                // Create some velocity if the right trigger is down.
                Vector3 modelVelocityAdd = Vector3.Zero;

                // Find out what direction we should be thrusting, 
                // using rotation.
                modelVelocityAdd.X = -(float)Math.Sin(modelRotation);
                modelVelocityAdd.Z = -(float)Math.Cos(modelRotation);

                // Now scale our direction by how hard the trigger is down.
                modelVelocityAdd *= currentGamePadState.Triggers.Right;

                if (currentGamePadState.Triggers.Right != 0f)
                {
                    engineon = true;
                }

                // Finally, add this vector to our velocity.
                modelVelocity += modelVelocityAdd;

                GamePad.SetVibration(PlayerIndex.One,
                    currentGamePadState.Triggers.Right,
                    currentGamePadState.Triggers.Right);


                // In case you get lost, press A to warp back to the center.
                if (currentGamePadState.Buttons.A == ButtonState.Pressed)
                {
                    modelPosition = Vector3.Zero;
                    modelVelocity = Vector3.Zero;
                    modelRotation = 0.0f;
                }
            }

            if (currentKeyState.IsKeyDown(Keys.A))
                modelRotation += 0.10f;

            if (currentKeyState.IsKeyDown(Keys.D))
                modelRotation -= 0.10f;


            if (currentKeyState.IsKeyDown(Keys.W))
            {
                Vector3 modelVelocityAdd = Vector3.Zero;

                // Find out what direction we should be thrusting, 
                // using rotation.
                modelVelocityAdd.X = -(float)Math.Sin(modelRotation);
                modelVelocityAdd.Z = -(float)Math.Cos(modelRotation);
                modelVelocity += modelVelocityAdd;
                engineon = true;
            }

            if (currentKeyState.IsKeyDown(Keys.X))
            {
                modelPosition = Vector3.Zero;
                modelVelocity = Vector3.Zero;
                modelRotation = 0.0f;
                soundHyperspaceActivation.Play();
            }

            if (engineon)
            {
                if (soundEngineInstance.State == SoundState.Stopped)
                {
                    soundEngineInstance.Volume = 0.75f;
                    soundEngineInstance.IsLooped = true;
                    soundEngineInstance.Play();
                }
                else
                {
                    soundEngineInstance.Resume();
                }
            }
            else
            {
                soundEngineInstance.Pause();
            }

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            myBackground.Draw(spriteBatch);
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] *
                        Matrix.CreateRotationY(modelRotation)
                        * Matrix.CreateTranslation(modelPosition);
                    effect.View = Matrix.CreateLookAt(cameraPosition,
                        Vector3.Zero, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), aspectRatio,
                        1.0f, 10000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
            base.Draw(gameTime);

        }

    }
}
