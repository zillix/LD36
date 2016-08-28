using UnityEditor;
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
					float gridForThis = grid;
					if (colMesh.gameObject.GetComponent<Collectible>() != null)
					{
						gridForThis *= .5f;
					}
					Vector2 point = colMesh.transform.position;
					int mod = (int)(point.x % gridForThis);
					point.x = mod * gridForThis + Mathf.RoundToInt((point.x - mod) / (float)gridForThis) * gridForThis;
					mod = (int)(point.y % gridForThis);
					point.y = mod * gridForThis + Mathf.RoundToInt((point.y - mod) / (float)gridForThis) * gridForThis;
					colMesh.transform.position = point;
				}
			}
		}
		EditorGUILayout.EndHorizontal();
	}
}
