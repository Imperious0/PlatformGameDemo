using UnityEngine;

[CreateAssetMenu(menuName = "GameSettings/PlayerSetting")]
public class playerSettings : ScriptableObject
{
    [SerializeField]
    private float _movementSpeed;
    [SerializeField]
    private float _backwardSpeed;
    [SerializeField]
    private float _maxHorizontalSpeed;
    [SerializeField]
    private float _rotationSpeed;
    [SerializeField]
    private float _maxJumpDistance;
 
    public float MovementSpeed { get => _movementSpeed; }
    public float MaxJumpDistance { get => _maxJumpDistance; }
    public float RotationSpeed { get => _rotationSpeed; }
    public float BackwardSpeed { get => _backwardSpeed; }
    public float MaxHorizontalSpeed { get => _maxHorizontalSpeed; }
}
