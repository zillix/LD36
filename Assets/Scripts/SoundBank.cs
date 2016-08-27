using UnityEngine;
using System.Collections;

public class SoundBank : MonoBehaviour
{

	public AudioSource player;


	// Use this for initialization
	void Start()
	{
		player = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
