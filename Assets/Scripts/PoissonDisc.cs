using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PoissonDiscSampling 
{
	//technique for generating tightly packed points with minimum distance away from another

	public static List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30)
	{
		float cellSize = radius / Mathf.Sqrt(2);

		int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)]; //to know how many cells on the x and y axes, it tells for each cell what the index of the point in the point list that in that cell
		List<Vector2> points = new List<Vector2>(); // to hold all of the generated points
		List<Vector2> spawnPoints = new List<Vector2>(); //  

		spawnPoints.Add(sampleRegionSize / 2); // adding a starting point as a spawn point in the middle
		while (spawnPoints.Count > 0)          //when spawn point list is not empty
		{
			int spawnIndex = Random.Range(0, spawnPoints.Count); //pick a random spawn point
			Vector2 spawnCentre = spawnPoints[spawnIndex];   // create new point some way around the spawn center, if it fails remove from the list          
			bool candidateAccepted = false;  

			for (int i = 0; i < numSamplesBeforeRejection; i++) 
			{
				float angle = Random.value * Mathf.PI * 2; //creating random angle
				Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)); // creating vector2 direction
				Vector2 candidate = spawnCentre + dir * Random.Range(radius, 2 * radius); // creating candidate point
				if (IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid)) 
				{
					points.Add(candidate);
					spawnPoints.Add(candidate);
					grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;     // if candidate point accepted
					candidateAccepted = true;
					break;
				}
			}
			if (!candidateAccepted)
			{
				spawnPoints.RemoveAt(spawnIndex);

			}

		}

		return points; // if it is not valid return to the points list
	}

	static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
	{
		if (candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y) 
		{
			int cellX = (int)(candidate.x / cellSize);
			int cellY = (int)(candidate.y / cellSize);
			int searchStartX = Mathf.Max(0, cellX - 2); // creating a start point on the x axes ,that can never go below 0 (Out of Bounds)
			int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1); // end point on x axes
			int searchStartY = Mathf.Max(0, cellY - 2); // creating a start point on the y axes,that can never go below 0.
			int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1); // end point on y axes

			for (int x = searchStartX; x <= searchEndX; x++) // searching start point for x
			{
				for (int y = searchStartY; y <= searchEndY; y++) // searching start point for y
				{
					int pointIndex = grid[x, y] - 1; 
					if (pointIndex != -1) // if the point index equals negative, that means there is no point in that cell
					{
						float sqrDst = (candidate - points[pointIndex]).sqrMagnitude; //when there is no point ,calculate distance between candidate point and that point 
						if (sqrDst < radius * radius) 
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		return false;
	}
}