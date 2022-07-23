using UnityEngine;

public class WindmillController : MonoBehaviour
{
    [SerializeField][Range(0,10)]
    private float windmillSpeed;
    [SerializeField]
    private GameObject Propeller;

    private Transform windmillTransform;
    private Transform propTransform;

    private void Awake()
    {
        windmillTransform = transform;
        propTransform = Propeller.transform;
    }

    private void FixedUpdate()
    {
        propTransform.Rotate(windmillTransform.forward * windmillSpeed);
    }
}
