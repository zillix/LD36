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

		Vector2 furthestPoint = self.points[0];
		foreach (Vector2 point in self.points)
		{
			if (Vector2.Distance(center, point) > Vector2.Distance(center, furthestPoint))
			{
				furthestPoint = point;
			}
		}

		Vector2[] newPoints = new Vector2[self.points.Length];
		for (int i = 0; i < self.points.Length; ++i)
		{
			newPoints[i] = self.points[i] - furthestPoint;
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
	public static void Align(this EdgeCollider2D self, float grid)
	{
		if (self.points.Length == 0)
		{
			return;
		}

		Vector2[] newPoints = new Vector2[self.points.Length];
		for (int i = 0; i < self.points.Length; ++i)
		{
			Vector2 point = self.points[i];
			int mod = (int)(point.x % grid);
			point.x = mod * grid + Mathf.RoundToInt((point.x - mod) / (float)grid) * grid;
			mod = (int)(point.y % grid);
			point.y = mod * grid + Mathf.RoundToInt((point.y - mod) / (float)grid) * grid;
			newPoints[i] = point;
		}

		self.points = newPoints;
	}
}
