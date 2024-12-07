using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveArrowUpAndDown : MonoBehaviour
{
    public float amplitude = 1f; // Height of the movement
    public float speed = 1f;     // Speed of the movement

    private Vector3 startPosition;

    void Start()
    {
        // Store the starting position of the object
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the new position
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
