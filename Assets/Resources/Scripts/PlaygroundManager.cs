using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaygroundManager
{
    private GameObject prefab;

    private GameObject playground;
    public List<Transform> walls = new();

    private readonly float horizontalWallOffset;
    private readonly float verticalWallOffset;
    private readonly float wallSegmentSize;

    public PlaygroundManager(GameObject playground, GameObject wallSegmentPrefab, SimulationSettings settings)
    {
        this.playground = playground;
        prefab = wallSegmentPrefab;
        horizontalWallOffset = settings.horizontalWallOffset;
        verticalWallOffset = settings.verticalWallOffset;
        wallSegmentSize = settings.wallSegmentSize;
    }

    public void StartPlayground()
    {
        RegisterExistingObstacles();
        SpawnWalls();
    }

    private void SpawnWalls()
    {
        for( float x = -verticalWallOffset; x <= verticalWallOffset; x += wallSegmentSize )
        {
            SpawnVerticalWallPair(x);
        }
        for( float y = -horizontalWallOffset; y <= horizontalWallOffset; y += wallSegmentSize )
        {
            SpawnHorizontalWallPair(y);
        }
    }

    private void SpawnVerticalWallPair(float x)
    {
        Vector3 positionHigh = new(horizontalWallOffset, x, 0f);
        Vector3 positionLow = new(-horizontalWallOffset, x, 0f);

        SpawnWall(positionHigh);
        SpawnWall(positionLow);
    }

    private void SpawnHorizontalWallPair(float y)
    {
        Vector3 positionLeft = new(y, -verticalWallOffset, 0f);
        Vector3 positionRight = new(y, verticalWallOffset, 0f);

        SpawnWall(positionLeft);
        SpawnWall(positionRight);
    }

    private void SpawnWall(Vector3 position)
    {
        Transform wall = GameObject.Instantiate(prefab, position, Quaternion.identity, playground.transform).transform;
        walls.Add(wall);
    }

    private void RegisterExistingObstacles()
    {
        foreach(Transform obj in playground.transform)
        {
            walls.Add(obj);
        }
    }
}
