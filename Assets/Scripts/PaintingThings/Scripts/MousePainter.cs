using System;
using UnityEngine;

public class MousePainter : MonoBehaviour
{
    private Camera cam;

    [Space, SerializeField]
    private MotionCapturer mCapture;
    [Space]
    public Color paintColor;

    public float radius = 1;
    public float strength = 1;
    public float hardness = 1;


    private bool isReadyBrush = false;
    private void Awake()
    {
  
    }
    private void Start()
    {
        cam = Camera.main;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().PlayerFinishRun += runningEndListener;
        GameManager.Instance.TimeUpEvent += gameInitialEndListener;
    }
    void Update()
    {

        if (mCapture.getCurrentMotion().Equals(MotionType.MOVEMENT) && isReadyBrush)
        {
            Vector3 position = mCapture.getCurrentTouch();
            Ray ray = cam.ScreenPointToRay(position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                
                //Debug.DrawRay(ray.origin, hit.point - ray.origin, Color.red, 0.2f);
                //transform.position = hit.point;
                Paintable p = hit.collider.GetComponent<Paintable>();

                if (p != null)
                {
                    //Debug.LogError(hit.transform.gameObject.name);
                    //Debug.LogError(position);
                    PaintManager.Instance.paint(p, hit.point, radius, hardness, strength, paintColor);
                }
            }
        }

    }

    public void gameInitialEndListener(object sender, EventArgs e)
    {
        isReadyBrush = false;
    }
    public void runningEndListener(object sender, EventArgs e)
    {
        isReadyBrush = true;
    }

}