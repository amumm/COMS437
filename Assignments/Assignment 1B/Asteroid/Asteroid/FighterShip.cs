using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using ConversionHelper;
using Microsoft.Xna.Framework.Input;
using System;

namespace Asteroid
{
    public class FighterShip : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        private Model model;
        private Sphere physicsObject;

        SpriteFont fuelText;
        private float fuel = 100.0f;
        private float forwardFuelDepletionRate = 0.1f;
        private float reverseFuelDepletionRate = 0.05f;
        private float rotationFuelDepletionRate = 0.01f;

        private float forwardMovementModifier = 1.0f;
        private float reverseMovementmodifier = -0.5f;

        SpriteFont healthText;
        private float health = 100.0f;

        SpriteFont torpedoText;
        private int torpedoeStock = 5;

        private float rotationSpeed = 0.05f;

        float yaw = 0f;
        float pitch = 0f;
        float roll = 0f;

        private Vector3 CurrentPosition
        {
            get
            {
                return MathConverter.Convert(physicsObject.Position);
            }

            set { }
        }

        private Matrix rotationMatrix;

        public FighterShip(Game game) : base(game)
        {
            game.Components.Add(this);
        }

        public FighterShip(Game game, Vector3 pos, float mass) : this(game)
        {
            physicsObject = new Sphere(MathConverter.Convert(pos), 1)
            {
                AngularDamping = 0f,
                LinearDamping = 0f,
                Mass = mass
            };

            CurrentPosition = pos;

            Game.Services.GetService<Space>().Add(physicsObject);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //model = Game.Content.Load<Model>("asteroid");
            model = Game.Content.Load<Model>("ship");
            physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius * .18f;

            fuelText = Game.Content.Load<SpriteFont>("fuel");
            healthText = Game.Content.Load<SpriteFont>("health");
            torpedoText = Game.Content.Load<SpriteFont>("torpedo");
            //physicsObject.Radius = 1f;

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            //float yaw = 0f;
            //float pitch = 0f;
            //float roll = 0f;

            if (keyState.IsKeyDown(Keys.F))
            {
                shoot();
            }

            if (keyState.IsKeyDown(Keys.A))
            {
                yaw -= rotationSpeed;
                //setRotation(yaw, pitch, roll);

            }

            if (keyState.IsKeyDown(Keys.D))
            {
                yaw += rotationSpeed;
                //setRotation(yaw, pitch, roll);

            }

            if (keyState.IsKeyDown(Keys.S))
            {
                pitch -= rotationSpeed;
                //setRotation(yaw, pitch, roll);

            }

            if (keyState.IsKeyDown(Keys.W))
            {
                pitch += rotationSpeed;
                //setRotation(yaw, pitch, roll);

            }

            if (keyState.IsKeyDown(Keys.Q))
            {
                roll -= rotationSpeed;
                //setRotation(yaw, pitch, roll);

            }

            if (keyState.IsKeyDown(Keys.E))
            {
                roll += rotationSpeed;
                //setRotation(yaw, pitch, roll);
            }

            rotationMatrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);

            //Main.CameraDirection = rotationMatrix.Translation;
            //Main.CameraPosition = rotationMatrix.Translation + (rotationMatrix.Backward * 300f) + (rotationMatrix.Up * 200f);
            //Main.CameraUp = Vector3.Normalize(rotationMatrix.Up);

            if (keyState.IsKeyDown(Keys.Space) && keyState.IsKeyDown(Keys.LeftShift))
                setPosition(reverseMovementmodifier);
            else if (keyState.IsKeyDown(Keys.Space))
                setPosition(forwardMovementModifier);

            //Main.CameraDirection = rotationMatrix.Translation;
            //Main.CameraPosition = rotationMatrix.Translation + (rotationMatrix.Backward * 300f) + (rotationMatrix.Up * 200f);
            //Main.CameraUp = Vector3.Normalize(rotationMatrix.Up);

            base.Update(gameTime);
        }

        private void setRotation(float yaw, float pitch, float roll)
        {
            fuel -= rotationFuelDepletionRate;
            rotationMatrix += Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);

            Main.CameraDirection = rotationMatrix.Translation;
            Main.CameraPosition = rotationMatrix.Translation + (rotationMatrix.Backward* 300f) + (rotationMatrix.Up* 200f);
            Main.CameraUp = Vector3.Normalize(rotationMatrix.Up);
        }

        private void shoot()
        {
            torpedoeStock -= 1;
        }

        private void setPosition(float direction)
        {
            if (direction > 0)
            {
                fuel -= forwardFuelDepletionRate;
                physicsObject.Position += MathConverter.Convert(rotationMatrix.Forward) * direction;
            }
            else{
                fuel -= reverseFuelDepletionRate;
                physicsObject.Position += MathConverter.Convert(rotationMatrix.Forward) * direction;

            }

            
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(fuelText, "Fuel: " + fuel.ToString("0.00"), new Vector2((Main.ScreenWidth) - 110, Main.ScreenHeight - 50), Color.AntiqueWhite);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.DrawString(healthText, "Health: " + health.ToString("0.00"), new Vector2((Main.ScreenWidth) - 110, Main.ScreenHeight - 100), Color.AntiqueWhite);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.DrawString(torpedoText, "Torpedoes: " + torpedoeStock, new Vector2((Main.ScreenWidth) - 110, Main.ScreenHeight - 150), Color.AntiqueWhite);
            spriteBatch.End();

            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Alpha = 0.8f;

                    var worldMatrix = Matrix.CreateScale(0.05f) * rotationMatrix * MathConverter.Convert(physicsObject.WorldTransform);
                    effect.World = worldMatrix;

                    effect.View = Matrix.CreateLookAt(Main.CameraPosition, Main.CameraDirection, Main.CameraUp);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(Main.FieldOfView, Main.AspectRatio, Main.NearClipPlane, Main.FarClipPlane);
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }

    }
}
