using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

[RequireComponent(typeof(GenericCar))]
public class StartStopCarService : MonoBehaviour
{

    [SerializeField]private string _vehicleName;
    private GenericCar carController;


    // Start is called before the first frame update
    void Start()
    {
        carController = gameObject.GetComponent<GenericCar>();
        carController.Break(1);
        carController.Accelarate(0);
        ROSConnection.GetOrCreateInstance().ImplementService<SetBoolRequest, SetBoolResponse>(_vehicleName+"/set_engine_state", SetCarEngineOnOff);
    }


    private SetBoolResponse SetCarEngineOnOff(SetBoolRequest request){
        Debug.Log("Car Service received message!");
        if(request.data)
        {
            Debug.Log("Engine on");
            carController.Break(0);
            carController.Accelarate(1);    
        }else
        {
            Debug.Log("Engine off");
            carController.Break(1);
            carController.Accelarate(0);
        }
        return new SetBoolResponse(true,"");
    }
}
