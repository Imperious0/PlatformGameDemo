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
    [SerializeField]
    private playerSettings aiSettings;

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

    float timerForDeadline = 30f;
    float initialdistance;
    float lastDistance = 0;

    [SerializeField]
    float obstacleDistance = 0;
    [SerializeField]
    float obstacleAngle = 0;

    bool isDead = true;
    bool isGrounded = true;
    private void Start()
    {
        nextObstaclesOffsets = new float[16];
        nextObstacle = null;
        aiRigidbody = this.GetComponent<Rigidbody>();
        aiAnimator = this.GetComponent<Animator>();
        initialdistance = Vector3.Distance(this.transform.localPosition , destinationPosition.localPosition);
    }
    private void FixedUpdate()
    {
        lastDistance = Vector3.Distance(this.transform.localPosition, destinationPosition.localPosition);
        bool isFirstScan = true;
        for(int i = 0; i < 16; i++) 
        {
            Vector3 tmp = new Vector3(2f / 16f, 0f, 0f);
            tmp *= (i + 1);
            
            if (Physics.Raycast(this.transform.localPosition, this.transform.TransformDirection(Vector3.forward + (Vector3.left + tmp)), out hit, 30f))
            {

                if (hit.transform.gameObject.CompareTag("JumpablePlatform"))
                {
                    nextObstaclesOffsets[i] = hit.distance;
                    if(isFirstScan)
                    {
                        nextObstacle = hit.transform.gameObject.transform;
                        obstacleAngle = Vector3.SignedAngle(this.transform.TransformDirection(Vector3.forward), this.transform.TransformDirection(Vector3.forward + (Vector3.left + tmp)), transform.up);
                        obstacleDistance = hit.distance;
                        isFirstScan = false;
                    }
                    else 
                    {
                        if(hit.distance < obstacleDistance) 
                        {
                            nextObstacle = hit.transform.gameObject.transform;
                            obstacleAngle = Vector3.SignedAngle(this.transform.TransformDirection(Vector3.forward), this.transform.TransformDirection(Vector3.forward + (Vector3.left + tmp)), transform.up);
                            obstacleDistance = hit.distance;
                            Debug.DrawRay(this.transform.localPosition, this.transform.TransformDirection(Vector3.forward), Color.green);
                            Debug.DrawRay(this.transform.localPosition + new Vector3(0f, 1f, 0f), this.transform.TransformDirection(Vector3.forward + (Vector3.left + tmp)) * hit.distance, Color.black);
                        }
                    }
                    
                    
                    Debug.DrawRay(this.transform.localPosition, transform.TransformDirection(Vector3.forward + (Vector3.left + tmp)) * hit.distance, Color.yellow);

                }else if(hit.transform == null) 
                {
                    nextObstaclesOffsets[i] = -1f;
                }
            }
            else 
            {
                Debug.DrawRay(this.transform.localPosition, transform.TransformDirection(Vector3.forward + (Vector3.left + tmp)) * Mathf.Infinity, Color.red);
            }
        }

        if (timerForDeadline > 0f) 
        {
            timerForDeadline -= Time.deltaTime;
        }else
        {
            timerForDeadline = 30f;
            
            if (lastDistance < initialdistance)
                if(!isDead)
                    SetReward(0.5f);
            initialdistance = lastDistance;
            EndEpisode();
        }
    }
    public override void OnEpisodeBegin()
    {
        this.transform.localPosition = new Vector3(UnityEngine.Random.Range(respawnPoint.position.x - 3, respawnPoint.position.x + 3), UnityEngine.Random.Range(respawnPoint.position.y - 3, respawnPoint.position.y + 3), UnityEngine.Random.Range(respawnPoint.position.z - 3, respawnPoint.position.z + 3));
        this.transform.localRotation = Quaternion.identity;
        isDead = false;
        timerForDeadline = 30f;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(destinationPosition.localPosition);
        sensor.AddObservation(aiRigidbody.velocity);
        sensor.AddObservation(lastDistance);
        sensor.AddObservation(obstacleDistance);
        sensor.AddObservation(obstacleAngle);
    }
   
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        needJump = Mathf.FloorToInt(actions.ContinuousActions[2]) == 0 ? false : true;

        if (nextObstacle == null)
            SetReward(-0.0001f);
        if(obstacleAngle < -10 && obstacleAngle > 10) 
        {
            SetReward(-0.0001f);
        }


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

        if (needJump && isGrounded && obstacleDistance < 3.25f)
        {
            SetReward(0.5f);
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
            SetReward(0.3f);
            EndEpisode();
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
        if (collision.gameObject.name.Equals("Terrain"))
        {
            isDead = true;
            SetReward(-1);
            EndEpisode();
        }
    }
}
