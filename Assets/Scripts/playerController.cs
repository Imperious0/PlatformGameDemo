using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField]
    private playerSettings pSettings;

    private Animator pAnimator;
    private Rigidbody pRigidbody;

    private Vector3 moveForce;
    private Vector3 rotateForce;

    bool isStop = true;
    bool isGrounded = true;
    // Start is called before the first frame update
    void Start()
    {
        pAnimator = this.GetComponent<Animator>();
        pRigidbody = this.GetComponent<Rigidbody>();
        moveForce = Vector3.zero;
        rotateForce = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetAxis("Vertical") != 0 && isGrounded) 
        {
            if(Input.GetAxis("Vertical") < 0)
                moveForce = transform.forward * Input.GetAxis("Vertical") * 10 * pSettings.BackwardSpeed;
            else
                moveForce = transform.forward * Input.GetAxis("Vertical") * 10 * pSettings.MovementSpeed;
            float velocity = Input.GetAxis("Vertical") > 0 ? moveForce.magnitude : (moveForce.magnitude * -1);
            pAnimator.SetFloat("VelocityZ", velocity);
            if(isStop)
                isStop = false;
        }
        if (Input.GetAxis("Horizontal") != 0)
        {
            rotateForce.x = Input.GetAxis("Horizontal") * 10 * pSettings.RotationSpeed;
            float velocity = Input.GetAxis("Horizontal") > 0 ? rotateForce.magnitude : (rotateForce.magnitude * -1);

            //Backward Movement Rotating
            rotateForce *= Input.GetAxis("Vertical") < 0 ? -1 : 1;
            velocity = Input.GetAxis("Vertical") < 0 ? velocity * -1 : velocity;

            pAnimator.SetFloat("VelocityX", velocity);
        }
        if(Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0 && !isStop)
        {
            moveForce = Vector3.zero;
            rotateForce = Vector3.zero;
            pAnimator.SetFloat("VelocityZ", moveForce.z);
            pAnimator.SetFloat("VelocityX", moveForce.x);
            isStop = true;
        }
        if (Input.GetAxis("Jump") > 0 && isGrounded) 
        {
            pAnimator.SetFloat("VelocityZ", moveForce.z);
            pAnimator.SetFloat("VelocityX", moveForce.x);
            pAnimator.SetTrigger("Jump");
            pRigidbody.AddForce(transform.up * pSettings.JumpPower, ForceMode.Force);
            isGrounded = false;
        }
    }
    private void FixedUpdate()
    {
        if(moveForce.sqrMagnitude > 0) 
        {
            moveForce.y = pRigidbody.velocity.y;

            pRigidbody.velocity = moveForce;
       
        }
        if(rotateForce.sqrMagnitude > 0) 
        {
            transform.Rotate((transform.up * rotateForce.x));
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Platform" && !isGrounded)
        {
            isGrounded = true;
        }
    }
}
