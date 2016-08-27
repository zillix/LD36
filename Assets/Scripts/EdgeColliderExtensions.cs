using UnityEngine;
using System.Collections;

public static class EdgeColliderExtensions
{
	public static void CenterPoints(this EdgeCollider2D self)
	{
		if (self.points.Length == 0)
		{
			return;
		}

		Vector2 center = new Vector2();
		foreach (Vector2 point in self.points)
		{
			center += point;
		}
		center /= self.points.Length;

		Vector2[] newPoints = new Vector2[self.points.Length];
		for (int i = 0; i < self.points.Length; ++i)
		{
			newPoints[i] = self.points[i] - center;
		}

		self.points = newPoints;
	}


	public static void RotatePoints(this EdgeCollider2D self, float degrees)
	{
		if (self.points.Length == 0)
		{
			return;
		}

		Vector2[] newPoints = new Vector2[self.points.Length];
		for (int i = 0; i < self.points.Length; ++i)
		{
			newPoints[i] = Quaternion.Euler(0, 0, degrees) * self.points[i];
		}

		self.points = newPoints;
	}

	public static void Rescale(this EdgeCollider2D self, float rescale)
	{
		if (self.points.Length == 0)
		{
			return;
		}

		Vector2[] newPoints = new Vector2[self.points.Length];
		for (int i = 0; i < self.points.Length; ++i)
		{
			newPoints[i] = self.points[i] * rescale;
		}

		self.points = newPoints;
	}

	public static void Flip(this EdgeCollider2D self, bool horizontal, bool vertical)
	{
		if (self.points.Length == 0)
		{
			return;
		}

		Vector2[] newPoints = new Vector2[self.points.Length];
		for (int i = 0; i < self.points.Length; ++i)
		{
			Vector2 point = self.points[i];
			newPoints[i] = new Vector2(horizontal ? -point.x : point.x, vertical ? -point.y : point.y);
		}

		self.points = newPoints;
	}
}
