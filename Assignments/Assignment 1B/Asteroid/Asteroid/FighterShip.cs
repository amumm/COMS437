﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using ConversionHelper;
using Microsoft.Xna.Framework.Input;
using System;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace Asteroid
{
    public class FighterShip : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private Model model;
        private Sphere physicsObject;
        private Vector3 shipPosition;

        // The text displayed if you win the game
        private SpriteFont gameWinningText;

        // The text displayed if you lose the game
        private SpriteFont gameOverText;

        private float entitySizeScaler = 0.005f;
        private float modelSizeScaler = 0.01f;

        private SpriteFont fuelText;
        private float fuel = 200.0f;
        private float forwardFuelDepletionRate = 0.1f;
        private float reverseFuelDepletionRate = 0.05f;
        private float rotationFuelDepletionRate = 0.01f;

        private float rotationSpeed = 0.02f;
        private float forwardMovementModifier = 70.0f;
        private float reverseMovementmodifier = -40.0f;

        private SpriteFont healthText;
        private float health = 100.0f;

        private Texture2D reticle;
        private Vector2 reticleCenter;
        private Vector2 reticlePosition;
        private float reticleSpeed = 2.5f;

        private SpriteFont torpedoText;
        private int torpedoeStock = 10;
        private float torpedoReloadTime = 600.0f;
        private float timeSinceFiring = 0.0f;
        private Model torpedoModel;
        private Sphere torpedoPhysicsObject;

        private SpriteFont shieldText;
        private bool shieldStatus = false;
        private string shieldStatusText = "OFF";
        private float shieldDepletionRate = 0.05f;
        private float shieldSwitchTime = 200.0f;
        private float timeSinceShieldSwitch = 0.0f;

        private Matrix rotationMatrix;

        public FighterShip(Game game, Vector3 pos, float mass) : base(game)
        {
            physicsObject = new Sphere(MathConverter.Convert(pos), 1)
            {
                Mass = mass,
                LinearMomentum = new BEPUutilities.Vector3(),
                AngularMomentum = new BEPUutilities.Vector3(),
                AngularDamping = 0f,
                LinearDamping = 0f,
                Tag = this
            };

            game.Services.GetService<Space>().Add(physicsObject);
            game.Components.Add(this);
        }

        public override void Initialize()
        {
            shipPosition = Vector3.Zero;
            rotationMatrix = Matrix.Identity;
            reticlePosition = new Vector2(Main.ScreenWidth / 2, Main.ScreenHeight / 2);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            model = Game.Content.Load<Model>("ship");
            physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius * entitySizeScaler;
            physicsObject.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;

            torpedoModel = Game.Content.Load<Model>("missile");
            torpedoPhysicsObject = new Sphere(MathConverter.Convert(new Vector3(0, 0, 100)), 1)
            {
                AngularDamping = 0f,
                AngularMomentum = new BEPUutilities.Vector3(),
                LinearDamping = 0f,
                LinearMomentum = new BEPUutilities.Vector3(),
                Mass = 4

            };
            torpedoPhysicsObject.Radius = torpedoModel.Meshes[0].BoundingSphere.Radius * .1f;

            fuelText = Game.Content.Load<SpriteFont>("fuel");
            healthText = Game.Content.Load<SpriteFont>("health");
            torpedoText = Game.Content.Load<SpriteFont>("torpedo");
            shieldText = Game.Content.Load<SpriteFont>("shield");
            gameOverText = Game.Content.Load<SpriteFont>("gameover");
            gameWinningText = Game.Content.Load<SpriteFont>("gamewon");

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
            if ((health <= 0 || fuel <= 0) && !Main.gameWon)
            {
                Main.gameLost = true;
            }

            if (!Main.gameLost)
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
            }

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

            if (keyState.IsKeyDown(Keys.Left) || gamePadState.ThumbSticks.Left.X < 0.0f)
            {
                temp.X -= reticleSpeed;
                if (canMoveReticle(temp))
                {
                    reticlePosition.X -= reticleSpeed;
                }
            }

            if (keyState.IsKeyDown(Keys.Right) || gamePadState.ThumbSticks.Left.X > 0.0f)
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
            if (keyState.IsKeyDown(Keys.F) || gamePadState.IsButtonDown(Buttons.B))
            {
                if (timeSinceFiring > torpedoReloadTime && torpedoeStock > 0)
                {
                    timeSinceFiring = 0.0f;
                    torpedoeStock -= 1;

                    var xOffset = (Main.ScreenWidth / 2) - reticlePosition.X;
                    var yOffset = (Main.ScreenHeight / 2) - reticlePosition.Y;

                    var torpedoDirectionHorizontal = physicsObject.WorldTransform.Left * xOffset;
                    var torpedoDirectionVertical = physicsObject.WorldTransform.Up * yOffset * .5f;
                    var torpedoDirectionForward = physicsObject.WorldTransform.Forward * 100;

                    var torpedoPositionHorizontal = physicsObject.WorldTransform.Left * xOffset * .005f;
                    var torpedoPositionVertical = physicsObject.WorldTransform.Up * yOffset * .05f;
                    var torpedoPostionForward = physicsObject.position + (physicsObject.WorldTransform.Forward * 15);

                    var torpedoPosition = MathConverter.Convert(torpedoPostionForward + torpedoPositionHorizontal + torpedoPositionVertical);
                    var torpedoDirection = MathConverter.Convert(torpedoDirectionForward + torpedoDirectionHorizontal + torpedoDirectionVertical);
                    new Torpedo(Game, pos: torpedoPosition, mass: 10, linMomentum: torpedoDirection * 5, angMomentum: Vector3.Zero);
                }
            }
        }

        private void handleShield(KeyboardState keyState, GamePadState gamePadState)
        {

            if (keyState.IsKeyDown(Keys.Tab) || gamePadState.IsButtonDown(Buttons.Y))
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
            if (fuel < 0)
                fuel = 0;
        }

        private void setRotation(KeyboardState keyState, GamePadState gamePadState)
        {
            float yaw = 0f;
            float pitch = 0f;
            float roll = 0f;

            if (keyState.IsKeyDown(Keys.D) || gamePadState.ThumbSticks.Right.X > 0.0f)
                yaw -= rotationSpeed;
            if (keyState.IsKeyDown(Keys.A) || gamePadState.ThumbSticks.Right.X < 0.0f)
                yaw += rotationSpeed;
            if (keyState.IsKeyDown(Keys.S) || gamePadState.ThumbSticks.Right.Y < 0.0f)
                pitch -= rotationSpeed;
            if (keyState.IsKeyDown(Keys.W) || gamePadState.ThumbSticks.Right.Y > 0.0f)
                pitch += rotationSpeed;
            if (keyState.IsKeyDown(Keys.Q) || gamePadState.IsButtonDown(Buttons.LeftShoulder) || gamePadState.IsButtonDown(Buttons.DPadLeft))
                roll -= rotationSpeed;
            if (keyState.IsKeyDown(Keys.E) || gamePadState.IsButtonDown(Buttons.RightShoulder) || gamePadState.IsButtonDown(Buttons.DPadRight))
                roll += rotationSpeed;

            if (yaw != 0 || pitch != 0 || roll != 0)
            {
                fuel -= rotationFuelDepletionRate;
                if (fuel < 0)
                    fuel = 0;

                rotationMatrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);

                physicsObject.WorldTransform = MathConverter.Convert(rotationMatrix) * physicsObject.WorldTransform;
            }
        }

        private void setPosition(KeyboardState keyState, GamePadState gamePadState)
        {
            physicsObject.LinearMomentum *= 0.95f;
            physicsObject.AngularMomentum *= 0.95f;
            float direction = 0;
            float fuelDepletion = 0;
            bool canMove = false;
            if ((keyState.IsKeyDown(Keys.Space) && keyState.IsKeyDown(Keys.LeftShift)) || gamePadState.IsButtonDown(Buttons.LeftTrigger))
            {
                canMove = true;
                direction = reverseMovementmodifier;
                fuelDepletion = reverseFuelDepletionRate;
            }
            else if ((keyState.IsKeyDown(Keys.Space)) || gamePadState.IsButtonDown(Buttons.RightTrigger))
            {
                canMove = true;
                direction = forwardMovementModifier;
                fuelDepletion = forwardFuelDepletionRate;
            }

            var forward = physicsObject.WorldTransform.Forward;
            if (canMove)
            {
                fuel -= fuelDepletion;
                physicsObject.LinearMomentum += forward * direction;
                Main.skyboxPosition = MathConverter.Convert(physicsObject.position);
                if (fuel < 0)
                    fuel = 0;
            }

            var position = MathConverter.Convert(physicsObject.position + (physicsObject.WorldTransform.Up * Main.CameraHightScaler) + (physicsObject.WorldTransform.Backward * Main.CameraDepthScaler));
            var lookDirection = MathConverter.Convert(physicsObject.position + forward * 20);
            var up = MathConverter.Convert(physicsObject.WorldTransform.Up);

            Main.View = Matrix.CreateLookAt(position, lookDirection, up);
        }

        public override void Draw(GameTime gameTime)
        {
            
            if (health > 0)
            {
                foreach (var mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.Alpha = 0.8f;

                        var worldMatrix = Matrix.CreateScale(modelSizeScaler) * MathConverter.Convert(physicsObject.WorldTransform);

                        effect.World = worldMatrix;
                        effect.View = Main.View;
                        effect.Projection = Matrix.CreatePerspectiveFieldOfView(Main.FieldOfView, Main.AspectRatio, Main.NearClipPlane, Main.FarClipPlane);
                    }
                    mesh.Draw();
                }
            }

            var statusOffset = 160;
            var statusSpacing = 30;
            spriteBatch.Begin();
            spriteBatch.DrawString(fuelText, "Fuel: " + fuel.ToString("0.00"), new Vector2((Main.ScreenWidth) - statusOffset, Main.ScreenHeight - (statusSpacing * 1)), Color.AntiqueWhite);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.DrawString(healthText, "Health: " + health.ToString("0.00"), new Vector2((Main.ScreenWidth) - statusOffset, Main.ScreenHeight - (statusSpacing * 2)), Color.AntiqueWhite);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.DrawString(torpedoText, "Torpedoes: " + torpedoeStock, new Vector2((Main.ScreenWidth) - statusOffset, Main.ScreenHeight - (statusSpacing * 3)), Color.AntiqueWhite);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.DrawString(shieldText, "Shield: " + shieldStatusText, new Vector2((Main.ScreenWidth) - statusOffset, Main.ScreenHeight - (statusSpacing * 4)), Color.AntiqueWhite);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw(reticle, reticlePosition, null, Color.White, 0f, reticleCenter, Vector2.One, SpriteEffects.None, 0f);
            spriteBatch.End();

            if (Main.gameLost)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(gameOverText, "Game Over ", new Vector2((Main.ScreenWidth / 2) - 165, Main.ScreenHeight / 2), Color.White);
                spriteBatch.End();
            }

            if (Main.gameWon)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(gameWinningText, "You Win! ", new Vector2((Main.ScreenWidth / 2) - 110, Main.ScreenHeight / 2), Color.AntiqueWhite);
                spriteBatch.End();
            }

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);
        }

        void HandleCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            var otherEntityInformation = other as EntityCollidable;
            if (otherEntityInformation != null)
            {
                var otherGameComponent = otherEntityInformation.Entity.Tag as IGameComponent;
                var otherType = otherGameComponent.GetType().ToString().Substring(9);

                var senderGameComponent = sender.Entity.Tag as IGameComponent;

                switch (otherType)
                {
                    case "Asteroids":
                        if (!shieldStatus && health > 0)
                            health -= 10.0f;
                        break;
                    case "Buoy":
                        var buoy = otherGameComponent as Buoy;
                        handleBuoyCollision(buoy.buoyType);
                        break;
                }
            }
        }

        private void handleBuoyCollision(int type)
        {
            switch (type)
            {
                case 0: // Health
                    health += 20;
                    if (health > 100)
                        health = 100;
                    break;
                case 1: // Fuel
                    fuel += 20;
                    if (fuel > 200)
                        fuel = 200;
                    break;
                case 2: // Torpedos
                    torpedoeStock += 2;
                    if (torpedoeStock > 10)
                        torpedoeStock = 10;
                    break;

            }
        }
    }
}
