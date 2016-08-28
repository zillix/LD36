using UnityEngine;
using System.Collections;

public class ViewPoint : MonoBehaviour
{
	public float CollectDist = 2f;
	private PlayerController player;

	protected virtual void Start()
	{
		player = GameManager.instance.player;

		
	}

	protected virtual void Update()
	{

		if (Vector2.Distance(player.Physics.Center, transform.position) < CollectDist)
		{
			GameManager.instance.mainCamera.SetViewPoint(true);
		}


	}

	void OnDrawGizmos()
	{
		Color color = Color.cyan;
		color.a = .3f;
		Gizmos.color = color;
		Gizmos.DrawSphere(transform.position, CollectDist);
	}
}