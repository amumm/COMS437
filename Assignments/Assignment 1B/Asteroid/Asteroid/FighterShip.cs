using XNA = Microsoft.Xna.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using ConversionHelper;
using Microsoft.Xna.Framework.Input;
using System;

namespace Asteroid
{
    internal class FighterShip : DrawableGameComponent
    {

        private Model model;
        private Sphere physicsObject;
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
            model = Game.Content.Load<Model>("asteroid");
            physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius * .18f;

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.A))
                yaw -= 0.10f;

            if (keyState.IsKeyDown(Keys.D))
                yaw += 0.10f;

            if (keyState.IsKeyDown(Keys.S))
                pitch -= 0.10f;

            if (keyState.IsKeyDown(Keys.W))
                pitch += 0.10f;

            if (keyState.IsKeyDown(Keys.Q))
                roll -= 0.10f;

            if (keyState.IsKeyDown(Keys.E))
                roll += 0.10f;

            if (keyState.IsKeyDown(Keys.Space) && keyState.IsKeyDown(Keys.LeftShift))
            {
                setPostiotion(-1);
            }
            else if (keyState.IsKeyDown(Keys.Space))
            {
                setPostiotion(1);
            }

            base.Update(gameTime);
        }

        private void setPostiotion(int direction)
        {
            Vector3 modelVelocityAdd = Vector3.Zero;

            modelVelocityAdd.X = -(float)Math.Sin(yaw) * -(float)Math.Cos(pitch);
            modelVelocityAdd.Y = -(float)Math.Cos(yaw);
            modelVelocityAdd.Z = -(float)Math.Sin(pitch) * -(float)Math.Sin(yaw);
            physicsObject.Position += MathConverter.Convert(modelVelocityAdd) * direction;
        }

        public override void Draw(GameTime gameTime)
        {
            float aspectRatio = Game.GraphicsDevice.Viewport.AspectRatio;
            float fieldOfView = XNA.MathHelper.PiOver4;
            float nearClipPlane = 0.3f;
            float farClipPlane = 200f;

            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Alpha = 0.8f;

                    var rotation = Matrix.CreateRotationX(pitch) * Matrix.CreateRotationY(yaw) * Matrix.CreateRotationZ(roll);

                    effect.World = XNA.Matrix.CreateScale(0.25f) * rotation * MathConverter.Convert(physicsObject.WorldTransform);
                    //effect.View = XNA.Matrix.CreateLookAt(Main.CameraPosition, Main.CameraDirection, XNA.Vector3.Up);
                    effect.Projection = XNA.Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }

    }
}
