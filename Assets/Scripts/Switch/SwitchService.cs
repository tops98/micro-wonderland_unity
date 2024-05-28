using RosMessageTypes.MicrowunderlandInterfaces;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using System;
using System.Collections.Generic;


[RequireComponent(typeof(SwitchController))]
public class SwitchService : MonoBehaviour
{  
    private SwitchController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<SwitchController>();
        ROSConnection.GetOrCreateInstance().ImplementService<SetStateRequest, SetStateResponse>("set_state", SetState);
        ROSConnection.GetOrCreateInstance().ImplementService<GetAvailableStatesRequest, GetAvailableStatesResponse>("get_available_states", GetAvailableStates);
        ROSConnection.GetOrCreateInstance().ImplementService<GetCurrentStateRequest, GetCurrentStateResponse>("get_current_state", GetCurrentState);
    }

    private SetStateResponse SetState(SetStateRequest request)
    {
        Debug.Log(request);
        var response = new SetStateResponse();
        try
        {
            controller.SetState(request.switch_name,request.state_name);
            response.status =0;
        }catch(Exception ex)
        {
            response.status = (byte)ReturnErrorCode(ex);
        }
        return response;
    }

    private GetAvailableStatesResponse GetAvailableStates(GetAvailableStatesRequest request)
    {
        Debug.Log(request);
        var response = new GetAvailableStatesResponse();
        var statesDict = new Dictionary<string, int>();
        var statesList = new List<StringUint16PairMsg>();
        try
        {
            statesDict = controller.GetAvailableStates(request.switch_name);
        }catch(Exception ex)
        {
            response.status = (byte)ReturnErrorCode(ex);
            return response;
        }

        foreach (var state in statesDict)
        {
            statesList.Add(new StringUint16PairMsg(state.Key,(ushort)state.Value));
        }

        response.states = statesList.ToArray();        
        return response;
    }

    private GetCurrentStateResponse GetCurrentState(GetCurrentStateRequest request)
    {
        Debug.Log(request);
        var response = new GetCurrentStateResponse();
        try
        {
            response.state_name = controller.GetCurrentState(request.switch_name);
        }catch(Exception ex)
        {
            response.status = (byte)ReturnErrorCode(ex);
        }
        return response;
    }

    // TODO: add real error codes
    private int ReturnErrorCode(Exception ex)
    {
        switch(ex)
        {
            case StateNotFoundException:
                return 1;
            case SwitchNotFoundException:
                return 1;
            default:
                throw ex;
        }
    }
}
