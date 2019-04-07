using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using ConversionHelper;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using System;

namespace Asteroid
{
    internal class Mothership : DrawableGameComponent
    {

        private Model model;
        private Sphere physicsObject;

        private float entitySizeScaler = 11.0f;
        private float modelSizeScaler = 1.0f;

        public Mothership(Game game, Vector3 pos, float mass, Vector3 linMomentum, Vector3 angMomentum): base(game)
        {
            physicsObject = new Sphere(MathConverter.Convert(pos), 1)
            {
                Mass = mass,
                AngularDamping = 0f,
                LinearDamping = 0f,
                AngularMomentum = MathConverter.Convert(angMomentum),
                LinearMomentum = MathConverter.Convert(linMomentum),
                Tag = this
            };

            game.Services.GetService<Space>().Add(physicsObject);
            game.Components.Add(this);

        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("mothership");
            physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius * 11.0f;
            physicsObject.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;

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
                    var num = 10;
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.DiffuseColor = new Vector3(num, num, num);
                    effect.DirectionalLight1.DiffuseColor = new Vector3(num, num, num);
                    effect.DirectionalLight2.DiffuseColor = new Vector3(num, num, num);
                    effect.PreferPerPixelLighting = true;

                    effect.Alpha = 0.5f;

                    effect.World = Matrix.CreateScale(1f) * MathConverter.Convert(physicsObject.WorldTransform);
                    effect.View = Main.View;
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(Main.FieldOfView, Main.AspectRatio, Main.NearClipPlane, Main.FarClipPlane);
                }
                mesh.Draw();
            }
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

                Console.WriteLine(otherType);
                switch (otherType)
                {
                    case "FighterShip":
                        Console.WriteLine("Hit the fighter");
                        break;
                    case "":
                        break;
                    default:
                        Console.WriteLine("Hit Unknown Object");
                        break;
                }
                //Game.Services.GetService<Space>().Remove(otherEntityInformation.Entity);
                //Game.Components.Remove(otherGameComponent);
            }
        }


    }
}
