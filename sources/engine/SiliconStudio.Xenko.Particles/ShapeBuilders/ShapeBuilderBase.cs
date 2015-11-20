﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Graphics;

namespace SiliconStudio.Xenko.Particles.ShapeBuilders
{
    [DataContract("ShapeBuilderBase")]
    public abstract class ShapeBuilderBase
    {
        public abstract int BuildVertexBuffer(MappedResource vertexBuffer, Vector3 invViewX, Vector3 invViewY, ref int remainingCapacity, ParticlePool pool);
    }
}
