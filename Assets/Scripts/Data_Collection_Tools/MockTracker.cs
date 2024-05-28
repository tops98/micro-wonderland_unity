using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.MicrowunderlandInterfaces;
using FRJ.Sensor;
using Unity.Mathematics;
using System.IO;
using System.Text; 

[RequireComponent(typeof(IRBGCamera))]
[RequireComponent(typeof(Camera))]
public class MockTracker : MonoBehaviour
{
    
    [Serializable]struct TrackedObject
    {
        public Transform trackedObject;
        public int objectID;
    }

    private string _exportPath = "Assets/Exports/mock_tracker";
    [SerializeField]bool createFile = false;
    [SerializeField]string _fileName = "mock_tracker.csv";
    [SerializeField] private bool _useOccluders = true;
    [SerializeField]TrackedObject[] _trackedObjects;
    [SerializeField] private string _topicName = "tracked_objects";
    [Tooltip("sample rate in herz")]
    [SerializeField]int _sampleRate = 15;
    private ROSConnection _ros;
    private TrackedObjectsMsg _message;
    private float _timeElapsed = 0f;
    private Camera _cam;
    private IRBGCamera _rgbCamera;



    void Start()
    {
        if(createFile){
            if(_exportPath.LastIndexOf('/')<_exportPath.Length)
            {
                _exportPath += '/';
            }
            System.IO.Directory.CreateDirectory(_exportPath);
            if (!File.Exists(_exportPath+_fileName)) {
                // Creating a file with below content
                string createText = "time undetected x y" + Environment.NewLine;
                File.WriteAllText(_exportPath+_fileName, createText);
            }
        }

        _rgbCamera = gameObject.GetComponent<IRBGCamera>();
        _cam = gameObject.GetComponent<Camera>();

        CheckTrackedObjects();
         // setup ROS
        _ros = ROSConnection.GetOrCreateInstance();
        _ros.RegisterPublisher<TrackedObjectsMsg>(this._topicName);

        // setup ROS Message
        _message = new TrackedObjectsMsg();
    }

    // Update is called once per frame
    void Update()
    {
        this._timeElapsed += Time.deltaTime;

        if(this._timeElapsed > (1f/_sampleRate))
        {
            _message.objects = GetTrackedObjects();
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

    private TrackedObjectMsg[] GetTrackedObjects()
    {
        TrackedObjectMsg[] objs = new TrackedObjectMsg[_trackedObjects.Length];
        for (int i = 0; i < _trackedObjects.Length; i++)
        {
            objs[i] = new TrackedObjectMsg();
            var pos =  new Vector2DMsg();   
            var coords = ConvertToPixelCoords(_trackedObjects[i].trackedObject.transform.position);

            pos.x = coords.x;
            pos.y = coords.y;

            if(_useOccluders)
                objs[i].undetected = IsOccluded(_trackedObjects[i].trackedObject.transform.position);
            else
                objs[i].undetected = false;

            objs[i].position = pos;
            objs[i].id =  (ushort)_trackedObjects[i].objectID;

            if(createFile){
                string newRow =  Time.time.ToString() +" " + objs[i].undetected + " " + objs[i].position.x + " " + objs[i].position.y;
                newRow = newRow.Replace(',','.');
                File.AppendAllText(_exportPath+_fileName, newRow + Environment.NewLine);
            }
        }
        return objs;
    }

    private bool IsOccluded(Vector3 pos)
    {
        pos.y += 1000;
        bool occluded = false;
        RaycastHit hit;
        Ray ray = new Ray(pos, Vector3.down);
        
        
        if(Physics.Raycast(ray,out hit,math.INFINITY))
        {
            if( hit.transform.tag == "Occluder")
            {
                occluded = true;
            }
        }
        return occluded;
    }

    private Vector3 ConvertToPixelCoords(Vector3 pos)
    {
        var p1 = _cam.WorldToScreenPoint(pos);
        var res = _rgbCamera.GetResolution();
        // move origin to top left
        p1.y = res.y -1 - p1.y;    
        return p1;
    }
}
