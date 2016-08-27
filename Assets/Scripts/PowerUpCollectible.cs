using UnityEngine;
using System.Collections;

public class PowerUpCollectible : Collectible {

	public PowerUpType Type;

	protected override void collect()
	{
		base.collect();

		GameManager.instance.player.CollectPowerUp(Type);
	}
}

public enum PowerUpType
{
	Rotate,
	Drop,
	Flip
}

