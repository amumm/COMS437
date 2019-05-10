using BEPUphysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;

namespace Asteroid
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        // True if you have won the game false if still playing
        public static bool gameWon;

        // True if you have lost the game false if you are still playing
        public static bool gameLost;

        // The star background of the game
        private Model skyboxModel;

        // The location of the skybox in the world
        public static Vector3 skyboxPosition;

        // The size of the skybox and consequently the playable world
        private static float skyboxSize;

        // The location of the camera in the world
        public static Vector3 CameraPosition;

        // The direction the camera is looking in the world
        public static Vector3 CameraDirection;

        // The current up direction from the perspective of the player
        public static Vector3 CameraUp;

        public static Matrix View;

        public static Matrix Translation;

        // The distance behind the player that the camera is offset
        public static float CameraDepthScaler = 1.0f;

        // The distance above the player that the camera is offset
        public static float CameraHightScaler = 8.0f;

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

        // The screens height
        public static float ScreenHeight
        {
            get;
            private set;
        }

        // The screens width
        public static float ScreenWidth
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

            // Creates the mothership that is the objective of this game
            new Mothership(this, pos: new Vector3(40, 200, -3000), mass: 10000, linMomentum: new Vector3(5000, 10000, -20000), angMomentum: new Vector3(0, 0, 0));

            Random rand = new Random();

            // Creates all of the obstacles in the game
            createAsteroids(rand);

            // Creates all of the refuels and resupplies in the game
            createBuoys(rand);

            Player = new FighterShip(this, pos: new Vector3(x: 0, y: 0, z: 0), mass: 10);

            skyboxPosition = Vector3.Zero;
            skyboxSize = 100000f;

            CameraDirection = new Vector3(0, 0, 0);

            CameraPosition = new Vector3(0, 100, 100);

            CameraUp = Vector3.Up;

            View = Matrix.CreateLookAt(CameraPosition, CameraDirection, CameraUp);

            Translation = Matrix.CreateLookAt(CameraPosition, CameraDirection, CameraUp);

            FieldOfView = MathHelper.ToRadians(45);

            NearClipPlane = 0.1f;
            FarClipPlane = 1000000f;

            ScreenHeight = graphics.PreferredBackBufferHeight;
            ScreenWidth = graphics.PreferredBackBufferWidth;
            AspectRatio = ScreenWidth / ScreenHeight;

            base.Initialize();
        }

        private void createAsteroids(Random rand)
        {
            int i = 0;
            while(i < 1000)
            {
                int posX = getRandomInRange(rand, -2000, 2000);  
                int posY = getRandomInRange(rand, -1000, 1000);
                int posZ = getRandomInRange(rand, -3000, 1500);
                Vector3 position = new Vector3(posX, posY, posZ);

                int linX = getRandomInRange(rand, -4000, 4000);
                int linY = getRandomInRange(rand, -4000, 4000);
                int linZ = getRandomInRange(rand, -4000, 4000);
                Vector3 linearMomentum = new Vector3(linX, linY, linZ);

                int angX = getRandomInRange(rand, -500, 500);
                int angY = getRandomInRange(rand, -500, 500);
                int angZ = getRandomInRange(rand, -500, 500);
                Vector3 angularMomentum = new Vector3(angX, angY, angZ);

                int mass = getRandomInRange(rand, 0, 800);
                int size = rand.Next(0, 3);

                new Asteroids(this, size: size, pos: position, mass: mass, linMomentum: linearMomentum, angMomentum: angularMomentum);
                i++;
            }
            new Asteroids(this, size: 0, pos: new Vector3(300, -5, -450), mass: 200, linMomentum: new Vector3(-2000f, 0, 0), angMomentum: new Vector3(0.3f, 0.5f, 0.5f));
            new Asteroids(this, size: 0, pos: new Vector3(40, -5, -45), mass: 200, linMomentum: new Vector3(-1000f, 0, 0), angMomentum: new Vector3(0.3f, 0.5f, 0.5f));
            new Asteroids(this, size: 0, pos: new Vector3(-40, -5, -50), mass: 300, linMomentum: new Vector3(1000f, 0, 0), angMomentum: new Vector3(-0.5f, -0.6f, 0.2f));
        }

        private void createBuoys(Random rand)
        {
            int i = 0;
            while (i < 100)
            {
                int posX = getRandomInRange(rand, -750, 750);
                int posY = getRandomInRange(rand, -400, 400);
                int posZ = getRandomInRange(rand, -1250, 750);
                Vector3 position = new Vector3(posX, posY, posZ);

                int type = rand.Next(0, 3);

                new Buoy(this, pos: position, type: type);
                i++;
            }
            new Buoy(this, pos: new Vector3(20, -5, 100), type: 0);
            new Buoy(this, pos: new Vector3(0, -5, 100), type: 1);
            new Buoy(this, pos: new Vector3(-20, -5, 100), type: 2);
        }

        private int getRandomInRange(Random rand, int min, int max)
        {
            return min + rand.Next(0, max * 2);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //skyboxModel = Content.Load<Model>("skybox");
            skyboxModel = Content.Load<Model>("skybox_3");
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

            Matrix world = Matrix.CreateScale(skyboxSize) * Matrix.CreateTranslation(skyboxPosition);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearClipPlane, FarClipPlane);
            foreach (ModelMesh mesh in skyboxModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //var num = 10;
                    //effect.EnableDefaultLighting();
                    //effect.DirectionalLight0.DiffuseColor = new Vector3(num, num, num);
                    //effect.DirectionalLight1.DiffuseColor = new Vector3(num, num, num);
                    //effect.DirectionalLight2.DiffuseColor = new Vector3(num, num, num);
                    //effect.PreferPerPixelLighting = true;

                    effect.Alpha = 1.0f;
                    //effect.Alpha = 0.5f;
                    effect.World = world;
                    effect.View = View;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);
        }
    }
}