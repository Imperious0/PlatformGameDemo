using UnityEngine;

public class RotatingPlatformController : MonoBehaviour
{
    [SerializeField]
    private bool clockwiseTurn = true;
    [SerializeField]
    private float spinPower = 0f;
    // Update is called once per frame
    void FixedUpdate()
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
            collision.gameObject.GetComponent<Rigidbody>().AddForce((clockwiseTurn ? Vector3.left : Vector3.right) * spinPower * 100f, ForceMode.Acceleration);
        }
    }
}
