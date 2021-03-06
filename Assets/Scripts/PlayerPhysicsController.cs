﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerPhysicsController : MonoBehaviour, ITickable {
	public float MoveAcceleration = 20f;
	public float AirAcceleration = 5f;

	public Vector2 Dimensions = new Vector2(1, .5f);

	public Vector3 Velocity;
	private Vector3 acceleration;
	public Vector3 Position;
	public float DropAcceleration = 20f;
	public float DropMaxSpeed = 5f;
	public int dropFrames { get; set; }
	public int DropDelayFrames = 30;
	public int DropStunFrames = 20;
	public int DropShakeFrames = 20;
	public float DropShakeMagnitude = 1;

	private int stunFrames = 0;
	public bool IsStunned {  get { return stunFrames > 0; } }

	public Vector3 GroundFriction = new Vector3(20, 2);
	public Vector3 AirFriction = new Vector3(20, 2);

	public Vector3 MaxSpeed = new Vector3(25, 20);
	public float DodgeSpeed = 20;
	public float GroundGravity = -20f;
	public float AirGravity = -20f;
	public float HoverDist = .05f;
	public float RaycastDist = .06f;
	public float JumpSpeed = 20f;
	public float RotateThreshold = .3f;

	public float SnapDist = .2f;

	private float startZ;
	public bool IsDropping = false;

	public bool CanFlip
	{
		get; set;
	}
	public bool CanDrop
	{
		get; set;
	}
	public bool CanRotate
	{
		get; set;
	}
	
	public bool IsGrounded
	{
		get; set;
	}

	public bool Jumped { get; set; }

	public Vector3 Up { get; private set; }
	public Vector3 Right {  get {
			if (!IsGrounded && !CanRotate)
			{
				return new Vector3(1, 0, 0);
			}

			return new Vector3(Up.y, -Up.x, -Up.z); } }

	public void SetUp(Vector3 up) { Up = up; }

	private Collider2D surface = null;
	private Vector2 surfaceNormal;

	private int lastDirectionHeld = 0;

	int groundLayer;
	int rotateGroundLayer;
	int flipGroundLayer;

	int groundLayerMask;
	int rotateGroundLayerMask;
	int flipGroundLayerMask;
	int allTerrainMask;

	public bool DisableGravity = false;

	public bool UncapSpeeds { get; set; }

	private GameObject leftBound;
	private GameObject rightBound;



	private Vector2 lastSafeUp;
	private Vector3 lastSafePos;

	void Awake()
	{
		Velocity = Vector3.zero;
		Up = new Vector3(0, 1, 0);
		Position = transform.position;
		groundLayer = LayerMask.NameToLayer(LayerUtil.GROUND);
		rotateGroundLayer = LayerMask.NameToLayer(LayerUtil.ROTATE_GROUND);
		flipGroundLayer = LayerMask.NameToLayer(LayerUtil.FLIP_GROUND);
		groundLayerMask = LayerUtil.GetLayerMask(LayerUtil.GROUND);
		rotateGroundLayerMask = LayerUtil.GetLayerMask(LayerUtil.ROTATE_GROUND);
		flipGroundLayerMask = LayerUtil.GetLayerMask(LayerUtil.FLIP_GROUND);
		allTerrainMask = groundLayerMask | rotateGroundLayerMask | flipGroundLayerMask;

		startZ = transform.position.z;
	}

	void Start()
	{
		leftBound = GameObject.Find("LeftBound");
		rightBound = GameObject.Find("RightBound");
	}

	public void TickFrame()
	{
		if (!CanRotate && !IsGrounded)
		{
			Up = new Vector3(0, 1, 0);
		}

		if (IsDropping)
		{
			dropFrames--;
			if (dropFrames < 0)
			{
				Velocity.y += -DropAcceleration *FrameController.deltaTime;
			}
		}

		if (stunFrames > 0)
		{
			stunFrames--;
		}

		Vector3 gravityUp = CanRotate ? (Vector2)Up : Vector2.up;


		if (!DisableGravity && !IsDropping)
		{
			if (IsGrounded)
			{
				Velocity.x += GroundGravity * gravityUp.x * FrameController.deltaTime;
				Velocity.y += GroundGravity * gravityUp.y * FrameController.deltaTime;
			}
			else
			{
				Velocity.x += AirGravity * gravityUp.x * FrameController.deltaTime;
				Velocity.y += AirGravity * gravityUp.y * FrameController.deltaTime;
			}
		}

		Velocity.x += acceleration.x * Right.x * FrameController.deltaTime;
		Velocity.y += acceleration.x * Right.y * FrameController.deltaTime;

		float rightDot = Vector3.Dot(Velocity, Right);

		// Cap velocity at max speed
		if (!UncapSpeeds)
		{
			float maxSpeedX = MaxSpeed.x;

			if (rightDot > maxSpeedX)
			{
				float upDot = Vector3.Dot(Velocity, Up);
				Velocity = upDot * Up + (Vector3)(maxSpeedX * Right);
			}
			else if (rightDot < -maxSpeedX)
			{
				float upDot = Vector3.Dot(Velocity, Up);
				Velocity = upDot * Up + (Vector3)(-maxSpeedX * Right);
			}
			rightDot = Vector3.Dot(Velocity, Right); // need to recalculate
			float maxSpeedY = IsDropping ? DropMaxSpeed : MaxSpeed.y;

			if (Vector3.Dot(Up, Velocity) < -maxSpeedY)
			{
				Velocity = rightDot * Right +(Vector3) Up * -maxSpeedY;
			}
		}

		// Recalculate velocity
		rightDot = Vector3.Dot(Velocity, Right);

		// Lose velocity to friction
		if ((lastDirectionHeld == 0 || !MathUtil.SignsMatch(acceleration.x, Vector3.Dot(Velocity, Right)))
			&& Velocity.sqrMagnitude > .01f)
		{
			Vector2 Friction = IsGrounded ? GroundFriction : AirFriction;

			if (rightDot > 0)
			{
				float upDot = Vector3.Dot(Velocity, Up);
				Velocity = upDot * Up + (Vector3)(Mathf.Max(0, rightDot - Friction.x * FrameController.deltaTime) * Right);
			}
			else if (rightDot < 0)
			{
				float upDot = Vector3.Dot(Velocity, Up);
				Velocity = upDot * Up + (Vector3)(Mathf.Min(0, rightDot + Friction.x * FrameController.deltaTime) * Right);
			}
		}

		// Don't check feet if moving up
		if (Vector3.Dot(Velocity, Up) < .1f)
		{
			float raycastDist = Velocity.magnitude * FrameController.deltaTime + RaycastDist;
			RaycastHit2D hit = getHighestRaycastHit(Position, Up * -1, raycastDist, allTerrainMask);
			if (hit.collider != null)
			{
				IsGrounded = snapToSurface(hit, Up * -1);
			}
			else
			{
				IsGrounded = false;

			}
		}
		else
		{
			/*RaycastHit2D hit = Physics2D.Raycast(Position, Up * -1, SnapDist, allTerrainMask);
			if (hit.collider != null)
			{
				snapToSurface(hit, )
			}*/

			IsGrounded = false;
		}

		if (Velocity.sqrMagnitude > .001f)
		{
			movePosition();
		}

		if (Position.x <= leftBound.transform.position.x
			|| Position.x >= rightBound.transform.position.x)
		{
			Velocity = new Vector3(0, -10, 0);
		}
		Position.x = Mathf.Max(leftBound.transform.position.x, Mathf.Min(rightBound.transform.position.x, Position.x));

		Up = new Vector3(Up.x, Up.y, 0);
		Position.z = startZ;

		if (IsGrounded && !GameManager.instance.player.IsInside
			&& !GameManager.instance.player.IsFallingDead
			&& !GameManager.instance.player.IsCollapsed)
		{
			RaycastHit2D hit = Physics2D.Raycast(Position + .02f * Up, -1 * Up, .08f, allTerrainMask);
            if (hit.collider != null)
			{
				lastSafePos = Position;
				lastSafeUp = Up;
			}
		}
	}

	public void Respawn()
	{
		acceleration = Vector3.zero;
		SetVelocity(Vector3.zero);
		SetUp(lastSafeUp);
		Position = lastSafePos;
		IsDropping = false;

	}

	private bool snapToSurface(RaycastHit2D hit, Vector3 velocityNormal)
	{
		bool success = Vector3.Dot(hit.normal, Up) >= RotateThreshold;

		if (hit.distance == 0)
		{
			Debug.LogWarning("Collision with dist 0");
		}


		if (success)
		{
			Jumped = false;


			surface = hit.collider;
			surfaceNormal = hit.normal;

			if ((hit.collider.gameObject.layer == rotateGroundLayer
					|| hit.collider.gameObject.layer == flipGroundLayer)
					&& (CanRotate || Vector2.Dot(hit.normal, Vector2.up) > .3f))
			{
				Up = hit.normal;
			}
			else
			{
				Up = Vector3.up;
			}

			if (IsDropping)
			{
				onDropLand();
			}
			UncapSpeeds = false;

			if (!IsGrounded)
			{
				GameManager.instance.player.OnLand();
				IsGrounded = true;

			}
		}

		Position += velocityNormal * (hit.distance - HoverDist);
		Position.z = startZ;
		// Set position to the collision point
		//position = hit.point;

		//if (CanRotate || Vector2.Dot(hit.normal, Vector2.up) > .4f)
		{
			// Cancel out velocity parallel to the normal
			float dot = Vector3.Dot(Velocity, hit.normal);
			Vector3 normalProjection = dot * hit.normal;
			Velocity -= normalProjection;
		}
		//else
		{
			// Only when not rotating, don't stick into walls
			//Velocity.x = 0;
			//Velocity.y = Mathf.Min(0, Velocity.y);
		}

		acceleration.x = lastDirectionHeld * MoveAcceleration;

		return success;
	}

	private void onDropLand()
	{
		IsDropping = false;
		stunFrames = DropStunFrames;
		GameManager.instance.sounds.player.PlayOneShot(GameManager.instance.sounds.heavyLand);
		GameManager.instance.mainCamera.BeginCameraShake(DropShakeFrames, DropShakeMagnitude);
	}

	private RaycastHit2D getHighestRaycastHit(Vector3 position,
		Vector3 projectNormal,
		float dist,
		int mask)
	{
		RaycastHit2D[] hits = Physics2D.RaycastAll(position, projectNormal, dist, mask);
		RaycastHit2D appliedHit = new RaycastHit2D();
		if (hits.Length> 0)
		{
			return hits[0];
		}

		/*

		float bestDot = 0;
		// Find the 'highest' hit, relative to the ground we are on
		foreach (RaycastHit2D hit in hits)
		{
			Vector3 delta = hit.point - (Vector2)Position;
			float dot = Vector3.Dot(Up, delta);
			if (appliedHit.collider == null
				|| dot > bestDot)
			{
				bestDot = dot;
				appliedHit = hit;
			}
		}*/

		return appliedHit;
	}

	private void movePosition()
	{
		float cachedVelocityMagnitude = Velocity.magnitude;
		float totalDistToTravel = Velocity.magnitude * FrameController.deltaTime;
		float distRemainingToTravel = totalDistToTravel;
		
		int maxIterations = 10;
		while (maxIterations > 0
			&& distRemainingToTravel > 0)
		{
			List<Vector3> startPoints = new List<Vector3>
			{
				Position,
				Position + Up * Dimensions.y
			};
			RaycastHit2D appliedHit = new RaycastHit2D();
			foreach (Vector3 point in startPoints)
			{
				RaycastHit2D hit = getHighestRaycastHit(point, Velocity.normalized, distRemainingToTravel, allTerrainMask);
				if (hit.collider != null)
				{
					appliedHit = hit;
					break;
				}
			}

			if (appliedHit.collider != null && appliedHit.distance == 0)
			{
				Debug.LogWarning("Collision with dist 0");
			}


			if (appliedHit.collider != null)
			{

				distRemainingToTravel -= appliedHit.distance;

				bool didSnap = snapToSurface(appliedHit, Velocity.normalized);

				if (didSnap)
				{
					// Restore the velocity to what it was before (effectively just rotating the velocity to parallel the surface)
					Velocity = Velocity.normalized * cachedVelocityMagnitude;
				}
			}
			else
			{
				// Move the whole way, unimpeded
				Position += Velocity.normalized * distRemainingToTravel;
				distRemainingToTravel = 0;
			}

			maxIterations--;
		}

		if (maxIterations == 0)
		{
			Debug.LogWarning("Caught an infinite loop");
		}
	}

	public Vector3 Center {  get {  return Position + Up * Dimensions.y / 2; ; } }

	public void SetVelocity(Vector3 velocity)
	{
		Velocity = velocity;
	}

	public void Move(int direction)
	{
		// Don't accelerate into walls
		 if (direction != 0 )
		{
			RaycastHit2D wallHit = Physics2D.Raycast(Center, Right * direction, .05f, allTerrainMask);
			if (wallHit.collider != null
				&& Vector2.Dot(wallHit.normal, Right) < -.8f)
			{
				direction = 0;
			}
		}

		if (direction == lastDirectionHeld || IsDropping || IsStunned)
		{
			return;
		}

		lastDirectionHeld = direction;
		float speed = direction * (IsGrounded ? MoveAcceleration : AirAcceleration);
		acceleration.x = speed;
	}

	public bool BeginDrop()
	{
		if (!CanDrop || IsDropping)
		{
			return false;
		}

		if (IsGrounded && Vector2.Dot(Up, Vector2.down) < -.03)
		{
			return false;
		}

		if (IsGrounded)
		{
			Position += Dimensions.y * Up; 

		}

		dropFrames = DropDelayFrames;
		IsDropping = true;
		IsGrounded = false;
		SetUp(Vector3.up);
		SetVelocity(new Vector3(0, -.01f, 0));
		acceleration = Vector3.zero;
		lastDirectionHeld = 0;
		return true;

	}


	public bool Flip()
	{
		if (!CanFlip)
		{
			return false;
		}

		if (surface != null
			&& surface.gameObject.layer == flipGroundLayer)
		{
			RaycastHit2D obstructedHit = Physics2D.Raycast(transform.position, -Up, .3f, groundLayerMask | rotateGroundLayerMask);
			if (obstructedHit.collider == null)
			{


				Vector3 newPos = transform.position + Up * -.3f;
				newPos.z = startZ;
				Position = newPos;
				transform.position = newPos;
				Up *= -1;
				acceleration.x *= -1;

				return true;
			}
			else
			{
				return false;
			}

		}

		return false;

	}

	void OnDrawGizmos()
	{
		Gizmos.DrawSphere(Position, .1f);
		GizmoUtil.GizmosDrawArrow(Position, Position + Velocity * .2f, Color.yellow);

		GizmoUtil.GizmosDrawArrow(Position, Position + Up, Color.white);

		Gizmos.color = Color.red;
		Gizmos.DrawLine(Position, Position + Up * Dimensions.y);
	}
}
