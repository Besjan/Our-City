using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(AddFrustumPlanesSystem))]
public class CalculateFrustumPlanesSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithoutBurst()
            .ForEach((Entity entity, in Camera camera) =>
            {
                Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
 
                BufferFromEntity<FrustumPlanes> lookup = GetBufferFromEntity<FrustumPlanes>();
                var buffer = lookup[entity];
                
                for (int i = 0; i < frustumPlanes.Length; i++)
                {
                    buffer[i] = frustumPlanes[i];
                }
            }).Run();
    }
}
