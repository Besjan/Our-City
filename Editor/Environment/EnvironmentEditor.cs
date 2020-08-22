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
	using AwesomeTechnologies.TerrainSystem;
	using AwesomeTechnologies.TouchReact;

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
			// Add VSP Manager
			var vspManager = FindObjectOfType<VegetationStudioManager>();
			if (FindObjectOfType<VegetationStudioManager>())
			{
				DestroyImmediate(vspManager.gameObject);

				// Remove all VSP Unity Terrains
				var oldVSPUnityTerrains = GameObject.FindObjectsOfType<UnityTerrain>();
				for (int i = 0; i < oldVSPUnityTerrains.Length; i++)
				{
					DestroyImmediate(oldVSPUnityTerrains[i]);
				}
			};

			GameObject vspManagerGO = new GameObject { name = Config.CityName.Value + " VegetationStudioPro" };
			vspManagerGO.AddComponent<VegetationStudioManager>();

			// Add VSP System
			GameObject vspSystemGO = new GameObject { name = Config.CityName.Value + " VegetationSystemPro" };
			vspSystemGO.transform.SetParent(vspManagerGO.transform);
			VegetationSystemPro vspSystem = vspSystemGO.AddComponent<VegetationSystemPro>();
			vspSystem.AddAllUnityTerrains();
			vspSystem.SeaLevel = Config.TerrainHeightRange.Value.x;
			vspSystem.AutomaticBoundsCalculation = false;

			// Setup VSP Unity Terrains
			var vspTerrains = GameObject.FindObjectsOfType<UnityTerrain>();
			for (int i = 0; i < vspTerrains.Count(); i++)
			{
				vspTerrains[i].AutoAddToVegegetationSystem = true; 
				vspTerrains[i].DisableTerrainTreesAndDetails = true;
			}
		}
		#endregion
#endif
	}
}
