using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalTargeting : MonoBehaviour
{
    public float lineHeight = 9999f;
    public float lineWidth = 0.1f;
    public Color lineColor = Color.red;
    public int additionalLines = 3;
    public float maxRadius = 5f;
    public float radiusDecreasePerCircle = 0.5f; // New variable for radius decrease
    public float animationDuration = 1f;
    public Transform targetTransform; // New parameter for targeting
    public float targetingDuration = 2f; // Duration of the targeting period
    public float blinkInterval = 0.1f; // Interval for blinking during targeting

    private LineRenderer centralLine;
    private GameObject[] orbitingLineObjects;
    private LineRenderer[] orbitingLines;
    private GameObject[] circleObjects;
    private LineRenderer[] circleRenderers;
    private ParticleSystem[] circleParticleSystems;
    private ParticleSystemRenderer[] circleParticleRenderers; // New array to store particle renderers
    private bool isTargeting = true; // Flag to indicate if we're in targeting mode

    void Start()
    {
        CreateCentralLine();
        CreateOrbitingLines();
        CreateCircles();
        StartCoroutine(TargetingSequence());
        Destroy(gameObject, 13);
    }

    void CreateCentralLine()
    {
        centralLine = gameObject.AddComponent<LineRenderer>();
        centralLine.useWorldSpace = false; // Use local space
        centralLine.positionCount = 2;
        centralLine.startWidth = lineWidth;
        centralLine.endWidth = lineWidth;
        centralLine.material = new Material(Shader.Find("Sprites/Default"));
        centralLine.startColor = lineColor;
        centralLine.endColor = lineColor;
        centralLine.SetPosition(0, Vector3.zero);
        centralLine.SetPosition(1, Vector3.back * lineHeight);
    }

    void CreateOrbitingLines()
    {
        orbitingLineObjects = new GameObject[additionalLines];
        orbitingLines = new LineRenderer[additionalLines];

        for (int i = 0; i < additionalLines; i++)
        {
            orbitingLineObjects[i] = transform.Find($"OrbitingLine_{i}").gameObject;
            //orbitingLineObjects[i].transform.SetParent(transform, false);
            orbitingLines[i] = orbitingLineObjects[i].GetComponent<LineRenderer>();
            SetupLineRenderer(orbitingLines[i]);
        }
    }

    void CreateCircles()
    {
        circleObjects = new GameObject[3];
        circleRenderers = new LineRenderer[3];
        circleParticleSystems = new ParticleSystem[3];
        circleParticleRenderers = new ParticleSystemRenderer[3]; // Initialize the new array

        for (int i = 0; i < 3; i++)
        {
            circleObjects[i] = transform.Find($"Circle{i}").gameObject;
            circleObjects[i].transform.SetParent(transform, false);
            circleRenderers[i] = circleObjects[i].GetComponent<LineRenderer>();
            circleParticleSystems[i] = circleObjects[i].GetComponent<ParticleSystem>();
            circleParticleRenderers[i] = circleObjects[i].GetComponent<ParticleSystemRenderer>(); // Get the ParticleSystemRenderer component
            SetupLineRenderer(circleRenderers[i]);
            circleRenderers[i].loop = true;
        }
    }

    IEnumerator TargetingSequence()
    {
        // Disable particle renderers during targeting
        SetParticleRenderersEnabled(false);

        // Targeting period
        float elapsedTime = 0f;
        bool isVisible = true;

        while (elapsedTime < targetingDuration)
        {
            // Update position to follow target
            if (targetTransform != null)
            {
                transform.position = targetTransform.position;
            }

            // Blink effect
            isVisible = !isVisible;
            SetLineVisibility(isVisible);

            elapsedTime += blinkInterval;
            yield return new WaitForSeconds(blinkInterval);
        }

        // Ensure lines are visible after targeting
        SetLineVisibility(true);

        // Re-enable particle renderers after targeting
        SetParticleRenderersEnabled(true);

        // Switch to animation mode
        isTargeting = false;

        // Start the animation sequence
        StartCoroutine(AnimateCirclesSequentially());
    }

    void SetLineVisibility(bool visible)
    {
        centralLine.enabled = visible;
        foreach (var line in orbitingLines)
        {
            line.enabled = visible;
        }
        // Also set visibility for circle renderers
        foreach (var circleRenderer in circleRenderers)
        {
            circleRenderer.enabled = visible;
        }
    }

    // New method to enable/disable particle renderers
    void SetParticleRenderersEnabled(bool enabled)
    {
        foreach (var renderer in circleParticleRenderers)
        {
            if (renderer != null)
            {
                renderer.enabled = enabled;
            }
        }
    }

    IEnumerator AnimateCirclesSequentially()
    {
        yield return new WaitForSeconds(1f); // Wait a moment before starting the animation

        for (int circleIndex = 0; circleIndex < 3; circleIndex++)
        {
            float elapsedTime = 0f;
            float circleMaxRadius = maxRadius - (circleIndex * radiusDecreasePerCircle); // Calculate max radius for this circle
            while (elapsedTime < animationDuration)
            {
                float t = elapsedTime / animationDuration;
                float currentRadius = Mathf.Lerp(0, circleMaxRadius, t);

                if (circleIndex == 1) // Only animate orbiting lines for Circle1
                {
                    for (int i = 0; i < additionalLines; i++)
                    {
                        float angle = (360f / additionalLines) * i + t * 360f;
                        Vector3 orbitPosition = Quaternion.Euler(0, 0, angle) * Vector3.right * currentRadius; // Changed to rotate around z-axis
                        orbitingLineObjects[i].transform.localPosition = orbitPosition;
                        orbitingLines[i].SetPosition(0, Vector3.zero);
                        orbitingLines[i].SetPosition(1, Vector3.back * lineHeight);
                    }
                }

                DrawCircle(circleRenderers[circleIndex], currentRadius);
                UpdateParticleSystemRadius(circleParticleSystems[circleIndex], currentRadius);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    void SetupLineRenderer(LineRenderer lineRenderer)
    {
        lineRenderer.useWorldSpace = false; // Use local space
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.back * lineHeight);
    }

    void DrawCircle(LineRenderer renderer, float radius)
    {
        int segments = 36;
        renderer.positionCount = segments + 1;
        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * 360f * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0); // Changed to xy plane
            renderer.SetPosition(i, pos);
        }
    }

    void UpdateParticleSystemRadius(ParticleSystem particleSystem, float radius)
    {
        if (particleSystem != null)
        {
            var shape = particleSystem.shape;
            shape.radius = radius;
        }
    }

    void Update()
    {
        if (isTargeting && targetTransform != null)
        {
            transform.position = targetTransform.position;
        }

        // No need to update line positions manually each frame
        // as they are now in local space and will move with the object
        // Rotate the entire object slowly around the z-axis (which is the vertical axis in this case)
        transform.Rotate(Vector3.forward * Time.deltaTime * -15f); // Adjust the 15f to change rotation speed

        // Fun comment: Look at me, I'm spinning! Weeeee! 🌀
    }
}
