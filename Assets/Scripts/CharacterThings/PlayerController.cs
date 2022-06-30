using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterController
{
    private Vector3 moveForce;
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isHit)
            return;

        if (isFinished || isGameOver)
        {

            cRigidbody.velocity = new Vector3(0f, cRigidbody.velocity.y, 0f);
            cAnimator.SetFloat("VelocityZ", 0f);
            return;
        }
        destinationDistance = Mathf.FloorToInt(Vector3.Distance(destinationPoint, this.transform.position));
        if (!isGrounded)
            return;

        Vector3 movement = new Vector3(cRigidbody.velocity.x, cRigidbody.velocity.y, 10 * cSettings.MovementSpeed);
        this.cRigidbody.velocity = movement;
        cAnimator.SetFloat("VelocityZ", 10 * cSettings.MovementSpeed);
        if (Input.GetAxis("Horizontal") < 0)
        {
            cRigidbody.AddForce(Vector3.left * 10 * cSettings.MovementSpeed);
        }
        else if (Input.GetAxis("Horizontal") == 0)
        {
            //NOP
            cRigidbody.velocity = new Vector3(0f, cRigidbody.velocity.y, cRigidbody.velocity.z);
        }
        else
        {
            cRigidbody.AddForce(Vector3.right * 10 * cSettings.MovementSpeed);
        }
        if (Input.GetAxis("Jump") > 0 && isGrounded)
        {
            cAnimator.SetFloat("VelocityZ", moveForce.z);
            cAnimator.SetFloat("VelocityX", moveForce.x);
            cAnimator.SetTrigger("Jump");
            cRigidbody.AddForce(transform.up * cSettings.JumpPower, ForceMode.Force);
            isGrounded = false;
        }
    }
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.gameObject.CompareTag("Finish"))
        {
            //GetComponent<AudioSource>().PlayOneShot(myClips[2]);
        }
    }
    public override void restartPlayer()
    {
        base.restartPlayer();
    }
}
