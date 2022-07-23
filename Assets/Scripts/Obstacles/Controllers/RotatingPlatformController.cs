using System.Collections.Generic;
using UnityEngine;
public class RotatingPlatformController : MonoBehaviour
{
    [SerializeField]
    private bool clockwiseTurn = true;
    [SerializeField]
    private float spinPower = 0f;
    Transform platformTransform;

    List<Rigidbody> tempRigidbody;

    private void Awake()
    {
        platformTransform = transform;
        tempRigidbody = new List<Rigidbody>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (clockwiseTurn)
            platformTransform.Rotate(platformTransform.forward * spinPower);
        else
            platformTransform.Rotate(platformTransform.forward * spinPower * -1f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody tmp = collision.gameObject.GetComponent<Rigidbody>();

        if (tmp != null)
        {
            tempRigidbody.Add(tmp);
            
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if(tempRigidbody.Count > 0)
        {
            int count = tempRigidbody.Count;
            for (int i = 0; i < count; i++)
            {
                tempRigidbody[i].AddForce((clockwiseTurn ? Vector3.left : Vector3.right) * spinPower * 100f, ForceMode.Acceleration);
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if(tempRigidbody.Count > 0)
        {
            tempRigidbody.Remove(collision.gameObject.GetComponent<Rigidbody>());
        }
    }
}
