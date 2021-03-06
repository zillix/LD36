﻿using UnityEngine;
using System.Collections;

public class LayerUtil {

	public static string GROUND = "ground";
	public static string ROTATE_GROUND = "rotateGround";
	public static string FLIP_GROUND = "flipGround";

	public static int GetLayerMask(string layerName)
	{
		int layer = LayerMask.NameToLayer(layerName);
		return 1 << layer;
	}
}

