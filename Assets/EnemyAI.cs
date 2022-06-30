using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class EnemyAI : Agent
{
    [SerializeField]
    private Transform destination;
    [SerializeField]
    private Transform respawn;
    //AILearn
    private Vector3 destinationBase;
    private Vector3 respawnBase;

    [SerializeField]
    private Transform jumpableCollider;
    private Vector3 jumpableColliderBase;
    [SerializeField]
    private Transform nextPlatform;
    private Vector3 nextPlatformBase;

    //Observations

    //Constants
    [SerializeField]
    private float movementEmpower = 10f;
    [SerializeField]
    private int visionSensorRange = 16;
    [SerializeField]
    private float eagleEye = 30f;

    private float nonVisibleRange = 32767;
    [SerializeField]
    private float jumpDistance = 3.25f;

    //AI Variables
    [SerializeField]
    private Rigidbody aiBody;
    [SerializeField]
    private Animator aiAnimator;
    [SerializeField]
    private playerSettings aiSettings;

    //Variables
    [SerializeField]
    private float[] onVisionElementRanges;
    [SerializeField]
    private Transform focusObject;
    [SerializeField]
    private float focusObjectDistance;
    [SerializeField]
    private float focusObjectAngle;
    [SerializeField]
    private float focusLatency;

    [SerializeField]
    private Transform lastOnGround;

    //Triggers
    private bool isGrounded;
    private bool isDead;

    //GameVariables
    [SerializeField]
    private float timeCountDown = 30f;
    private float countDown = 0f;


    public override void Initialize()
    {
        //Initials
        destinationBase = destination.localPosition;
        respawnBase = respawn.localPosition;
        jumpableColliderBase = jumpableCollider.localPosition;
        nextPlatformBase = nextPlatform.localPosition;
        //Observations
        onVisionElementRanges = new float[visionSensorRange];

        focusObject = null;
        focusObjectDistance = nonVisibleRange;
        focusObjectAngle = nonVisibleRange;
        focusLatency = 0f;

        lastOnGround = null;

        isGrounded = false;
        isDead = false;
        //GameSettings
        countDown = timeCountDown;
    }
    public override void OnEpisodeBegin()
    {
        //Respawn Random
        this.transform.localPosition = new Vector3(UnityEngine.Random.Range(respawnBase.x - 5f, respawnBase.x + 5f), UnityEngine.Random.Range(respawnBase.y+ 0.1f, respawnBase.y + 0.5f), UnityEngine.Random.Range(respawnBase.z - 1f, respawnBase.z + 1f));
        this.transform.localRotation = Quaternion.identity;
        destination.localPosition = new Vector3(UnityEngine.Random.Range(destinationBase.x - 3f, destinationBase.x + 3f), UnityEngine.Random.Range(destinationBase.y - 0.5f, destinationBase.y + 0.5f), UnityEngine.Random.Range(destinationBase.z - 1f, destinationBase.z + 1f));
        jumpableCollider.localPosition = new Vector3(UnityEngine.Random.Range(jumpableColliderBase.x - 0.5f, jumpableColliderBase.x + 0.5f), UnityEngine.Random.Range(jumpableColliderBase.y - 0.1f, jumpableColliderBase.y + 0.1f), UnityEngine.Random.Range(jumpableColliderBase.z - 0.1f, jumpableColliderBase.z + 0.1f));
        nextPlatform.localPosition = new Vector3(UnityEngine.Random.Range(nextPlatformBase.x - 1f, nextPlatformBase.x + 1f), UnityEngine.Random.Range(nextPlatformBase.y - 0.5f, nextPlatformBase.y + 0.5f), UnityEngine.Random.Range(nextPlatformBase.z - 0.5f, nextPlatformBase.z + 0.5f));

        jumpableCollider.GetComponent<jumpablePlatformController>().deleteFromList(this.gameObject.name);

        //Observations
        focusObject = null;
        focusObjectDistance = nonVisibleRange;
        focusObjectAngle = nonVisibleRange;
        focusLatency = 0f;

        lastOnGround = null;

        isGrounded = false;
        isDead = false;

        //GameSettings
        countDown = timeCountDown;
    }
    private void FixedUpdate()
    {
        observeWorld();
        if(countDown > 0f) 
        {
            countDown -= Time.deltaTime;
        }
        else 
        {
            AddReward(-1f);
            EndEpisode();
            countDown = timeCountDown;
        }
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(destination.localPosition);
        sensor.AddObservation(aiBody.velocity.x);
        sensor.AddObservation(aiBody.velocity.z);
        sensor.AddObservation(focusObjectAngle);
        sensor.AddObservation(focusObjectDistance);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!isGrounded)
            return;

        //Continuous
        float MoveX = actions.ContinuousActions[0];
        float MoveZ = actions.ContinuousActions[1];

        //Discrete
        bool isNeedJump = actions.DiscreteActions[0] == 0 ? false : true;

        //Movement
        MoveZ = MoveZ > 0 ? (MoveZ * aiSettings.MovementSpeed) : (MoveZ * aiSettings.BackwardSpeed);

        Vector3 movement = this.transform.forward * MoveZ * movementEmpower;
        movement.y = aiBody.velocity.y;
        aiBody.velocity = movement;

        //Animator
        float velocity = MoveZ < 0 ? (movement.magnitude * -1) : movement.magnitude;
        aiAnimator.SetFloat("VelocityZ", velocity);

        //Rotation
        float rotateForce = MoveX * aiSettings.RotationSpeed * movementEmpower;
        rotateForce *= MoveZ < 0 ? -1 : 1;

        //Animator
        aiAnimator.SetFloat("VelocityX", rotateForce);

        transform.Rotate(transform.up * rotateForce, Space.Self);

        //Jumps
        if(isNeedJump && isGrounded && focusObjectDistance < jumpDistance && (focusObjectDistance != nonVisibleRange)) 
        {
            aiBody.AddForce(transform.up * aiSettings.JumpPower, ForceMode.Force);
            isGrounded = false;
            aiAnimator.SetTrigger("Jump");
        }

        //Add Some Penalty For Be Hard To AI Learn
        AddReward(-0.000005f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Finish"))
        {
            AddReward(1f);
            EndEpisode();
            return;
        }
        if (other.gameObject.CompareTag("JumpablePlatform")) 
        {
            AddReward(0.1f);
            return;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
        if (lastOnGround == null)
        {
            lastOnGround = collision.gameObject.transform;
            return;
        }
        if (collision.gameObject.CompareTag("Terrain")) 
        {
            AddReward(-1f);
            EndEpisode();
            isDead = true;
            return;
        }
        if (!collision.gameObject.name.Equals(lastOnGround.name)) 
        {
            AddReward(0.2f);
            lastOnGround = collision.gameObject.transform;
        }
    }

    private void observeWorld() 
    {
        Vector3 centerOfVision = this.transform.position + new Vector3(0f, 1f, 0f);

        Transform focusedObstacle = null;
        float focusedDistance = nonVisibleRange;
        int selectedSensor = 0;
        bool isContactAny = false;
        for (int i = 0; i < visionSensorRange; i++) 
        {

            Vector3 sensorOffset = new Vector3(2f / visionSensorRange, 0f, 0f);
            sensorOffset *= (i + 1);
            int layerMask = ~(1 << LayerMask.NameToLayer("JumpingObjects"));

            RaycastHit[] hits = Physics.RaycastAll(centerOfVision, this.transform.TransformDirection(Vector3.forward + Vector3.left + sensorOffset), eagleEye, layerMask);
            if(hits.Length == 0) 
            {
                Debug.DrawRay(centerOfVision, this.transform.TransformDirection(Vector3.forward + Vector3.left + sensorOffset) * eagleEye, Color.white);
                continue;
            }
            for (int j = 0; j < hits.Length; j++)
            {
                if (hits[j].collider.gameObject.CompareTag("JumpablePlatform") && !hits[j].collider.gameObject.GetComponent<jumpablePlatformController>().isContainsPlayer(this.gameObject.name)) 
                {
                    isContactAny = true;
                    onVisionElementRanges[i] = hits[j].distance;
                    if(hits[j].distance < focusedDistance) 
                    {
                        if(focusedDistance != nonVisibleRange)
                            Debug.DrawRay(centerOfVision, this.transform.TransformDirection(Vector3.forward + Vector3.left + new Vector3(2f / visionSensorRange, 0f, 0f) * selectedSensor) * focusedDistance, Color.red);

                        focusedObstacle = hits[j].collider.transform.gameObject.transform;
                        focusedDistance = hits[j].distance;
                        selectedSensor = i+1;

                    }
                    else 
                    {
                        Debug.DrawRay(centerOfVision, this.transform.TransformDirection(Vector3.forward + Vector3.left + sensorOffset) * hits[j].distance, Color.red);
                    }
                }
                else 
                {
                    Debug.DrawRay(centerOfVision, this.transform.TransformDirection(Vector3.forward + Vector3.left + sensorOffset) * hits[j].distance, Color.yellow);
                }
            }
        }
        if (!isContactAny)
        {
            focusObject = null;
            focusObjectAngle = nonVisibleRange;
            focusObjectDistance = nonVisibleRange;
            focusLatency += Time.deltaTime;
        }
        else 
        {
            focusObject = focusedObstacle;
            focusObjectDistance = focusedDistance;
            focusObjectAngle = Vector3.SignedAngle(this.transform.TransformDirection(Vector3.forward), (focusObject.position - this.transform.position), transform.up);
            Debug.DrawRay(centerOfVision, this.transform.TransformDirection(Vector3.forward + Vector3.left + ( new Vector3(2f / visionSensorRange, 0f, 0f) * (selectedSensor) ) ) * focusObjectDistance, Color.green);
            
        }


    }
}
