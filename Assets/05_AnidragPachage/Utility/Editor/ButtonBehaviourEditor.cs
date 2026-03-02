#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ButtonBehaviour))]
public class ButtonBehaviourEditor : Editor
{
    SerializedProperty resolveTypeProp;
    SerializedProperty targetProp;
    SerializedProperty audioClipProp;
    SerializedProperty audioOutputProp;
    SerializedProperty childIndexProp;
    SerializedProperty grandchildIndexProp;

    private void OnEnable()
    {
        resolveTypeProp = serializedObject.FindProperty("resolveType");
        targetProp = serializedObject.FindProperty("target");
        audioClipProp = serializedObject.FindProperty("audioClip");
        audioOutputProp = serializedObject.FindProperty("audioAutput");
        childIndexProp = serializedObject.FindProperty("childIndex");
        grandchildIndexProp = serializedObject.FindProperty("grandchildIndex");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(resolveTypeProp);
        EditorGUILayout.PropertyField(targetProp);
        EditorGUILayout.PropertyField(audioClipProp);
        EditorGUILayout.PropertyField(audioOutputProp);

        // Show indexes based on enum
        var action = (ButtonBehaviour.ButtonActivationResult)resolveTypeProp.enumValueIndex;
        bool showIndexes = action == ButtonBehaviour.ButtonActivationResult.DissableChildObject ||
                           action == ButtonBehaviour.ButtonActivationResult.DissableGrandChildObject;

        if (showIndexes)
        {
            EditorGUILayout.LabelField("Child/Grandchild Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(childIndexProp);

            if (action == ButtonBehaviour.ButtonActivationResult.DissableGrandChildObject)
                EditorGUILayout.PropertyField(grandchildIndexProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
