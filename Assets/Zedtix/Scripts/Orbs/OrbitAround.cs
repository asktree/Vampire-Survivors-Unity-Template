using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitAround : MonoBehaviour
{
    private Transform target; // The object to circle around.
    public float orbitSpeed = 150.0f; // Adjust this value to change the circle speed.
    public float circleRadius = 1.0f; // Adjust this value to set the radius of the circle.

    public float angle = 0;

    private void Start()
    {
        target = transform.parent;
    }
    void Update()
    {
        // Calculate the position on the circle based on the angle.
        float x = target.position.x + circleRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = target.position.y + circleRadius * Mathf.Sin(angle * Mathf.Deg2Rad);

        // Update the position of the orbiting object.
        transform.position = new Vector3(x, y, transform.position.z);

        // Increment the angle to make the object move in a circle.
        angle += orbitSpeed * Time.deltaTime;
    }
}