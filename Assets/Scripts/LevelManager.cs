using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ObjectListType
{
    Character,
    Obstacle,
}

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;

    public static LevelManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public Action OnLevelLoad;
    public Action<int, SpeedDistanceDependance> OnCharacterLevelUp;

    private Camera _mainCamera;

    [SerializeField] private int _levelNumber = 1;

    [Space]
    [SerializeField] private int _initialNumOfObstacles = 1;
    [SerializeField] private int _initialNumOfCharacters = 2;

    private int _numOfObstacles;
    private int _numOfCharacters;

    private List<Transform> _obstaclesList = new List<Transform>();
    private List<Transform> _charactersList = new List<Transform>();
    private Transform _affiliationTransform;

    [Space]
    [SerializeField] private LayerMask _obstacleLayerMask;
    [SerializeField] private LayerMask _characterLayerMask;

    private float _borderScaling = 0.95f;
    private float _objectMinDistance = 1.0f;

    private bool _fadeCharacters = false;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        Character.OnCharacterDestroyed += checkLevelCompletion;
        Obstacle.OnObstacleDestroy += removeObstacleFromList;

        _mainCamera = Camera.main;

        _numOfCharacters = _initialNumOfCharacters;
        _numOfObstacles = _initialNumOfObstacles;

        initializePlayground();

        InvokeRepeating("fadeCharacter", UnityEngine.Random.Range(0.5f, 2.0f), UnityEngine.Random.Range(0.5f, 1.5f));
    }

    private void OnDestroy()
    {
        Character.OnCharacterDestroyed -= checkLevelCompletion;
        Obstacle.OnObstacleDestroy -= removeObstacleFromList;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            initializePlayground();
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            //TESTING
            Transform randomCharacterTransform = _charactersList[UnityEngine.Random.Range(0, _charactersList.Count)];
            randomCharacterTransform.GetComponent<IDamagable>().DamageThis();
        }
    }

    private void initializePlayground()
    {
        foreach(ObjectListType objectList in System.Enum.GetValues(typeof(ObjectListType)))
        {
            if (GetObjectList(objectList) != null)
            {
                foreach (Transform spawnedObject in GetObjectList(objectList))
                {
                    Destroy(spawnedObject.gameObject);
                }

                GetObjectList(objectList).Clear();
            }
        }

        if (_affiliationTransform != null)
            Destroy(_affiliationTransform.gameObject);

        initializeObjects(GameAssets.Instance.ObstacleObject, _numOfObstacles, _obstacleLayerMask, _obstaclesList);
        initializeObjects(GameAssets.Instance.CharacterObject, _numOfCharacters, _characterLayerMask, _charactersList);

        if (AlpacaUtils.ChanceFunc(90))
            _affiliationTransform = spawnObject(GameAssets.Instance.AffiliationTrigger);
    }

    private void initializeObjects(Transform objectTransform, int numOfObjects, LayerMask layerToAvoid, List<Transform> listToStoreObjects = null)
    {
        if (objectTransform == null || numOfObjects == 0)
            return;

        for (int i = 0; i < numOfObjects; i++)
        {
            Transform objectToSpawn = spawnObject(objectTransform);

            if (listToStoreObjects != null)
                listToStoreObjects.Add(objectToSpawn);
            
            if (checkObjectEnvironment(objectToSpawn, _objectMinDistance, layerToAvoid))
            {
                if (listToStoreObjects != null)
                    listToStoreObjects.Remove(objectToSpawn);

                Destroy(objectToSpawn.gameObject);
                i--;
                continue;
            }

            Character character = objectToSpawn.GetComponent<Character>();
            if (character != null)
            {
                if (i == (numOfObjects - 1) && getCharacterTypeAmount(CharacterType.Negative) == 0)
                    character.AssignCharacterType(CharacterType.Negative);
            }
        }
    }

    private Transform spawnObject(Transform objectTransform)
    {
        Vector2 position = _mainCamera.ScreenToWorldPoint(AlpacaUtils.GetRandomScreenPosition(_borderScaling));
        return Instantiate(objectTransform, position, Quaternion.identity);
    }

    private bool checkObjectEnvironment(Transform objectToCheck, float environmentRadius, LayerMask objectsToAvoid)
    {
        float scale = objectToCheck.localScale.x;

        Collider2D[] hits = Physics2D.OverlapCircleAll(objectToCheck.position, environmentRadius * scale, objectsToAvoid);

        if (hits.Length > 1)
            return true;
        else
            return false;
    }

    public List<Transform> GetObjectList(ObjectListType listType)
    {
        switch (listType)
        {
            case ObjectListType.Character:
                if (_charactersList.Count != 0)
                    return _charactersList;
                else
                    return null;
            case ObjectListType.Obstacle:
                if (_obstaclesList.Count != 0)
                    return _obstaclesList;
                else
                    return null;
        }

        return null;
    }

    private void reloadGame()
    {
        //killed all good guys - loss
        _levelNumber = 1;
        _fadeCharacters = false;
        OnLevelLoad?.Invoke();

        _numOfCharacters = _initialNumOfCharacters;
        _numOfObstacles = _initialNumOfObstacles;

        initializePlayground();

        Debug.Log("Reload Level");
    }

    private void loadNewLevel()
    {
        _levelNumber++;

        //increase the number of obstacles and characters
        alterLevelObstaclesAndCharactersPerLevel(_levelNumber);

        //black screen

        //re-initialize level
        OnLevelLoad?.Invoke();
        initializePlayground();

        alterCharactersPerLevel(_levelNumber);

        Debug.Log("Next level!");
    }

    private void alterLevelObstaclesAndCharactersPerLevel(int levelNumber)
    {
        if (levelNumber <= 3)
        {
            _numOfCharacters += 1;
        }
        else if (levelNumber > 3 && levelNumber <= 7)
        {
            _numOfCharacters += 1;
            _numOfObstacles += 1;

            _objectMinDistance *= 0.95f;
        }
        else if (levelNumber > 7 && levelNumber <= 12)
        {
            _numOfObstacles += 1;

            _objectMinDistance *= 0.9f;
        }
    }

    private void alterCharactersPerLevel(int levelNumber)
    {
        if (levelNumber >= 2 && levelNumber < 3)
        {
            Debug.Log("Increase the characters' speed");
            _fadeCharacters = false;
            OnCharacterLevelUp?.Invoke(20, SpeedDistanceDependance.Medium);
        }
        else if (levelNumber >= 3 && levelNumber < 4)
        {
            Debug.Log("Activate fading option on characters");
            _fadeCharacters = true;
            OnCharacterLevelUp?.Invoke(0, SpeedDistanceDependance.Medium);
        }
        else if (levelNumber >= 4)
        {
            Debug.Log("Increase the speed and activate fading on characters");
            _fadeCharacters = true;
            OnCharacterLevelUp?.Invoke(40, SpeedDistanceDependance.High);
        }
        else
            return;
    }

    private void checkLevelCompletion(Character characterDestroyed)
    {
        Debug.Log(characterDestroyed.GetComponent<AffiliationTrigger>() == null);

        _charactersList.Remove(characterDestroyed.transform);

        int badCharactersNum = getCharacterTypeAmount(CharacterType.Negative);
        int goodCharactersNum = getCharacterTypeAmount(CharacterType.Positive);

        if (goodCharactersNum > 0)
        {
            if (badCharactersNum == 0)
                loadNewLevel();
            else
                return;
        }

        if (goodCharactersNum == 0)
        {
            if (characterDestroyed.GetCharacterType().Equals(CharacterType.Positive))
                reloadGame();
            else
            {
                if (badCharactersNum == 0)
                    loadNewLevel();
                else
                    return;
            }
        }
    }

    private int getCharacterTypeAmount(CharacterType type)
    {
        int characterTypeNumber = 0;

        foreach (Transform charTransform in _charactersList)
        {
            Character character = charTransform.GetComponent<Character>();
            if (character.GetCharacterType().Equals(type))
                characterTypeNumber++;
        }

        return characterTypeNumber;
    }

    private void removeObstacleFromList(Obstacle obstacle)
    {
        if (obstacle != null)
            _obstaclesList.Remove(obstacle.transform);
    }

    public void SetFadeCharacters(bool value)
    {
        _fadeCharacters = value;
    }

    private void fadeCharacter()
    {
        if (_fadeCharacters)
        {
            Transform characterTransform = _charactersList[UnityEngine.Random.Range(0, _charactersList.Count)];
            FadeObject fadeObject = characterTransform.GetComponent<FadeObject>();

            if (fadeObject != null)
                fadeObject.ActivateFade(!fadeObject.IsInvisible());
        }
    }
}