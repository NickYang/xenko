﻿// Copyright (c) 2014-2016 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using SiliconStudio.Core;
using SiliconStudio.Xenko.Games;
using System.Collections.Generic;
using System.Linq;
using SiliconStudio.Xenko.Engine;

namespace SiliconStudio.Xenko.Physics
{
    public class Bullet2PhysicsSystem : GameSystem, IPhysicsSystem
    {
        private class PhysicsScene
        {
            public PhysicsProcessor Processor;
            public Simulation Simulation;
        }

        private readonly List<PhysicsScene> scenes = new List<PhysicsScene>();

        static Bullet2PhysicsSystem()
        {
            // Preload proper libbulletc native library (depending on CPU type)
            NativeLibrary.PreloadLibrary("libbulletc.dll");
        }

        public Bullet2PhysicsSystem(IServiceRegistry registry)
            : base(registry)
        {
            UpdateOrder = -1000; //make sure physics runs before everything
            registry.AddService(typeof(IPhysicsSystem), this);

            Enabled = true; //enabled by default
        }

        private PhysicsSettings physicsConfiguration;

        public override void Initialize()
        {
            physicsConfiguration = Game?.Settings != null ? Game.Settings.Configurations.Get<PhysicsSettings>() : new PhysicsSettings();
        }

        protected override void Destroy()
        {
            base.Destroy();

            lock (this)
            {
                foreach (var scene in scenes)
                {
                    scene.Simulation.Dispose();
                }
            }
        }

        public Simulation Create(PhysicsProcessor sceneProcessor, PhysicsEngineFlags flags = PhysicsEngineFlags.None)
        {
            var scene = new PhysicsScene { Processor = sceneProcessor, Simulation = new Simulation(sceneProcessor, physicsConfiguration) };
            lock (this)
            {
                scenes.Add(scene);
            }
            return scene.Simulation;
        }

        public void Release(PhysicsProcessor processor)
        {
            lock (this)
            {
                var scene = scenes.SingleOrDefault(x => x.Processor == processor);
                if (scene == null) return;
                scenes.Remove(scene);
                scene.Simulation.Dispose();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Simulation.DisableSimulation) return;

            lock (this)
            {
                //read skinned meshes bone positions
                foreach (var physicsScene in scenes)
                {
                    //read skinned meshes bone positions and write them to the physics engine
                    physicsScene.Processor.UpdateBones();
                    //simulate physics
                    physicsScene.Simulation.Simulate((float)gameTime.Elapsed.TotalSeconds);
                    //update character bound entity's transforms from physics engine simulation
                    physicsScene.Processor.UpdateCharacters();

                    physicsScene.Simulation.BeginContactTesting();

                    //finally process any needed cleanup
                    physicsScene.Processor.UpdateRemovals();

                    
                    //handle frame contacts
                    physicsScene.Processor.UpdateContacts();

                    physicsScene.Simulation.EndContactTesting();

                    //send contact events
                    physicsScene.Simulation.SendEvents();                   
                }
            }
        }
    }
}
