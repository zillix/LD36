﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {

	private float grid = 1f;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("Update all Meshes"))
		{
			ColliderToMesh[] allMeshes = GameObject.FindObjectsOfType<ColliderToMesh>();
			foreach (ColliderToMesh colMesh in allMeshes)
			{
				colMesh.CreateMesh();
			}
		}

		if (GUILayout.Button("Center all Meshes"))
		{
			PolygonCollider2D[] allCol = GameObject.FindObjectsOfType<PolygonCollider2D>();
			foreach (PolygonCollider2D colMesh in allCol)
			{
				colMesh.CenterPoints();
			}

			EdgeCollider2D[] allEdg = GameObject.FindObjectsOfType<EdgeCollider2D>();
			foreach (EdgeCollider2D colMesh in allEdg)
			{
				colMesh.CenterPoints();
			}
		}

		EditorGUILayout.BeginHorizontal();
		{
			grid = EditorGUILayout.FloatField("Grid:", grid);
			if (GUILayout.Button("Align"))
			{
				ColliderToMesh[] allMeshes = GameObject.FindObjectsOfType<ColliderToMesh>();
				foreach (ColliderToMesh colMesh in allMeshes)
				{
					Vector2 point = colMesh.transform.position;
					int mod = (int)(point.x % grid);
					point.x = mod * grid + Mathf.RoundToInt((point.x - mod) / (float)grid) * grid;
					mod = (int)(point.y % grid);
					point.y = mod * grid + Mathf.RoundToInt((point.y - mod) / (float)grid) * grid;
					colMesh.transform.position = point;
				}
			}
		}
		EditorGUILayout.EndHorizontal();
	}
}