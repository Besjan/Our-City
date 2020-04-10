namespace Cuku.City
{
    using AX;
    using AXGeometry;
	using Cuku.Geo;
	using Sirenix.OdinInspector;
    using System.Linq;
    using UnityEngine;

    [RequireComponent(typeof(AXModel))]
    public class CityLimitsModel : MonoBehaviour
    {
        string n_boundaryShape = "BoundaryShape";

        [Button]
        public void Create()
        {
            var axModel = GetComponent<AXModel>();

            Reset();

            SetBoundaryShape(axModel, Features.GetBoundaryPoints());

            axModel.autobuild();
        }

        [Button]
        private void Reset()
        {
            var axModel = GetComponent<AXModel>();
            SetBoundaryShape(axModel, null);

            axModel.autobuild();
        }

        private void SetBoundaryShape(AXModel axModel, Vector3[] boundaryPoints)
        {
            var boundary = axModel.parametricObjects.FirstOrDefault(p => p.Name == n_boundaryShape);
            boundary.curve.Clear();

            if (boundaryPoints == null) return;

            for (int i = 0; i < boundaryPoints.Length; i++)
            {
                Vector3 point = boundaryPoints[i];
                boundary.curve.Add(new CurveControlPoint2D(point.x, point.z));
            }
        }
    }
}