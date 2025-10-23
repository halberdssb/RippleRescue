using UnityEditor;
using UnityEngine;

/*
 * Custom editor for squid class - used for handles for movement paths
 * 
 * Jeff Stevenson
 * 10.23.25
 */

[CustomEditor(typeof(Squid))]
public class SquidEditor : Editor
{
    // create buttons for adding/removin gpath points
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Squid squid = (Squid)target;

        // add movement point button
        if (GUILayout.Button("Add New Path Point"))
        {
            // spawn new handle slightly above player on z axis
            squid.movePositions.Add(squid.transform.position + Vector3.forward);
            SceneView.RepaintAll();
        }
        // delete movement point button
        if (GUILayout.Button("Remove Last Movement Point"))
        {
            squid.movePositions.RemoveAt(squid.movePositions.Count - 1);
            SceneView.RepaintAll();
        }
    }

    // draw position handles for path positions
    public void OnSceneGUI()
    {
        Squid squid = (Squid)target;

        // create handle and update move position for each point in squid path
        for (int i = 0; i < squid.movePositions.Count; i++)
        {
            EditorGUI.BeginChangeCheck();

            // create transform handle
            Vector3 movePosition = Handles.PositionHandle(squid.movePositions[i], Quaternion.identity);
            Handles.Label(movePosition, "Position " + (i + 1));

            // update path move point position
            if (EditorGUI.EndChangeCheck())
            {
                // make sure y values stay on same plane as squid
                movePosition.y = squid.transform.position.y;
                squid.movePositions[i] = movePosition;
            }
        }
    }
}
