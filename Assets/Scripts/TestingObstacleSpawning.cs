using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public class TestingObstacleSpawning : MonoBehaviour
{
    private GameAssets _gameAssets;

    private int _numOfObjects = 10;
    private float _emptyRadius = 2.0f;
    private float _borderMargin = 1.2f;

    void Start()
    {
        _gameAssets = GameAssets.Instance;

        spawnObjectsAtRandomPositions(_gameAssets.ObstacleObject, _numOfObjects, _emptyRadius, _borderMargin);
    }

    private void spawnObjectsAtRandomPositions(Transform objectToSpawn, int numberOfObjects, float emptyRadius = 0.0f, float borderMargin = 1.0f)
    {
        List<Vector2> listOfLocations = Utilities.GetListOfRandom2DLocations(numberOfObjects, emptyRadius, borderMargin);

        for (int i = 0; i < listOfLocations.Count; i++)
        {
            Instantiate(objectToSpawn, listOfLocations[i], Quaternion.identity, null);
        }
    }
}
