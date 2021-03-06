﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ITickable {
	public static int FPS = 60;

	public static bool DEBUG = false;

	public static GameManager instance;

	public PlayerController player;
	public Text fpsText;
	public Text frameText;
	public Image title;
	public CameraController mainCamera;

	public TextManager text;
	public Text zillixText;

	public string version = "v0.36";
	public float versionCountdown = 15f;

	public FrameController frameController;
	public int currentFrame {  get { return frameController.currentFrame; } }

	private FPSCounter fpsCounter;

	public Text versionText;

	public Vector3 colorCollected = new Vector3();
	public Vector3 totalColors = new Vector3();

	public SoundBank sounds;


	public bool RotateGravity = true;
	public Vector3 Up {  get { return player.Physics.Up; } }

	public bool hasStartedGame { get; set; }

	public GameObject blueBlocker;
	public GameObject redBlocker;
	public GameObject greenBlocker;

	public void Awake()
	{
		GameManager.instance = this;
		frameController = gameObject.AddComponent<FrameController>();
		frameController.game = this;
		fpsCounter = GetComponent<FPSCounter>();

    }

	public void Start()
	{
		if (versionText != null)
		{
			Color c = versionText.color;
			c.a = 0;
			versionText.color = c;
		}

		sounds = GameObject.Find("SoundBank").GetComponent<SoundBank>();
		versionText.text = version;
	}
	

	public void TickFrame()
	{
		if (versionText != null && !hasStartedGame)
		{

			versionCountdown -= Time.fixedDeltaTime;
			if (versionCountdown <= 0)
			{
				Color versionAlpha = versionText.color;
				versionAlpha.a += Time.fixedDeltaTime;
				versionText.color = versionAlpha;
				versionText.text = version;

			
			}
		}

		if (hasStartedGame)
		{
			Color titleColor = title.color;
			titleColor.a -= Time.fixedDeltaTime;
			title.color = titleColor;

			Color zillixColor = zillixText.color;
			zillixColor.a -= Time.fixedDeltaTime;
			zillixText.color = zillixColor;

			versionText.text = "";

			
		}
		else if (Input.anyKeyDown)
		{
			hasStartedGame = true;
			mainCamera.StartGame();
		}
		player.TickFrame();
		mainCamera.TickFrame();

		if (DEBUG)
		{
			fpsText.text = "FPS: " + fpsCounter.FPS;
			frameText.text = "Current Frame: " + frameController.currentFrame;
		}
		else
		{
			fpsText.text = "";
			frameText.text = "";
		}
	}



	public void RegisterColor(ColorType color)
	{
		switch (color)
		{
			case ColorType.Red:
				totalColors.x++;
				break;
			case ColorType.Green:
				totalColors.y++;
				break;
			case ColorType.Blue:
				totalColors.z++;
				break;
		}
	}

	public void CollectColor(ColorType color)
	{
		sounds.player.PlayOneShot(sounds.collectColor, .7f);

		switch (color)
		{
			case ColorType.Red:
				redBlocker.SetActive(false);
				colorCollected.x++;
				break;
			case ColorType.Green:
				greenBlocker.SetActive(false);
				colorCollected.y++;
				break;
			case ColorType.Blue:
				blueBlocker.SetActive(false);
				colorCollected.z++;
				break;
		}

		Color newFilterColor = new Color(
			Mathf.Min(1, colorCollected.x / Mathf.Max(1, totalColors.x)),
			Mathf.Min(1, colorCollected.y / Mathf.Max(1, totalColors.y)),
			Mathf.Min(1, colorCollected.z / Mathf.Max(1, totalColors.z))
			);

		Color flashColor = Color.red;
		switch (color)
		{
			case ColorType.Red:
				flashColor = Color.red;
				break;
			case ColorType.Green:
				flashColor = Color.green;
				break;
			case ColorType.Blue:
				flashColor = Color.blue;
				break;
		}

		mainCamera.Flash(flashColor, delegate ()
		{
			mainCamera.SetFilterColor(newFilterColor);
		});
	}

	public void TriggerEndGame()
	{
		mainCamera.IsGameOver = true;
		player.DisableJumping = true;
	}

	public void TriggerFadeOut()
	{
		mainCamera.Flash(Color.black, restartGame, 1f, 2f, 1f);
	}

	private void restartGame()
	{
		SceneManager.LoadScene(0);
	}
}
