using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics;

namespace AsteroidsPhysicsExample
{
    internal class Asteroid : DrawableGameComponent
    {

        private Model model;
        private Texture2D moonTexture;
        private BEPUphysics.Entities.Prefabs.Sphere physicsObject;
        private Vector3 CurrentPosition
        {
            get
            {
                return ConversionHelper.MathConverter.Convert(physicsObject.Position);
            }
        }

        public Asteroid(Game game) : base(game)
        {
            game.Components.Add(this);
        }

        public Asteroid(Game game, Vector3 pos) : this(game)
        {
            physicsObject = new BEPUphysics.Entities.Prefabs.Sphere(ConversionHelper.MathConverter.Convert(pos), 1);
            physicsObject.AngularDamping = 0f;
            physicsObject.LinearDamping = 0f;
            Game.Services.GetService<Space>().Add(physicsObject);
        }

        public Asteroid(Game game, Vector3 pos, float mass) : this(game, pos )
        {
            physicsObject.Mass = mass;
        }

        public Asteroid(Game game, Vector3 pos, float mass, Vector3 linMomentum) : this(game, pos, mass)
        {
            physicsObject.LinearMomentum = ConversionHelper.MathConverter.Convert(linMomentum);
        }

        public Asteroid(Game game, Vector3 pos, float mass, Vector3 linMomentum, Vector3 angMomentum) : this(game, pos, mass, linMomentum)
        {
            physicsObject.AngularMomentum = ConversionHelper.MathConverter.Convert(angMomentum);
        }
 
        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            moonTexture = Game.Content.Load<Texture2D>("moonsurface");
            model = Game.Content.Load<Model>("moon");
            physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius;


            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach( var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                { 
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.DiffuseColor = new Vector3(10, 10, 10);
                    effect.DirectionalLight1.DiffuseColor = new Vector3(10, 10, 10);
                    effect.DirectionalLight2.DiffuseColor = new Vector3(10, 10, 10);
                    effect.PreferPerPixelLighting = true;
                    effect.World = ConversionHelper.MathConverter.Convert(physicsObject.WorldTransform);
                    effect.View = Matrix.CreateLookAt(Ape.CameraPosition, Ape.CameraDirection, Vector3.Up);
                    float aspectRatio = Game.GraphicsDevice.Viewport.AspectRatio;
                    float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
                    float nearClipPlane = 0.3f;
                    float farClipPlane = 200f;
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }

    }
}
