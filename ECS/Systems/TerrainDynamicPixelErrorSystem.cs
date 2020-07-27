namespace Cuku.OurCity
{
    using Unity.Mathematics;
    using System;
    using Unity.Collections;
    using Unity.Entities;
    using UnityEngine;

    public class TerrainDynamicPixelErrorSystem : SystemBase
    {
        private float pixelErrorFactor = 4000;

        private EntityQuery _cameraQuery;

        protected override void OnCreate()
        {
            _cameraQuery = GetEntityQuery(new EntityQueryDesc()
            {
                All = new[]
                {
                    ComponentType.ReadOnly<Camera>(),
                    ComponentType.ReadOnly<Transform>()
                }
            });
        }

        protected override void OnUpdate()
        {
            var cameras = _cameraQuery.ToEntityArray(Allocator.Temp);

            Entities
                .WithoutBurst()
                .ForEach((Terrain terrain, in Bounds bounds) =>
                {
                    var closestCameraDistance = Single.MaxValue;

                    for (int e = 0; e < cameras.Length; e++)
                    {
                        var cameraPosition = EntityManager.GetComponentObject<Transform>(cameras[e]).position;
                        closestCameraDistance = math.min(closestCameraDistance,
                            math.distance(cameraPosition, bounds.Value.center));
                    }

                    terrain.heightmapPixelError = closestCameraDistance / pixelErrorFactor;
                }).Run();
        }
    }
}