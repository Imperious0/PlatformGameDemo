using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class windmillScript : MonoBehaviour
{
    [SerializeField][Range(0,10)]
    private float windmillSpeed;
    [SerializeField]
    private GameObject Propeller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        Propeller.transform.Rotate(transform.forward * windmillSpeed);
    }
    // Update is called once per frame
    void Update()
    {
      
    }
}
