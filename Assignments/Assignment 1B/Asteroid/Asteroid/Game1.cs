﻿using BEPUphysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        // The current up direction from the perspective of the player
        public static Vector3 CameraUp;

        public static Matrix View;

        public static Matrix Translation;

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

            new Mothership(this, pos: new Vector3(40, -5, -500), mass: 10000, linMomentum: new Vector3(0, 0, 0), angMomentum: new Vector3(0, 0, 0));
            new Asteroids(this, pos: new Vector3(40, -5, -45), mass: 200, linMomentum: new Vector3(-1000f, 0, 0), angMomentum: new Vector3(0.3f, 0.5f, 0.5f));
            new Asteroids(this, pos: new Vector3(-40, -5, -50), mass: 300, linMomentum: new Vector3(1000f, 0, 0), angMomentum: new Vector3(-0.5f, -0.6f, 0.2f));
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

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //skyboxModel = Content.Load<Model>("skybox");
            skyboxModel = Content.Load<Model>("skybox_2");
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
                    var num = 10;
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.DiffuseColor = new Vector3(num, num, num);
                    effect.DirectionalLight1.DiffuseColor = new Vector3(num, num, num);
                    effect.DirectionalLight2.DiffuseColor = new Vector3(num, num, num);
                    effect.PreferPerPixelLighting = true;

                    effect.Alpha = 0.5f;
                    effect.World = world;
                    effect.View = View;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }
    }
}