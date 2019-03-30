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

        private Model model;
        private Sphere physicsObject;

        private float yaw = 0f;
        private float pitch = 0f;
        private float roll = 0f;

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
            //model = Game.Content.Load<Model>("asteroid");
            model = Game.Content.Load<Model>("ship");
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
                setPosition(-1);
            else if (keyState.IsKeyDown(Keys.Space))
                setPosition(1);


            base.Update(gameTime);
        }

        private void setPosition(int direction)
        {
            Vector3 modelVelocityAdd = Vector3.Zero;

            Matrix velocityMatrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);

            physicsObject.Position += MathConverter.Convert(velocityMatrix.Forward) * direction;

            Main.CameraDirection = MathConverter.Convert(physicsObject.position);

            var cameraDepthOffset = Vector3.Normalize(Main.CameraDirection) * 1f;
            var cameraVerticleOffset = Vector3.Up * 1f;
            Main.CameraPosition = -Vector3.Normalize(Main.CameraDirection) * 200f;
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Alpha = 0.8f;

                    var rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);

                    effect.World = Matrix.CreateScale(0.05f) * rotation * MathConverter.Convert(physicsObject.WorldTransform);
                    effect.View = Matrix.CreateLookAt(Main.CameraPosition, Main.CameraDirection, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(Main.FieldOfView, Main.AspectRatio, Main.NearClipPlane, Main.FarClipPlane);
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }

    }
}
