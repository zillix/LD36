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

	public float ScaleSize = 2f;

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
		if (IsOutside
			&& Physics.IsGrounded
			&& input.GetButtonDown(Button.Up))
		{
			Physics.SetVelocity(Physics.Velocity + Physics.Up * Physics.JumpSpeed);
		}

		if (!Physics.IsDodging
			&& Physics.IsGrounded
				&& input.GetButtonDown(Button.Flip))
		{
			Physics.Flip();
			facing = facing == Direction.Left ? Direction.Right : Direction.Left;
			Side = IsInside ? Side.Outside : Side.Inside;
		}
    }
	
	// Update is called once per frame
	public void TickFrame () {

		bool invertControls = IsInside;

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

		animator.SetFloat("Speed", Mathf.Abs(Physics.Velocity.magnitude));

		transform.position = Physics.Position;

		Quaternion rotation = Quaternion.Euler(0, 0, MathUtil.VectorToAngle(RotationUp) - 90);
		transform.rotation = rotation;
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
			return Physics.Up;
		}
	}
}
