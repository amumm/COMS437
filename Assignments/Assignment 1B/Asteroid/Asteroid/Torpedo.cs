﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using ConversionHelper;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace Asteroid
{
    internal class Torpedo : DrawableGameComponent
    {
        private Model model;
        private Sphere physicsObject;

        private float entitySizeScaler = 10.0f;
        private float modelSizeScaler = 2.0f;

        public Torpedo(Game game, Vector3 pos, float mass, Vector3 linMomentum, Vector3 angMomentum) : base(game)
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
            model = Game.Content.Load<Model>("missile");
            physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius * entitySizeScaler;
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
                    case "Asteroids":
                        Game.Services.GetService<Space>().Remove(sender.Entity);
                        Game.Components.Remove(senderGameComponent);
                        break;
                    case "Mothership":
                        Game.Services.GetService<Space>().Remove(sender.Entity);
                        Game.Components.Remove(senderGameComponent);
                        break;
                }
            }
        }
    }
}
