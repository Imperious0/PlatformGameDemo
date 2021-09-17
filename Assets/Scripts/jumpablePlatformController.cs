using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpablePlatformController : MonoBehaviour
{
    [SerializeField]
    private bool isNeedToBeJump = false;

    public bool IsNeedToBeJump { get => isNeedToBeJump; }
}
