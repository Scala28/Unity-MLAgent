using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{

    public GameObject Player;
    public GameObject Child;
    public float speed;
    // Start is called before the first frame update
    void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("CameraTarget");
        Child = Player.transform.Find("camera constraint").gameObject;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        try
        {
            gameObject.transform.position = Vector3.Lerp(transform.position, Child.transform.position, Time.deltaTime * speed);
            gameObject.transform.LookAt(Player.gameObject.transform.position);
        }
        catch
        {
            Player = GameObject.FindGameObjectWithTag("CameraTarget");
            Child = Player.transform.Find("camera constraint").gameObject;
        }
    }
}
