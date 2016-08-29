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
			if (!displayedMessage && !GameManager.instance.text.isBusy)
			{
				GameManager.instance.text.enqueue(getMessage(message));
			}

			displayedMessage = true;


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
		Vector3 total = GameManager.instance.totalColors;
		Vector3 collected = GameManager.instance.colorCollected;


		List<PlayText> msgs = new List<PlayText>();
		switch (msg)
		{
			case Message.Default:
				enqueue(msgs, "default");
				break;
			case Message.Intro:
				enqueue(msgs, "-captain's quarters-");
				enqueue(msgs, "status: happily concerned");
				break;
			case Message.Center:
				enqueue(msgs, "-research center-");
				string status = "chromatic reconstruction status: ";
				for (int i = 0; i < collected.x; ++i)
				{
					status += i < total.x ? "R" : "r";
				}
				for (int i = 0; i < collected.y; ++i)
				{
					status += i < total.y ? "G" : "g";
				}
				for (int i = 0; i < collected.z; ++i)
				{
					status += i < total.z ? "B" : "r";

				}
				enqueue(msgs, status);
				break;
			case Message.Flip:
				enqueue(msgs, "-brig-");
				enqueue(msgs, "epiphany status: in progress");
				break;
			case Message.Loop:
				enqueue(msgs, "-cargo hold-");
				enqueue(msgs, "status: plundered");
				break;
			case Message.Top:
				enqueue(msgs, "-forecastle-");
				enqueue(msgs, "status: wishing for a better view");
				break;
			case Message.SecretRoof:
				enqueue(msgs, "-landing pad-");
				enqueue(msgs, "status: confused about purpose");
				break;
			case Message.ShipCenter:
				enqueue(msgs, "error: occupant type unrecognized");
				enqueue(msgs, "suggestion: reformat occupant");
				break;
			case Message.ShipLeft:
				enqueue(msgs, "praise the sun!");
				break;
			case Message.ShipRight:
				enqueue(msgs, "starting adventure...");
				enqueue(msgs, "adventure failed to start");
				break;
			case Message.Victory:
				int totalAmt = (int)( total.x + total.y + total.z);
				int collectedAmt = (int)( collected.x + collected.y + collected.z);
				string ackStatus = "chromatic acknowledgement status:";
				if (collectedAmt == 0)
				{
					ackStatus += " in denial";
				}
				else if (collectedAmt < totalAmt / 2)
				{
					ackStatus += " adequate";
				}
				else if (collectedAmt < totalAmt)
				{
					ackStatus += " vivid";
                }
				else
				{
					ackStatus += " fully accepted";
				}
				enqueue(msgs, "-departure chamber-");
				enqueue(msgs, "disengaging...", GameManager.instance.TriggerEndGame);
				enqueue(msgs, "undocking status: success");
				enqueue(msgs, ackStatus);
				break;
			/*case Message.ShipSecret:
				enqueue(msgs, "ship secret");
				break;*/
			case Message.SecretAlcove:
				enqueue(msgs, "-remote rudder-");
				enqueue(msgs, "status: it's complicated");
				break;
			case Message.SecretStart:
				enqueue(msgs, "-satellite buoy-");
				enqueue(msgs, "status: single and loving it!");
				break;
			case Message.SecretDrop:
				enqueue(msgs, "-observatory-");
				enqueue(msgs, "fortune: never forget where you came from");
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