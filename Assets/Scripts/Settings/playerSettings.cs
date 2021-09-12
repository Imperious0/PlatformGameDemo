using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSettings/PlayerSetting")]
public class playerSettings : ScriptableObject
{
    [SerializeField]
    private float _movementSpeed;
    [SerializeField]
    private float _jumpPower;
 
    public float MovementSpeed { get => _movementSpeed; }
    public float JumpPower { get => _jumpPower; }
}
