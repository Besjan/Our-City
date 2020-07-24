namespace Cuku.OurCity
{
    using Unity.Collections;
    using Unity.Entities;
    using UnityEngine;
    using Bounds = Bounds;
    
    public class AddTerrainBoundsSystem : SystemBase
    {
        EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
        protected override void OnCreate()
        {
            base.OnCreate();
            // Find the ECB system once and store it for later usage
            m_EndSimulationEcbSystem = World
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
            
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
            
            m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}