using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float followSpeed;
    [SerializeField]
    private Vector3 cameraOffset;

    private Vector3 targetPos;
    private Vector3 smoothedPosition;

    private void Start()
    {
        targetPos = target.position + cameraOffset;
        smoothedPosition = Vector3.Lerp(transform.position, targetPos, followSpeed);
    }
    private void FixedUpdate()
    {
        
        if ((target.position + cameraOffset - transform.position).sqrMagnitude < 0.1f) 
        {
            return;
        }
 
        targetPos = target.position + cameraOffset;
        smoothedPosition = Vector3.Lerp(transform.position, targetPos, followSpeed);
        transform.position = smoothedPosition;


    }
}
