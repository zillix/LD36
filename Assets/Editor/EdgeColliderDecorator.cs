﻿using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EdgeCollider2D))]
public class EdgeColliderDecorator : DecoratorEditor {

	private float rescaleamt = 1.0f;

	public EdgeColliderDecorator() : base("EdgeCollider2DEditor")
	{ }

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		EdgeCollider2D collider = serializedObject.targetObject as EdgeCollider2D;

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