using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Waypoint : MonoBehaviour
{
    public List<Waypoint> neighbors;
    public Waypoint accessableNeigbor;
    public bool isSwitch;

    [HideInInspector]public Waypoint predecessor;
    [HideInInspector]public Mesh arrow;

    Waypoint(){
        neighbors = new List<Waypoint>();
    }
    
    void OnDrawGizmos()
    {
        isSwitch = neighbors.Count>1;

        var sphereColor = Color.blue;
        var sphereSize = 0.5f;
        if(isSwitch){
            sphereColor = Color.green;
            sphereSize = 0.75f;
        }
        Gizmos.color = sphereColor;
        Gizmos.DrawSphere(transform.position,sphereSize);
        DrawConnections();
    }

    
     // very inefficent
    private void getArrow()
    {
        Debug.Log("getting arrow");
        foreach(var mesh in Resources.FindObjectsOfTypeAll<Mesh>()){
            if(mesh.name =="arrow")
            {
                arrow = mesh;
                Debug.Log("get arrow");
                break;
            }
        }
    }
    
    private void DrawConnections()
    {
        if(neighbors.Count <= 0)
        {
            return;
        }

        Vector3 center = new Vector3();
        foreach(Waypoint currentNeighor in neighbors)
        {
            if(currentNeighor == null)
            {
                neighbors.Remove(currentNeighor);
                break;
            } 
            center = transform.position + Vector3.Normalize(currentNeighor.transform.position-transform.position)*2.0f;
            Gizmos.color = Color.yellow;
            if(isSwitch)
            {
                if(currentNeighor == accessableNeigbor)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawMesh(arrow,center,Quaternion.LookRotation(currentNeighor.transform.position-transform.position,Vector3.forward));
                }else
                {
                    Gizmos.color = Color.red;
                }
            }  

            Gizmos.DrawLine(transform.position,currentNeighor.transform.position);
            if(arrow == null)
            {
                getArrow();
            }
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

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fixedHeight = 60;
        buttonStyle.fixedWidth = 160;
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.fontSize = 15;

        if(GUILayout.Button("New Waypoint",buttonStyle) == true){
            GameObject newWaypoint = new GameObject("waypoint");
            newWaypoint.transform.position = waypoint.transform.position;
            newWaypoint.transform.position += newWaypoint.transform.forward *1;
            newWaypoint.AddComponent<Waypoint>();
            var wpScript = newWaypoint.GetComponent<Waypoint>();
            wpScript.arrow = waypoint.arrow;
            wpScript.predecessor = waypoint;

            if(wpScript.neighbors.Count == 0)
                wpScript.accessableNeigbor =wpScript;
            
            waypoint.neighbors.Add(wpScript);
            Selection.activeGameObject = newWaypoint;
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}
