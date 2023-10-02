using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    public CameraFollowScript _camera;
    public GameObject needle;
    private CarController _controller;

    private float startPosition = 220f, endPosition = -41f;
    private float desiredPosition;
    public float carSpeed;
    // Start is called before the first frame update
    void Start()
    {
        _controller = _camera.Player.GetComponent<CarController>();   
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            carSpeed = _controller.LocalVelocity.magnitude;
            UpdateNeedle();
        }
        catch { }
    }
    private void UpdateNeedle()
    {
        desiredPosition = startPosition - endPosition;
        float temp = carSpeed / 195f;
        needle.transform.eulerAngles = new Vector3(0, 0, (startPosition - temp * desiredPosition));

    }
}
