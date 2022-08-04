using UnityEngine;

public interface ICharacterMove
{
    public void ActivateRotation();
    public void NearbyHitDetectedAt(Vector3 worldPosition);
    public void SetCharacterSpeedPerc(int integerPercentage);
    public void SetDistanceDependance(SpeedDistanceDependance speedDistanceDependence);
}
