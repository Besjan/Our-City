namespace Cuku.City
{
    using AX;
    using AXGeometry;
    using Sirenix.OdinInspector;
    using System.Collections;
    using System.Linq;
    using UnityEngine;

    [RequireComponent(typeof(AXModel))]
    public class CityLimitsModel : MonoBehaviour
    {
        string n_plateNode = "Plate";
        string n_platePosX = "Trans_X";
        string n_platePosY = "Trans_Y";
        string n_plateWidth = "width";
        string n_plateHeight = "height";

        string n_boundary = "Boundary";


        public void Create(Vector2 platePosition, Vector2 plateSize, Vector3[] boundaryPoints)
        {
            ResetAXModel();

            StartCoroutine(Define(platePosition, plateSize, boundaryPoints));
        }

        IEnumerator Define(Vector2 platePosition, Vector2 plateSize, Vector3[] boundaryPoints)
        {
            var axModel = GetComponent<AXModel>();

            SetPlate(axModel, platePosition, plateSize);
            axModel.autobuild();
            yield return new WaitForSeconds(1);

            SetBoundary(axModel, boundaryPoints);
            axModel.autobuild();
            yield return new WaitForSeconds(1);
        }

        private void SetPlate(AXModel axModel, Vector2 platePosition, Vector2 plateSize)
        {
            var plate = axModel.parametricObjects.FirstOrDefault(p => p.Name == n_plateNode);

            plate.setParameterValueByName(n_platePosX, platePosition.x);
            plate.setParameterValueByName(n_platePosY, platePosition.y);
            plate.setParameterValueByName(n_plateWidth, plateSize.x);
            plate.setParameterValueByName(n_plateHeight, plateSize.y);
        }

        private void SetBoundary(AXModel axModel, Vector3[] boundaryPoints)
        {
            var boundary = axModel.parametricObjects.FirstOrDefault(p => p.Name == n_boundary);
            boundary.curve.Clear();

            if (boundaryPoints == null) return;

            for (int i = 0; i < boundaryPoints.Length; i++)
            {
                Vector3 point = boundaryPoints[i];
                boundary.curve.Add(new CurveControlPoint2D(point.x, point.z));
            }
        }

        [Button]
        public void ResetAXModel()
        {
            var axModel = GetComponent<AXModel>();

            SetPlate(axModel, Vector2.zero, new Vector2(3, 2));
            SetBoundary(axModel, null);

            axModel.autobuild();
        }
    }
}