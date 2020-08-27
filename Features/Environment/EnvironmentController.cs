namespace Cuku.OurCity
{
    using UnityEngine;

    public class EnvironmentController : MonoBehaviour
    {
        private void OnEnable()
        {
            GameObject.FindObjectOfType<AwesomeTechnologies.VegetationSystem.VegetationSystemPro>().AddCamera(Camera.main);
        }

        private void OnDisable()
        {
            GameObject.FindObjectOfType<AwesomeTechnologies.VegetationSystem.VegetationSystemPro>().RemoveCamera(Camera.main);
        }
    }
}