using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackleTrail : MonoBehaviour
{
    // Configuration parameters
    public int numberOfPoints = 20;
    public float maxTangentialOffset = 0.5f;
    public float trailLength = 2f;
    public float averageUpdateInterval = 0.05f;
    public float updateIntervalVariance = 0.02f;
    public Color trailColor = Color.cyan;

    // Components
    private LineRenderer lineRenderer;

    // State
    private List<Vector3> trailPoints;
    private float timeSinceLastUpdate;
    private float currentUpdateInterval;
    private Vector3 movementDirection;
    private Vector3 lastPosition;

    private void Start()
    {
        // Initialize components
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        Rigidbody2D parentRigidbody = GetComponentInParent<Rigidbody2D>();
        movementDirection = parentRigidbody.velocity.normalized;

        // Configure line renderer
        lineRenderer.positionCount = numberOfPoints;
        lineRenderer.startColor = trailColor;
        lineRenderer.endColor = trailColor;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.02f;

        // Initialize trail points
        trailPoints = new List<Vector3>(numberOfPoints);
        for (int i = 0; i < numberOfPoints; i++)
        {
            trailPoints.Add(transform.position);
        }

        lastPosition = transform.position;
        SetNewUpdateInterval();
    }

    public void Update()
    {
        Vector3 currentPosition = transform.position;
        Vector3 movement = currentPosition - lastPosition;
        float distanceMoved = movement.magnitude;

        if (distanceMoved > 0)
        {
            movementDirection = movement.normalized;
            float remainingDistance = distanceMoved;

            while (remainingDistance > 0 || timeSinceLastUpdate >= currentUpdateInterval)
            {
                float stepDistance = Mathf.Min(remainingDistance, currentUpdateInterval * distanceMoved / Time.deltaTime);
                Vector3 stepPosition = lastPosition + movementDirection * stepDistance;

                UpdateTrail(stepPosition);

                remainingDistance -= stepDistance;
                timeSinceLastUpdate -= currentUpdateInterval;
                SetNewUpdateInterval();
            }

            lastPosition = currentPosition;
        }


        timeSinceLastUpdate += Time.deltaTime;

        // Update line renderer
        lineRenderer.SetPositions(trailPoints.ToArray());
    }

    private void SetNewUpdateInterval()
    {
        currentUpdateInterval = averageUpdateInterval + Random.Range(-updateIntervalVariance, updateIntervalVariance);
        currentUpdateInterval = Mathf.Max(currentUpdateInterval, 0.01f); // Ensure it's not too small
    }

    private void UpdateTrail(Vector3 newPosition)
    {
        // Shift all points back
        for (int i = numberOfPoints - 1; i > 0; i--)
        {
            trailPoints[i] = trailPoints[i - 1];
        }

        // Calculate new point
        Vector3 newPoint;
        if (newPosition != trailPoints[0])
        {
            Vector2 tangent = new Vector2(-movementDirection.y, movementDirection.x);
            float randomOffset = GenerateNormalDistributedRandom() * maxTangentialOffset;
            newPoint = newPosition + (Vector3)(tangent * randomOffset);
        }
        else
        {
            // If we haven't moved, just copy the latest point
            newPoint = trailPoints[0];
        }

        // Add new point to the front
        trailPoints[0] = newPoint;

        // Ensure trail length
        /* float totalDistance = 0f;
        for (int i = 1; i < numberOfPoints; i++)
        {
            totalDistance += Vector3.Distance(trailPoints[i], trailPoints[i - 1]);
            if (totalDistance > trailLength)
            {
                lineRenderer.positionCount = i;
                break;
            }
        } */
    }

    private float GenerateNormalDistributedRandom()
    {
        float u1 = 1.0f - Random.value; // Uniform(0,1] random number
        float u2 = 1.0f - Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        return randStdNormal;
    }
}

// TODO: In the Unity Editor:
// 1. Attach this script to a child object of the bullet
// 2. Adjust the public parameters in the inspector as needed
// 3. You may want to add a material to the Line Renderer for better visual effect
