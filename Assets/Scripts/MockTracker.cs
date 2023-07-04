using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.MicrowunderlandInterfaces;

public class MockTracker : MonoBehaviour
{
    
    [Serializable]struct TrackedObject
    {
        public Transform trackedObject;
        public string objectID;
    }

    [SerializeField]TrackedObject[] _trackedObjects;
    [SerializeField] private string _topicName = "tracked_objects";
    [Tooltip("sample rate in herz")]
    [SerializeField]int _sampleRate = 15;
    private ROSConnection _ros;
    private ListNamed2DPositionsMsg _message;
    private float _timeElapsed = 0f;

    void Start()
    {

        CheckTrackedObjects();
         // setup ROS
        _ros = ROSConnection.GetOrCreateInstance();
        _ros.RegisterPublisher<ListNamed2DPositionsMsg>(this._topicName);

        // setup ROS Message
        _message = new ListNamed2DPositionsMsg();
    }

    // Update is called once per frame
    void Update()
    {
        this._timeElapsed += Time.deltaTime;

        if(this._timeElapsed > (1f/_sampleRate))
        {
            _message.named_positions = GetTrackedObjects();
            _ros.Publish(_topicName,_message);
            _timeElapsed = 0;
        }
    }

    private void CheckTrackedObjects()
    {
        if(_trackedObjects == null || _trackedObjects.Length ==0)
        {
            throw new Exception("No objects to track!\ntracked_objects list is empty");
        }

        foreach (var obj in _trackedObjects)
        {
            if(obj.trackedObject == null || obj.objectID.Equals(""))
            {
                throw new Exception("All values of a tracked object must be set!");
            }
        }
    }

    private Named2DPositionMsg[] GetTrackedObjects()
    {
        Named2DPositionMsg[] objs = new Named2DPositionMsg[_trackedObjects.Length];
        for (int i = 0; i < _trackedObjects.Length; i++)
        {
            objs[i] = new Named2DPositionMsg();
            objs[i].name = _trackedObjects[i].objectID;
            objs[i].x = _trackedObjects[i].trackedObject.transform.position.x;
            objs[i].y = _trackedObjects[i].trackedObject.transform.position.z;
        }
        return objs;
    }
}
