namespace Cuku.OurCity
{
    using System;
    using Unity.Entities;

    [Serializable]
    public struct Bounds : IComponentData
    {
        public UnityEngine.Bounds Value;
    }

}