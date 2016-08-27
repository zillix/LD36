using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Color Adjustments/ColorFilter")]
	public class ColorFilter : ImageEffectBase
	{
		public Color filter;

		// Called by camera to apply image effect
		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			material.SetColor("_FilterColor", filter);
			Graphics.Blit(source, destination, material);
		}
	}
}
