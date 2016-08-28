using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.ImageEffects;

using System;

public class CameraController : MonoBehaviour, ITickable {

	public float speed;
	private PlayerController player;
	float deadDist = 2f;

	public Vector2 InsideOffset = new Vector2(0, 3);
	public Vector2 OutsideOffset = new Vector2(0, -10);
	public float InsideDist = -10f;
	public float OutsideDist = -20f;
	public float ZoomSpeed = 12f;

	public float lerpSpeed = 1f;
	private float cameraAngle = 0;
	public float InsideOrtho = 60f;
	public float OutsideOrtho = 120f;
	public float ViewPointOrtho = 60f;
	public float VictoryOrtho = 130f;

	private Camera mainCamera;

	private float cameraShakeMagnitude;
	private float cameraShakeFramesRemaining;

	public Image CameraFlash;

	public bool Rotate = true;

	private ColorFilter filter;

	private bool onViewPoint = false;

	ViewPoint[] viewPoints;

	public float VictorySpeed = 5;

	private Vector3 topBound;

	public bool IsGameOver { get; set; }
	private bool reachedEndGame = false;



	// Use this for initialization
	void Start () {
		player = GameManager.instance.player;
		transform.position = calculateTargetPosition();
		Vector3 newPos = transform.position;
		newPos.z = -10;
		transform.position = newPos;
		mainCamera = GetComponent<Camera>();
		mainCamera.orthographicSize = InsideOrtho;
		filter = GetComponent<ColorFilter>();

		viewPoints = FindObjectsOfType<ViewPoint>();
		topBound = (GameObject.Find("TopBound") as GameObject).transform.position;
	}

	public void SetFilterColor(Color color)
	{
		filter.filter = color;
	}

	// Update is called once per frame
	public void TickFrame()
	{

		if (IsGameOver && transform.position.y > topBound.y && !reachedEndGame)
		{
			reachedEndGame = true;
			GameManager.instance.TriggerFadeOut();
		}

		int cullingMask = -1;

		onViewPoint = false;
		foreach (ViewPoint point in viewPoints)
		{
			if (point.message == Message.Victory)
			{
				continue;
			}

			if (Vector2.Distance(player.Physics.Center, point.transform.position) < point.CollectDist)
			{
				onViewPoint = true;
				break;
			}
		}

		if (player.IsInside)
		{
		//	cullingMask &= ~LayerUtil.GetLayerMask(LayerUtil.MOON_SIGN);
		}

		mainCamera.cullingMask = cullingMask;

		float cacheZ = transform.position.z;
		Vector3 targetPosition = calculateTargetPosition();

		Vector2 dist = (Vector2)(targetPosition - transform.position);
		if (dist.magnitude > deadDist)
		{

			if (dist.magnitude - deadDist < speed * Time.fixedDeltaTime)
			{
				// Set exactly to dead dist
				transform.position = (Vector2)targetPosition - dist.normalized * deadDist;
			}
			else
			{
				transform.position += (Vector3)(speed * Time.fixedDeltaTime * dist.normalized);
			}

			dist = targetPosition - transform.position;

			Vector3 transPos = transform.position;
			transPos.z = cacheZ;
			transform.position = transPos;
		}

		float targetDist = player.IsInside ? InsideDist : OutsideDist;
		Vector3 newPos = transform.position;
		if (Mathf.Abs(transform.position.z - targetDist) < Time.fixedDeltaTime * ZoomSpeed)
		{
			newPos.z = targetDist;
		}
		else
		{
			if (targetDist > transform.position.z)
			{
				newPos.z += Time.fixedDeltaTime * ZoomSpeed;
			}
			else
			{
				newPos.z -= Time.fixedDeltaTime * ZoomSpeed;
			}
		}

		float targetOrthographicSize = player.IsInside ? InsideOrtho : OutsideOrtho;
		if (onViewPoint)
		{
			targetOrthographicSize = ViewPointOrtho;
		}
		if (IsGameOver)
		{
			targetOrthographicSize = VictoryOrtho;
		}

		if (Mathf.Abs(mainCamera.orthographicSize - targetOrthographicSize) < Time.fixedDeltaTime * ZoomSpeed)
		{
			mainCamera.orthographicSize = targetOrthographicSize;
		}
		else
		{
			if (targetOrthographicSize > mainCamera.orthographicSize)
			{
				mainCamera.orthographicSize += Time.fixedDeltaTime * ZoomSpeed;
			}
			else
			{
				mainCamera.orthographicSize -= Time.fixedDeltaTime * ZoomSpeed;
			}
		}


		if (cameraShakeFramesRemaining > 0)
		{
			cameraShakeFramesRemaining--;
			Vector3 shakeOffset = UnityEngine.Random.Range(-cameraShakeMagnitude, cameraShakeMagnitude) * MathUtil.AngleToVector(UnityEngine.Random.Range(-180, 180));
			newPos += shakeOffset;
		}

		transform.position = newPos;

		if (Rotate && player.Physics.CanRotate)
		{
			float offset = 270f;

			float targetAngle = MathUtil.VectorToAngle(player.Physics.Up) + offset;
			if (player.IsInside)
			{
				//targetAngle += 180;
			}
			cameraAngle = MathUtil.RotateAngle(cameraAngle, targetAngle, lerpSpeed * Time.fixedDeltaTime);

		}
		else
		{
			cameraAngle = 0;
		}
		transform.rotation = Quaternion.Euler(0, 0, cameraAngle);

		
	}

	private Vector3 calculateTargetPosition()
	{
		Vector3 targetPosition = player.transform.position;
		if (IsGameOver)
		{
			targetPosition = transform.position + new Vector3(0, 10000);
		}

		Vector2 offset = player.IsInside ? InsideOffset : OutsideOffset;
		
		targetPosition.y += player.RotationUp.y * offset.y;
		targetPosition.x += player.RotationUp.x * offset.y;

		return targetPosition;
	}

	public void BeginCameraShake(int frames, float magnitude)
	{
		cameraShakeMagnitude = magnitude;
		cameraShakeFramesRemaining = frames;
	}

	public IEnumerator Fade(float startAlpha,
		float endAlpha,
		float waitDuration,
		Color color)
	{
		Image image = CameraFlash;
		if (image.color.a == startAlpha)
		{
			color.a = startAlpha;
			image.color = color;

			for (float i = 0; i < 1.0; i += Time.deltaTime * (1 / waitDuration))
			{ //for the length of time

				color.a = Mathf.Lerp(startAlpha, endAlpha, i);
				image.color = color;
				yield return null;
				color.a = endAlpha;
				image.color = color;
			} //end for
		}
	}

	public void SetViewPoint(bool onViewPoint)
	{
		this.onViewPoint = onViewPoint;
	}

	public void Flash(Color flashColor, Action callback = null, float fadeIn = .3f, float wait = .01f, float maxAlpha = .8f)
	{
		StartCoroutine(performFlash(flashColor, fadeIn, wait, maxAlpha, callback));
    }

	private IEnumerator performFlash(Color color, float fadeIn, float wait, float maxAlpha, Action callback = null)
	{
		StartCoroutine(Fade(0, maxAlpha, fadeIn, color));
		yield return new WaitForSeconds(fadeIn + wait);

		if (callback != null)

		{
			callback();
		}
		StartCoroutine(Fade(maxAlpha, 0, fadeIn, color));
	}
}
