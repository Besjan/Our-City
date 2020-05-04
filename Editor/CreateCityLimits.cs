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

	public static class CreateCityLimits
    {
        const float boundaryHeight = 100.0f;

        const int curveOffset = 20;
        const int curveSampleRate = 15;

        const float smoothDistance = 20.0f;
        const float smoothFactor = 0.3f;


        [MenuItem("Cuku/City/Limits/Lower Outer Terrains")]
        static void LowerOuterTerrain()
        {
            var startTime = DateTime.Now;

            var boundaryPoints = Features.GetBoundaryPoints().AddTileIntersectionPoints();
            var boundaryPoints2D = boundaryPoints.ProjectToXZPlane();

            var hitTerrains = boundaryPoints.GetHitTerrainsAndBoundaryPoints();

            for (int tc = 0; tc < hitTerrains.Count; tc++)
            {
                var keyPair = hitTerrains.ElementAt(tc);
                var terrain = keyPair.Key;
                var boundaryCurve = keyPair.Value.GetCurve(curveSampleRate, true);

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

                        if (distanceFromCurve > smoothDistance)
                        {
                            heights[j, i] = 0;
                            continue;
                        }

                        var smoothAmountMeter = distanceFromCurve * smoothFactor;
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

        [MenuItem("Cuku/City/Limits/Add Void")]
        static void AddVoid()
        {
            var voidPlane = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
            voidPlane.name = "Void";
            voidPlane.position = Vector3.up * 24;
            voidPlane.eulerAngles = Vector3.right * 90;
            voidPlane.localScale = new Vector3(100000, 100000, 1);
            voidPlane.GetComponent<MeshRenderer>().material = Resources.Load<Material>("CityLimits/VoidMaterial");
            GameObject.DestroyImmediate(voidPlane.GetComponent<Collider>());
        }

        [MenuItem("Cuku/City/Limits/Create Boundary")]
        static void CreateBoundary()
        {
            var boundaryPoints = Features.GetBoundaryPoints().AddTileIntersectionPoints();

            boundaryPoints.CreateWall("Boundary");
        }

        static Dictionary<Terrain, Vector3[]> GetHitTerrainsAndBoundaryPoints(this Vector3[] boundaryPoints)
        {
            Dictionary<Terrain, int[]> terrainLimitsIds = new Dictionary<Terrain, int[]>();
            for (int i = 0; i < boundaryPoints.Length - 1; i++)
            {
                var terrain = boundaryPoints[i].GetHitTerrain();
                if (!terrainLimitsIds.ContainsKey(terrain))
                {
                    terrainLimitsIds.Add(terrain, new int[] { i, i });
                    continue;
                };

                var limits = terrainLimitsIds[terrain];
                limits[0] = Mathf.Min(limits[0], i);
                limits[1] = Mathf.Max(limits[1], i);

                terrainLimitsIds[terrain] = limits;
            }

            Dictionary<Terrain, Vector3[]> terrains = new Dictionary<Terrain, Vector3[]>();
            foreach (var terrain in terrainLimitsIds)
            {
                var limits = terrain.Value;
                var startId = limits[0] - curveOffset;
                var endId = limits[1] + curveOffset;

                var points = new List<Vector3>();
                if (limits[0] == 0) points.AddRange(boundaryPoints.Skip(boundaryPoints.Count() - curveOffset));
                points.AddRange(boundaryPoints.Skip(startId).Take(endId - startId));

                terrains.Add(terrain.Key, points.ToArray());
            }

            return terrains;
        }

        private static void CreateWall(this Vector3[] boundaryPoints, string name)
        {
            // Create vertices
            var wallVertices = new List<Vector3>();

            for (int p = 0; p < boundaryPoints.Length - 1; p++)
            {
                var point0 = boundaryPoints[p];
                var point1 = boundaryPoints[p + 1];

                wallVertices.Add(point0);
                wallVertices.Add(point1);
                wallVertices.Add(new Vector3(point0.x, point0.y + boundaryHeight, point0.z));
                wallVertices.Add(new Vector3(point1.x, point1.y + boundaryHeight, point1.z));
            }

            var sharedVertices = new List<SharedVertex>();

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

            wall.SetMaterial(faces, Resources.Load<Material>("2Sided"));

            wall.gameObject.name = wall.name = name;
            wall.transform.SetParent(null, true);
        }

        #region Points
        static Vector3[] AddTileIntersectionPoints(this Vector3[] points)
        {
            var allPoints = new List<Vector3>();
            var intersectionPointIds = new List<int>();
            for (int i = 1; i < points.Length; i++)
            {
                var point1 = points[i - 1];
                var point2 = points[i];

                if (point1.GetHitTerrainName() != point2.GetHitTerrainName())
                {
                    var intersectionPoint = GetTileIntersectionPoint(point1, point2);
                    allPoints.Add(intersectionPoint);
                    intersectionPointIds.Add(allPoints.Count - 1);
                }

                allPoints.Add(point2);
            }

            // Shift points to match the start of a tile
            var shiftedPoints = new Vector3[allPoints.Count + 1];
            for (int i = 0; i < intersectionPointIds.Count; i++)
            {
                if (allPoints[intersectionPointIds[i]].GetHitTerrainName() == allPoints[intersectionPointIds[i + 1]].GetHitTerrainName()) continue;

                var startId = intersectionPointIds[i + 1];

                for (int j = startId; j < allPoints.Count; j++)
                {
                    shiftedPoints[j - startId] = allPoints[j];
                }
                for (int j = 0; j < startId; j++)
                {
                    shiftedPoints[(allPoints.Count - startId) + j] = allPoints[j];
                }
                shiftedPoints[shiftedPoints.Length - 1] = shiftedPoints[0];

                break;
            }

            return shiftedPoints;
        }

        static Vector3 GetTileIntersectionPoint(Vector3 point1, Vector3 point2)
        {
            var terrain = point1.GetHitTerrain().transform.GetComponent<Terrain>();
            var terrainAnglePoints = terrain.GetTerrainAnglePoints();

            var A1 = new Vector2(point1.x, point1.z);
            var A2 = new Vector2(point2.x, point2.z);

            bool found;
            Vector2[] intersections = new Vector2[4];
            intersections[0] = Utilities.GetLinesIntersectionPoint(A1, A2, terrainAnglePoints[0], terrainAnglePoints[1], out found);
            intersections[1] = Utilities.GetLinesIntersectionPoint(A1, A2, terrainAnglePoints[0], terrainAnglePoints[2], out found);
            intersections[2] = Utilities.GetLinesIntersectionPoint(A1, A2, terrainAnglePoints[1], terrainAnglePoints[3], out found);
            intersections[3] = Utilities.GetLinesIntersectionPoint(A1, A2, terrainAnglePoints[2], terrainAnglePoints[3], out found);

            var closest = 0;
            for (int i = 1; i < intersections.Length; i++)
            {
                if (Vector3.Distance(A1, intersections[i]) < Vector3.Distance(A1, intersections[closest]))
                {
                    closest = i;
                }
            }

            return intersections[closest].GetHitTerrainPosition();
        }
        #endregion
    }
}
