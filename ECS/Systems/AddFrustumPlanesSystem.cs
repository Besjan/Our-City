using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateBefore(typeof(CalculateFrustumPlanesSystem))]
public class AddFrustumPlanesSystem : SystemBase
{
    EntityQuery entityQuery;

    private bool Initialized;

    protected override void OnCreate()
    {
        entityQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All = new ComponentType[] {ComponentType.ReadOnly<Camera>()}
        });
    }

    protected override void OnUpdate()
    {
        if (Initialized) return;
        
        var entities = entityQuery.ToEntityArray(Allocator.Temp);
        for (int e = 0; e < entities.Length; e++)
        {
            var buffer = EntityManager.AddBuffer<FrustumPlanes>(entities[e]);
        
            for (int b = 0; b < 6; b++)
            {
                buffer.Add(new Plane());
            }
        }

        Initialized = true;
    }
}
