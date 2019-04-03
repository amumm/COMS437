﻿using Microsoft.Xna.Framework;
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
        private SpriteBatch spriteBatch;
        private Model model;
        private Sphere physicsObject;

        private SpriteFont fuelText;
        private float fuel = 100.0f;
        private float forwardFuelDepletionRate = 0.1f;
        private float reverseFuelDepletionRate = 0.05f;
        private float rotationFuelDepletionRate = 0.01f;

        private float rotationSpeed = 0.02f;
        private float forwardMovementModifier = 1.0f;
        private float reverseMovementmodifier = -0.5f;

        private SpriteFont healthText;
        private float health = 100.0f;

        private Texture2D reticle;
        private Vector2 reticleCenter;
        private Vector2 reticlePosition;
        private float reticleSpeed = 2.5f;

        private SpriteFont torpedoText;
        private int torpedoeStock = 5;
        private float torpedoReloadTime = 600.0f;
        private float timeSinceFiring = 0.0f;

        private SpriteFont shieldText;
        private bool shieldStatus = false;
        private string shieldStatusText = "OFF";
        private float shieldDepletionRate = 0.05f;
        private float shieldSwitchTime = 100.0f;
        private float timeSinceShieldSwitch = 0.0f;



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
            reticlePosition = new Vector2(Main.ScreenWidth / 2, Main.ScreenHeight / 2);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            model = Game.Content.Load<Model>("ship");
            physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius * .18f;

            fuelText = Game.Content.Load<SpriteFont>("fuel");
            healthText = Game.Content.Load<SpriteFont>("health");
            torpedoText = Game.Content.Load<SpriteFont>("torpedo");
            shieldText = Game.Content.Load<SpriteFont>("shield");

            reticle = Game.Content.Load<Texture2D>("reticle");
            reticleCenter = new Vector2(reticle.Width / 2, reticle.Height / 2);


            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            timeSinceFiring += gameTime.ElapsedGameTime.Milliseconds;
            timeSinceShieldSwitch += gameTime.ElapsedGameTime.Milliseconds;

            aim(keyState, gamePadState);
            shoot(keyState, gamePadState);

            handleShield(keyState, gamePadState);

            setRotation(keyState, gamePadState);
            setPosition(keyState, gamePadState);

            base.Update(gameTime);
        }

        private void aim(KeyboardState keyState, GamePadState gamePadState)
        {
            Vector2 temp = new Vector2(reticlePosition.X, reticlePosition.Y);

            if (keyState.IsKeyDown(Keys.Up) || gamePadState.ThumbSticks.Left.Y > 0.0f)
            {
                temp.Y -= reticleSpeed;
                if (canMoveReticle(temp))
                {
                    reticlePosition.Y -= reticleSpeed;
                }
            }
            if (keyState.IsKeyDown(Keys.Down) || gamePadState.ThumbSticks.Left.Y < 0.0f)
            {
                temp.Y += reticleSpeed;
                if (canMoveReticle(temp))
                {
                    reticlePosition.Y += reticleSpeed;
                }
            }

            if (keyState.IsKeyDown(Keys.Left) || gamePadState.ThumbSticks.Left.X > 0.0f)
            {
                temp.X -= reticleSpeed;
                if (canMoveReticle(temp))
                {
                    reticlePosition.X -= reticleSpeed;
                }
            }

            if (keyState.IsKeyDown(Keys.Right) || gamePadState.ThumbSticks.Left.X < 0.0f)
            {
                temp.X += reticleSpeed;
                if (canMoveReticle(temp))
                {
                    reticlePosition.X += reticleSpeed;
                }
            }

        }

        private bool canMoveReticle(Vector2 position)
        {
            float xDiff = position.X - (Main.ScreenWidth / 2);
            float yDiff = position.Y - (Main.ScreenHeight / 2);
            float distance = (float)Math.Sqrt(Math.Pow(xDiff, 2.0f) + Math.Pow(yDiff, 2.0f));
            
            return distance <= 100.0f;
        }

        private void shoot(KeyboardState keyState, GamePadState gamePadState)
        {
            if (keyState.IsKeyDown(Keys.F))
            {
                if (timeSinceFiring > torpedoReloadTime && torpedoeStock > 0)
                {
                    timeSinceFiring = 0.0f;
                    torpedoeStock -= 1;
                }
            }
        }

        private void handleShield(KeyboardState keyState, GamePadState gamePadState)
        {

            if (keyState.IsKeyDown(Keys.Tab))
            {
                if (timeSinceShieldSwitch > shieldSwitchTime)
                {
                    timeSinceShieldSwitch = 0.0f;
                    shieldStatus = !shieldStatus;
                    if (shieldStatus)
                        shieldStatusText = "ON";
                    else
                        shieldStatusText = "OFF";
                }
            }

            if (shieldStatus && fuel - shieldDepletionRate >= 0)
                fuel -= shieldDepletionRate;
        }

        private void setRotation(KeyboardState keyState, GamePadState gamePadState)
        {
            float yaw = 0f;
            float pitch = 0f;
            float roll = 0f;

            if (keyState.IsKeyDown(Keys.A))
                yaw -= rotationSpeed;
            if (keyState.IsKeyDown(Keys.D))
                yaw += rotationSpeed;
            if (keyState.IsKeyDown(Keys.W))
                pitch -= rotationSpeed;
            if (keyState.IsKeyDown(Keys.S))
                pitch += rotationSpeed;
            if (keyState.IsKeyDown(Keys.Q))
                roll -= rotationSpeed;
            if (keyState.IsKeyDown(Keys.E))
                roll += rotationSpeed;

            if (fuel  - rotationFuelDepletionRate > 0 &&
                (yaw != 0 || pitch != 0 || roll != 0))
            {
                fuel -= rotationFuelDepletionRate;
                rotationMatrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
                Main.View *= rotationMatrix;
            }
        }

        private void setPosition(KeyboardState keyState, GamePadState gamePadState)
        {
            float direction = 0;
            float fuelDepletion = 0;
            if (keyState.IsKeyDown(Keys.Space) && keyState.IsKeyDown(Keys.LeftShift))
            {
                direction = reverseMovementmodifier;
                fuelDepletion = reverseFuelDepletionRate;
            }
            else if (keyState.IsKeyDown(Keys.Space))
            {
                direction = forwardMovementModifier;
                fuelDepletion = forwardFuelDepletionRate;
            }

            if (fuel - fuelDepletion > 0)
            {
                fuel -= fuelDepletion;
                Main.View *= Matrix.CreateTranslation(0, 0, direction);
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

            spriteBatch.Begin();
            spriteBatch.DrawString(shieldText, "Shield: " + shieldStatusText, new Vector2((Main.ScreenWidth) - 110, Main.ScreenHeight - 200), Color.AntiqueWhite);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw(reticle, reticlePosition, null, Color.White, 0f, reticleCenter, Vector2.One, SpriteEffects.None, 0f);
            spriteBatch.End();

            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Alpha = 0.8f;

                    var worldMatrix = Matrix.CreateScale(0.05f) * Matrix.CreateTranslation(0, 0, 0);
                    effect.World = worldMatrix;

                    effect.View = Matrix.CreateLookAt(new Vector3(0, 45, 20), new Vector3(0, 0, -100), Main.CameraUp);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(Main.FieldOfView, Main.AspectRatio, Main.NearClipPlane, Main.FarClipPlane);
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }

    }
}
