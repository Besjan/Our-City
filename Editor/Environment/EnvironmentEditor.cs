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
		public CityLimitsConfig Config;

		private bool IsConfigValid()
		{
			return Config != null;
		}
		#endregion

		#region Actions
		[ShowIf("IsConfigValid"), PropertySpace(20), Button(ButtonSizes.Large)]
		public void AddVegetationStudioPro()
		{
			if (GetVegetationStudioManager()) return;

			GameObject go = new GameObject { name = "VegetationStudioPro" };
			go.AddComponent<VegetationStudioManager>();
		}
		#endregion

		VegetationStudioManager GetVegetationStudioManager()
		{
			return FindObjectOfType<VegetationStudioManager>();
		}
	}
}
