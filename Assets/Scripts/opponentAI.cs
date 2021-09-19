using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.AI;
using Unity.MLAgents.Sensors;

public class opponentAI : Agent {
    [SerializeField]
    private Transform respawnPoint;
    [SerializeField]
    private Transform destinationPosition;
    private Vector3 destinationTmpPos;
    [SerializeField]
    private playerSettings aiSettings;
    [SerializeField]
    private GameObject hitCollider;

    [SerializeField]
    private Transform nextObstacle;
    [SerializeField]
    private float[] nextObstaclesOffsets;

    RaycastHit hit;
    private Rigidbody aiRigidbody;
    private Animator aiAnimator;
    private Vector3 movementForce;
    private Vector3 rotatingForce;
    bool needJump = false;

    float initialDeadline = 30f;
    float timerForDeadline;

    float initialdistance;
    float lastDistance = 0;

    [SerializeField]
    float obstacleDistance = 0;
    [SerializeField]
    float obstacleAngle = 0;
    [SerializeField]
    float obstacleDetectionTime = 0;
    [SerializeField]
    int obstacleCollisonCount = 0;
    Transform lastBox;
    bool isDead = true;
    bool isGrounded = true;
    private void Start()
    {
        timerForDeadline = initialDeadline;
        nextObstaclesOffsets = new float[16];
        nextObstacle = null;
        lastBox = null;
        aiRigidbody = this.GetComponent<Rigidbody>();
        aiAnimator = this.GetComponent<Animator>();
        initialdistance = Vector3.Distance(this.transform.localPosition , destinationPosition.localPosition);
        destinationTmpPos = destinationPosition.position;
    }
    private void FixedUpdate()
    {
        bool isFirstScan = true;
        for (int i = 0; i < 16; i++) 
        {
            Vector3 tmp = new Vector3(2f / 16f, 0f, 0f);
            tmp *= (i + 1);
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = ~(1 << LayerMask.NameToLayer("JumpingObjects"));


            if (Physics.Raycast(this.transform.position + new Vector3(0, 1f, 0f), this.transform.TransformDirection(Vector3.forward + (Vector3.left + tmp)), out hit, 30f, layerMask))
            {
                if (hit.collider.gameObject.CompareTag("JumpablePlatform") && !hit.collider.gameObject.GetComponent<jumpablePlatformController>().isContainsPlayer(this.gameObject.name))
                {
                    nextObstaclesOffsets[i] = hit.distance;
                    if(isFirstScan)
                    {
                        nextObstacle = hit.collider.transform.gameObject.transform;
                        //obstacleAngle = Vector3.SignedAngle(this.transform.TransformDirection(Vector3.forward), this.transform.TransformDirection(Vector3.forward + (Vector3.left + tmp)), transform.up);
                        obstacleAngle = Vector3.SignedAngle(this.transform.TransformDirection(Vector3.forward), (nextObstacle.position - this.transform.position), transform.up);
                        obstacleDistance = hit.distance;
                        isFirstScan = false;
                    }
                    else 
                    {
                        if(hit.distance < obstacleDistance) 
                        {
                            nextObstacle = hit.collider.transform.gameObject.transform;
                            obstacleAngle = Vector3.SignedAngle(this.transform.TransformDirection(Vector3.forward), (nextObstacle.position - this.transform.position), transform.up);
                            obstacleDistance = hit.distance;
                            Debug.DrawRay(this.transform.position + new Vector3(0, 1f, 0f), this.transform.TransformDirection(Vector3.forward), Color.green);
                            Debug.DrawRay(this.transform.position + new Vector3(0f, 1f, 0f), this.transform.TransformDirection(Vector3.forward + (Vector3.left + tmp)) * hit.distance, Color.black);
                        }
                    }
                    
                    Debug.DrawRay(this.transform.position + new Vector3(0, 1f, 0f), transform.TransformDirection(Vector3.forward + (Vector3.left + tmp)) * hit.distance, Color.yellow);
                }
                else if(hit.transform == null) 
                {
                    nextObstaclesOffsets[i] = -1f;
                    Debug.DrawRay(this.transform.position + new Vector3(0, 1f, 0f), transform.TransformDirection(Vector3.forward + (Vector3.left + tmp)) * Mathf.Infinity, Color.red);
                }
                Debug.DrawRay(this.transform.position + new Vector3(0, 1.1f, 0f), transform.TransformDirection(Vector3.forward + (Vector3.left + tmp)) * Mathf.Infinity, Color.white);
            }
            else 
            {

            }
        }
        if(isFirstScan)
        {
            nextObstacle = null;
            obstacleAngle = 32767;
            obstacleDistance = 32767;
            obstacleDetectionTime = 0;
        }
        else
        {
            obstacleDetectionTime += Time.deltaTime;
        }
        if (timerForDeadline > 0f) 
        {
            timerForDeadline -= Time.deltaTime;
        }else
        {
            timerForDeadline = initialDeadline;
            if(Vector3.Distance(this.destinationPosition.localPosition , this.transform.localPosition) < initialdistance && !isDead) 
            {
                initialdistance = Vector3.Distance(this.destinationPosition.localPosition ,this.transform.localPosition);
            }
            else 
            {
            }
            SetReward(-1f);
            EndEpisode();
        }
    }
    public override void OnEpisodeBegin()
    {
        this.transform.localPosition = new Vector3(UnityEngine.Random.Range(respawnPoint.localPosition.x - 4, respawnPoint.localPosition.x + 4), UnityEngine.Random.Range(respawnPoint.localPosition.y - 4, respawnPoint.localPosition.y + 4), respawnPoint.localPosition.z);
        this.transform.localRotation = Quaternion.identity;
        destinationPosition.transform.localPosition = new Vector3(UnityEngine.Random.Range(destinationTmpPos.x - 1, destinationTmpPos.x + 1), UnityEngine.Random.Range(destinationTmpPos.y - 0.5f, destinationTmpPos.y + 0.5f), UnityEngine.Random.Range(destinationTmpPos.z - 0.5f, destinationTmpPos.z + 0.5f));
        isDead = false;
        timerForDeadline = initialDeadline;
        lastBox = null;
        hitCollider.GetComponent<jumpablePlatformController>().deleteFromList(this.gameObject.name);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(destinationPosition.localPosition);
        sensor.AddObservation(destinationPosition.localPosition.y);
        sensor.AddObservation(this.transform.rotation.y);
        sensor.AddObservation(initialdistance);
        sensor.AddObservation(obstacleAngle);
        sensor.AddObservation(obstacleDistance);
    }
   
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!isGrounded)
            return;

        lastDistance = obstacleDistance;

        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        bool isNeedJump = actions.DiscreteActions[0] == 0 ? false : true;
        initialdistance = Vector3.Distance(this.destinationPosition.localPosition, this.transform.localPosition);

        AddReward(-0.00002f);
        if (moveZ < 0)
            movementForce = transform.forward * moveZ * 10 * aiSettings.BackwardSpeed;
        else
            movementForce = transform.forward * moveZ * 10 * aiSettings.MovementSpeed;
        float velocity = moveZ > 0 ? movementForce.magnitude : (movementForce.magnitude * -1);
        aiAnimator.SetFloat("VelocityZ", velocity);

        movementForce.y = aiRigidbody.velocity.y;
        aiRigidbody.velocity = movementForce;

        rotatingForce.x = moveX * 10 * aiSettings.RotationSpeed;
        velocity = moveX > 0 ? rotatingForce.magnitude : (rotatingForce.magnitude * -1);

        //Backward Movement Rotating
        rotatingForce *= moveZ < 0 ? -1 : 1;
        velocity = moveZ < 0 ? velocity * -1 : velocity;

        aiAnimator.SetFloat("VelocityX", velocity);

        transform.Rotate(transform.up * rotatingForce.x, Space.Self);

        if (isNeedJump && isGrounded && obstacleDistance < 3.25f && obstacleDistance > 0)
        {
            aiRigidbody.AddForce(transform.up * aiSettings.JumpPower, ForceMode.Force);
            isGrounded = false;
        }


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("FinishCube")) 
        {
            SetReward(1);
            EndEpisode();
        }
        if(other.gameObject.CompareTag("JumpablePlatform"))
        {
            AddReward(0.01f);
            nextObstacle = null;
            obstacleAngle = 32767;
            obstacleDistance = 32767;
            other.gameObject.GetComponent<jumpablePlatformController>().addMeToList(this.gameObject.name);
        }

    }
    private void OnCollisionStay(Collision collision)
    {
    }
    private void OnCollisionEnter(Collision collision)
    {
        
        isGrounded = true;
        if(lastBox == null) 
        {
            lastBox = collision.gameObject.transform;
        }
        if (!collision.gameObject.name.Equals(lastBox.gameObject.name) && !collision.gameObject.name.Equals("Terrain"))
        {
            lastBox = collision.gameObject.transform;
         
            AddReward(0.2f);
         
        }

        if (collision.gameObject.name.Equals("Terrain"))
        {
            isDead = true;
            AddReward(-1);
            EndEpisode();
        }
    }
}
