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
    public float animationDuration = 1f;

    private LineRenderer centralLine;
    private GameObject[] orbitingLineObjects;
    private LineRenderer[] orbitingLines;
    private GameObject circleObject;
    private LineRenderer circleRenderer;

    void Start()
    {
        CreateCentralLine();
        CreateOrbitingLines();
        StartCoroutine(AnimateOrbitalLines());
    }

    void CreateCentralLine()
    {
        centralLine = gameObject.AddComponent<LineRenderer>();
        centralLine.positionCount = 2;
        centralLine.SetPosition(0, Vector3.zero);
        centralLine.SetPosition(1, Vector3.up * lineHeight);
        centralLine.startWidth = lineWidth;
        centralLine.endWidth = lineWidth;
        centralLine.material = new Material(Shader.Find("Sprites/Default"));
        centralLine.startColor = lineColor;
        centralLine.endColor = lineColor;
    }

    void CreateOrbitingLines()
    {
        orbitingLineObjects = new GameObject[additionalLines];
        orbitingLines = new LineRenderer[additionalLines];

        circleObject = new GameObject("CircleRenderer");
        circleObject.transform.SetParent(transform);
        circleRenderer = circleObject.AddComponent<LineRenderer>();

        for (int i = 0; i < additionalLines; i++)
        {
            orbitingLineObjects[i] = new GameObject($"OrbitingLine_{i}");
            orbitingLineObjects[i].transform.SetParent(transform);
            orbitingLines[i] = orbitingLineObjects[i].AddComponent<LineRenderer>();
            SetupLineRenderer(orbitingLines[i]);
        }

        SetupLineRenderer(circleRenderer);
        circleRenderer.loop = true;
    }

    IEnumerator AnimateOrbitalLines()
    {
        yield return new WaitForSeconds(1f); // Wait a moment before starting the animation

        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            float currentRadius = Mathf.Lerp(0, maxRadius, t);

            for (int i = 0; i < additionalLines; i++)
            {
                float angle = (360f / additionalLines) * i + t * 360f;
                Vector3 orbitPosition = Quaternion.Euler(0, angle, 0) * Vector3.forward * currentRadius;
                orbitingLineObjects[i].transform.position = orbitPosition;
            }

            DrawCircle(currentRadius);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    void SetupLineRenderer(LineRenderer lineRenderer)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }

    void DrawCircle(float radius)
    {
        int segments = 36;
        circleRenderer.positionCount = segments + 1;
        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * 360f * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Sin(angle) * radius, 0, Mathf.Cos(angle) * radius);
            circleRenderer.SetPosition(i, pos);
        }
    }

    void Update()
    {
        // Rotate the entire object slowly
        float rotationSpeed = 30f; // Adjust this value to change rotation speed
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        // Update line positions to match their game objects
        for (int i = 0; i < additionalLines; i++)
        {
            Vector3 lineStart = orbitingLineObjects[i].transform.position;
            orbitingLines[i].SetPosition(0, lineStart);
            orbitingLines[i].SetPosition(1, lineStart + Vector3.up * lineHeight);
        }
    }
}
