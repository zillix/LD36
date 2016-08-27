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

	private Camera mainCamera;

	private float cameraShakeMagnitude;
	private float cameraShakeFramesRemaining;

	public Image CameraFlash;

	private ColorFilter filter;



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
	}

	public void SetFilterColor(Color color)
	{
		filter.filter = color;
	}

	// Update is called once per frame
	public void TickFrame()
	{

		int cullingMask = -1;

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
		
	}

	private Vector3 calculateTargetPosition()
	{
		Vector3 targetPosition = player.transform.position;

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

	public void Flash(Color flashColor, Action callback = null)
	{
		StartCoroutine(performFlash(flashColor, .3f, .01f, 1f, callback));
    }

	private IEnumerator performFlash(Color color, float fadeIn, float wait, float maxAlpha, Action callback = null)
	{
		StartCoroutine(Fade(0, 0.8f, fadeIn, color));
		yield return new WaitForSeconds(fadeIn + wait);

		if (callback != null)

		{
			callback();
		}
		StartCoroutine(Fade(0.8f, 0, fadeIn, color));
	}
}
