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

	public string version = "v.1";
	public float versionCountdown = 15f;

	public FrameController frameController;
	public int currentFrame {  get { return frameController.currentFrame; } }

	private FPSCounter fpsCounter;

	public Text versionText;


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
}
