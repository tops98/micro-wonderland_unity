using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.IO;
using System.Text;


[Serializable]
public class StateNotFoundException: Exception
{
    public StateNotFoundException() {}
    public StateNotFoundException(string stateName, string switchName)
    : base(String.Format("state [{0}] is not a valid state for switch [{1}]", stateName,switchName)){}
}

[Serializable]
public class SwitchNotFoundException: Exception
{
    public SwitchNotFoundException() {}
    public SwitchNotFoundException( string switchName)
    : base(String.Format("canot find [{0}] in available switches", switchName)){}
}

public class SwitchController : MonoBehaviour
{
    // oviously an dictionalry would be better but
    // unfortunety unity is not suppportting the display
    // of Dictionaries in the Inspector window
    [Serializable]
    public struct InspectorState{
        public string stateName;
        public Waypoint link;
    }
    [Serializable]
    public struct InspectorSwitch{
        public string switchName;
        public Waypoint origin; 
        public string currentState;
        public string initialState;
        public List<InspectorState> states;
    }

    public struct Switch{
        public string switchName;
        public Waypoint origin; 
        public string currentState;
        public string initialState;
        public Dictionary<string,Waypoint> states;
    }

    public List<InspectorSwitch> switches;
    private Dictionary<string,Switch> switchDict;
    public float forawardToll = 20;
    public string _exportPath = "Assets/Exports/Maps/";
    public string _fileName = "newMap";

    public Camera cam;

    public SwitchController(){
        switches = new List<InspectorSwitch>();
    }

    void Start(){

        switchDict = new Dictionary<string, Switch>();
        foreach (var item in switches)
        {
            var states = new Dictionary<string,Waypoint>();
            var newSwitch = new Switch();
            
            newSwitch.switchName = item.switchName;
            newSwitch.states = states;
            newSwitch.origin = item.origin;
            newSwitch.initialState = item.initialState;
            newSwitch.currentState = item.currentState;
            
            foreach (var state in item.states)
            {
                states.Add(state.stateName,state.link);
            }
               
            switchDict.Add(item.switchName,newSwitch);
        }
    }

    public Dictionary<string,int> GetAvailableStates(string switchId){
        CheckSwitchId(switchId);

        Switch selectedSwitch = switchDict[switchId];
        var dict = new Dictionary<string, int>();
        var origin = new Vector2(selectedSwitch.origin.transform.position.x,selectedSwitch.origin.transform.position.z);
        var target = new Vector2();

        foreach(var state in selectedSwitch.states){
            target.x = state.Value.transform.position.x;
            target.y = state.Value.transform.position.z;
            var angel = Vector2.Angle(origin,target);
                
            dict.Add(state.Key,(int)angel);
        }
        return dict;
    }

    public string GetCurrentState(string switchId){
        CheckSwitchId(switchId);
        return switchDict[switchId].currentState;
    }

    public void SetState(string switchId, string stateName){
        CheckSwitchId(switchId);
        var selectedSwitch = switchDict[switchId];
        
        if(!selectedSwitch.states.ContainsKey(stateName))
        {
            throw new StateNotFoundException(stateName,switchId);
        }
        selectedSwitch.currentState = selectedSwitch.currentState = stateName;
        selectedSwitch.origin.accessableNeigbor = selectedSwitch.states[stateName];
    }

    void CheckSwitchId(string id)
    {
        if(!switchDict.ContainsKey(id))
        {
            throw new SwitchNotFoundException(id);
        }
    }
}

[CustomEditor(typeof(SwitchController))]
public class SwitchControllerInspector: Editor{
     Rect buttonRect;
    public override void OnInspectorGUI()
    {
        
        var controller = (SwitchController) target;
        
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        GUILayout.Space(15);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fixedHeight = 60;
        buttonStyle.fixedWidth = 160;
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.fontSize = 15;
        
        if(GUILayout.Button("Find Swithes",buttonStyle))
        {
            controller.switches.Clear();
            GetSwitches(controller);
        }
        
        // GUILayout.FlexibleSpace();
        GUILayout.Space(15);

        if(GUILayout.Button(new GUIContent("Export Map","Exports a list of connected switches"),buttonStyle))
        {
            ExportMap(controller);
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private Vector3 ConvertToPixelCoords(Camera cam, Vector3 pos)
    {
        var p1 = cam.WorldToScreenPoint(pos);
        var resY = cam.pixelHeight;
        // move origin to top left
        p1.y = resY -1 - p1.y;    
        return p1;
    }

    private void ExportMap(SwitchController controller)
    {   
        var switches =controller.switches;
        StringBuilder sb2 = new StringBuilder("---\nnodes:");
        StringBuilder sb = new StringBuilder();   
        foreach(var currentSwitch in switches)
        {
            var pixelPos = ConvertToPixelCoords(controller.cam, currentSwitch.origin.transform.position);
            sb.AppendFormat("\n  - node_name: {0}\n",currentSwitch.switchName);
            sb.Append("    trigger_radius: 25.\n");
            sb.AppendFormat("    position:\n      x: {0}\n      y: {1}"
            ,pixelPos.x
            ,pixelPos.y).Replace(",",".");
            
            sb.Append("\n    connections:\n");
            foreach(var neighbor in currentSwitch.states)
            {
                List<Waypoint> visitedWps = new List<Waypoint>();
                var currentWp = neighbor.link;

                while(!visitedWps.Contains(currentWp))
                {
                    visitedWps.Add(currentWp);
                    if(currentWp.isSwitch)
                    {
                        sb.AppendFormat("      - {0}",currentWp.name);
                        break;
                    }else
                    {
                        if(currentWp.accessableNeigbor == null)
                            break;
                        currentWp = currentWp.accessableNeigbor;
                    }
                }
                sb.Append("\n");
            }
        }
        System.IO.Directory.CreateDirectory(controller._exportPath);
        File.WriteAllText(controller._exportPath+controller._fileName+".yaml",sb2.Append(sb.ToString()).ToString());
    }

    void GetSwitches(SwitchController controller)
    {
        var waypoints = GameObject.FindObjectsOfType<Waypoint>();
        int count =0;
        
        foreach (var waypoint in waypoints)
        {
            if(!waypoint.isSwitch) continue;
            count++;
            waypoint.name = "Switch_"+count;
            var newSwitch = new SwitchController.InspectorSwitch();
            newSwitch.switchName = waypoint.name;
            newSwitch.origin = waypoint;
            newSwitch.states = new List<SwitchController.InspectorState>();
            
            if(waypoint.neighbors.Count < 0)continue;
            int stateCount =0;
            foreach (var neighbor in waypoint.neighbors)
            {
                stateCount++;
                var newState = new SwitchController.InspectorState();
                newState.stateName = GetStateNameByDirection(controller,waypoint,neighbor.transform);
                newState.link = neighbor;
                newSwitch.states.Add(newState);
            }
            newSwitch.initialState = newSwitch.states[0].stateName;
            newSwitch.currentState = newSwitch.initialState;
            controller.switches.Add(newSwitch);
        }
    }

    // NOTE: this is just worikng due to the particular arangment of switches in the current version.
    // It will not work with a switch having more then one incoming connection and it will also not work with
    // multiple switches conected to each other!!!! 
    string GetStateNameByDirection(SwitchController controller, Waypoint currentWP,Transform target){
        var a = new Vector2(currentWP.transform.position.x,currentWP.transform.position.z);
        var b = new Vector2(currentWP.predecessor.transform.position.x,currentWP.predecessor.transform.position.z);
        var c = new Vector2(target.position.x,target.position.z) - a;

        var direction = a-b;
        direction.Normalize();
        var angle = Vector2.SignedAngle(direction,c);

        if(angle> controller.forawardToll){
            return "left";
        }else if(angle < controller.forawardToll*-1){
            return "right";
        }else{
            return "forward";
        }  
    }
}