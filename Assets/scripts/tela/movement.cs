using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    public float speed;

    // Start is called before the first frame update
    public float smooth = 5.0f;
    public float tiltAngle = 60.0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.forward * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.back * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            transform.Rotate(transform.right, tiltAngle, Space.World);
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            transform.Rotate(transform.right, -tiltAngle, Space.World);
        }

        if (Input.GetKey(KeyCode.Alpha3))
        {
            transform.Rotate(transform.forward, tiltAngle, Space.World);
        }

        if (Input.GetKey(KeyCode.Alpha4))
        {
            transform.Rotate(transform.forward, -tiltAngle, Space.World);
        }

        if (Input.GetKey(KeyCode.Alpha5))
        {
            transform.Rotate(transform.up, tiltAngle, Space.World);
        }

        if (Input.GetKey(KeyCode.Alpha6))
        {
            transform.Rotate(transform.up, -tiltAngle, Space.World);
        }

        /*float tiltAroundZ = Input.GetAxis("Horizontal") * tiltAngle;
        float tiltAroundX = Input.GetAxis("Vertical") * tiltAngle;
        // Rotate the cube by converting the angles into a quaternion.
        Quaternion target = Quaternion.Euler(tiltAroundX, 0, tiltAroundZ);

        // Dampen towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);*/
    }
}