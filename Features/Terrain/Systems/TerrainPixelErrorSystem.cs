namespace Cuku.OurCity
{
    using Unity.Mathematics;
    using Unity.Entities;
    using UnityEngine;

    public class TerrainPixelErrorSystem : SystemBase
    {
        private float pixelErrorFactor = 4000;

        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .ForEach((Camera camera, in Transform transform) =>
                {
                    var cameraPosition = transform.position;

                    Entities
                        .WithoutBurst()
                        .ForEach((Terrain terrain, in Bounds bounds) =>
                        {
                            var cameraDistance = math.distance(cameraPosition, bounds.Value.center);
                            terrain.heightmapPixelError = cameraDistance / pixelErrorFactor;
                        }).Run();

                }).Run();
        }
    }
}