using UnityEngine;
using System.Collections.Generic;

public class MessageCollectible : Collectible
{
	public Message message;

	protected override void collect()
	{
		GameManager.instance.text.enqueue(getMessage(message));

		base.collect();
	}

	public List<PlayText> getMessage(Message msg)
	{
		List<PlayText> msgs = new List<PlayText>();
		switch (msg)
		{
			case Message.Default:
				enqueue(msgs, "Default");
				break;
		}

		return msgs;
	}

	private void enqueue(List<PlayText> msgs, string message)
	{
		msgs.Add(new PlayText(message));
	}
}

public enum Message
{
	Default
}


