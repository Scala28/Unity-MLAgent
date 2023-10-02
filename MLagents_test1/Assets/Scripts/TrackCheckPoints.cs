using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrackCheckPoints : MonoBehaviour
{
    private List<CheckPoint> checkPoints;
    private List<Transform> CheckPointsPos;
    [SerializeField]
    private List<Transform> cars;

    private List<int> nextCheckPointIndexes;
    private List<int> lapsDone;

    public event EventHandler<CarThroughtCheckPointEventArgs> OnCarCorrectCheckPoint;
    public event EventHandler<CarThroughtCheckPointEventArgs> OnCarWrongCheckPoint;

    public int laps;
    private void Awake()
    {
        Transform checkPointsManager = transform.Find("CheckPointManager");
        checkPoints = new List<CheckPoint>();
        CheckPointsPos = new List<Transform>();
        nextCheckPointIndexes = new List<int>();
        lapsDone = new List<int>();
        foreach(Transform checkPoint in checkPointsManager)
        {
            CheckPointsPos.Add(checkPoint);
            CheckPoint cp = checkPoint.GetComponent<CheckPoint>();
            cp.SetTrackCkeckPoint(this);
            checkPoints.Add(cp);
        }
        foreach(Transform car in cars)
        {
            nextCheckPointIndexes.Add(0);
            lapsDone.Add(0);
        }

    }
    public void CarThroughtCheckPoint(CheckPoint checkPoint, Transform car)
    {
        int nextCheckPointIndex = nextCheckPointIndexes[cars.IndexOf(car)];
        int carLapsDone = lapsDone[cars.IndexOf(car)];
        if (checkPoints.IndexOf(checkPoint) == nextCheckPointIndex)
        {
            //Debug.Log("Correct!");
            nextCheckPointIndex = (nextCheckPointIndex + 1) % checkPoints.Count;
            nextCheckPointIndexes[cars.IndexOf(car)] = nextCheckPointIndex;
            if(nextCheckPointIndex == 0)//+1 lap done
            {
                carLapsDone++;
                lapsDone[cars.IndexOf(car)] = carLapsDone;
                if(carLapsDone == laps)
                {
                    OnCarCorrectCheckPoint?.Invoke(this, new CarThroughtCheckPointEventArgs(car, true));
                }
                else
                {
                    OnCarCorrectCheckPoint?.Invoke(this, new CarThroughtCheckPointEventArgs(car, false));
                }
            }
            else
            {
                OnCarCorrectCheckPoint?.Invoke(this, new CarThroughtCheckPointEventArgs(car, false));
            }
            
        }
        else {
            //Debug.Log("incorrect!");
            OnCarWrongCheckPoint?.Invoke(this, new CarThroughtCheckPointEventArgs(car, false));
        }
    }
    public Transform GetNextCheckPointPosition(Transform car)
    {
        return CheckPointsPos[nextCheckPointIndexes[cars.IndexOf(car)]];
    }
    public void ResetCheckPoints(Transform car)
    {
        nextCheckPointIndexes[cars.IndexOf(car)] = 0;
        lapsDone[cars.IndexOf(car)] = 0;
    }
    public int TotCheckPoint() { return checkPoints.Count; }
}
public class CarThroughtCheckPointEventArgs: EventArgs
{
    public Transform carPosition { get; private set; }
    public bool finished { get; private set; }
    public CarThroughtCheckPointEventArgs(Transform carPos, bool finished) { carPosition = carPos; this.finished = finished; }
}
