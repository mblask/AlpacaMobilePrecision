using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public enum SpeedDistanceDependance
{
    None,
    Medium,
    High,
}

public class CharacterMovement : MonoBehaviour, ICharacterMove
{
    [Header("Waypoints")]
    [SerializeField] private bool _waypointsDependOnObstacles = false;
    private List<Vector3> _waypointPositions = new List<Vector3>();
    private Vector2 _waypointOriginPoint = new Vector2();

    private bool _isRotating = false;
    private int _rotationDirection = 1;
    private float _rotationSpeed = 300.0f;

    private bool _isWaiting = false;
    private float _waitTime = 0.0f;
    private Vector2 _waitInterval = new Vector2(0.5f, 1.0f);
    private float _waitingTimer = 0.0f;

    private Vector3 _nearbyHit;

    [Header("Character Speed")]
    [SerializeField] private float _baseSpeed = 4.0f;
    [Header("Read-only")][SerializeField] private float _characterSpeed = 4.0f;
    [SerializeField] [Tooltip("Current speed is equal to base speed")] private bool _equalSpeeds;
    //if distance is great, the speed is greatly increased, if distance is short the speed is slightly increased
    [SerializeField] [Tooltip("Speed depending on distance factor")] private float _distanceDependance = 0.03f;

    private LevelManager _levelManager;

    private void OnEnable()
    {
        if (_equalSpeeds)
            _characterSpeed = _baseSpeed;

        SetDistanceDependance(SpeedDistanceDependance.None);
    }

    private void LateUpdate()
    {
        characterMovement();
    }

    private void levelUpCharacter(CharacterLevelUpProperties properties)
    {
        SetCharacterSpeedPerc(properties.PercentageSpeedIncrease);
        SetDistanceDependance(properties.SpeedDistanceDependance);
        
        ISpawnCharacters characterSpawning = GetComponent<ISpawnCharacters>();
        characterSpawning?.ActivateSpawning(properties.CharactersSpawnNewCharacters);
    }

    private void characterMovement()
    {
        if (_isWaiting)
        {
            _waitingTimer += Time.deltaTime;

            if (_waitingTimer >= _waitTime)
            {
                _waitingTimer = 0.0f;
                _isWaiting = false;
            }

            return;
        }

        if (_nearbyHit != default(Vector3))
            generateWaypointOppositeOfCharacterMovement();

        if (_waypointPositions.Count == 0)
            generateWaypoints(_waypointOriginPoint);
        else
            move();

        if (_isRotating)
            rotateCharacter();
    }

    private void generateWaypoints(Vector2 origin = new Vector2())
    {
        if (_levelManager == null)
            _levelManager = LevelManager.Instance;

        List<Transform> obstaclesList = _levelManager.GetObjectList(ObjectListType.Obstacle);

        float borderScaling = 1.2f;
        if (obstaclesList != null && _waypointsDependOnObstacles)
        {
            if (Utilities.ChanceFunc(50))
            {
                _waypointPositions.Add(obstaclesList[UnityEngine.Random.Range(0, obstaclesList.Count)].position);
            }
            else
            {
                Vector2 randomPosition = Utilities.GetRandomWorldPosition(borderScaling, origin);
                _waypointPositions.Add(randomPosition);
            }
        }
        else
        {
            Vector2 randomPosition = Utilities.GetRandomWorldPosition(borderScaling, origin);
            _waypointPositions.Add(randomPosition);
        }
    }

    private void generateWaypointOppositeOfCharacterMovement()
    {
        if (_isWaiting)
            _isWaiting = false;

        Vector3 hitVector = transform.position - _nearbyHit;
        Vector3 oppositeDirectionOfHit = hitVector.normalized;

        //delete current waypoint
        _waypointPositions.Clear();

        //check how far the nearest edge is
        Vector2 distanceToNearestEdges = Utilities.GetDistanceToNearestWorldEdges(transform.position);

        //create new waypoint
        float minDistanceToEdges = Mathf.Min(distanceToNearestEdges.x, distanceToNearestEdges.y);
        float newWaypointDistance = minDistanceToEdges / hitVector.magnitude;

        if (newWaypointDistance > minDistanceToEdges)
            newWaypointDistance = minDistanceToEdges;

        Vector2 newWaypointLocation = transform.position + newWaypointDistance * oppositeDirectionOfHit;
        _waypointPositions.Add(newWaypointLocation);

        //reset nearby hit
        _nearbyHit = new Vector3();
    }

    private bool isBehindObstacle()
    {
        Collider2D[] hitInfo = Physics2D.OverlapCircleAll(transform.position, 2.0f);

        if (hitInfo.Length <= 1)
            return false;

        float obstacleHalfScale = 1.5f / 2;
        foreach (Collider2D hit in hitInfo)
        {
            Vector2 relativePosition = hit.transform.position - transform.position;

            if (relativePosition.magnitude < 0.01f)
                continue;

            if (Mathf.Abs(relativePosition.x) < obstacleHalfScale && Mathf.Abs(relativePosition.y) < obstacleHalfScale)
                return true;
        }

        return false;
    }

    private void move()
    {
        Vector3 distanceBeforeVector = _waypointPositions[0] - transform.position;
        Vector3 direction = distanceBeforeVector.normalized;

        transform.position += direction * (_characterSpeed + _distanceDependance * distanceBeforeVector.sqrMagnitude) * Time.deltaTime;

        float distanceAfter = Vector2.Distance(transform.position, _waypointPositions[0]);
        if (distanceAfter < 0.2f)
        {
            _waypointPositions.Clear();

            //check if it's behind an obstacle
            if (isBehindObstacle())
            {
                _isWaiting = true;
                _waitTime = UnityEngine.Random.Range(_waitInterval.x, _waitInterval.y);
            }
            else
                _isWaiting = false;
        }
    }

    public void SetCharacterSpeed(float value)
    {
        _characterSpeed = value;
    }

    public void SetCharacterSpeedPerc(int value)
    {
        if (value != 0)
            _characterSpeed = _baseSpeed * (1.0f + value / 100.0f);
        else
            ResetCharacterSpeed();
    }

    public void SetDistanceDependance(SpeedDistanceDependance distanceDependance)
    {
        switch (distanceDependance)
        {
            case SpeedDistanceDependance.None:
                _distanceDependance = 0.0f;
                break;
            case SpeedDistanceDependance.Medium:
                _distanceDependance = 0.01f;
                break;
            case SpeedDistanceDependance.High:
                _distanceDependance = 0.02f;
                break;
            default:
                break;
        }
    }

    public void ResetCharacterSpeed()
    {
        _characterSpeed = _baseSpeed;
    }

    public void ActivateRotation()
    {
        if (_isRotating)
            return;

        _isRotating = true;

        float minRotSpeed = 300.0f;
        float maxRotSpeed = 500.0f;

        _rotationDirection = Utilities.ChanceFunc(50) ? 1 : -1;
        _rotationSpeed = UnityEngine.Random.Range(minRotSpeed, maxRotSpeed);

        Invoke(nameof(deactivateRotation), 1.0f);
    }

    private void deactivateRotation()
    {
        _isRotating = false;
    }

    private void rotateCharacter()
    {
        Quaternion eulerRotation = Quaternion.Euler(transform.rotation.eulerAngles + _rotationDirection * new Vector3(0.0f, 0.0f, _rotationSpeed * Time.deltaTime));

        transform.rotation = eulerRotation;
    }

    public void NearbyHitDetectedAt(Vector3 position)
    {
        _nearbyHit = position;
    }

    public void SetWaypointOrigin(Vector2 origin)
    {
        _waypointOriginPoint = origin;
    }

    public Vector2 GetWaypointOrigin()
    {
        return _waypointOriginPoint;
    }

    public float GetCharacterSpeed()
    {
        return _characterSpeed;
    }
}