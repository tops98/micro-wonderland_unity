using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Waypoint : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> neighbors;
    public GameObject accessableNeigbor;

    public Mesh arrow;

    Waypoint(){
        neighbors = new List<GameObject>();
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position,0.5f);
        DrawConnections();
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

        foreach(GameObject currentNeighor in neighbors){
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
            newWaypoint.GetComponent<Waypoint>().arrow = waypoint.arrow;
            
            if(waypoint.neighbors.Count == 0)
                waypoint.accessableNeigbor =newWaypoint;
            
            waypoint.neighbors.Add(newWaypoint);
            Selection.activeGameObject = newWaypoint;
        }
    }
}
