using UnityEngine;

[CreateAssetMenu(menuName = "GameSettings/RotatorSettings")]
public class rotatorSettings : ScriptableObject
{
    [SerializeField]
    private float _spinMultiplier;
    [SerializeField]
    private bool _spinClockwise;
    public float SpinMultiplier { get => _spinMultiplier; }
    public bool SpinClockwise { get => _spinClockwise; }
}
