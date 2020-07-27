using Unity.Mathematics;

namespace Cuku.OurCity
{
    using Unity.Collections;
    using Unity.Entities;
    using UnityEngine;
    using System.Linq;

    public class TerrainPixelErrorSystem : SystemBase
    {
        private float distanceFromTerrain = 3000;

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
                    float pixelError = 200;

                    for (int e = 0; e < cameras.Length; e++)
                    {
                        var cameraPosition = EntityManager.GetComponentObject<Transform>(cameras[e]).position;

                        if (math.distance(cameraPosition, bounds.Value.center) < distanceFromTerrain)
                        {
                            pixelError = 1;
                            continue;
                        }
                    }

                    terrain.heightmapPixelError = pixelError;
                }).Run();
        }
    }
}