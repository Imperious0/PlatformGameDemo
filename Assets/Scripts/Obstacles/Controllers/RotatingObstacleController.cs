using UnityEngine;
public class RotatingObstacleController : MonoBehaviour
{
    [SerializeField]
    private rotatorSettings rSettings;

    private Vector3 isSpinClockwise;
    
    Transform obsTransform;

    private void Awake()
    {
        obsTransform = transform;
        isSpinClockwise = rSettings.SpinClockwise ? obsTransform.up : obsTransform.up * -1;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        obsTransform.Rotate(isSpinClockwise * rSettings.SpinMultiplier);
    }
}
