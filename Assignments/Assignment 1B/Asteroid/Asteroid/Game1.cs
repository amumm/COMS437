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
        private Model skyboxModel;

        // The location of the skybox in the world
        private Vector3 skyboxPosition;

        // The size of the skybox and consequently the playable world
        private static float skyboxSize;

        // The location of the camera in the world
        public static Vector3 CameraPosition;

        // The direction the camera is looking in the world
        public static Vector3 CameraDirection;

        // The distance behind the player that the camera is offset
        private float cameraDepthScaler = 1.0f;

        // The distance above the player that the camera is offset
        private float cameraHightScaler = 1.0f;

        // The distance to the near view plane
        public static float NearClipPlane
        {
            get;
            private set;
        }

        // The distance to the far view plane
        public static float FarClipPlane
        {
            get;
            private set;
        }

        // The aspect ratio for the screen
        public static float AspectRatio
        {
            get;
            private set;
        }

        // The games field of view
        public static float FieldOfView
        {
            get;
            private set;
        }

        // The player
        public static FighterShip Player
        {
            get;
            private set;
        }

        private float rotation;

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

            //new Asteroids(this, new Vector3(-7, -5, -50), 2, new Vector3(1.4f, 0, 0), new Vector3(0.3f, 0.5f, 0.5f));
            //new Asteroids(this, pos: new Vector3(x: 7, y: 5, z: -50), mass: 3, linMomentum: new Vector3(x: -1.4f, y: 0, z: 0), angMomentum: new Vector3(-0.5f, -0.6f, 0.2f));
            Player = new FighterShip(this, pos: new Vector3(x: 0, y: 0, z: 0), mass: 3);

            skyboxPosition = Vector3.Zero;
            skyboxSize = 1000f;

            CameraDirection = new Vector3(0, 0, 0);

            CameraPosition = new Vector3(0, 200, 300);

            FieldOfView = MathHelper.ToRadians(45);

            NearClipPlane = 0.1f;
            FarClipPlane = 10000f;

            var windowHeight = graphics.PreferredBackBufferHeight;
            var windowWidth = graphics.PreferredBackBufferWidth;
            AspectRatio = windowWidth / windowHeight;

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

            skyboxModel = Content.Load<Model>("skybox");
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
            GraphicsDevice.Clear(Color.Black);

            //Matrix world = Matrix.CreateScale(skyboxSize) * Matrix.CreateRotationY(rotation) * Matrix.CreateTranslation(skyboxPosition);
            Matrix world = Matrix.CreateScale(skyboxSize) * Matrix.CreateTranslation(skyboxPosition);
            Matrix view = Matrix.CreateLookAt(CameraPosition, CameraDirection, Vector3.UnitY);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearClipPlane, FarClipPlane);
            foreach (ModelMesh mesh in skyboxModel.Meshes)
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