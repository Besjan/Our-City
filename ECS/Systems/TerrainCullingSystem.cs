namespace Cuku.OurCity
{
    using Unity.Collections;
    using Unity.Entities;
    using UnityEngine;
    using System.Linq;

    public class TerrainCullingSystem : SystemBase
    {
        private EntityQuery _cameraQuery;

        protected override void OnCreate()
        {
            _cameraQuery = GetEntityQuery(new EntityQueryDesc()
            {
                All = new[] {ComponentType.ReadOnly<Camera>()}
            });
        }

        protected override void OnUpdate()
        {
            var cameras = _cameraQuery.ToEntityArray(Allocator.Temp);

            Entities
                .WithoutBurst()
                .ForEach((Terrain terrain, in Bounds bounds) =>
                {
                    bool isInFrustum = false;

                    for (int e = 0; e < cameras.Length; e++)
                    {
                        var planes = GeometryUtility.CalculateFrustumPlanes(
                            EntityManager.GetComponentObject<Camera>(cameras[e]));

                        isInFrustum = GeometryUtility.TestPlanesAABB(planes.ToArray(), bounds.Value);

                        if (isInFrustum) continue;
                    }

                    terrain.drawHeightmap = isInFrustum;
                }).Run();
        }
    }
}