using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;


[CustomEditor(typeof(EnemySpawner))]
public class EnemySpawnerEditor : Editor
{
    private bool placementMode = false;
    private bool snapToSurface = false;
    private const float pickDistance = 30f;
    private int draggingIndex = -1;
    private Vector3 dragStartPos;
    private RaycastHit debugHit; // for visualisation

    private EnemySpawner spawner;

    private void OnEnable()
    {
        spawner = (EnemySpawner)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Spawnpoint Tools", EditorStyles.boldLabel);

        placementMode = GUILayout.Toggle(placementMode, "Placement Mode (Click to add, right?drag to move)", "Button");
        if (placementMode)
        {
            EditorGUILayout.HelpBox(
                "Left?click: add spawnpoint at geometry hit (or fallback ground plane).\n" +
                "Right?click drag: move the nearest spawnpoint (surface snap if enabled).",
                MessageType.Info);

            snapToSurface = EditorGUILayout.Toggle("Snap to Surface (while dragging)", snapToSurface);
        }

        if (GUILayout.Button("Clear All Spawnpoints"))
        {
            if (EditorUtility.DisplayDialog("Clear Spawnpoints", "Are you sure you want to remove all spawnpoints?", "Yes", "No"))
            {
                foreach (Transform t in spawner.spawnpoints)
                {
                    if (t != null)
                        Undo.DestroyObjectImmediate(t.gameObject);
                }
                spawner.spawnpoints.Clear();
                EditorUtility.SetDirty(spawner);
            }
        }
    }

    private void OnSceneGUI()
    {
        if (!placementMode) return;

        Event e = Event.current;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        // Draw a line and hit point for the current drag (debug)
        if (draggingIndex >= 0 && snapToSurface)
        {
            Handles.color = Color.cyan;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Handles.DrawLine(ray.origin, ray.origin + ray.direction * 100f);
            if (debugHit.collider != null)
            {
                Handles.color = Color.red;
                Handles.SphereHandleCap(0, debugHit.point, Quaternion.identity, 0.2f, EventType.Repaint);
            }
        }

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0) // Left click – add
                {
                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    Vector3 hitPoint;

                    if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, spawner.groundLayers))
                    {
                        hitPoint = hit.point;
                        Debug.Log($"Placed on {hit.collider.gameObject.name} (layer {LayerMask.LayerToName(hit.collider.gameObject.layer)})");
                    }
                    else
                    {
                        Plane ground = new Plane(Vector3.up, new Vector3(0, spawner.groundPlaneY, 0));
                        if (ground.Raycast(ray, out float distance))
                        {
                            hitPoint = ray.GetPoint(distance);
                            Debug.Log("Placed on fallback plane");
                        }
                        else break;
                    }

                    GameObject go = new GameObject("Spawnpoint");
                    go.transform.position = hitPoint;
                    go.transform.rotation = Quaternion.identity;
                    go.transform.parent = spawner.transform;
                    Undo.RegisterCreatedObjectUndo(go, "Create Spawnpoint");
                    spawner.spawnpoints.Add(go.transform);
                    EditorUtility.SetDirty(spawner);
                    e.Use();
                }
                else if (e.button == 1) // Right click – start drag
                {
                    // Find nearest spawnpoint (screen distance)
                    float minDist = pickDistance;
                    int nearest = -1;
                    for (int i = 0; i < spawner.spawnpoints.Count; i++)
                    {
                        if (spawner.spawnpoints[i] == null) continue;
                        Vector3 screenPos = HandleUtility.WorldToGUIPoint(spawner.spawnpoints[i].position);
                        float dist = Vector2.Distance(e.mousePosition, screenPos);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            nearest = i;
                        }
                    }

                    if (nearest >= 0)
                    {
                        draggingIndex = nearest;
                        dragStartPos = spawner.spawnpoints[nearest].position;
                        Undo.RecordObject(spawner.spawnpoints[nearest], "Move Spawnpoint");
                        e.Use();
                    }
                }
                break;

            case EventType.MouseDrag:
                if (e.button == 1 && draggingIndex >= 0 && draggingIndex < spawner.spawnpoints.Count)
                {
                    Transform sp = spawner.spawnpoints[draggingIndex];
                    if (sp == null)
                    {
                        draggingIndex = -1;
                        break;
                    }

                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    Vector3 newPos;

                    if (snapToSurface)
                    {
                        // Raycast against ground layers, ignoring the spawnpoint's own collider if any
                        if (Physics.Raycast(ray, out debugHit, float.PositiveInfinity, spawner.groundLayers))
                        {
                            newPos = debugHit.point;
                            Debug.Log($"Snap to {debugHit.collider.gameObject.name} (layer {LayerMask.LayerToName(debugHit.collider.gameObject.layer)})");
                        }
                        else
                        {
                            newPos = sp.position; // stay put if nothing hit
                            Debug.Log("Snap raycast missed – staying put");
                        }
                    }
                    else
                    {
                        // Fallback to ground plane
                        Plane ground = new Plane(Vector3.up, new Vector3(0, spawner.groundPlaneY, 0));
                        if (ground.Raycast(ray, out float distance))
                        {
                            newPos = ray.GetPoint(distance);
                        }
                        else
                        {
                            newPos = sp.position; // should not happen
                        }
                    }

                    sp.position = newPos;
                    EditorUtility.SetDirty(sp);
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                if (e.button == 1 && draggingIndex >= 0)
                {
                    draggingIndex = -1;
                    e.Use();
                }
                break;

            case EventType.Layout:
                if (e.button == 1)
                    HandleUtility.AddDefaultControl(controlID);
                break;
        }

        // Highlight dragged spawnpoint
        if (draggingIndex >= 0 && draggingIndex < spawner.spawnpoints.Count && spawner.spawnpoints[draggingIndex] != null)
        {
            Handles.color = Color.yellow;
            Handles.SphereHandleCap(0, spawner.spawnpoints[draggingIndex].position, Quaternion.identity, 0.5f, EventType.Repaint);
        }

        if (e.type == EventType.MouseDrag)
            HandleUtility.Repaint();
    }
}
#endif