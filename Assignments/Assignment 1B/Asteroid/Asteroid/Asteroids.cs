using XNA = Microsoft.Xna.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using BEPUphysicsDrawer.Models;
using BEPUutilities;

namespace Asteroid
{
    internal class Asteroids : DrawableGameComponent
    {

        private Model model;
        private Texture2D moonTexture;
        //private BEPUphysics.Entities.Prefabs.Sphere physicsObject;
        private BEPUphysics.Entities.Prefabs.ConvexHull physicsObject;
        private XNA.Vector3 CurrentPosition
        {
            get
            {
                return ConversionHelper.MathConverter.Convert(physicsObject.Position);
            }

            set {}
        }

        public Asteroids(Game game) : base(game)
        {
            game.Components.Add(this);
        }

        public Asteroids(Game game, XNA.Vector3 pos) : this(game)
        {
            //physicsObject = new BEPUphysics.Entities.Prefabs.Sphere(ConversionHelper.MathConverter.Convert(pos), 1)
            //{
            //    AngularDamping = 0f,
            //    LinearDamping = 0f
            //};

            CurrentPosition = pos;

            
            //Game.Services.GetService<Space>().Add(physicsObject);
        }

        public Asteroids(Game game, XNA.Vector3 pos, float mass) : this(game, pos)
        {
            physicsObject.Mass = mass;
        }

        public Asteroids(Game game, XNA.Vector3 pos, float mass, XNA.Vector3 linMomentum) : this(game, pos, mass)
        {
            physicsObject.LinearMomentum = ConversionHelper.MathConverter.Convert(linMomentum);
        }

        public Asteroids(Game game, XNA.Vector3 pos, float mass, XNA.Vector3 linMomentum, XNA.Vector3 angMomentum) : this(game, pos, mass, linMomentum)
        {
            physicsObject.AngularMomentum = ConversionHelper.MathConverter.Convert(angMomentum);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //moonTexture = Game.Content.Load<Texture2D>("moonsurface");
            //moonTexture = Game.Content.Load<Texture2D>("asteroid_surface");
            model = Game.Content.Load<Model>("asteroid");
            //model = Game.Content.Load<Model>("moon");
            //physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius * 0.1f;

            physicsObject = new BEPUphysics.Entities.Prefabs.ConvexHull(ConversionHelper.MathConverter.Convert(pos), 1)
            {
                AngularDamping = 0f,
                LinearDamping = 0f
            };
            Game.Services.GetService<Space>().Add(physicsObject);

            BEPUutilities.Vector3[] vertices;
            int[] indices;
            ModelDataExtractor.GetVerticesAndIndicesFromModel(model, out vertices, out indices);

            //Create an entity based on the model.
            ConvexHull hull = new ConvexHull(vertices, 10);
            Game.Services.GetService<Space>().Add(hull);
            //Create a graphic for the hull.  The BEPUphysicsDrawer will also automatically receive a visualization of the convex hull which we can compare our graphic against.
            //The graphic will be offset from the collision shape because we have not attempted to offset it to match the collision shape's origin.
            var graphic = new DisplayEntityModel(hull, model, Game.ModelDrawer);
            Game.ModelDrawer.Add(graphic);


            //Now let's create another entity from the same vertices.
            hull = new ConvexHull(vertices, 10);
            Space.Add(hull);

            //Note that the prefab entity type ConvexHull uses the ConvexHullShape constructor internally, which outputs the computed center of the shape
            //prior to offset.  The ConvexHull constructor sets the entity's Position to that position so that the initial world space location of the vertices
            //matches what was passed into the constructor.
            //The same process could be performed using this code:

            //Vector3 computedCenter;
            //var shape = new ConvexHullShape(vertices, out computedCenter); //<-- It spits out the computed center here; this is what is used to offset the graphic!
            //var entity = new Entity(shape, 10);
            //entity.Position = computedCenter;
            //Space.Add(entity);

            //For more information about constructing entities, check out the EntityConstructionDemo.

            //But for now, let's just use the prefab entity type.  As mentioned earlier, the constructor set the entity's Position using the computed center.
            //Since we didn't overwrite it with some other position yet, we can still use it.
            graphic = new DisplayEntityModel(hull, model, game.ModelDrawer);
            graphic.LocalTransform = Matrix.CreateTranslation(-hull.Position);
            game.ModelDrawer.Add(graphic);

            //This graphic is perfectly aligned with the collision shape!  Hooray!


            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            float aspectRatio = Game.GraphicsDevice.Viewport.AspectRatio;
            float fieldOfView = MathHelper.PiOver4;
            float nearClipPlane = 0.3f;
            float farClipPlane = 200f;

            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting();
                    //effect.DirectionalLight0.DiffuseColor = new Vector3(10, 10, 10);
                    //effect.DirectionalLight1.DiffuseColor = new Vector3(10, 10, 10);
                    //effect.DirectionalLight2.DiffuseColor = new Vector3(10, 10, 10);
                    //effect.PreferPerPixelLighting = true;

                    effect.Alpha = 0.8f;

                    effect.World = Matrix.CreateScale(0.25f) * ConversionHelper.MathConverter.Convert(physicsObject.WorldTransform);
                    effect.View = Matrix.CreateLookAt(Main.CameraPosition, Main.CameraDirection, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }

    }
}
