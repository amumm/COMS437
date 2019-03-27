using BEPUphysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Asteroid
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // The star background of the game
        Model skybox;


        Vector3 modelPosition;

        // The location of the camera in the world
        public static Vector3 CameraPosition
        {
            get;
            private set;
        }

        // The direction the camera is looking in the world
        public static Vector3 CameraDirection
        {
            get;
            private set;
        }


        float rotation;

        // The height of the screen
        float windowHeight;

        // The width of the screen
        float windowWidth;

        // The aspect ratio for the screen
        float aspectRatio;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef
            };

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

            // Make our BEPU Physics space a service
            Services.AddService<Space>(new Space());

            // Create two asteroids.  Note that asteroids automatically add themselves to
            // as a DrawableGameComponent as well as add an object into Bepu physics
            // that represents the asteroid.

            new Asteroids(this, new Vector3(-5, -5, -50), 2, new Vector3(0.2f, 0, 0), new Vector3(0.3f, 0.5f, 0.5f));
            new Asteroids(this, new Vector3(5, 5, -50), 3, new Vector3(-0.2f, 0, 0), new Vector3(-0.5f, -0.6f, 0.2f));

            modelPosition = Vector3.Zero;

            CameraPosition = new Vector3(0, 0, 0.1f);
            CameraDirection = new Vector3(0, 0, -1);

            windowHeight = graphics.PreferredBackBufferHeight;
            windowWidth = graphics.PreferredBackBufferWidth;
            aspectRatio = windowWidth / windowHeight;

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

            skybox = Content.Load<Model>("skybox");

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            rotation += 0.005f;

            // Update the physics engine based on how many seconds have passed since last update.
            Services.GetService<Space>().Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Yellow);

            Matrix world = Matrix.CreateScale(10.0f) * Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(rotation) * Matrix.CreateTranslation(modelPosition);
            Matrix view = Matrix.CreateLookAt(CameraPosition, modelPosition, Vector3.UnitY);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), aspectRatio, 0.01f, 1000f);
            foreach (ModelMesh mesh in skybox.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Alpha = 0.8f;
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }
    }
}