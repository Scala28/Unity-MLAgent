using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private TrackCheckPoints trackCheckPoints;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<CarController>(out CarController car))
        {
            trackCheckPoints.CarThroughtCheckPoint(this, other.transform);
        }
    }
    public void SetTrackCkeckPoint(TrackCheckPoints trackCheckPoints)
    {
        this.trackCheckPoints = trackCheckPoints;
    }
}
