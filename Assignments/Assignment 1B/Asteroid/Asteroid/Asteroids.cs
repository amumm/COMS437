using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using ConversionHelper;


namespace Asteroid
{
    internal class Asteroids : DrawableGameComponent
    {

        private Model model;
        private Sphere physicsObject;
        private Vector3 CurrentPosition
        {
            get
            {
                return MathConverter.Convert(physicsObject.Position);
            }

            set {}
        }

        public Asteroids(Game game) : base(game)
        {
            game.Components.Add(this);
        }

        public Asteroids(Game game, Vector3 pos) : this(game)
        {
            physicsObject = new Sphere(MathConverter.Convert(pos), 1)
            {
                AngularDamping = 0f,
                LinearDamping = 0f
            };

            CurrentPosition = pos;


            Game.Services.GetService<Space>().Add(physicsObject);
        }

        public Asteroids(Game game, Vector3 pos, float mass) : this(game, pos)
        {
            physicsObject.Mass = mass;
        }

        public Asteroids(Game game, Vector3 pos, float mass, Vector3 linMomentum) : this(game, pos, mass)
        {
            physicsObject.LinearMomentum = MathConverter.Convert(linMomentum);
        }

        public Asteroids(Game game, Vector3 pos, float mass, Vector3 linMomentum, Vector3 angMomentum) : this(game, pos, mass, linMomentum)
        {
            physicsObject.AngularMomentum = MathConverter.Convert(angMomentum);
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
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Alpha = 0.8f;

                    effect.World = Matrix.CreateScale(0.25f) * MathConverter.Convert(physicsObject.WorldTransform);
                    effect.View = Main.View;
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(Main.FieldOfView, Main.AspectRatio, Main.NearClipPlane, Main.FarClipPlane);
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }

    }
}
