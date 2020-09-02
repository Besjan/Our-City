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

	public class CityLimitsEditor : OdinEditorWindow
	{
		#region Editor
		[MenuItem("Cuku/Our City/City Limits Editor")]
		private static void OpenWindow()
		{
			var window = GetWindow<CityLimitsEditor>();
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
		public void CreateBoundary()
		{
			var boundaryPoints = Config.BoundaryGeoData.GetPathInStreamingAssets().GetBoundaryPoints().AddTileIntersectionPoints();

			// Create vertices
			var wallVertices = new List<Vector3>();

			for (int p = 0; p < boundaryPoints.Length - 1; p++)
			{
				var point0 = boundaryPoints[p];
				var point1 = boundaryPoints[p + 1];

				wallVertices.Add(point0);
				wallVertices.Add(point1);
				wallVertices.Add(new Vector3(point0.x, point0.y + Config.BoundaryHeight, point0.z));
				wallVertices.Add(new Vector3(point1.x, point1.y + Config.BoundaryHeight, point1.z));
			}

			// Create faces
			var faces = new List<Face>();
			for (int f = 0; f < wallVertices.Count - 3; f += 4)
			{
				var faceVertices = new int[] { f, f + 1, f + 2, f + 1, f + 3, f + 2 };
				faces.Add(new Face(faceVertices));
			}

			var wall = ProBuilderMesh.Create(wallVertices, faces);

			Normals.CalculateNormals(wall);
			Normals.CalculateTangents(wall);
			Smoothing.ApplySmoothingGroups(wall, faces, 30);
			wall.ToMesh();
			wall.Refresh();
			EditorMeshUtility.Optimize(wall);

			var meshRenderer = wall.GetComponent<MeshRenderer>();
			meshRenderer.material = Config.BoundaryMaterial;
			meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;

			wall.gameObject.name = wall.name = Path.GetFileNameWithoutExtension(Config.BoundaryGeoData).Replace('_', ' ');
			wall.transform.SetParent(null, true);
		}

		[ShowIf("IsConfigValid"), PropertySpace(20), Button(ButtonSizes.Large)]
		public void AddVoid()
		{
			var voidPlane = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
			GameObject.DestroyImmediate(voidPlane.GetComponent<Collider>());

			voidPlane.name = Config.CityName.Value + " Void";
			voidPlane.position = Vector3.up * 24;
			voidPlane.eulerAngles = Vector3.right * 90;
			voidPlane.localScale = new Vector3(100000, 100000, 1);

			var meshRenderer = voidPlane.GetComponent<MeshRenderer>();
			meshRenderer.material = Config.VoidMaterial;
			meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
		}

#if DREAMTECK_SPLINES
		[ShowIf("IsConfigValid"), PropertySpace(20), Button(ButtonSizes.Large)]
		public void SmoothOuterTerrain()
		{
			var startTime = DateTime.Now;

			var boundaryPoints = Config.BoundaryGeoData.GetPathInStreamingAssets().GetBoundaryPoints().AddTileIntersectionPoints();
			var boundaryPoints2D = boundaryPoints.ProjectToXZPlane();

			var hitTerrains = boundaryPoints.GetHitTerrainsAndBoundaryPoints(Config.CurveOffset);

			for (int tc = 0; tc < hitTerrains.Count; tc++)
			{
				var keyPair = hitTerrains.ElementAt(tc);
				var terrain = keyPair.Key;
				var boundaryCurve = keyPair.Value.CreateCurve(Config.CurveSampleRate, true);

				var position = terrain.GetPosition();
				var size = terrain.terrainData.size;
				var hmResolution = terrain.terrainData.heightmapResolution;
				var heights = terrain.terrainData.GetHeights(0, 0, hmResolution, hmResolution);

				for (int i = 0; i < hmResolution; i++)
				{
					for (int j = 0; j < hmResolution; j++)
					{
						float posX = size.x * i / (hmResolution - 1) + position.x;
						float posZ = size.z * j / (hmResolution - 1) + position.z;

						var pointPosition2D = new Vector2(posX, posZ);

						if (pointPosition2D.IsInside(boundaryPoints2D)) continue;

						var positionOnXZPlane = new Vector3(posX, 0, posZ);
						var positionOnCurve = boundaryCurve.EvaluatePosition(boundaryCurve.Project(positionOnXZPlane).percent);
						var distanceFromCurve = Vector3.Distance(positionOnXZPlane, positionOnCurve);

						if (distanceFromCurve > Config.SmoothDistance)
						{
							heights[j, i] = 0;
							continue;
						}

						var smoothAmountMeter = distanceFromCurve * Config.SmoothFactor;
						smoothAmountMeter *= smoothAmountMeter;
						var smoothAmountPercent = (positionOnCurve.GetHitTerrainHeight() - smoothAmountMeter) / size.y;

						heights[j, i] = smoothAmountPercent;
					}
				}

				terrain.terrainData.SetHeights(0, 0, heights);

				GameObject.DestroyImmediate(boundaryCurve.gameObject);

				Debug.Log(terrain.name + ": " + DateTime.Now.Subtract(startTime).TotalMinutes);
			}
		}
#endif
		#endregion
	}
}
