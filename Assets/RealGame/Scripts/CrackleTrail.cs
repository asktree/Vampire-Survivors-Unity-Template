using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackleTrail : MonoBehaviour
{
    // Configuration parameters
    public int numberOfPoints = 20;
    public Shader trailShader;
    public float maxTangentialOffset = 0.5f;
    public float trailLength = 2f;
    public float updateDistance = 0.1f;
    public float updateDistanceVariance = 0.02f;
    public Color trailColor = Color.cyan;
    public float trailThickness = 0.1f;
    public float trailThicknessMod = 1f;

    public float dissipatedness = 0f;

    // Components
    private LineRenderer lineRenderer;

    // State
    private List<Vector3> trailPoints;
    private Vector3 movementDirection;
    private Vector3 lastPosition;
    private float distanceSinceLastUpdate = 0f;
    private float currentUpdateDistance;

    // Debug
    private List<Vector3> debugPoints = new List<Vector3>();
    private Animator animator;

    private bool isDead = false;

    private void Start()
    {
        // Get the Animator component
        animator = GetComponent<Animator>();

        // Disable the Animator at start
        if (animator != null)
        {
            animator.enabled = false;
        }

        // Initialize components
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        Material trailMaterial = new Material(trailShader);
        lineRenderer.material = trailMaterial;
        Rigidbody2D parentRigidbody = GetComponentInParent<Rigidbody2D>();
        movementDirection = parentRigidbody.velocity.normalized;

        // Configure line renderer
        lineRenderer.positionCount = numberOfPoints;
        lineRenderer.startColor = trailColor;
        lineRenderer.endColor = trailColor;
        lineRenderer.startWidth = trailThickness;
        lineRenderer.endWidth = trailThickness * 0.2f;
        lineRenderer.numCapVertices = 4;
        lineRenderer.numCornerVertices = 4;
        lineRenderer.sortingOrder = 9;
        lineRenderer.textureMode = LineTextureMode.Static;
        lineRenderer.material.SetFloat("_Scale", 3f);


        // Initialize trail points
        trailPoints = new List<Vector3>(numberOfPoints);
        for (int i = 0; i < numberOfPoints; i++)
        {
            trailPoints.Add(transform.position);
        }

        lastPosition = transform.position;
        SetNewUpdateDistance();
    }

    public void Die()
    {
        if (!isDead)
        {
            isDead = true;

            // Enable the Animator and trigger the death animation
            if (animator != null)
            {
                animator.enabled = true;
                //animator.SetTrigger("Die");
            }

        }
    }

    public void Update()
    {
        lineRenderer.startColor = trailColor;
        lineRenderer.endColor = trailColor;
        lineRenderer.startWidth = trailThickness * trailThicknessMod;
        lineRenderer.endWidth = trailThickness * 0.2f * trailThicknessMod;
        // Set the _Trim, or Noise trim, parameter of this material's shader to be equal to dissipatedness
        lineRenderer.material.SetFloat("_Trim", dissipatedness);
        Vector3 currentPosition = transform.position;
        Vector3 movement = currentPosition - lastPosition;
        float distanceMoved = movement.magnitude;
        //debugPoints.Add(currentPosition);


        if (distanceMoved > 0)
        {
            movementDirection = movement.normalized;
            distanceSinceLastUpdate += distanceMoved;

            while (distanceSinceLastUpdate >= currentUpdateDistance)
            {
                Vector3 updatePosition = lastPosition + movementDirection * currentUpdateDistance;

                UpdateTrail(updatePosition);

                distanceSinceLastUpdate -= currentUpdateDistance;
                lastPosition = updatePosition;
                SetNewUpdateDistance();
            }

            lastPosition = currentPosition;
        }

        // Update line renderer
        lineRenderer.SetPositions(trailPoints.ToArray());
    }

    private void SetNewUpdateDistance()
    {
        currentUpdateDistance = updateDistance + Random.Range(-updateDistanceVariance, updateDistanceVariance);
        currentUpdateDistance = Mathf.Max(currentUpdateDistance, 0.01f); // Ensure it's not too small
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
        if (newPosition != lastPosition)
        {
            Vector2 tangent = new Vector2(-movementDirection.y, movementDirection.x);
            float randomOffset = GenerateNormalDistributedRandom() * maxTangentialOffset;
            newPoint = newPosition + (Vector3)(tangent * randomOffset);
        }
        else
        {
            // If we haven't moved, just copy the latest point
            newPoint = trailPoints[1];
        }
        trailPoints[0] = newPoint;


        // Add new point to the front

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Vector3 point in debugPoints)
        {
            Gizmos.DrawSphere(point, 0.05f);
        }
    }
}

// TODO: In the Unity Editor:
// 1. Attach this script to a child object of the bullet
// 2. Adjust the public parameters in the inspector as needed
// 3. Assign the desired shader to the trailShader field in the inspector
