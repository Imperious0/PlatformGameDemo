using UnityEngine;
public class cameraController : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float followSpeed;
    [SerializeField]
    private Vector3 cameraOffset;

    private Transform currentTransform;

    private Vector3 targetPos;
    private Vector3 smoothedPosition;

    private void Start()
    {
        currentTransform = transform;
        targetPos = target.position + cameraOffset;
        smoothedPosition = Vector3.Lerp(currentTransform.position, targetPos, followSpeed);
        GameManager.Instance.PhaseChangeEvent += gamePhaseChecker;
    }
    private void FixedUpdate()
    {
        
        if ((target.position + cameraOffset - currentTransform.position).sqrMagnitude < 0.1f) 
        {
            return;
        }
 
        targetPos = target.position + cameraOffset;
        smoothedPosition = Vector3.Lerp(currentTransform.position, targetPos, followSpeed);
        currentTransform.position = smoothedPosition;


    }

    public void gamePhaseChecker(object sender, GamePhaseChangeEventArgs e)
    {
        target = e.CurrentCameraTrack;
        cameraOffset = e.CurrentTrackOffset;
    }
}
