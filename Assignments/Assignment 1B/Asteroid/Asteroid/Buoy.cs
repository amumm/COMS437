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
    internal class Buoy : DrawableGameComponent
    {
        private Model model;
        private Sphere physicsObject;

        private float entitySizeScaler;
        private float modelSizeScaler;

        public int buoyType
        {
            get;
            private set;
        }

        public Buoy(Game game, Vector3 pos, int type) : base(game)
        {
            physicsObject = new Sphere(MathConverter.Convert(pos), 1)
            {
                Tag = this
            };

            buoyType = type;

            game.Services.GetService<Space>().Add(physicsObject);
            game.Components.Add(this);

        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            if (buoyType == 0)
            {
                entitySizeScaler = 0.8f;
                modelSizeScaler = 0.5f;

                model = Game.Content.Load<Model>("heart");
                physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius * entitySizeScaler;
                physicsObject.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;
            }

            if (buoyType == 1)
            {
                entitySizeScaler = 1.4f;
                modelSizeScaler = 2;

                model = Game.Content.Load<Model>("fuel_can");
                physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius * entitySizeScaler;
                physicsObject.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;

                var rotation = MathConverter.Convert(Matrix.CreateFromYawPitchRoll(0,-30,0));
                physicsObject.WorldTransform = rotation * physicsObject.WorldTransform;
            }

            if (buoyType == 2)
            {
                entitySizeScaler = 7;
                modelSizeScaler = 6;

                model = Game.Content.Load<Model>("missile");
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

                switch (otherType)
                {
                    case "FighterShip":
                        Game.Services.GetService<Space>().Remove(sender.Entity);
                        Game.Components.Remove(senderGameComponent);
                        break;
                }
            }
        }
    }
}
