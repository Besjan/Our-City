namespace Cuku.City
{
	using Sirenix.OdinInspector;
	using UnityEngine;

    public class CityLimitsConfig : SerializedScriptableObject
    {
		[PropertySpace, Title("Boundary")]
		[InfoBox("From 0, in Meters.", InfoMessageType.None)]
		public float BoundaryHeight = 100.0f;

		[PropertySpace(20), Title("Outer Terrain"), VerticalGroup("Outer Terrain")]
		[InfoBox("Sample curve offset from terrain tile (higher - more accurate - slower).", InfoMessageType.None)]
		public int CurveOffset = 20;
		[PropertySpace, VerticalGroup("Outer Terrain")]
		[InfoBox("Sample curve rate (higher - more accurate - slower).", InfoMessageType.None)]
		public int CurveSampleRate = 15;

		[PropertySpace(20), VerticalGroup("Outer Terrain")]
		[InfoBox("Horizontal and vertical smooth distance from boundary.", InfoMessageType.None)]
		public float SmoothDistance = 20.0f;
		[PropertySpace, VerticalGroup("Outer Terrain")]
		[InfoBox("Higher - steepest.", InfoMessageType.None)]
		public float SmoothFactor = 0.3f;

		[PropertySpace(20), Title("Void"), AssetsOnly]
		public Material VoidMaterial;
	}
}
