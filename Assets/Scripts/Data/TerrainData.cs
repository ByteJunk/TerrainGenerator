using UnityEngine;

[CreateAssetMenu]
public class TerrainData : UpdatableData
{
    public bool useFlatShading;
    public bool useFalloff;
    public float meshHeightMultiplier;
    public AnimationCurve heightCurve;
    public float uniformScale = 2.5f;
}
