using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Generator))]
public class GeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Generate"))
        {
            Generator generator = (Generator)target;
            generator.Clear();
            generator.Generate();
        }

        if (GUILayout.Button("Clear"))
        {
            Generator generator = (Generator)target;
            generator.Clear();
        }
    }
}
