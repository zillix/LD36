using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour
{
	public static float COLLECT_DIST = 1;
	private PlayerController player;

	private bool collected = false;

	public static float ROTATE_SPEED = 60;

	public static float PULSE_MAX_SIZE = 1.3f;
	public static float PULSE_ROTATE_SPEED = 60f;

	private float pulseAngle = 0;

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

		if (!player.IsFallingDead
			&& !player.IsCollapsed
			&& Vector2.Distance(player.Physics.Center, transform.position) < COLLECT_DIST)
		{
			collect();
		}

		float rotateSpeed = -ROTATE_SPEED * Time.fixedDeltaTime;
        transform.Rotate(new Vector3(0,0, rotateSpeed));

		pulseAngle += Time.fixedDeltaTime * PULSE_ROTATE_SPEED;
		float scale = Mathf.Max(1, PULSE_MAX_SIZE * Mathf.Sin(MathUtil.toRadians(pulseAngle)));
		transform.localScale = Vector3.one * scale;


	}

	protected virtual void collect()
	{
		collected = true;
		Destroy(gameObject);
	}
}