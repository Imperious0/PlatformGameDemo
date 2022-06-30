using UnityEngine;

public class RotatingObstacleController : MonoBehaviour
{
    [SerializeField]
    private rotatorSettings rSettings;

    private Vector3 isSpinClockwise;

    void Start()
    {
        isSpinClockwise = rSettings.SpinClockwise ? this.transform.up : this.transform.up * -1;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.Rotate(isSpinClockwise * rSettings.SpinMultiplier);
    }
}
