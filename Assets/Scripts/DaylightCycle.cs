using UnityEngine;

public class DaylightCycle : MonoBehaviour
{
    Vector3 rot = Vector3.zero;

    //this value will control the rotational speed of the directional light(6 degrees per second)
    float degpersec = 6;

    // Update is called once per frame
    void Update()
    {
        //calculates the actual speed by multiplying the degrees amount per Delta t
        rot.x = degpersec * Time.deltaTime;
        transform.Rotate(rot, Space.World);
    }
}
