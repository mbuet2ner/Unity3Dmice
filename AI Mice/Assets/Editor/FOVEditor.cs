using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (WolfFOV))]
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        WolfFOV wolf = (WolfFOV)target;

        //draws the main FOV cirlce
        Handles.color = Color.white;
        Handles.DrawWireArc(wolf.transform.position, Vector3.up, Vector3.forward, 360, wolf.fovRadius);
        //Calculating the angles for the actual FOV
        Vector3 viewAngle1 = wolf.Direction(-wolf.fovAngel / 2, false);
        Vector3 viewAngle2 = wolf.Direction(wolf.fovAngel / 2, false);
        //draws the adjustable FOV of the Wolf
        Handles.DrawLine(wolf.transform.position, wolf.transform.position + viewAngle1 * wolf.fovRadius);
        Handles.DrawLine(wolf.transform.position, wolf.transform.position + viewAngle2 * wolf.fovRadius);

        Handles.color = Color.red;
        foreach (Transform mouseInView in wolf.mouseInView)
        {
            Handles.DrawLine(wolf.transform.position, mouseInView.position);
        }
        
    }
}
