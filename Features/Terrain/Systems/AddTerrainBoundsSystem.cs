namespace Cuku.OurCity
{
    using Unity.Entities;
    using UnityEngine;

    public class AddTerrainBoundsSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem _endInitializationEcbSystem;

        protected override void OnCreate()
        {
            _endInitializationEcbSystem = World
                .GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _endInitializationEcbSystem.CreateCommandBuffer();

            Entities
                .WithoutBurst()
                .WithNone<Bounds>()
                .ForEach((Entity entity, in Terrain terrain) =>
                {
                    var shift = terrain.terrainData.size / 2;

                    var bounds = new Bounds
                    {
                        Value = new UnityEngine.Bounds
                        {
                            center = terrain.GetPosition() + shift,
                            extents = terrain.terrainData.size
                        }
                    };

                    ecb.AddComponent(entity, bounds);
                }).Run();

            _endInitializationEcbSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}