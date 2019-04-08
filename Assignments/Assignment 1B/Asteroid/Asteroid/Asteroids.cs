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
    internal class Asteroids : DrawableGameComponent
    {

        private Model model;
        private Sphere physicsObject;

        private float entitySizeScaler;
        private float modelSizeScaler;

        private int asteroidSize;

        public Asteroids(Game game, int size, Vector3 pos, float mass, Vector3 linMomentum, Vector3 angMomentum) : base(game)
        {
            physicsObject = new Sphere(MathConverter.Convert(pos), 1)
            {
                Mass = mass,
                AngularMomentum = MathConverter.Convert(angMomentum),
                LinearMomentum = MathConverter.Convert(linMomentum),
                AngularDamping = 0f,
                LinearDamping = 0f,
                Tag = this
            };

            asteroidSize = size;

            Game.Services.GetService<Space>().Add(physicsObject);
            game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("asteroid");
            if (asteroidSize == 0)
            {
                entitySizeScaler = 0.2f;
                modelSizeScaler = 0.25f;
                physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius * entitySizeScaler;
                physicsObject.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;
            }
            if (asteroidSize == 1)
            {
                entitySizeScaler = 0.8f;
                modelSizeScaler = 1.0f;
                physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius * entitySizeScaler;
                physicsObject.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;
            }
            if (asteroidSize == 2)
            {
                entitySizeScaler = 3.6f;
                modelSizeScaler = 5.0f;
                physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius * entitySizeScaler;
                physicsObject.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;
            }

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

                    effect.World = Matrix.CreateScale(modelSizeScaler) * MathConverter.Convert(physicsObject.WorldTransform);
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

                if (sender != null)
                {
                    Space space = Game.Services.GetService<Space>();
                    var spaceObject = sender.entity as ISpaceObject;
                    if (spaceObject.Space == space)
                    { 
                        switch (otherType)
                        {
                            case "Asteroids":
                                Random rand = new Random();
                                var destroy = rand.Next(0, 10);
                                if (destroy == 0)
                                {
                                    space.Remove(sender.Entity);
                                    Game.Components.Remove(senderGameComponent);
                                }
                                break;
                            case "Mothership":
                                space.Remove(sender.Entity);
                                Game.Components.Remove(senderGameComponent);
                                break;
                            case "Torpedo":
                                space.Remove(sender.Entity);
                                Game.Components.Remove(senderGameComponent);
                                break;
                        }
                    }
                }
            }
        }
    }
}
