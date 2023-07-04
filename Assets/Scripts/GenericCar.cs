using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



// TODOS:
// [x] check why performance is so bad!!!!!!!!!!!!
// [x] seperate waypoint logic from car logic
// [x] make car stop when there are no waypoints
// [x] add openLink meber to waypoint class (holds the transform of the link that is accessable)
// [ ] add switch componet 
// [ ] add a ros2 service wrapper that usese ros2
// [ ] google camera sensor (sim) for unity ros
[RequireComponent(typeof(Rigidbody))]
public class GenericCar : MonoBehaviour
{
    [Header("Wheel Collider")]
    [SerializeField] private WheelCollider front_left;
    [SerializeField] private WheelCollider front_right;
    [SerializeField] private WheelCollider back_left;
    [SerializeField] private WheelCollider back_right;
    
    [Space(10)]
    [Header("Wheel Meshes")]
    [SerializeField] private Transform mesh_front_left;
    [SerializeField] private Transform mesh_front_right;
    [SerializeField] private Transform mesh_back_left;
    [SerializeField] private Transform mesh_back_right;
    [SerializeField] private bool flipWheelRotation = false;
    
    [Space(10)]
    [Header("Car Attributes")]
    public float maxSteeringAngle = 20;
    public float accelaration = 500;
    public float breakingForce = 400;
    [Space(10)]

    public Transform steeringCenter = null;
    

    private float currentSteeringAngle = 0;
    private float currentAccelaration = 0;
    private float currentBreakingForce = 0;
    private float wheelRotOffset = 90;

    void Awake(){
        if(flipWheelRotation){
            wheelRotOffset = 0;
        }
    }

    void FixedUpdate()
    {   
        ApplyTorquesToWheels();
        ApplySteering();
    }

    public void Accelarate(float percentage)
    {
        if(Math.Abs(percentage) >1){
            throw new Exception("percentage musst be bewtween -1 and 1");
        }
        currentAccelaration = accelaration * percentage;
    }

    public void Break(float percentage){
        if(Math.Abs(percentage) >1){
            throw new Exception("percentage musst be bewtween -1 and 1");
        }
        currentBreakingForce = breakingForce * percentage;
    }

    public void SetSteeringAngle(float angle){
       if(angle > maxSteeringAngle)
            angle = maxSteeringAngle;
        else if( angle < maxSteeringAngle*-1)
            angle = maxSteeringAngle *-1;
        currentSteeringAngle = angle;
    }

    private void ApplySteering()
    {
        front_left.steerAngle = currentSteeringAngle;
        front_right.steerAngle = currentSteeringAngle;

        MoveWheelMesh(mesh_front_right.transform,front_right);
        MoveWheelMesh(mesh_front_left.transform,front_left);
        MoveWheelMesh(mesh_back_right.transform,back_right);
        MoveWheelMesh(mesh_back_left.transform,back_left);
    }

    private void ApplyTorquesToWheels()
    {
        back_left.motorTorque = currentAccelaration;
        back_right.motorTorque = currentAccelaration;

        front_left.brakeTorque = currentBreakingForce;
        front_right.brakeTorque = currentBreakingForce;
        back_left.brakeTorque = currentBreakingForce;
        back_right.brakeTorque = currentBreakingForce;
    }

    private void MoveWheelMesh(Transform trans, WheelCollider coll)
    {
        Quaternion rot;
        Vector3 pos;

        coll.GetWorldPose(out pos,out rot);
        trans.position = pos;
        trans.rotation = rot* Quaternion.Euler(0, wheelRotOffset, 0);;
    }
}
