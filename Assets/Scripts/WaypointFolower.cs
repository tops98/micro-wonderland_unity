using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RosMessageTypes.Std;


[RequireComponent(typeof(GenericCar))]
public class WaypointFolower : MonoBehaviour
{
    [SerializeField] private float minDistacneToWaypoint = 3;
    [SerializeField] private float collisionTestDistance = 3;
    
    public Waypoint currentWaypoint = null;
    private GenericCar carController;
    private bool previouslySuspended = false;
    [SerializeField]private bool manualMotorControll = false;

    
    void Awake()
    {
        carController = gameObject.GetComponent<GenericCar>();
        TurnMotorOff(false);
        if(currentWaypoint == null)
            currentWaypoint = GetClosestWaypoint(carController.steeringCenter);
    }

    void Update()
    {   
        if(GetNextWaypoint(carController.steeringCenter,minDistacneToWaypoint) && !ObstacleDetected())
        {
            var steerAngle = GetSteeringAngleForTarget(currentWaypoint.transform,carController.steeringCenter);
            carController.SetSteeringAngle(steerAngle);
            if(previouslySuspended)
            {
                TurnMotorOff(false);
                previouslySuspended = false;
            }
        }else{
            TurnMotorOff(true);
            previouslySuspended = true;
        }
    }

    private bool ObstacleDetected()
    {
        RaycastHit hit;
        if (Physics.Raycast(carController.steeringCenter.transform.position, transform.TransformDirection(Vector3.forward), out hit,collisionTestDistance))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
            return true;
        }else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            return false;
        }
    }

    private void TurnMotorOff(bool value)
    {
        if(!manualMotorControll)
        {
            float breakForce = value?0f:1f;
            float accelarationForce = value?1f:0f;
            carController.Accelarate(breakForce);
            carController.Break(accelarationForce);
        }
    }

    private float GetSteeringAngleForTarget(Transform target, Transform steeringPos)
    {
        var a = steeringPos.forward;
        var b = target.position - steeringPos.position;
        // ignore y-Axis
        b.y = a.y;
        var angle = Vector3.SignedAngle(a,b,Vector3.up);

        return angle;
    }

    private Waypoint GetClosestWaypoint(Transform steeringPos)
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

    private bool GetNextWaypoint(Transform steeringPos, float minDist)
    {
        var dist = Vector3.Distance(steeringPos.position,currentWaypoint.transform.position);
        bool nextWpAvailable = true;
        if(dist < minDist)
        {            
            if(currentWaypoint.accessableNeigbor == null)
             nextWpAvailable= false;
            else
                currentWaypoint = currentWaypoint.accessableNeigbor;
        }

        
        return nextWpAvailable;
    }
}
