namespace Cuku.OurCity
{
    using Unity.Collections;
    using Unity.Entities;
    using UnityEngine;
    using System.Linq;

    public class TerrainCullingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .ForEach((Camera camera) =>
                {
                    var planes = GeometryUtility.CalculateFrustumPlanes(camera);
                    
                    Entities
                        .WithoutBurst()
                        .ForEach((Terrain terrain, in Bounds bounds) =>
                        {
                            terrain.drawHeightmap = GeometryUtility.TestPlanesAABB(planes, bounds.Value);
                        }).Run();
                }).Run();
        }
    }
}