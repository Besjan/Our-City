namespace Cuku.OurCity
{
    using Unity.Entities;
    using UnityEngine;

    public class AddTerrainBoundsSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEcbSystem;

        protected override void OnCreate()
        {
            _endSimulationEcbSystem = World
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _endSimulationEcbSystem.CreateCommandBuffer();

            Entities
                .WithoutBurst()
                .WithNone<OurCity.Bounds>()
                .ForEach((Entity entity, in Terrain terrain) =>
                {
                    var shift = terrain.terrainData.size / 2;

                    var bounds = new OurCity.Bounds
                    {
                        Value = new UnityEngine.Bounds
                        {
                            center = terrain.GetPosition() + shift,
                            extents = terrain.terrainData.size
                        }
                    };

                    ecb.AddComponent(entity, bounds);
                }).Run();

            _endSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}