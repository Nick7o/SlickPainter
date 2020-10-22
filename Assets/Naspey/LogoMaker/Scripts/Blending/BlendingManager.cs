using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Naspey.LogoMaker.Blending
{
    public static class BlendingManager
    {
        public static Dictionary<BlendModes, IBlendPixel> BlendingServices = new Dictionary<BlendModes, IBlendPixel>()
        {
            { BlendModes.Normal, new NormalBlendMode() },
            { BlendModes.Eraser, new AlphaEraserBlendMode() }
        };

        public static IBlendPixel GetBlendingService(BlendModes blendingMode)
        {
            if (!BlendingServices.TryGetValue(blendingMode, out var service))
                throw new System.Exception("Provided blending doesn't have a registered service.");

            return service;
        }
    }
}
