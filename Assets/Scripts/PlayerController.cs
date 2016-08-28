using UnityEngine;
using System.Collections;

public enum Direction
{
	None,
	Left,
	Right
}

public enum Side
{
	None,
	Inside,
	Outside
}

[RequireComponent(typeof(PlayerPhysicsController))]
public class PlayerController : MonoBehaviour, ITickable {

	private InputController input = new InputController();
	public PlayerPhysicsController Physics { get; private set; }
	private Animator animator;

	private Renderer myrenderer;

	private SoundBank sounds;

	public Direction facing = Direction.Right;

	public bool IsInside { get { return Side == Side.Inside; } }
	public bool IsOutside { get { return Side == Side.Outside; } }
	public Side Side { get; private set; }

	private float currentAngle = 0;

	public float RotationSpeed = 360;

	public float ScaleSize = 2f;

	public int DeathFallFrames = 60;
	public int ContinueDeathFallFrames = 20;
	public int CollapseFrames = 60;
	public int DropDeathFrames = 60;
	private int fallingFrames = 0;

	private int collapsedFrames = 0;
	public bool IsCollapsed { get; set; }

	public bool IsFallingDead {
		get; set; }

	public bool DisableJumping { get; set; }

	private bool temporarilyInvertingControls = false;

	void Awake()
	{
		Side = Side.Outside;
		Physics = GetComponent<PlayerPhysicsController>();
		animator = GetComponent<Animator>();
		myrenderer = GetComponentInChildren(typeof(Renderer)) as Renderer;
		sounds = GameObject.Find("SoundBank").GetComponent<SoundBank>();
		GameManager.instance.player = this;
	}

	// Use this for initialization
	void Start () {

		GameManager.instance.player = this;
	}

	void Update()
	{
		if (GameManager.DEBUG)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				GameManager.instance.mainCamera.Flash(Color.red);
			}

			if (Input.GetKeyDown(KeyCode.R))
			{
				respawn();
			}

			if (Input.GetKeyDown(KeyCode.T))
			{
				GameManager.instance.mainCamera.Rotate = !GameManager.instance.mainCamera.Rotate;
			}

			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				Physics.CanRotate = !Physics.CanRotate;
			}

			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				Physics.CanFlip = !Physics.CanFlip;
			}

			if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				Physics.CanDrop = !Physics.CanDrop;
			}

			if (Input.GetKeyDown(KeyCode.J))
			{
				Physics.JumpSpeed *= 2f;
			}
			if (Input.GetKeyDown(KeyCode.K))
			{
				Physics.JumpSpeed *= .5f;
			}


		}

		if (IsOutside
			&& !DisableJumping
			&& !IsCollapsed
			&& !IsFallingDead
			&& !Physics.IsStunned
			&& (Physics.IsGrounded ||
				(!Physics.Jumped && Vector3.Dot(Physics.Velocity, Physics.Up * -1) > 0
					&& Vector3.Dot(Physics.Velocity, Physics.Up * -1) < 2f))
			&& input.GetButtonDown(Button.Up))
		{
			sounds.player.PlayOneShot(sounds.jump);
			Physics.SetVelocity(RotationUp * Physics.JumpSpeed);
			Physics.IsGrounded = false;

			Physics.Jumped = true;
		}

		bool didFlip = false;
		if (Physics.IsGrounded
				&& input.GetButtonDown(Button.Flip))
		{
			didFlip = Physics.Flip();

		}


		if (didFlip)
		{

			sounds.player.PlayOneShot(sounds.flip);
			temporarilyInvertingControls = true;
			rotate(10000);
			facing = facing == Direction.Left ? Direction.Right : Direction.Left;
			Side = IsInside ? Side.Outside : Side.Inside;

		}
		else if (input.GetButtonDown(Button.Flip) && !IsInside)
		{
			bool didDrop = Physics.BeginDrop();
			if (didDrop)
			{
				fallingFrames = 0;
				rotate(10000);
				sounds.player.PlayOneShot(sounds.drop);

			}

		}
	}
	
	// Update is called once per frame
	public void TickFrame () {

		bool invertControls = temporarilyInvertingControls;

		if (!Physics.IsDropping && !IsCollapsed
			&& GameManager.instance.hasStartedGame)
		{
			if (!Physics.IsStunned)
			{
				if (input.GetButton(Button.Left))
				{
					Physics.Move(invertControls ? 1 : -1);
					facing = invertControls ? Direction.Right : Direction.Left;
				}
				else if (input.GetButton(Button.Right))
				{
					Physics.Move(invertControls ? -1 : 1);
					facing = invertControls ? Direction.Left : Direction.Right;
				}
				else
				{
					temporarilyInvertingControls = false;
					Physics.Move(0);
				}
			}
			else
			{
				Physics.Move(0);
			}
		}
		else
		{
			Physics.Move(0);
		}

		if (facing == Direction.Left)
		{
			transform.localScale = new Vector3(-ScaleSize, ScaleSize, ScaleSize);
		}
		else
		{
			transform.localScale = new Vector3(ScaleSize, ScaleSize, ScaleSize);
		}

		Physics.TickFrame();

		animator.SetFloat("Speed", Mathf.Abs(Vector3.Dot(Physics.Velocity, Physics.Right)));

		float fallSpeed = 0;
		if (Vector3.Dot(Physics.Up, Physics.Velocity) < -.1)
		{
			fallSpeed = 1;
		}
		animator.SetFloat("FallSpeed", fallSpeed);
		animator.SetBool("Collapsing", IsCollapsed);
		animator.SetBool("FallToDeath", IsFallingDead);
		animator.SetBool("Jumped", !Physics.IsGrounded);

		transform.position = Physics.Position;

		rotate(RotationSpeed);

		if (Physics.IsGrounded)
		{
			fallingFrames = 0;
		}
		else if (Vector3.Dot(Physics.Velocity, Physics.Up) < 0
			&& (!Physics.IsDropping || Physics.dropFrames <= 0))
		{
			int deathFallFrames = Physics.IsDropping ? DropDeathFrames : DeathFallFrames;

			fallingFrames++;
			if (fallingFrames >= deathFallFrames)
			{
				if (!IsFallingDead)
				{
					IsFallingDead = true;
					sounds.player.PlayOneShot(sounds.fallToDeath);

				}
				if (fallingFrames >= deathFallFrames + ContinueDeathFallFrames)
				{
					respawn();
				}
			}
		}

		if (IsCollapsed)
		{
			collapsedFrames++;
			if (collapsedFrames >= CollapseFrames)
			{
				respawn();
			}
		}
	}

	public void OnLand()
	{
		if (IsFallingDead)
		{
			Collapse();
			GameManager.instance.mainCamera.BeginCameraShake(
				Physics.DropShakeFrames,
				Physics.DropShakeMagnitude);

			sounds.player.PlayOneShot(sounds.heavyLand);
		}
		else if (GameManager.instance.hasStartedGame)
		{
			sounds.player.PlayOneShot(sounds.land);
		}
	}

	public void Collapse()
	{
		IsCollapsed = true;
		
	}

	private void rotate(float speed)
	{
		float targetAngle = MathUtil.VectorToAngle(RotationUp) - 90;
		currentAngle = MathUtil.RotateAngle(currentAngle, targetAngle, speed * Time.fixedDeltaTime);


		Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
		transform.rotation = rotation;
	}

	private void respawn()
	{
		Physics.Respawn();
		Side = Side.Outside;
		fallingFrames = 0;
		collapsedFrames = 0;
		fallingFrames = 0;
		IsFallingDead = false;
		IsCollapsed = false;
		sounds.player.PlayOneShot(sounds.respawn);
	}

	public Vector3 RotationUp
	{
		get
		{
			/*Vector3 up = transform.position.normalized;
			if (IsInside)
			{
				up *= -1;
			}
			return up;*/

			if (!Physics.CanRotate)
			{
				return Vector2.up;
			}
			return Physics.Up;
		}
	}

	public void CollectPowerUp(PowerUpType type)
	{

		sounds.player.PlayOneShot(sounds.collectPowerUp);
		switch (type)
		{
			case PowerUpType.Rotate:
				Physics.CanRotate = true;
				break;
			case PowerUpType.Flip:
				Physics.CanFlip = true;
				break;
			case PowerUpType.Drop:
				Physics.CanDrop = true;
				break;
		}

		GameManager.instance.mainCamera.Flash(Color.white);
	}
}
