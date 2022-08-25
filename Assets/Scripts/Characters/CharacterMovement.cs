using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    private bool _isRotating = false;
    private int _rotationDirection = 1;
    private float _rotationSpeed = 300.0f;

    private bool _isWaiting = false;
    private float _waitTime = 0.0f;
    private Vector2 _waitInterval = new Vector2(0.5f, 1.0f);
    private float _waitingTimer = 0.0f;

    private Vector3 _nearbyHit;

    [Header("Character Speed")]
    [SerializeField]
    [Range(4.0f, 8.0f)] private float _baseSpeed = 4.0f;
    private float _characterSpeed = 4.0f;
    [SerializeField] [Tooltip("Current speed is equal to base speed")] private bool _equalSpeeds;
    //if distance is great, the speed is greatly increased, if distance is short the speed is slightly increased
    [SerializeField] [Tooltip("Speed depending on distance factor")] [Range(0.0f, 0.1f)] private float _distanceDependance = 0.07f;

    private LevelManager _levelManager;

    private void Start()
    {
        _levelManager = LevelManager.Instance;

        if (_equalSpeeds)
            _characterSpeed = _baseSpeed;

        SetDistanceDependance(SpeedDistanceDependance.None);
    }

    private void LateUpdate()
    {
        characterMovement();
    }

    public void MoveTo(Vector2 position, Action funcToPerform = null)
    {
        Debug.Log("Move to: " + position + ", then do an action.");
        funcToPerform?.Invoke();
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
            generateWaypoints();
        else
            move();

        if (_isRotating)
            rotateCharacter();
    }

    private void generateWaypoints()
    {
        List<Transform> obstaclesList = _levelManager.GetObjectList(ObjectListType.Obstacle);

        if (obstaclesList != null && _waypointsDependOnObstacles)
        {
            if (AlpacaUtils.ChanceFunc(50))
            {
                _waypointPositions.Add(obstaclesList[UnityEngine.Random.Range(0, obstaclesList.Count)].position);
            }
            else
            {
                Vector2 randomPosition = AlpacaUtils.GetRandomWorldPosition(2);
                _waypointPositions.Add(randomPosition);
            }
        }
        else
        {
            Vector2 randomPosition = AlpacaUtils.GetRandomWorldPosition(2);
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
        Vector2 distanceToNearestEdges = AlpacaUtils.GetDistanceToNearestWorldEdges(transform.position);

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
                _distanceDependance = 0.05f;
                break;
            case SpeedDistanceDependance.High:
                _distanceDependance = 0.1f;
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

        _rotationDirection = AlpacaUtils.ChanceFunc(50) ? 1 : -1;
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
}