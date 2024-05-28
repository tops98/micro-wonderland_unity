using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using RosMessageTypes.MicrowunderlandInterfaces;
using System.Linq;
using System;


public class TinycarService : MonoBehaviour
{

    
    Dictionary<string,GenericCar> carControllers;


    // Start is called before the first frame update
    void Start()
    {
        carControllers = new Dictionary<string, GenericCar>();
        foreach(var car in FindObjectsOfType<GenericCar>().ToList()){
            car.Break(0);
            car.Accelerate(0);
            carControllers.Add(car.gameObject.name, car);     
        }
        var rosCon = ROSConnection.GetOrCreateInstance();
        rosCon.ImplementService<SetUint32KeyValueRequest, SetUint32KeyValueResponse>("set_speed", SetSpeed);
        rosCon.ImplementService<SetUint32KeyValueRequest, SetUint32KeyValueResponse>("set_head_light", SetHeadlight);
        rosCon.ImplementService<SetUint32KeyValueRequest, SetUint32KeyValueResponse>("set_tail_light", SetTaillight);
        rosCon.ImplementService<SetUint32KeyValueRequest, SetUint32KeyValueResponse>("set_blinker", SetBlinker);
    }

    private SetUint32KeyValueResponse SetSpeed(SetUint32KeyValueRequest request){

        Debug.Log(request);
        var response = new SetUint32KeyValueResponse();
        var carAddress = request.key;
        var speed = request.value/100.0f;

        var carController = carControllers[carAddress];
        carController.Accelerate(speed);
        if(speed ==0 ){
            carController.Break(1);
        }else{
            carController.Break(0);
        }
        response.status =0;
        
        return response;
    }

    private SetUint32KeyValueResponse SetHeadlight(SetUint32KeyValueRequest request){

        Debug.Log(request);
        var response = new SetUint32KeyValueResponse();
        var carAddress = request.key;
        var state = request.value != 0;
                                    
        carControllers[carAddress].SetHeadlightState(state);
        response.status =0;
        
        return response;
    }

    private SetUint32KeyValueResponse SetTaillight(SetUint32KeyValueRequest request){

        Debug.Log(request);
        var response = new SetUint32KeyValueResponse();
        var carAddress = request.key;
        var state = (GenericCar.TaillightStates)request.value;
                                    
        carControllers[carAddress].SetTaillightState(state);
        response.status =0;
        
        return response;
    }

    private SetUint32KeyValueResponse SetBlinker(SetUint32KeyValueRequest request){

        Debug.Log(request);
        var response = new SetUint32KeyValueResponse();
        var carAddress = request.key;
        var state = (GenericCar.BlinkerStates)request.value;
                                    
        carControllers[carAddress].SetBlinkerState(state);
        response.status =0;
        
        return response;
    }
}
