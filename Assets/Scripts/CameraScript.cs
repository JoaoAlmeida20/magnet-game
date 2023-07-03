using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject followTarget;
    public float minSpeedIncrease;
    public float maxSizeMultiplier;

    Camera cam;
    float defaultSize;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        defaultSize = cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 velocity = followTarget.GetComponent<Rigidbody2D>().velocity;
        transform.position = new Vector3(followTarget.transform.position.x, followTarget.transform.position.y, transform.position.z);
        float speed = Mathf.Abs(velocity.x);

        if (speed >= minSpeedIncrease) {
            float sizeMultiplier = Mathf.Min(maxSizeMultiplier, speed / minSpeedIncrease);
            Debug.Log("Speed: " + speed + " SizeMultiplier: " + sizeMultiplier);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, defaultSize * sizeMultiplier, 4.0f * Time.deltaTime);
        }
        else {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, defaultSize, 4.0f * Time.deltaTime);
        }
    }
}
