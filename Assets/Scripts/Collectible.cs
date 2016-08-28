using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour
{
	public static float COLLECT_DIST = 1;
	private PlayerController player;

	private bool collected = false;

	public static float ROTATE_SPEED = 60;

	protected virtual void Start()
	{
		player = GameManager.instance.player;
	}

	protected virtual void Update()
	{

		if (collected)
		{
			return;
		}

		if (Vector2.Distance(player.Physics.Center, transform.position) < COLLECT_DIST)
		{
			collect();
		}

		transform.Rotate(new Vector3(0, 0, -ROTATE_SPEED * Time.fixedDeltaTime));


	}

	protected virtual void collect()
	{
		collected = true;
		Destroy(gameObject);
	}
}