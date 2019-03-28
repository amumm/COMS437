﻿using XNA = Microsoft.Xna.Framework;
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
        private XNA.Vector3 CurrentPosition
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

        public Asteroids(Game game, XNA.Vector3 pos) : this(game)
        {
            physicsObject = new BEPUphysics.Entities.Prefabs.Sphere(MathConverter.Convert(pos), 1)
            {
                AngularDamping = 0f,
                LinearDamping = 0f
            };

            CurrentPosition = pos;


            Game.Services.GetService<Space>().Add(physicsObject);
        }

        public Asteroids(Game game, XNA.Vector3 pos, float mass) : this(game, pos)
        {
            physicsObject.Mass = mass;
        }

        public Asteroids(Game game, XNA.Vector3 pos, float mass, XNA.Vector3 linMomentum) : this(game, pos, mass)
        {
            physicsObject.LinearMomentum = MathConverter.Convert(linMomentum);
        }

        public Asteroids(Game game, XNA.Vector3 pos, float mass, XNA.Vector3 linMomentum, XNA.Vector3 angMomentum) : this(game, pos, mass, linMomentum)
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
            //rotation += 0.005f;
            
            base.Update(gameTime);
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

                    effect.World = XNA.Matrix.CreateScale(0.25f) * MathConverter.Convert(physicsObject.WorldTransform);
                    //effect.View = XNA.Matrix.CreateLookAt(Main.CameraPosition, Main.CameraDirection, XNA.Vector3.Up);
                    effect.Projection = XNA.Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }

    }
}
