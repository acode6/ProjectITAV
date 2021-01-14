using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
    Camera cam;
    public float speed = 10f;
    public float zoomSpeed = 10f;
    public float rotationSpeed = 20f;
    public GAgent displayAgent;
    // Start is called before the first frame update
    void Start()
    {
        cam = this.GetComponentInChildren<Camera>();
        cam.gameObject.transform.LookAt(this.transform.position);
        displayAgent = null;
    }

    // Update is called once per frame
    void Update()
    {
        float translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float translation2 = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        if(displayAgent == null)
        {
            this.transform.Translate(0, 0, translation);
            this.transform.Translate(translation2, 0, 0);
        }
        else
        {
            this.transform.position = displayAgent.transform.position;
        }
        

        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);

        if (Input.GetKey(KeyCode.E))
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)

            cam.gameObject.transform.Translate(0, 0, zoomSpeed * Time.deltaTime);
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            cam.gameObject.transform.Translate(0, 0, -zoomSpeed * Time.deltaTime);
        if (Input.GetMouseButton(0))
        {
            cam.gameObject.transform.Translate(Vector3.up * speed * Time.deltaTime);
            cam.gameObject.transform.LookAt(this.transform.position);
        }
        if (Input.GetMouseButton(1))
        {
            cam.gameObject.transform.Translate(-Vector3.up * speed * Time.deltaTime);
            cam.gameObject.transform.LookAt(this.transform.position);
        }
    }
}
