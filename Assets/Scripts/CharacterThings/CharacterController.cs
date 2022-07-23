using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    [SerializeField]
    protected playerSettings cSettings;
    [SerializeField]
    protected Transform respawnPoint;
    [SerializeField]
    protected Vector3 destinationPoint;

    protected Animator cAnimator;
    protected Rigidbody cRigidbody;

    public float destinationDistance;

    protected bool isGameOver = false;
    protected bool isFinished = false;
    protected bool isGrounded = false;
    protected bool isHit = false;
    public Vector3 DestinationPoint { get => destinationPoint; set => destinationPoint = value; }
    public bool IsFinished { get => isFinished; }

    protected virtual void Awake()
    {
        cAnimator = GetComponent<Animator>();
        cRigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void FixedUpdate()
    {

        destinationDistance = Mathf.FloorToInt(Vector3.Distance(destinationPoint, this.transform.position));
    }

    public void StopIT(bool isGameEnd)
    {
        this.isGameOver = isGameEnd;
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            this.isFinished = true;
            MusicManager.Instance.SfxHandler.playClipSelf("Finish");
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform") && !isGrounded)
        {
            isGrounded = true;
        }
        if (collision.gameObject.GetComponent(typeof(IContactable)))
        {
            IContactable go = collision.gameObject.GetComponent(typeof(IContactable)) as IContactable;

            if (go.isKiller())
            {
                restartPlayer();
            }
        }
        if (collision.gameObject.GetComponent<Obstacle>() && !isHit)
        {
            isHit = true;
            Vector3 dir = Vector3.zero;

            List<ContactPoint> cPoints = new List<ContactPoint>();
            int count = collision.GetContacts(cPoints);
            foreach (ContactPoint item in cPoints)
            {
                dir += item.point;
            }
            dir /= count;

            // We then get the opposite (-Vector3) and normalize it
            dir = -dir.normalized;
            // And finally we add force in the direction of dir and multiply it by force. 
            // This will push back the player
            cRigidbody.AddForce(dir * 25f, ForceMode.Impulse);
            MusicManager.Instance.SfxHandler.playClipSelf("Hit");
            StartCoroutine(awaitHit());
        }
    }
    
    public virtual void restartPlayer()
    {
        this.transform.position = new Vector3(UnityEngine.Random.Range(respawnPoint.position.x - 3, respawnPoint.position.x + 3), UnityEngine.Random.Range(respawnPoint.position.y - 3, respawnPoint.position.y + 3), UnityEngine.Random.Range(respawnPoint.position.z - 3, respawnPoint.position.z + 3));
        this.transform.rotation = Quaternion.identity;
        isFinished = false;
        isGrounded = true;
    }
    IEnumerator awaitHit()
    {
        yield return new WaitForSeconds(0.2f);
        isHit = false;
    }
    public void footStep()
    {
        //If Necessary Footstep Sfx Can Handled Here
        //Animation Event Handler used this.
        //MusicManager.Instance.SfxHandler.playClipSelf("Run");
    }
}
