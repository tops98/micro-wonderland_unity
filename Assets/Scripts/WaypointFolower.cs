using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[RequireComponent(typeof(GenericCar))]
public class WaypointFolower : MonoBehaviour
{
    public float minDistacneToWaypoint = 3;
    public Waypoint currentWaypoint = null;
    private GenericCar carController;

    
    void Awake()
    {
        carController = gameObject.GetComponent<GenericCar>();
        if(currentWaypoint == null)
            currentWaypoint = GetClosestWaypoint(carController.steeringCenter);
    }

    void Update()
    {   
        if(!GetNextWaypoint(carController.steeringCenter,minDistacneToWaypoint))
        {
            var steerAngle = GetSteeringAngleForTarget(currentWaypoint.transform,carController.steeringCenter);
            carController.SetSteeringAngle(steerAngle);
            carController.Break(0);
            carController.Accelarate(1);
        }else{
            carController.Accelarate(0);
            carController.Break(1);
        }
    }

    float GetSteeringAngleForTarget(Transform target, Transform steeringPos)
    {
        var a = steeringPos.forward;
        var b = target.position - steeringPos.position;
        // ignore y-Axis
        b.y = a.y;
        var angle = Vector3.SignedAngle(a,b,Vector3.up);

        return angle;
    }

    Waypoint GetClosestWaypoint(Transform steeringPos)
    {
        var wps = GameObject.FindGameObjectsWithTag("Waypoint");
        Transform closestWp = null;
        if(wps == null || wps.Length <= 0)
        {
            throw new Exception("No waypoints found");
        }

        float minDist = float.MaxValue;
        foreach (var wp in wps)
        {
            var dist = Vector3.Distance(steeringPos.position,wp.transform.position);
            if(dist < minDist)
            {
                minDist = dist;
                closestWp = wp.transform;
            }
        }
        return closestWp.GetComponent<Waypoint>();
    }

    bool GetNextWaypoint(Transform steeringPos, float minDist)
    {
        var dist = Vector3.Distance(steeringPos.position,currentWaypoint.transform.position);
        bool stop = false;
        if(dist < minDist)
        {            
            if(currentWaypoint.accessableNeigbor == null)
                stop= true;
            else
                currentWaypoint = currentWaypoint.accessableNeigbor;
        }

        
        return stop;
    }
}
