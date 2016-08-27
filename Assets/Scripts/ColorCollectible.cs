using UnityEngine;
using System.Collections;

public class ColorCollectible : MonoBehaviour
{
	public static float COLLECT_DIST = 1;
	public ColorType colorType;
	private PlayerController player;

	private bool collected = false;

	void Start()
	{
		player = GameManager.instance.player;
		GameManager.instance.RegisterColor(colorType);
	}

	void Update () {

		if (collected)
		{
			return;
		}

		if (Vector2.Distance(player.transform.position, transform.position) < COLLECT_DIST)
		{
			collect();
		}


	}

	private void collect()
	{
		collected = true;
		GameManager.instance.CollectColor(colorType);
		Destroy(gameObject);
	}
}

public enum ColorType
{
	Red,
	Green,
	Blue
}
