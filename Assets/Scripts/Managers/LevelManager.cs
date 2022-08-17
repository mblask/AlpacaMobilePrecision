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

    public Action OnInitializeGame;
    public Action<float> OnBeforeLoadLevel;
    public Action<int> OnLoadLevel;
    public Action OnGameReload;
    public Action<CharacterLevelUpProperties> OnCharacterLevelUp;
    public Action<float> OnActivateTimer;
    public Action<float> OnActivateAccuracy;
    public Action OnGrabPlayerAccuracy;

    private Camera _mainCamera;

    [Header("For testing")]
    [SerializeField] private bool _initializeGame = true;
    [SerializeField] bool _spawnSingleCharacter = false;
    [SerializeField] bool _spawnAffiliationTrigger = false;
    [SerializeField] private bool _charactersSpawnNewCharacters = false;

    [Header("Initial game settings")]
    [SerializeField] private int _levelNumber = 1;

    [Space]
    [SerializeField] private int _initialNumOfObstacles = 1;
    [SerializeField] private int _initialNumOfCharacters = 2;

    private int _numOfObstacles;
    private int _numOfCharacters;

    private List<Transform> _obstaclesList = new List<Transform>();
    private List<Transform> _charactersList = new List<Transform>();
    
    private bool _initializeAffiliationTrigger = false;
    private Transform _affiliationTransform;

    private GameAssets _gameAssets;

    [Space]
    [SerializeField] private LayerMask _obstacleLayerMask;
    [SerializeField] private LayerMask _characterLayerMask;

    private float _borderScaling = 0.95f;
    private float _objectMinDistance = 1.0f;

    private bool _fadeCharacters = false;
    private float _accuracyRequiredToPassLevel = 0.0f;
    private float _currentAccuracy = 0.0f;

    private bool _gameRunning = true;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        Character.OnCharacterDestroyed += checkLevelCompletion;
        Obstacle.OnObstacleDestroy += removeObstacleFromList;
        CharacterSpawning.OnCharacterSpawn += addCharacterToList;
        HitManager.Instance.OnSendPlayerAccuracy += getPlayerAccuracy;
        GameManager.Instance.OnQuitToMainMenu += ClearGame;
        GameManager.Instance.OnGameOver += resetGameSettings;

        _mainCamera = Camera.main;
        _gameAssets = GameAssets.Instance;

        _numOfCharacters = _initialNumOfCharacters;
        _numOfObstacles = _initialNumOfObstacles;

        if (_initializeGame)
            InitializeGame();

        //Testing purposes
        if (_spawnSingleCharacter)
            initializeObjects(_gameAssets.CharacterObject, 1, _characterLayerMask);

        //if (!_initializeGame && !_spawnSingleCharacter)
        //    InitializeGame();
    }

    private void OnDestroy()
    {
        Character.OnCharacterDestroyed -= checkLevelCompletion;
        Obstacle.OnObstacleDestroy -= removeObstacleFromList;
        CharacterSpawning.OnCharacterSpawn -= addCharacterToList;
        HitManager.Instance.OnSendPlayerAccuracy -= getPlayerAccuracy;
        GameManager.Instance.OnQuitToMainMenu += ClearGame;
        GameManager.Instance.OnGameOver -= resetGameSettings;
    }

    private void Update()
    {        
        if (Input.GetMouseButtonDown(1))
        {
            //TESTING
            if (_charactersList.Count != 0)
            {
                Transform randomCharacterTransform = _charactersList[UnityEngine.Random.Range(0, _charactersList.Count)];
                randomCharacterTransform.GetComponentInChildren<IDamagable>().DamageThis();
            }
        }
    }

    private void addCharacterToList(Transform characterToAdd)
    {
        _charactersList.Add(characterToAdd);
    }

    //public void ClearGame()
    //{
    //    foreach (Transform transform in _charactersList)
    //    {
    //        Destroy(transform.gameObject);
    //    }
    //
    //    foreach (Transform transform in _obstaclesList)
    //    {
    //        Destroy(transform.gameObject);
    //    }
    //
    //    _charactersList.Clear();
    //    _obstaclesList.Clear();
    //}

    public void ClearGame()
    {
        //cleaning the lists and destroying existing objects
        foreach (ObjectListType objectList in System.Enum.GetValues(typeof(ObjectListType)))
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
    }

    private void resetGameSettings()
    {
        _levelNumber = 1;
        _numOfCharacters = _initialNumOfCharacters;
        _numOfObstacles = _initialNumOfObstacles;
    }

    public void InitializeGame()
    {
        initializePlayground();
        InvokeRepeating(nameof(fadeCharacter), UnityEngine.Random.Range(0.5f, 2.0f), UnityEngine.Random.Range(0.5f, 1.0f));
        OnInitializeGame?.Invoke();
    }

    private void initializePlayground()
    {
        ClearGame();

        //initializing new objects
        initializeObjects(_gameAssets.ObstacleObject, _numOfObstacles, _obstacleLayerMask, _obstaclesList);
        initializeObjects(_gameAssets.CharacterObject, _numOfCharacters, _characterLayerMask, _charactersList);

        if (AlpacaUtils.ChanceFunc(90) && _initializeAffiliationTrigger)
            _affiliationTransform = spawnObject(_gameAssets.AffiliationTrigger);

        //Testing purposes
        if (_spawnAffiliationTrigger)
            _affiliationTransform = spawnObject(_gameAssets.AffiliationTrigger);
        //Testing purposes
        OnCharacterLevelUp?.Invoke(new CharacterLevelUpProperties { PercentageSpeedIncrease = 0, SpeedDistanceDependance = SpeedDistanceDependance.None, CharactersSpawnNewCharacters = _charactersSpawnNewCharacters });
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

            if (i == (numOfObjects - 1) && getCharacterTypeAmount(CharacterType.Negative) == 0)
            {
                Character character = objectToSpawn.GetComponent<Character>();
                if (character != null)
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
        _levelNumber = 1;
        _fadeCharacters = false;

        float timer = 0.0f;
        _accuracyRequiredToPassLevel = 0.0f;

        OnActivateTimer?.Invoke(timer);
        OnActivateAccuracy?.Invoke(_accuracyRequiredToPassLevel);
        OnGameReload?.Invoke();

        _numOfCharacters = _initialNumOfCharacters;
        _numOfObstacles = _initialNumOfObstacles;

        initializePlayground();
    }

    private void beforeLoadNewLevel()
    {
        if (_accuracyRequiredToPassLevel != 0.0f)
        {
            float playerAccuracy = HitManager.GrabPlayerAccuracy();
            OnBeforeLoadLevel?.Invoke(playerAccuracy);
        }
    }

    private void loadNewLevel()
    {
        _levelNumber++;
        OnLoadLevel?.Invoke(_levelNumber);

        //increase the number of obstacles and characters
        alterLevelObstaclesAndCharactersPerLevel(_levelNumber);

        //re-initialize level
        initializePlayground();

        float timer = 0.0f;
        _accuracyRequiredToPassLevel = 0.0f;
        OnActivateTimer?.Invoke(timer);
        OnActivateAccuracy?.Invoke(_accuracyRequiredToPassLevel);

        changeLevelSettings(_levelNumber);
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

    private void changeLevelSettings(int levelNumber)
    {
        if (levelNumber >= 3 && levelNumber < 5)
        {
            //Debug.Log("Increase the characters' speed");
            _fadeCharacters = false;
            _accuracyRequiredToPassLevel = (levelNumber - 1) / 10.0f;
            OnActivateAccuracy?.Invoke(_accuracyRequiredToPassLevel);
            OnCharacterLevelUp?.Invoke(new CharacterLevelUpProperties { PercentageSpeedIncrease = 30, SpeedDistanceDependance = SpeedDistanceDependance.Medium });
        }
        else if (levelNumber >= 5 && levelNumber < 7)
        {
            //Debug.Log("Activate fading option on characters");
            _fadeCharacters = true;
            OnCharacterLevelUp?.Invoke(new CharacterLevelUpProperties { PercentageSpeedIncrease = 0, SpeedDistanceDependance = SpeedDistanceDependance.None });
        }
        else if (levelNumber >= 7 && levelNumber < 9)
        {
            //Debug.Log("Activate fading and release the affiliation trigger");
            _initializeAffiliationTrigger = true;
            _fadeCharacters = true;
            OnCharacterLevelUp?.Invoke(new CharacterLevelUpProperties { PercentageSpeedIncrease = 0, SpeedDistanceDependance = SpeedDistanceDependance.None });
        }
        else if (levelNumber >= 9 && levelNumber < 12)
        {
            //Debug.Log("Increase the speed and activate fading option");
            _initializeAffiliationTrigger = false;
            _fadeCharacters = true;
            OnCharacterLevelUp?.Invoke(new CharacterLevelUpProperties { PercentageSpeedIncrease = 45, SpeedDistanceDependance = SpeedDistanceDependance.Medium });
        }
        else if (levelNumber >= 12 && levelNumber < 15)
        {
            //Debug.Log("Increase the speed, activate fading and release the affiliation trigger");
            _initializeAffiliationTrigger = true;
            _fadeCharacters = true;
            OnCharacterLevelUp?.Invoke(new CharacterLevelUpProperties { PercentageSpeedIncrease = 60, SpeedDistanceDependance = SpeedDistanceDependance.High });
        }
        else if (levelNumber >= 15 && levelNumber < 18)
        {
            //Debug.Log("Increase the speed, activate fading, affiliation trigger and run time");
            float timerValue = 15.0f - (levelNumber - 15) * 3.0f;
            OnActivateTimer?.Invoke(timerValue);
            OnCharacterLevelUp?.Invoke(new CharacterLevelUpProperties { PercentageSpeedIncrease = 90, SpeedDistanceDependance = SpeedDistanceDependance.Medium });
        }
        else if (levelNumber >= 18 && levelNumber <= 35)
        {
            //Debug.Log("Increase the speed, activate fading, affiliation trigger, run time and spawn new characters");
            if (levelNumber >= 22)
            {
                _accuracyRequiredToPassLevel = levelNumber * 2.0f / 100.0f;
                OnActivateAccuracy?.Invoke(_accuracyRequiredToPassLevel);
            }

            float timerValue = Mathf.Floor(30.0f - (levelNumber - 18) * levelNumber / 25.0f);
            OnActivateTimer?.Invoke(timerValue);
            OnCharacterLevelUp?.Invoke(
                new CharacterLevelUpProperties { PercentageSpeedIncrease = (int)Mathf.Floor(40 * levelNumber/10.0f), SpeedDistanceDependance = SpeedDistanceDependance.High, CharactersSpawnNewCharacters = true }
                );
        }
        else
            return;
    }

    private void checkLevelCompletion(Character characterDestroyed)
    {
        AffiliationTrigger afiiliationTrigger = characterDestroyed.GetComponent<AffiliationTrigger>();

        if (afiiliationTrigger == null)
        {
            _charactersList.Remove(characterDestroyed.transform);
            checkAmountOfGoodBadChars(characterDestroyed);
        }
    }

    private void checkAmountOfGoodBadChars(Character lastCharacterDestroyed)
    {
        int badCharactersNum = getCharacterTypeAmount(CharacterType.Negative);
        int goodCharactersNum = getCharacterTypeAmount(CharacterType.Positive);

        if (goodCharactersNum == 0)
        {
            if (lastCharacterDestroyed.GetCharacterType().Equals(CharacterType.Positive))
                reloadGame();
            else
            {
                if (badCharactersNum == 0)
                    checkAccuracyRequirements(_accuracyRequiredToPassLevel);
                else
                    return;
            }
        }
        else
        {
            if (badCharactersNum == 0)
                checkAccuracyRequirements(_accuracyRequiredToPassLevel);
            else
                return;
        }
    }
    
    private void checkAccuracyRequirements(float accuracyRequired)
    {
        if (accuracyRequired > 0.0f)
        {
            requestPlayerAccuracy();
            if (_currentAccuracy >= accuracyRequired)
            {
                beforeLoadNewLevel();
                loadNewLevel();
            }
            else
                reloadGame();
        }
        else
        {
            beforeLoadNewLevel();
            loadNewLevel();
        }

        _currentAccuracy = 0.0f;
    }

    private void requestPlayerAccuracy()
    {
        OnGrabPlayerAccuracy?.Invoke();
    }

    private void getPlayerAccuracy(float value)
    {
        _currentAccuracy = value;
    }

    private int getCharacterTypeAmount(CharacterType type)
    {
        int characterTypeNumber = 0;

        foreach (Transform charTransform in _charactersList)
        {
            Character character = charTransform.GetComponentInChildren<Character>();
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
        if (_fadeCharacters && _gameRunning)
        {
            if (_charactersList.Count == 0)
                return;

            Transform characterTransform = _charactersList[UnityEngine.Random.Range(0, _charactersList.Count)];
            FadeObject fadeObject = characterTransform.GetComponent<FadeObject>();

            if (fadeObject != null)
                fadeObject.ActivateFade(!fadeObject.IsInvisible());
        }
    }

    public bool IsGameRunning()
    {
        return _gameRunning;
    }
}