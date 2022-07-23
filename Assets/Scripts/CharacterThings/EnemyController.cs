using UnityEngine;
using UnityEngine.AI;

public class EnemyController : CharacterController
{
    [SerializeField]
    private NavMeshAgent nmAgent;

    protected override void Awake()
    {
        base.Awake();
        nmAgent = GetComponent<NavMeshAgent>();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isHit)
            return;
        if (isFinished || isGameOver)
        {

            nmAgent.isStopped = true;
            cRigidbody.velocity = new Vector3(0f, cRigidbody.velocity.y, 0f);
            cAnimator.SetFloat("VelocityZ", 0f);
            return;
        }
        destinationDistance = Mathf.FloorToInt(Vector3.Distance(destinationPoint, this.transform.position));
        if (!isGrounded)
            return;

        cAnimator.SetFloat("VelocityZ", nmAgent.desiredVelocity.magnitude);


    }
    public override void restartPlayer()
    {
        nmAgent.enabled = false;

        nmAgent.Warp(this.transform.position);
        nmAgent.enabled = true;
        nmAgent.isStopped = false;
        nmAgent.SetDestination(destinationPoint);

        base.restartPlayer();
    }
    
}
