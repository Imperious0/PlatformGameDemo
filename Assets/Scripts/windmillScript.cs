using UnityEngine;

public class WindmillScript : MonoBehaviour
{
    [SerializeField][Range(0,10)]
    private float windmillSpeed;
    [SerializeField]
    private GameObject Propeller;

    private void FixedUpdate()
    {
        Propeller.transform.Rotate(transform.forward * windmillSpeed);
    }
}
