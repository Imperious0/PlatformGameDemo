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
    private Vector3 freezeForce;

    bool isStop = true;
    bool isGrounded = true;
    // Start is called before the first frame update
    void Start()
    {
        pAnimator = this.GetComponent<Animator>();
        pRigidbody = this.GetComponent<Rigidbody>();
        moveForce = Vector3.zero;
        freezeForce = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetAxis("Vertical") != 0 && isGrounded) 
        {
            moveForce.z = Input.GetAxis("Vertical") * 10 * pSettings.MovementSpeed;
            pAnimator.SetFloat("VelocityZ", moveForce.z);
            if(isStop)
                isStop = false;
        }
        if (Input.GetAxis("Horizontal") != 0)
        {
            moveForce.x = Input.GetAxis("Horizontal") * 10 * pSettings.MovementSpeed;
            pAnimator.SetFloat("VelocityX", moveForce.x);
            if (isStop)
                isStop = false;
        }
        if(Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0 && !isStop)
        {
            moveForce = Vector3.zero;
            pAnimator.SetFloat("VelocityZ", moveForce.z);
            pAnimator.SetFloat("VelocityX", moveForce.x);
            isStop = true;
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) 
        {
            pAnimator.SetFloat("VelocityZ", moveForce.z);
            pAnimator.SetFloat("VelocityX", moveForce.x);
            pAnimator.SetTrigger("Jump");
            pRigidbody.AddForce(Vector3.up * pSettings.JumpPower, ForceMode.Force);
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
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Platform" && !isGrounded)
        {
            isGrounded = true;
        }
    }
}
