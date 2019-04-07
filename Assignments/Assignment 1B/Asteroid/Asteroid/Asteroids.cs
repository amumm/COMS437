﻿using Microsoft.Xna.Framework;
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
            physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius * .2f;
            physicsObject.Tag = this;
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

                    effect.World = Matrix.CreateScale(0.25f) * MathConverter.Convert(physicsObject.WorldTransform);
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
                    case "Asteroids":
                        Game.Services.GetService<Space>().Remove(sender.Entity);
                        Game.Components.Remove(senderGameComponent);
                        break;
                    case "":
                        Console.WriteLine("Case 2");
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
