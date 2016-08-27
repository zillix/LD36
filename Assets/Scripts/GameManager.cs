using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, ITickable {
	public static int FPS = 60;

	public static bool DEBUG = true;

	public static GameManager instance;

	public PlayerController player;
	public Text fpsText;
	public Text frameText;
	public CameraController mainCamera;

	public TextManager text;

	public string version = "v.1";
	public float versionCountdown = 15f;

	public FrameController frameController;
	public int currentFrame {  get { return frameController.currentFrame; } }

	private FPSCounter fpsCounter;

	public Text versionText;

	private Vector3 colorCollected = new Vector3();
	public Vector3 totalColors = new Vector3();


	public bool RotateGravity = true;
	public Vector3 Up {  get { return player.Physics.Up; } }

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
	}
	

	public void TickFrame()
	{
		if (versionText != null)
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

		player.TickFrame();

		mainCamera.TickFrame();

		fpsText.text = "FPS: " + fpsCounter.FPS;
		frameText.text = "Current Frame: " + frameController.currentFrame;
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
		switch (color)
		{
			case ColorType.Red:
				colorCollected.x++;
				break;
			case ColorType.Green:
				colorCollected.y++;
				break;
			case ColorType.Blue:
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
}
