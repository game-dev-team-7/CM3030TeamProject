using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerTwoMovement : MonoBehaviour
{
    Camera cam;
    Rigidbody rb;

    [SerializeField]
    float movementSpeed;
    // Start is called before the first frame update

    void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        float forward = Input.GetAxisRaw("Vertical");
        float side = Input.GetAxisRaw("Horizontal");

        Vector3 newPos = new Vector3(side, 0, forward) * movementSpeed * Time.deltaTime;

        rb.MovePosition(transform.position + newPos);

        if(forward == 0 && side == 0)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }
}
