using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

public class CarAgent : Agent
{
    [SerializeField] private CarController _controller;
    [SerializeField] private TrackCheckPoints _track;

    private float cpReward;

    private Transform SpawnPosition;
    // Start is called before the first frame update
    private void Start()
    {
        _controller = GetComponent<CarController>();
        SpawnPosition = _track.transform.Find("SpawnPosition").transform;
        _track.OnCarCorrectCheckPoint += TrackCheckPoints_OnCArCorrectCheckPoint;
        _track.OnCarWrongCheckPoint += TrackCheckPoints_OnCArWrongCheckPoint;

        cpReward = 1f / _track.TotCheckPoint();
    }
    private void TrackCheckPoints_OnCArCorrectCheckPoint(object sender, CarThroughtCheckPointEventArgs e)
    {
        if(e.carPosition == transform)
        {
            AddReward(cpReward);
            if (e.finished)
            {
                Debug.Log(GetCumulativeReward());
                
                EndEpisode();
            }
        }
    }
    private void TrackCheckPoints_OnCArWrongCheckPoint(object sender, CarThroughtCheckPointEventArgs e)
    {
        if (e.carPosition == transform)
        {
            AddReward(-cpReward * 3);
            EndEpisode();
        }
    }
    public override void OnEpisodeBegin()
    {
        _controller.SetPosition(SpawnPosition.position + new Vector3(UnityEngine.Random.Range(-2.5f, +2.5f), 0, 0));
        transform.forward = SpawnPosition.forward;
        _track.ResetCheckPoints(transform);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Transform nextCheckPoint = _track.GetNextCheckPointPosition(transform);
        Vector2 carDirection = new Vector2(transform.forward.x, transform.forward.z);
        Vector2 cpDirection = new Vector2(nextCheckPoint.forward.x, nextCheckPoint.forward.z);
        float directionDot = Vector2.Dot(cpDirection, carDirection);
        sensor.AddObservation(directionDot);
        sensor.AddObservation(_controller.LocalVelocity.magnitude);
        sensor.AddObservation(_controller.ForwardSpeed);
        sensor.AddObservation((nextCheckPoint.position - transform.position).sqrMagnitude);
        AddReward(_controller.LocalVelocity.magnitude * (1f / 200) * (1f / MaxStep) * directionDot * _controller.ForwardSpeed);
        AddReward(-1f / MaxStep);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = 0f;
        float steerAmount = 0f;

        //REINFORCEMENT
        //switch (actions.DiscreteActions[0])
        //{
        //    case 0: forwardAmount =  0f; break;
        //    case 1: forwardAmount = .5f; break;
        //    case 2: forwardAmount = +1f; break;
        //    case 3: forwardAmount = -1f; break;
        //}
        //switch (actions.DiscreteActions[1])
        //{
        //    case 0: steerAmount = 0f; break;
        //    case 1: steerAmount = .5f; break;
        //    case 2: steerAmount = +1f; break;
        //    case 3: steerAmount = -.5f; break;
        //    case 4: steerAmount = -1f; break;
        //}
        //RL + IM
        forwardAmount = actions.ContinuousActions[0];
        steerAmount = actions.ContinuousActions[1];
        _controller.SetInputs(forwardAmount, steerAmount);
        //Debug.Log(forwardAmount);

    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        float forwardAction = 0;
        //if (Input.GetKey(KeyCode.UpArrow)) forwardAction = 2;
        //if (Input.GetKey(KeyCode.DownArrow)) forwardAction = 3;
        forwardAction = Input.GetAxis("Vertical");

        float turnActiom = 0;
        //if (Input.GetKey(KeyCode.RightArrow)) turnActiom = 2;
        //if (Input.GetKey(KeyCode.LeftArrow)) turnActiom = 4;
        turnActiom = Input.GetAxis("Horizontal");

        ActionSegment<float> actions = actionsOut.ContinuousActions;
        actions[0] = forwardAction;
        actions[1] = turnActiom;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Environment"))
        {
            AddReward(-.025f);
            //EndEpisode();
        }else if (other.CompareTag("Wall"))
        {
            AddReward(-.025f);
            //EndEpisode();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Environment"))
        {
            AddReward(-.05f/60);
        }else if (other.CompareTag("Wall"))
        {
            AddReward(-.05f/60);
        }
    }
}
