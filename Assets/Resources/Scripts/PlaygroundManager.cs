using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaygroundManager
{
    private GameObject prefab;

    private GameObject playground;
    public List<Transform> walls;

    private PlaygroundSettings settings;

    public PlaygroundManager(GameObject playground, GameObject wallSegmentPrefab, PlaygroundSettings settings)
    {
        this.playground = playground;
        prefab = wallSegmentPrefab;
        this.settings = settings;
    }

    public void StartPlayground()
    {
        RegisterExistingObstacles();
        SpawnWalls();
    }

    private void SpawnWalls()
    {
        for( float x = -settings.verticalWallOffset; x <= settings.verticalWallOffset; x += settings.wallSegmentSize )
        {
            SpawnVerticalWallPair(x);
        }
        for( float y = -settings.horizontalWallOffset; y <= settings.horizontalWallOffset; y += settings.wallSegmentSize )
        {
            SpawnHorizontalWallPair(y);
        }
    }

    private void SpawnVerticalWallPair(float x)
    {
        Vector3 positionHigh = new(settings.horizontalWallOffset, x, 0f);
        Vector3 positionLow = new(-settings.horizontalWallOffset, x, 0f);

        SpawnWall(positionHigh);
        SpawnWall(positionLow);
    }

    private void SpawnHorizontalWallPair(float y)
    {
        Vector3 positionLeft = new(y, -settings.verticalWallOffset, 0f);
        Vector3 positionRight = new(y, settings.verticalWallOffset, 0f);

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
