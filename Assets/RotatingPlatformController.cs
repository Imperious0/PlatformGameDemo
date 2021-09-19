using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatformController : MonoBehaviour
{
    [SerializeField]
    private bool clockwiseTurn = true;
    [SerializeField]
    private float spinPower = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (clockwiseTurn)
            this.transform.Rotate(this.transform.forward * spinPower);
        else
            this.transform.Rotate(this.transform.forward * spinPower * -1f);
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody>()) 
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce((clockwiseTurn ? Vector3.left : Vector3.right) * spinPower * 50f, ForceMode.Force);
        }
    }
}
