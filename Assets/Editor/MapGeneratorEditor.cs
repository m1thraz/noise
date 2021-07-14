using UnityEngine;
using System.Collections;
using UnityEditor;

//add custom button "Generate" to the unity editor
[CustomEditor (typeof (MapGenerator))]
public class MapGeneratorEditor : Editor {

	public override void OnInspectorGUI() {
		MapGenerator mapGen = (MapGenerator)target;

		//auto generate map when changing values
		if (DrawDefaultInspector ()) {
			if (mapGen.autoUpdate) {
				mapGen.DrawMapInEditor();
			}
		}

		if (GUILayout.Button ("Generate")) {
			mapGen.DrawMapInEditor();
		}
	}
}
