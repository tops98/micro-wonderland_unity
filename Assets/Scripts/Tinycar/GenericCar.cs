using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



[RequireComponent(typeof(Rigidbody))]
public class GenericCar : MonoBehaviour
{
    public enum BlinkerStates{
        OFF,
        LEFT,
        RIGHT,
        HAZARD
    }
    
    public enum TaillightStates{
        OFF,
        ON,
        BREAK
    }

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
    [Header("Lights")]
    [SerializeField] private Transform headlights;
    [SerializeField] private Transform taillights_sides;
    [SerializeField] private Transform taillight_center;
    [SerializeField] private Transform blinker_right;
    [SerializeField] private Transform blinker_left;
    [SerializeField] private float blinkerFrequency = 2;
    
    [Space(10)]
    [Header("Car Attributes")]
    public float maxSteeringAngle = 20;
    public float acceleration = 500;
    public float breakingForce = 400;
    [Space(10)]

    public Transform steeringCenter = null;
    

    private float currentSteeringAngle = 0;
    public float currentAcceleration = 0;
    public float currentBreakingForce = 0;
    private float wheelRotOffset = 90;
    public BlinkerStates blinkerState = BlinkerStates.OFF;
    private float last_blink = 0f;
    private bool flip = false;
    public bool useLights = false;

    void Awake(){

        if(flipWheelRotation){
            wheelRotOffset = 0;
        }

        if(useLights){
            InitLights();
        }
    }

    void FixedUpdate()
    {   
        ApplyTorquesToWheels();
        ApplySteering();
        
        if(useLights){
            SetLights();
        }
    }

    private void InitLights()
    {
        headlights.gameObject.SetActive(false);
        taillight_center.gameObject.SetActive(false);
        taillights_sides.gameObject.SetActive(false);
        blinker_left.gameObject.SetActive(false);
        blinker_right.gameObject.SetActive(false);
    }

    private void SetLights()
    {
        if(blinkerState != BlinkerStates.OFF)
        {
            last_blink += Time.deltaTime;

            if(last_blink > (1f/blinkerFrequency))
            {
                if(flip)
                    SetBlinker(BlinkerStates.OFF);
                else
                    SetBlinker(blinkerState);
                last_blink = 0;
                flip = !flip;
            }
        }
    }

     public void SetBlinkerState(BlinkerStates state)
     {
        blinkerState = state;
        if(state == BlinkerStates.OFF){
            SetBlinker(BlinkerStates.OFF);
        }
     }

    private void SetBlinker(BlinkerStates state)
    {
        switch(state){
            case BlinkerStates.OFF:
                blinker_left.gameObject.SetActive(false);
                blinker_right.gameObject.SetActive(false);
                break;
            case BlinkerStates.LEFT:
                blinker_left.gameObject.SetActive(true);
                blinker_right.gameObject.SetActive(false);
                break;
            case BlinkerStates.RIGHT:
                blinker_left.gameObject.SetActive(false);
                blinker_right.gameObject.SetActive(true);
                break;
            case BlinkerStates.HAZARD:
                 blinker_left.gameObject.SetActive(true);
                blinker_right.gameObject.SetActive(true);
                break;
        }
    }

    public void SetTaillightState(TaillightStates state)
    {
        switch(state){
            case TaillightStates.OFF:
                taillights_sides.gameObject.SetActive(false);
                taillight_center.gameObject.SetActive(false);
                break;
            case TaillightStates.ON:
                taillights_sides.gameObject.SetActive(true);
                taillight_center.gameObject.SetActive(false);
                break;
            case TaillightStates.BREAK:
                taillights_sides.gameObject.SetActive(false);
                taillight_center.gameObject.SetActive(true);
            break;
        }
    }

    public void SetHeadlightState(bool enable)
    {
        headlights.gameObject.SetActive(enable);
    }

    public void Accelerate(float percentage)
    {
        if(Math.Abs(percentage) >1){
            throw new Exception("percentage musst be bewtween -1 and 1");
        }
        currentAcceleration = acceleration * percentage;
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
        back_left.motorTorque = currentAcceleration;
        back_right.motorTorque = currentAcceleration;

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

