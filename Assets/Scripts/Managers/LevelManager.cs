using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AlpacaMyGames;

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
    public Action OnGamePassed;
    public Action OnResetUI;
    public Action<CharacterLevelUpProperties> OnCharacterLevelUp;
    public Action<int> OnCharacterDestroyedAtLevel;
    public Action<float> OnActivateTimer;
    public Action<float> OnActivateAccuracy;
    public Action OnGrabPlayerAccuracy;

    private Camera _mainCamera;

    [Header("For testing")]
    [SerializeField] private bool _initializeGame = true;
    [SerializeField] private bool _spawnSingleCharacter = false;
    [SerializeField] private bool _spawnAffiliationTrigger = false;
    [SerializeField] private bool _charactersSpawnNewCharacters = false;
    [SerializeField] private int _initializeLevelNumber = 1;

    private int _initialNumOfObstacles = 1;
    private int _initialNumOfCharacters = 1;

    [Header("Read-only")]
    [SerializeField] private int _levelNumber = 1;
    [SerializeField] private int _numOfObstacles;
    [SerializeField] private int _numOfCharacters;

    private List<Transform> _obstaclesList = new List<Transform>();
    private List<Transform> _charactersList = new List<Transform>();

    private bool _initializeAffiliationTrigger = false;
    private bool _initializeObstacleDestroyer = false;
    private Transform _affiliationTransform;
    private Transform _obstacleDestroyerTransform;

    private GameAssets _gameAssets;

    private string _obstacleLayerMaskName = "Obstacle";
    private string _characterLayerMaskName = "Character";

    private float _borderScaling = 0.95f;
    private float _objectMinDistance = 1.0f;

    private bool _fadeCharacters = false;
    private float _accuracyRequiredToPassLevel = 0.0f;
    private float _currentAccuracy = 0.0f;

    private bool _gameRunning = true;
    private bool _checkLevelCompletion = true;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        Character.OnCharacterDestroyed += checkLevelCompletion;
        CharacterSpawning.OnCharacterSpawn += addCharacterToList;
        Obstacle.OnObstacleDestroy += removeObstacleFromList;
        GameManager.Instance.OnQuitToMainMenu += ClearGame;
        GameManager.Instance.OnGameOver += resetGameSettings;
        PlayerTouchManager.Instance.OnDoubleTouch += reloadGame;

        _mainCamera = Camera.main;
        _gameAssets = GameAssets.Instance;

        _numOfCharacters = _initialNumOfCharacters;
        _numOfObstacles = _initialNumOfObstacles;

        if (_initializeGame)
            InitializeGame();

        //Testing purposes
        if (_spawnSingleCharacter)
            initializeObjects(_gameAssets.CharacterObject, 1, getLayerMask(_characterLayerMaskName));
    }

    private void OnDestroy()
    {
        Character.OnCharacterDestroyed -= checkLevelCompletion;
        CharacterSpawning.OnCharacterSpawn -= addCharacterToList;
        Obstacle.OnObstacleDestroy -= removeObstacleFromList;
        GameManager.Instance.OnQuitToMainMenu -= ClearGame;
        GameManager.Instance.OnGameOver -= resetGameSettings;
        PlayerTouchManager.Instance.OnDoubleTouch -= reloadGame;
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

        if (_obstacleDestroyerTransform != null)
            Destroy(_obstacleDestroyerTransform.gameObject);
    }


    public void InitializeGame()
    {
        if (_initializeLevelNumber > 1)
            loadLevel(_initializeLevelNumber);
        else
        {
            if (_levelNumber == 1)
            {
                resetGameSettings();
                OnResetUI?.Invoke();
            }

            loadLevel(_levelNumber);
            InvokeRepeating(nameof(fadeCharacter), UnityEngine.Random.Range(0.5f, 2.0f), UnityEngine.Random.Range(0.5f, 1.0f));
        }
        
        OnInitializeGame?.Invoke();
    }

    private void initializePlayground()
    {
        ClearGame();

        //initializing new objects
        initializeObjects(_gameAssets.ObstacleObject, _numOfObstacles, getLayerMask(_obstacleLayerMaskName), _obstaclesList);
        initializeObjects(_gameAssets.CharacterObject, _numOfCharacters, getLayerMask(_characterLayerMaskName), _charactersList);

        if (Utilities.ChanceFunc(90) && _initializeAffiliationTrigger)
            _affiliationTransform = spawnObject(_gameAssets.AffiliationTrigger);

        if (Utilities.ChanceFunc(75) && _initializeObstacleDestroyer)
            _obstacleDestroyerTransform = spawnObject(_gameAssets.ObstacleDestroyer);

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

            if (Utilities.CheckObjectEnvironment(objectToSpawn, _objectMinDistance, layerToAvoid))
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
        Vector2 position = _mainCamera.ScreenToWorldPoint(Utilities.GetRandomScreenPosition(_borderScaling));
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

    private void resetGameSettings()
    {
        _levelNumber = 1;
        editLevelSettings(false, false, false);

        float timer = 0.0f;
        _accuracyRequiredToPassLevel = 0.0f;
        
        OnActivateTimer?.Invoke(timer);
        OnActivateAccuracy?.Invoke(_accuracyRequiredToPassLevel);

        _numOfCharacters = _initialNumOfCharacters;
        _numOfObstacles = _initialNumOfObstacles;
    }

    private void reloadGame()
    {
        resetGameSettings();

        OnGameReload?.Invoke();

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

        if (_levelNumber <= 35)
            loadLevel(_levelNumber);
        else
            OnGamePassed?.Invoke();
    }

    private void loadLevel(int levelNumber)
    {
        _levelNumber = levelNumber;
        OnLoadLevel?.Invoke(levelNumber);

        //increase the number of obstacles and characters
        alterLevelObstacleCharacterSettings(levelNumber);

        float timer = 0.0f;
        _accuracyRequiredToPassLevel = 0.0f;
        OnActivateTimer?.Invoke(timer);
        OnActivateAccuracy?.Invoke(_accuracyRequiredToPassLevel);

        //re-initialize level
        initializePlayground();

        changeLevelSettings(levelNumber);
    }

    private void alterLevelObstaclesAndCharactersPerLevel(int levelNumber)
    {
        if (levelNumber <= 5)
        {
            _numOfCharacters += 1;
        }
        else if (levelNumber > 5 && levelNumber <= 10)
        {
            _numOfCharacters += 1;
            _numOfObstacles += 1;

            _objectMinDistance *= 0.95f;
        }
        else if (levelNumber > 10 && levelNumber <= 15)
        {
            _numOfObstacles += 1;

            _objectMinDistance *= 0.9f;
        }
    }

    //UPDATE THE PROCEDURE
    private void alterLevelObstacleCharacterSettings(int levelNumber)
    {
        if (levelNumber <= 10)
            _numOfCharacters = levelNumber;
        else
            _numOfCharacters = 10;

        int firstThresholdLevel = 4;

        if (levelNumber > firstThresholdLevel && levelNumber <= 15)
        {
            _numOfObstacles = (levelNumber - firstThresholdLevel + 1);
            _objectMinDistance = Mathf.Pow(0.95f, levelNumber - firstThresholdLevel);
        }
    }

    private void changeLevelSettings(int levelNumber)
    {
        if (levelNumber >= 2 && levelNumber < 5)
        {
            //SPEED, ACCURACY
            editLevelSettings(false, false, false);
            _accuracyRequiredToPassLevel = (levelNumber + 1) / 10.0f;
            editLevelEvents(new CharacterLevelUpProperties { PercentageSpeedIncrease = 10, SpeedDistanceDependance = SpeedDistanceDependance.None }, _accuracyRequiredToPassLevel, 0.0f);

            //OnActivateAccuracy?.Invoke(_accuracyRequiredToPassLevel);
            //OnCharacterLevelUp?.Invoke(new CharacterLevelUpProperties { PercentageSpeedIncrease = 30, SpeedDistanceDependance = SpeedDistanceDependance.Medium });
        }
        else if (levelNumber >= 5 && levelNumber < 7)
        {
            //FADING, DESTROYER
            editLevelSettings(true, false, true);
            editLevelEvents(new CharacterLevelUpProperties { PercentageSpeedIncrease = 0, SpeedDistanceDependance = SpeedDistanceDependance.Medium }, 0.0f, 0.0f);

            //OnCharacterLevelUp?.Invoke(new CharacterLevelUpProperties { PercentageSpeedIncrease = 0, SpeedDistanceDependance = SpeedDistanceDependance.None });
        }
        else if (levelNumber >= 7 && levelNumber < 9)
        {
            //FADING, AFFILIATION, TIMER
            editLevelSettings(true, true, false);
            editLevelEvents(new CharacterLevelUpProperties { PercentageSpeedIncrease = 0, SpeedDistanceDependance = SpeedDistanceDependance.None }, 0.0f, 0.0f);

            //OnCharacterLevelUp?.Invoke(new CharacterLevelUpProperties { PercentageSpeedIncrease = 0, SpeedDistanceDependance = SpeedDistanceDependance.None });
        }
        else if (levelNumber >= 9 && levelNumber < 12)
        {
            //SPEED, FADING, DESTROYER
            editLevelSettings(true, false, true);
            editLevelEvents(new CharacterLevelUpProperties { PercentageSpeedIncrease = 15, SpeedDistanceDependance = SpeedDistanceDependance.Medium }, 0.0f, 0.0f);

            //OnCharacterLevelUp?.Invoke(new CharacterLevelUpProperties { PercentageSpeedIncrease = 40, SpeedDistanceDependance = SpeedDistanceDependance.Medium });
        }
        else if (levelNumber >= 12 && levelNumber < 15)
        {
            //SPEED, FADING, AFFILIATION, TIMER
            editLevelSettings(true, true, false);
            float timerValue = 15.0f - (levelNumber - 12) * 2.0f;
            editLevelEvents(new CharacterLevelUpProperties { PercentageSpeedIncrease = 20, SpeedDistanceDependance = SpeedDistanceDependance.High }, 0.0f, timerValue);

            //OnActivateTimer?.Invoke(timerValue);
            //OnCharacterLevelUp?.Invoke(new CharacterLevelUpProperties { PercentageSpeedIncrease = 50, SpeedDistanceDependance = SpeedDistanceDependance.High });
        }
        else if (levelNumber >= 15 && levelNumber < 18)
        {
            //SPEED, FADING, AFFILIATION, DESTROYER, SPAWNING
            editLevelSettings(true, true, true);
            editLevelEvents(new CharacterLevelUpProperties { PercentageSpeedIncrease = 25, SpeedDistanceDependance = SpeedDistanceDependance.Medium, CharactersSpawnNewCharacters = true }, 0.0f, 0.0f);

            //OnCharacterLevelUp?.Invoke(new CharacterLevelUpProperties { PercentageSpeedIncrease = 70, SpeedDistanceDependance = SpeedDistanceDependance.Medium, CharactersSpawnNewCharacters = true });
        }
        else if (levelNumber >= 18 && levelNumber < 22)
        {
            //SPEED, FADING, AFFILIATION, DESTROYER, TIMER, SPAWNING
            editLevelSettings(true, true, true);
            float timerValue = Mathf.Floor(20.0f - (levelNumber - 18) * levelNumber / 25.0f);
            editLevelEvents(new CharacterLevelUpProperties { PercentageSpeedIncrease = (int)Mathf.Floor(12.0f * levelNumber / 10.0f), SpeedDistanceDependance = SpeedDistanceDependance.Medium, CharactersSpawnNewCharacters = false }, 0.0f, timerValue);

            //OnActivateTimer?.Invoke(timerValue);
            //OnCharacterLevelUp?.Invoke(
            //    new CharacterLevelUpProperties { PercentageSpeedIncrease = (int)Mathf.Floor(25 * levelNumber / 10.0f), SpeedDistanceDependance = SpeedDistanceDependance.High, //CharactersSpawnNewCharacters = true }
            //    );
        }
        else if (levelNumber >= 22 && levelNumber <= 35)
        {
            //SPEED, FADING, AFFILIATION, DESTROYER, TIMER, SPAWNING, ACCURACY
            editLevelSettings(true, true, true);
            _accuracyRequiredToPassLevel = levelNumber * 1.5f / 100.0f;
            float timerValue = Mathf.Floor(25.0f - (levelNumber - 22) * levelNumber / 30.0f);
            editLevelEvents(new CharacterLevelUpProperties { PercentageSpeedIncrease = (int)Mathf.Floor(9.0f * levelNumber / 10.0f), SpeedDistanceDependance = SpeedDistanceDependance.High, CharactersSpawnNewCharacters = true }, _accuracyRequiredToPassLevel, timerValue);

            //OnActivateAccuracy?.Invoke(_accuracyRequiredToPassLevel);
            //OnActivateTimer?.Invoke(timerValue);
            //OnCharacterLevelUp?.Invoke(
            //    new CharacterLevelUpProperties { PercentageSpeedIncrease = (int)Mathf.Floor(25 * levelNumber / 10.0f), SpeedDistanceDependance = SpeedDistanceDependance.High, //CharactersSpawnNewCharacters = true }
            //    );
        }
        else
            return;
    }

    private void editLevelSettings(bool fadeCharacters = false, bool initializeAffiliationTrigger = false, bool initializeObstacleCharacter = false)
    {
        _fadeCharacters = fadeCharacters;
        _initializeAffiliationTrigger = initializeAffiliationTrigger;
        _initializeObstacleDestroyer = initializeObstacleCharacter;
    }

    private void editLevelEvents(CharacterLevelUpProperties characterLevelUpProperties = null, float accuracy = 0.0f, float timer = 0.0f)
    {
        OnCharacterLevelUp?.Invoke(characterLevelUpProperties);
        OnActivateAccuracy?.Invoke(accuracy);
        OnActivateTimer?.Invoke(timer);
    }

    private void checkLevelCompletion(Character characterDestroyed)
    {
        if (!_checkLevelCompletion)
            return;

        AffiliationTrigger affiliationTrigger = characterDestroyed.GetComponent<AffiliationTrigger>();
        ObstacleDestroyer obstacleDestroyer = characterDestroyed.GetComponent<ObstacleDestroyer>();

        if (affiliationTrigger != null)
        {
            int badCharactersNum = getCharacterTypeAmount(CharacterType.Negative);

            if (badCharactersNum == 0)
                reloadGame();
        }

        if (affiliationTrigger == null && obstacleDestroyer == null)
        {
            _charactersList.Remove(characterDestroyed.transform);
            checkAmountOfGoodBadChars(characterDestroyed);

            if (characterDestroyed.GetCharacterType().Equals(CharacterType.Negative))
                OnCharacterDestroyedAtLevel?.Invoke(_levelNumber);
        }
    }

    public bool GetCheckLevelCompletion()
    {
        return _checkLevelCompletion;
    }

    public void SetCheckLevelCompletion(bool value)
    {
        _checkLevelCompletion = value;
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
            _currentAccuracy = HitManager.GrabPlayerAccuracy();
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

    private LayerMask getLayerMask(string layerName)
    {
        return LayerMask.NameToLayer(layerName);
    }

    public bool IsGameRunning()
    {
        return _gameRunning;
    }

    public static int GrabLevel()
    {
        return _instance.GetLevel();
    }

    public void SetLevel(int level)
    {
        _levelNumber = level;
    }
    public int GetLevel()
    {
        return _levelNumber;
    }
}