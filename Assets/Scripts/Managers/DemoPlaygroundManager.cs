using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public class DemoPlaygroundManager : MonoBehaviour
{
    private int _demoCharactersNumber = 10;
    private int _demoObstaclesNumber = 5;

    private GameAssets _gameAssets;

    private List<Transform> _characterList = new List<Transform>();
    private List<Transform> _obstacleList = new List<Transform>();
    private List<Transform> _bulletMarksList = new List<Transform>();
    private Transform _affiliationTrigger;
    private Transform _obstacleDestroyer;

    private float _borderScaling = 0.95f;

    private float _timer = 1.0f;

    private bool _isRunning = true;

    private void Start()
    {
        _gameAssets = GameAssets.Instance;

        LevelManager.Instance.OnInitializeGame += stopPlayground;
        GameManager.Instance.OnQuitToMainMenu += runPlayground;

        createDemoPlayground();
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnInitializeGame -= stopPlayground;
        GameManager.Instance.OnQuitToMainMenu -= runPlayground;
    }

    private void Update()
    {
        if (_isRunning)
            playgroundProcedure();
    }

    private void playgroundProcedure()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0.0f)
        {
            if (_characterList.Count == 0)
                createDemoPlayground();
            else
                destroyRandomCharacter();

            _timer = Random.Range(0.3f, 1.5f);
        }
    }

    private void runPlayground()
    {
        SetRunning(true);
        createDemoPlayground();
    }

    private void stopPlayground()
    {
        SetRunning(false);
        clearPlayground();
    }

    private void destroyRandomCharacter()
    {
        int chanceToHitCharacter = 75;

        if (Utilities.ChanceFunc(chanceToHitCharacter))
        {
            //destroy one non-special character

            Transform randomCharacter;
            if (_characterList.Count > 1)
            {
                int randomIndex = Random.Range(0, _characterList.Count - 1);
                randomCharacter = _characterList[randomIndex];
            }
            else
                randomCharacter = _characterList[0];

            if (randomCharacter != null)
            {
                Vector3 characterPosition = randomCharacter.transform.position;
                _bulletMarksList.Add(spawnObject(_gameAssets.BulletMark, characterPosition));

                Character character = randomCharacter.GetComponent<Character>();

                _characterList.Remove(randomCharacter);
                character.DamageThis();
            }            
        }
        else
        {
            //destroy one special character

            if (Utilities.ChanceFunc(50))
                if (_affiliationTrigger != null)
                    _affiliationTrigger.GetComponent<Character>().DamageThis();
            else
                if (_obstacleDestroyer != null)
                    _obstacleDestroyer.GetComponent<ObstacleDestroyer>().DamageThis();
        }
    }

    private void createDemoPlayground()
    {
        clearPlayground();

        instantiateObjects(_gameAssets.CharacterObject, _demoCharactersNumber, _characterList);
        instantiateObjects(_gameAssets.ObstacleObject, _demoObstaclesNumber, _obstacleList);

        if (Utilities.ChanceFunc(50))
            _affiliationTrigger = spawnObject(_gameAssets.AffiliationTrigger);

        if (Utilities.ChanceFunc(50))
            _obstacleDestroyer = spawnObject(_gameAssets.ObstacleDestroyer);
    }

    private void clearPlayground()
    {
        foreach (Transform character in _characterList)
        {
            if (character != null)
                Destroy(character.gameObject);
        }

        foreach (Transform obstacle in _obstacleList)
        {
            if (obstacle != null)
                Destroy(obstacle.gameObject);
        }

        foreach (Transform bulletMark in _bulletMarksList)
        {
            if (bulletMark != null)
                Destroy(bulletMark.gameObject);
        }

        _characterList.Clear();
        _obstacleList.Clear();
        _bulletMarksList.Clear();

        if (_affiliationTrigger != null)
            Destroy(_affiliationTrigger.gameObject);

        if (_obstacleDestroyer != null)
            Destroy(_obstacleDestroyer.gameObject);
    }

    private void instantiateObjects(Transform objectTransform, int numberOfObjects, List<Transform> storingList = null)
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            Transform spawnedObject = spawnObject(objectTransform);

            if (storingList != null)
                storingList.Add(spawnedObject);

            LayerMask layerMask = 9999;
            if (Utilities.CheckObjectEnvironment(spawnedObject, 1.0f, layerMask))
            {
                if (storingList != null)
                    storingList.Remove(spawnedObject);

                Destroy(spawnedObject.gameObject);
                i--;
                continue;
            }
        }
    }

    private Transform spawnObject(Transform objectTransform, Transform parent = null)
    {
        Vector2 position = Camera.main.ScreenToWorldPoint(Utilities.GetRandomScreenPosition(_borderScaling));
        return spawnObject(objectTransform, position, parent);
    }

    private Transform spawnObject(Transform objectTransform, Vector3 position, Transform parent = null)
    {
        return Instantiate(objectTransform, position, Quaternion.identity, parent);
    }

    public bool IsRunning()
    {
        return _isRunning;
    }

    public void SetRunning(bool value)
    {
        _isRunning = value;
    }
}
