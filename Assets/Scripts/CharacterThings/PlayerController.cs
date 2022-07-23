using UnityEngine;

public class PlayerController : CharacterController
{
    [SerializeField]
    private MotionCapturer mCapture;

    private Vector3 relativeVelocity;

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

        configureVelocity();


    }
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.gameObject.CompareTag("Finish"))
        {
            //GetComponent<AudioSource>().PlayOneShot(myClips[2]);
        }
    }
    private void configureVelocity()
    {
        relativeVelocity = cRigidbody.velocity;

        relativeVelocity.z = 10 * cSettings.MovementSpeed;
        cAnimator.SetFloat("VelocityZ", relativeVelocity.z);

        if (mCapture.getCurrentMotion().Equals(MotionType.MOVEMENT))
        {
            mCapture.signalMotion();
            relativeVelocity.x = mCapture.getHorizontalMovementForce() * cSettings.MaxHorizontalSpeed;
        }

        if (mCapture.getCurrentMotion().Equals(MotionType.TAP) && isGrounded)
        {
            //msManager.playSfx(SfxType.PLAYER_JUMP);
            mCapture.signalMotion();
            float jumpPower = Mathf.Sqrt(2 * cSettings.MaxJumpDistance * -Physics.gravity.y);
            isGrounded = false;
            relativeVelocity.y += jumpPower;

            cAnimator.SetFloat("VelocityZ", moveForce.z);
            cAnimator.SetFloat("VelocityX", moveForce.x);
            cAnimator.SetTrigger("Jump");

        }
        cRigidbody.velocity = relativeVelocity;
    }
    public override void restartPlayer()
    {
        base.restartPlayer();
    }
}
