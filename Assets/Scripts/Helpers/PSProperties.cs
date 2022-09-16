using UnityEngine;

public enum PSTextureType
{
    Character,
    Obstacle,
    AffTrigger,
    ObstDestroyer,
}

public struct PSProperties
{
    public Vector3 PSposition;
    public PSType PSType;
    public Color PSColor;
    public PSTextureType PSTextureType;
}
