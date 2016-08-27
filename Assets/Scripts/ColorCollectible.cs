using UnityEngine;
using System.Collections;

public class ColorCollectible : Collectible
{
	public ColorType colorType;

	protected override void Start()
	{
		base.Start();
		GameManager.instance.RegisterColor(colorType);
	}

	protected override void collect()
	{
		GameManager.instance.CollectColor(colorType);

		base.collect();
	}
}

public enum ColorType
{
	Red,
	Green,
	Blue
}
