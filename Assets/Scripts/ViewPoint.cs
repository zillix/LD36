using UnityEngine;
using System.Collections;

public class ViewPoint : MonoBehaviour
{
	public static float COLLECT_DIST = 2.5f;
	private PlayerController player;

	protected virtual void Start()
	{
		player = GameManager.instance.player;

		
	}

	protected virtual void Update()
	{

		if (Vector2.Distance(player.Physics.Center, transform.position) < COLLECT_DIST)
		{
			GameManager.instance.mainCamera.SetViewPoint(true);
		}


	}

	void OnDrawGizmos()
	{
		Color color = Color.cyan;
		color.a = .3f;
		Gizmos.color = color;
		Gizmos.DrawSphere(transform.position, COLLECT_DIST);
	}
}