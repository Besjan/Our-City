namespace Cuku.City
{
	using Cuku.ScriptableObject;
	using Sirenix.OdinInspector;
	using UnityEngine;

    public class CityLimitsConfig : SerializedScriptableObject
    {
		[PropertySpace(20), Title("City"), Required, InlineEditor]
		public StringSO CityName;

		[PropertySpace, Title("Boundary"), FilePath(ParentFolder = "Assets/StreamingAssets", RequireExistingPath = true)]
		public string BoundaryGeoData;

		[PropertySpace, AssetsOnly, Required]
		public Material BoundaryMaterial;

		[PropertySpace, InfoBox("From 0, in Meters.", InfoMessageType.None)]
		public float BoundaryHeight = 100.0f;


		[PropertySpace(20), Title("Void"), AssetsOnly, Required]
		public Material VoidMaterial;


		[PropertySpace(20), Title("Outer Terrain")]
		[InfoBox("Sample curve offset from terrain tile (higher - more accurate - slower).", InfoMessageType.None)]
		public int CurveOffset = 20;

		[PropertySpace, InfoBox("Sample curve rate (higher - more accurate - slower).", InfoMessageType.None)]
		public int CurveSampleRate = 15;

		[PropertySpace, InfoBox("Horizontal and vertical smooth distance from boundary.", InfoMessageType.None)]
		public float SmoothDistance = 20.0f;

		[PropertySpace, InfoBox("Higher - steepest.", InfoMessageType.None)]
		public float SmoothFactor = 0.3f;
	}
}
