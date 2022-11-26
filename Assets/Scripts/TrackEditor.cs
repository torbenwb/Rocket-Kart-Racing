using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TrackEditor))]
public class TrackEditorWindow : Editor
{  
    bool placePoint = true;
    Vector3 currentPoint;
    Vector3 direction;
    Vector3 lastPoint;
    void OnSceneGUI(){
        TrackEditor t = (TrackEditor)target;
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)){
            /*
            if (placePoint){
                if (Handles.Button(hit.point, Quaternion.identity, 1f, 1f, Handles.SphereHandleCap)){
                    t.NewTrackPoint(hit.point);
                }
            }   
            */
            Event e = Event.current;
            switch (e.type) {
                case EventType.MouseDrag: 
                    Debug.DrawLine(lastPoint, hit.point, Color.red);
                    lastPoint = hit.point;
                break;
            }
        }
    }
}

public class TrackEditor : MonoBehaviour
{
    public List<Transform> trackPoints = new List<Transform>();

    public void NewTrackPoint(Vector3 position){
        GameObject newTrackPoint = new GameObject();
        newTrackPoint.transform.SetParent(transform);
        newTrackPoint.transform.position = position;
        trackPoints.Add(newTrackPoint.transform);
    }

    private void OnDrawGizmos()
    {
        /*
        for(int i = 0; i < trackPoints.Count - 1; i++){
            Debug.DrawLine(trackPoints[i].position, trackPoints[i + 1].position, Color.red);
        }
        Debug.DrawLine(trackPoints[trackPoints.Count - 1].position, trackPoints[0].position, Color.red);
        */
    }
}
