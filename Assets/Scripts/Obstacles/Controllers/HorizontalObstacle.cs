using UnityEngine;

public class HorizontalObstacle : Obstacle
{
    [SerializeField]
    private Vector3 movementOffset;
    [SerializeField][Range(0,10)]
    private float movementSpeed;

    private Transform obsTransform;
    private Vector3 initialPos;

    void Awake()
    {
        obsTransform = transform;
        initialPos = obsTransform.position;
    }

    private void FixedUpdate()
    {
        if ((initialPos - obsTransform.position).magnitude > movementOffset.magnitude)
            movementOffset *= -1f;

        obsTransform.position = Vector3.Lerp(obsTransform.position, obsTransform.position + movementOffset, movementSpeed * Time.deltaTime);
    }
}
