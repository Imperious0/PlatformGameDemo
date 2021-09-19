using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class horizontalObsController : MonoBehaviour
{
    [SerializeField]
    private Vector3 movementOffset;
    [SerializeField][Range(0,10)]
    private float movementSpeed;

    private Vector3 initialPos;
    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
    }

    private void FixedUpdate()
    {
        if ((initialPos - this.transform.position).magnitude > movementOffset.magnitude)
            movementOffset *= -1f;

        this.transform.position = Vector3.Lerp(this.transform.position, this.transform.position + movementOffset, movementSpeed * Time.deltaTime);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<playerController>()) 
        {
            collision.gameObject.GetComponent<playerController>().hitSfx();
        }
    }
}
