﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Graphics;
using SiliconStudio.Xenko.Particles.VertexLayouts;

namespace SiliconStudio.Xenko.Particles.ShapeBuilders
{
    [DataContract("ShapeBuilderQuad")]
    [Display("Quad")]
    public class ShapeBuilderQuad : ShapeBuilderBase
    {
        public override int QuadsPerParticle { get; protected set; } = 1;

        public override unsafe int BuildVertexBuffer(ParticleVertexLayout vtxBuilder, Vector3 invViewX, Vector3 invViewY, ref int remainingCapacity,
            ref Vector3 spaceTranslation, ref Quaternion spaceRotation, float spaceScale, ParticlePool pool)
        {
            var vtxPerShape = 4 * QuadsPerParticle;

            var numberOfParticles = Math.Min(remainingCapacity / vtxPerShape, pool.LivingParticles);
            if (numberOfParticles <= 0)
                return 0;

            var positionField = pool.GetField(ParticleFields.Position);
            if (!positionField.IsValid())
                return 0;

            // Check if the draw space is identity - in this case we don't need to transform the position, scale and rotation vectors
            var trsIdentity = (spaceScale == 1f);
            trsIdentity = trsIdentity && (spaceTranslation.Equals(new Vector3(0, 0, 0)));
            trsIdentity = trsIdentity && (spaceRotation.Equals(new Quaternion(0, 0, 0, 1)));

            var sizeField = pool.GetField(ParticleFields.Size);

            var rotField = pool.GetField(ParticleFields.Quaternion);
            var hasRotation = rotField.IsValid();

            var renderedParticles = 0;

            // TODO Sorting

            foreach (var particle in pool)
            {
                var centralPos = particle.Get(positionField);

                var particleSize = sizeField.IsValid() ? particle.Get(sizeField) : 1f;

                var unitX = new Vector3(1, 0, 0); 
                var unitY = new Vector3(0, 0, 1); 

                if (hasRotation)
                {
                    var particleRotation = particle.Get(rotField);
                    particleRotation.Rotate(ref unitX);
                    particleRotation.Rotate(ref unitY);
                }

                // The TRS matrix is not an identity, so we need to transform the quad
                if (!trsIdentity)
                {
                    spaceRotation.Rotate(ref centralPos);
                    centralPos = centralPos * spaceScale + spaceTranslation;
                    particleSize *= spaceScale;

                    spaceRotation.Rotate(ref unitX);
                    spaceRotation.Rotate(ref unitY);
                }

                unitX *= particleSize;
                unitY *= particleSize;

                // vertex.Size = particleSize;

                var particlePos = centralPos - unitX + unitY;
                var uvCoord = new Vector2(0, 0);
                // 0f 0f
                vtxBuilder.SetPosition(ref particlePos);
                vtxBuilder.SetUvCoords(ref uvCoord);
                vtxBuilder.NextVertex();


                // 1f 0f
                particlePos += unitX * 2;
                uvCoord.X = 1;
                vtxBuilder.SetPosition(ref particlePos);
                vtxBuilder.SetUvCoords(ref uvCoord);
                vtxBuilder.NextVertex();


                // 1f 1f
                particlePos -= unitY * 2;
                uvCoord.Y = 1;
                vtxBuilder.SetPosition(ref particlePos);
                vtxBuilder.SetUvCoords(ref uvCoord);
                vtxBuilder.NextVertex();


                // 0f 1f
                particlePos -= unitX * 2;
                uvCoord.X = 0;
                vtxBuilder.SetPosition(ref particlePos);
                vtxBuilder.SetUvCoords(ref uvCoord);
                vtxBuilder.NextVertex();


                remainingCapacity -= vtxPerShape;

                if (++renderedParticles >= numberOfParticles)
                {
                    return renderedParticles;
                }
            }

            return renderedParticles;
        }
    }
}
