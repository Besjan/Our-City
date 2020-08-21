namespace Cuku.City
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using System;
	using System.Linq;
	using Geo;
	using Utilities;
	using UnityEngine.ProBuilder;
	using UnityEditor.ProBuilder;
	using Sirenix.OdinInspector.Editor;
	using Sirenix.Utilities.Editor;
	using Sirenix.Utilities;
	using UnityEngine.UI;
	using Sirenix.OdinInspector;
	using System.IO;
	using AwesomeTechnologies.VegetationStudio;
	using AwesomeTechnologies.VegetationSystem;

	public class EnvironmentEditor : OdinEditorWindow
	{
		#region Editor
		[MenuItem("Cuku/Our City/Environment Editor")]
		private static void OpenWindow()
		{
			var window = GetWindow<EnvironmentEditor>();
			window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
		}

		[PropertySpace, InlineEditor, Required]
		public EnvironmentConfig Config;

		private bool IsConfigValid()
		{
			return Config != null;
		}
		#endregion

#if VEGETATION_STUDIO_PRO
		#region Actions
		[ShowIf("IsConfigValid"), PropertySpace(20), Button(ButtonSizes.Large)]
		public void AddVegetationStudioPro()
		{
			AddVSPManager();
			AddVSPUnityTerrain();
		}
		#endregion

		private void AddVSPManager()
		{
			var vspManager = FindObjectOfType<VegetationStudioManager>();
			if (FindObjectOfType<VegetationStudioManager>())
			{
				DestroyImmediate(vspManager.gameObject);
			};

			// Add VSP Manager
			GameObject go = new GameObject { name = Config.CityName.Value + " Vegetation Studio Manager" };
			go.AddComponent<VegetationStudioManager>();
		}

		private void AddVSPUnityTerrain()
		{
			var terrains = GameObject.FindObjectsOfType<Terrain>();

			terrains[0].transform.parent.gameObject.SetActive(false);

			for (int i = 0; i < terrains.Length; i++)
			{
				var vspTerrain = terrains[i].gameObject.GetComponent<UnityTerrain>();
				if (vspTerrain == null) vspTerrain = terrains[i].gameObject.AddComponent<UnityTerrain>();

				vspTerrain.AutoAddToVegegetationSystem = true;
				vspTerrain.DisableTerrainTreesAndDetails = true;
			}

			terrains[0].transform.parent.gameObject.SetActive(true);
		}
#endif
	}
}
