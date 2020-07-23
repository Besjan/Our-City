using System;
using Unity.Entities;
using UnityEngine;

[InternalBufferCapacity(6)]
[GenerateAuthoringComponent]
public struct FrustumPlanes : IBufferElementData
{
    public Plane Value;

    // The following implicit conversions are optional, but can be convenient.
    public static implicit operator Plane(FrustumPlanes e)
    {
        return e.Value;
    }

    public static implicit operator FrustumPlanes(Plane e)
    {
        return new FrustumPlanes { Value = e };
    }
}
