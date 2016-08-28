using UnityEngine;
using System.Collections;

public class SoundBank : MonoBehaviour
{

	public AudioSource player;

	public AudioClip textBubble;
	public AudioClip jump;
	public AudioClip flip;
	public AudioClip land;
	public AudioClip collectColor;
	public AudioClip collectPowerUp;
	public AudioClip fallToDeath;
	public AudioClip heavyLand;
	public AudioClip respawn;
	public AudioClip drop;

	// Use this for initialization
	void Start()
	{
		//player = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
