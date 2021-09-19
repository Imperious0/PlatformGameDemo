using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class playerController : MonoBehaviour
{
    [SerializeField]
    private playerSettings pSettings;
    [SerializeField]
    private Transform respawnPoint;
    [SerializeField]
    public Transform destinatoin;

    private Animator pAnimator;
    private Rigidbody pRigidbody;

    private Vector3 moveForce;
    private Vector3 rotateForce;

    public bool isStop = true;
    bool isGameOver = false;
    bool isGrounded = true;

    public int destinationDistance = 0;
    [SerializeField]
    private bool isEnemy;
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
    }
    private void FixedUpdate()
    {

        if (isStop || isGameOver) 
        {
            if (isEnemy)
                GetComponent<NavMeshAgent>().isStopped = true;
            pRigidbody.velocity = new Vector3(0f, pRigidbody.velocity.y, 0f);
            pAnimator.SetFloat("VelocityZ", 0f);
            return;
        }
            destinationDistance = Mathf.FloorToInt(Vector3.Distance(destinatoin.position, this.transform.position));
        if (!isGrounded)
            return;
        if (isEnemy)
        {
            pAnimator.SetFloat("VelocityZ", GetComponent<NavMeshAgent>().desiredVelocity.magnitude);
        }
        else
        {
            Vector3 movement = new Vector3(pRigidbody.velocity.x, pRigidbody.velocity.y, 10 * pSettings.MovementSpeed);
            this.pRigidbody.velocity = movement;
            pAnimator.SetFloat("VelocityZ", 10 * pSettings.MovementSpeed);
            if (Input.GetAxis("Horizontal") < 0)
            {
                pRigidbody.AddForce(Vector3.left * 10 * pSettings.MovementSpeed);
            }
            else if (Input.GetAxis("Horizontal") == 0) 
            {
                //NOP
                pRigidbody.velocity = new Vector3(0f, pRigidbody.velocity.y, pRigidbody.velocity.z);
            }
            else
            {
                pRigidbody.AddForce(Vector3.right * 10 * pSettings.MovementSpeed);
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
        /*
        if (moveForce.sqrMagnitude > 0) 
        {
            moveForce.y = pRigidbody.velocity.y;

            pRigidbody.velocity = moveForce;
       
        }
        if(rotateForce.sqrMagnitude > 0) 
        {
            transform.Rotate((transform.up * rotateForce.x));
        }
        */
    }
    public void StopIT(bool isEnd) 
    {
        this.isGameOver = isEnd;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish")) 
        {
            this.isStop = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform") && !isGrounded)
        {
            isGrounded = true;
            
        }
        if(collision.gameObject.name == ("Terrain") || collision.gameObject.CompareTag("kObstacles")) 
        {
           
            this.transform.position = new Vector3(UnityEngine.Random.Range(respawnPoint.position.x - 3, respawnPoint.position.x + 3), UnityEngine.Random.Range(respawnPoint.position.y - 3, respawnPoint.position.y + 3), UnityEngine.Random.Range(respawnPoint.position.z - 3, respawnPoint.position.z + 3));
            this.transform.rotation = Quaternion.identity;
            isStop = false;
        }
        if (collision.gameObject.CompareTag("Obstacles")) 
        {
            // Calculate Angle Between the collision point and the player
            Vector3 dir = collision.contacts[0].point - transform.position;
            // We then get the opposite (-Vector3) and normalize it
            dir = -dir.normalized;
            // And finally we add force in the direction of dir and multiply it by force. 
            // This will push back the player
            GetComponent<Rigidbody>().AddForce(dir * 15f, ForceMode.Impulse);
        }
    }
    public void restartPlayer()
    {
        if (isEnemy)
            GetComponent<NavMeshAgent>().enabled = false;
        this.transform.position = new Vector3(UnityEngine.Random.Range(respawnPoint.position.x - 3, respawnPoint.position.x + 3), UnityEngine.Random.Range(respawnPoint.position.y - 3, respawnPoint.position.y + 3), UnityEngine.Random.Range(respawnPoint.position.z - 3, respawnPoint.position.z + 3));
        this.transform.rotation = Quaternion.identity;
        if (isEnemy) 
        {
            GetComponent<NavMeshAgent>().Warp(this.transform.position);
            
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<NavMeshAgent>().isStopped = false;
            GetComponent<NavMeshAgent>().SetDestination(this.destinatoin.position);
        }
          

        isStop = false;
        isGrounded = true;
    }
}
