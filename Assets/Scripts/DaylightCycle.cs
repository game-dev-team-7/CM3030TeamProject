using UnityEngine;

public class DaylightCycle : MonoBehaviour
{
    private Vector3 rot = Vector3.zero;

    // Total rotation from sunrise to sunset (typically 180 degrees)
    private static float totalRotation = 180.0f;

    // Time for complete rotation in seconds 
    private static float totalTime = 180.0f;

    // Calculate degrees per second
    private static float degpersec = totalRotation / totalTime;

    // Update is called once per frame
    private void LateUpdate()
    {
        //calculates the actual speed by multiplying the degrees amount per Delta t
        rot.x = degpersec * Time.deltaTime;
        transform.Rotate(rot, Space.World);
    }
}