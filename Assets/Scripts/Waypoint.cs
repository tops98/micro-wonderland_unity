using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Waypoint : MonoBehaviour
{
    // public List<Waypoint> neighbors;
    public List<Waypoint> neighbors;
    // public GameObject accessableNeigbor;
    public Waypoint accessableNeigbor;

    [HideInInspector]
    public Mesh arrow;

    Waypoint(){
        neighbors = new List<Waypoint>();
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position,0.5f);
        DrawConnections();
    }
    bool Same(Transform a,Transform b){
        return a==b;
    }
     // very inefficent
    private void getArrow()
    {
        Debug.Log("getting arrow");
        foreach(var mesh in Resources.FindObjectsOfTypeAll<Mesh>()){
            if(mesh.name =="arrow"){
                arrow = mesh;
                Debug.Log("get arrow");
                break;
            }
        }
    }
    
    private void DrawConnections(){
        if(neighbors.Count <= 0){
            return;
        }

        foreach(Waypoint currentNeighor in neighbors){
            if(currentNeighor == null){
                neighbors.Remove(currentNeighor);
                break;
            } 
            var center = currentNeighor.transform.position - (currentNeighor.transform.position-transform.position)/2.0f;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position,currentNeighor.transform.position);
            Gizmos.color = Color.cyan;
            if(arrow == null){
                getArrow();
            }
            Gizmos.DrawMesh(arrow,center,Quaternion.LookRotation(currentNeighor.transform.position-transform.position,Vector3.forward));
        }
    }
}

[CustomEditor(typeof(Waypoint))]
public class WaypointInspector: Editor{

    public override void OnInspectorGUI()
    {
        var waypoint = (Waypoint) target;
        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        GUILayout.Space(15);

        if(GUILayout.Button("Create new Waypoint") == true){
            GameObject newWaypoint = new GameObject("waypoint");
            newWaypoint.transform.position = waypoint.transform.position;
            newWaypoint.transform.position += newWaypoint.transform.forward *1;
            newWaypoint.AddComponent<Waypoint>();
            var wpScript = newWaypoint.GetComponent<Waypoint>();
            wpScript.arrow = waypoint.arrow;
            
            if(wpScript.neighbors.Count == 0)
                wpScript.accessableNeigbor =wpScript;
            
            waypoint.neighbors.Add(wpScript);
            Selection.activeGameObject = newWaypoint;
        }
    }
}
