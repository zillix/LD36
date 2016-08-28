using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayText {

	public string text;
	public float duration;
	public Action callback;
	public bool skippable;

	public static float DEFAULT_DURATION = 3f;

	public PlayText(string Text = "", float Duration = -1, Action callbackFn = null, bool skippable = true)
	{
		if (Duration < 0) {
			Duration = DEFAULT_DURATION;
		}
		callback = callbackFn;
		text = Text;
		duration = Duration;
		this.skippable = skippable;
	}

	public static void addText(List<PlayText> textList, string text,float duration = -1, Action callback = null, bool skippable = true)
	{
		if (duration < 0) {
			duration = DEFAULT_DURATION;
		}
		textList.Add (new PlayText(text, duration, callback, skippable));
	}
}
