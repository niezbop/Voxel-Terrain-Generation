using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Generation;

[CustomEditor(typeof(GeneratorScript))]
public class GeneratorInspector : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		GeneratorScript script = (GeneratorScript)target;

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Generation parameters", EditorStyles.boldLabel);
		script.Type = (GeneratorScript.GeneratorType)EditorGUILayout.EnumPopup("Generator type", script.Type);
		script.Width = EditorGUILayout.IntField("Width", script.Width);
		script.Length = EditorGUILayout.IntField("Length", script.Length);
		script.Height_Limit = EditorGUILayout.IntField("Height limit", script.Height_Limit);
		script.Use_seed = EditorGUILayout.Toggle("Use a seed", script.Use_seed);
		script.Seed = EditorGUILayout.IntField("Seed", script.Seed);

		EditorGUILayout.Space();
		if(GUILayout.Button("Initialize Map")) {
			script.InitializeMap();
		}
		if(GUILayout.Button("Generate")) {
			script.Generate();
		}
		GUI.enabled = script.generator is IVOXGenerator;
		if(GUILayout.Button("Create VOX")) {
			script.CreateVOXObject();
		}
	}
}
