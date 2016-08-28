using UnityEngine;
using System.Collections.Generic;
using System;

public class ViewPoint : MonoBehaviour
{
	public float CollectDist = 2f;
	private PlayerController player;

	public Message message;

	private bool displayedMessage = false;

	public static float ROTATE_SPEED = 60;

	public static float PULSE_MAX_SIZE = 1.3f;
	public static float PULSE_ROTATE_SPEED = 40f;

	private float pulseAngle = 0;

	private Color currentColor = Color.red;
	public static float COLOR_SPEED = .5f;

	public int increasingColorIndex = 1;

	private MeshRenderer meshRender;

	protected virtual void Start()
	{
		player = GameManager.instance.player;

		meshRender = GetComponent<MeshRenderer>();
	}

	protected virtual void Update()
	{

		if (Vector2.Distance(player.Physics.Center, transform.position) < CollectDist)
		{
			if (!displayedMessage)
			{
				GameManager.instance.text.enqueue(getMessage(message));
				displayedMessage = true;
			}
		}
		else
		{
			displayedMessage = false;
		}

		pulseAngle += Time.fixedDeltaTime * PULSE_ROTATE_SPEED;
		float scale = PULSE_MAX_SIZE * Mathf.Sin(MathUtil.toRadians(pulseAngle));
		if (scale < 1)
		{
			float delta = 1 - scale;
			scale = 1 + delta / 4;
		}
		Vector3 targetScale = Vector3.one * scale * CollectDist * 2 * .2f;//.59f;
		targetScale.y = .2f;
		transform.localScale = targetScale;

		Color nextColor = currentColor;
		switch (increasingColorIndex)
		{
			case 0:
				nextColor.r += Time.fixedDeltaTime * COLOR_SPEED;
				nextColor.b -= Time.fixedDeltaTime * COLOR_SPEED;
				if (nextColor.r >= 1)
				{
					increasingColorIndex = 1;
				}
				break;

			case 1:
				nextColor.g += Time.fixedDeltaTime * COLOR_SPEED;
				nextColor.r -= Time.fixedDeltaTime * COLOR_SPEED;
				if (nextColor.g >= 1)
				{
					increasingColorIndex = 2;
				}
				break;

			case 2:
				nextColor.b += Time.fixedDeltaTime * COLOR_SPEED;
				nextColor.g -= Time.fixedDeltaTime * COLOR_SPEED;
				if (nextColor.b >= 1)
				{
					increasingColorIndex = 0;
				}
				break;
		}
		//nextColor.a = .5f;
		currentColor = nextColor;
		meshRender.material.color = nextColor;


	}



	public List<PlayText> getMessage(Message msg)
	{
		List<PlayText> msgs = new List<PlayText>();
		switch (msg)
		{
			case Message.Default:
				enqueue(msgs, "default");
				break;
			case Message.Intro:
				enqueue(msgs, "wheelhouse");
				enqueue(msgs, "status: tired");
				break;
			case Message.Center:
				enqueue(msgs, "engine room");
				enqueue(msgs, "status: confused");
				break;
			case Message.Flip:
				enqueue(msgs, "brig");
				enqueue(msgs, "status: astray");
				break;
			case Message.Loop:
				enqueue(msgs, "cargo hold");
				enqueue(msgs, "status: lonely");
				break;
			case Message.Top:
				enqueue(msgs, "forecastle");
				enqueue(msgs, "status: disappointed");
				break;
			case Message.SecretRoof:
				enqueue(msgs, "landing pad");
				enqueue(msgs, "status: anxious");
				break;
			case Message.ShipCenter:
				enqueue(msgs, "error: unrecognized species");
				enqueue(msgs, "suggestion: reformat inhabitants");
				break;
			case Message.ShipLeft:
				enqueue(msgs, "error: unrecognized format");
				enqueue(msgs, "suggestion: reformat occupantsq");
				break;
			case Message.ShipRight:
				enqueue(msgs, "error: ");
				enqueue(msgs, "status: anxious");
				break;
			case Message.Victory:
				enqueue(msgs, "wqq", GameManager.instance.TriggerEndGame);
				enqueue(msgs, "wqq");
				enqueue(msgs, "status: anxious");
				break;
			case Message.ShipSecret:
				enqueue(msgs, "ship secret");
				break;
			case Message.SecretAlcove:
				enqueue(msgs, "secret alcove");
				break;
			case Message.SecretStart:
				enqueue(msgs, "secret alcove");
				break;
			case Message.SecretDrop:
				enqueue(msgs, "secret drop");
				break;
		}

		return msgs;
	}

	private void enqueue(List<PlayText> msgs, string message, Action callback = null)
	{
		msgs.Add(new PlayText(message, -1, callback));
	}

	void OnDrawGizmos()
	{
		Color color = Color.cyan;
		color.a = .3f;
		Gizmos.color = color;
		Gizmos.DrawSphere(transform.position, CollectDist);
	}
}

public enum Message
{
	Default,
	Intro,
	Center,
	Flip,
	Loop,
	Top,
	SecretRoof,
	ShipLeft,
	ShipCenter,
	ShipRight,
	Victory,
	SecretDrop,
	ShipSecret,
	SecretAlcove,
	SecretStart
}