using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(CountdownTimer))]
public class InspectorButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CountdownTimer cdtScript = (CountdownTimer)target;
        EditorGUILayout.Space();
        if (GUILayout.Button("Reset Timer"))
        {
            cdtScript.ResetTimer();
        }
    }
    
}
