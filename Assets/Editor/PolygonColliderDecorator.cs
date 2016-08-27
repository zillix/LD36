using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PolygonCollider2D))]
public class PolygonColliderDecorator : DecoratorEditor
{
	private float rescaleamt = 1.0f;
	public PolygonColliderDecorator() : base("PolygonCollider2DEditor")
	{ }

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		PolygonCollider2D collider = serializedObject.targetObject as PolygonCollider2D;

		if (GUILayout.Button("Center Points"))
		{
			collider.CenterPoints();
		}

		if (GUILayout.Button("Rotate Clockwise"))
		{
			collider.RotatePoints(-90);
		}

		EditorGUILayout.BeginHorizontal();
		{
			rescaleamt = EditorGUILayout.FloatField("Rescale:", rescaleamt);

			if (GUILayout.Button("Rescale"))
			{
				collider.Rescale(rescaleamt);
				rescaleamt = 1.0f;
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		{
			if (GUILayout.Button("Flip Horizontal"))
			{
				collider.Flip(true, false);
			}
			if (GUILayout.Button("Flip Vertical"))
			{
				collider.Flip(false, true);
			}
		}
		EditorGUILayout.EndHorizontal();
	}
}
