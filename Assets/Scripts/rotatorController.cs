using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class rotatorController : MonoBehaviour
{
    [SerializeField]
    private rotatorSettings rSettings;

    private Vector3 isSpinClockwise;

    private void Start()
    {
        isSpinClockwise = rSettings.SpinClockwise ? this.transform.up : this.transform.up * -1;
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(isSpinClockwise * rSettings.SpinMultiplier);
    }
}
