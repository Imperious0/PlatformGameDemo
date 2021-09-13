using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSettings/PlayerSetting")]
public class playerSettings : ScriptableObject
{
    [SerializeField]
    private float _movementSpeed;
    [SerializeField]
    private float _backwardSpeed;
    [SerializeField]
    private float _rotationSpeed;
    [SerializeField]
    private float _jumpPower;
 
    public float MovementSpeed { get => _movementSpeed; }
    public float JumpPower { get => _jumpPower; }
    public float RotationSpeed { get => _rotationSpeed; }
    public float BackwardSpeed { get => _backwardSpeed; }
}
