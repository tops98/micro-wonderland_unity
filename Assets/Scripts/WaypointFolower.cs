using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[RequireComponent(typeof(GenericCar))]
public class WaypointFolower : MonoBehaviour
{
    public float minDistacneToWaypoint = 3;
    public Transform currentWaypoint = null;
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
            var steerAngle = GetSteeringAngleForTarget(currentWaypoint,carController.steeringCenter);
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

    Transform GetClosestWaypoint(Transform steeringPos)
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
        return closestWp;
    }

    bool GetNextWaypoint(Transform steeringPos, float minDist)
    {
        var dist = Vector3.Distance(steeringPos.position,currentWaypoint.position);
        bool stop = false;
        if(dist < minDist)
        {
            Waypoint wp = currentWaypoint.GetComponent<Waypoint>();
            if(wp == null)
                throw new Exception("No waypoínt commponent attached to current waypoint");
            
            if(wp.accessableNeigbor == null)
                stop= true;
            else
                currentWaypoint = wp.accessableNeigbor.transform;
        }

        
        return stop;
    }
}
