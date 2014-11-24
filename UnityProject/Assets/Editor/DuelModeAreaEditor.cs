using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(DuelModeArea))]
public class DuelModeAreaEditor : Editor {
	// Make it so that the respawn points for each area can be dragged in the scene view
	void OnSceneGUI() {
		var area = target as DuelModeArea;

		var sp1 = area.squad1spawnPoint;
		var sp2 = area.squad2spawnPoint;
		var sp3 = area.pickupspawnPoint;
		Vector2 prevPos = area.transform.position;

		GUI.changed = false;
		area.squad1spawnPoint = (Vector2) Handles.PositionHandle(prevPos + sp1, Quaternion.identity) - prevPos;
		if (GUI.changed)
			EditorUtility.SetDirty(area);
		Undo.RecordObject(target, "Spawn Point Move");

		GUI.changed = false;
		area.squad2spawnPoint = (Vector2) Handles.PositionHandle(prevPos + sp2, Quaternion.identity) - prevPos;
		if (GUI.changed)
			EditorUtility.SetDirty(area);
		Undo.RecordObject(target, "Spawn Point Move");

		GUI.changed = false;
		area.pickupspawnPoint = (Vector2) Handles.PositionHandle(prevPos + sp3, Quaternion.identity) - prevPos;
		if (GUI.changed)
			EditorUtility.SetDirty(area);
		Undo.RecordObject(target, "Spawn Point Move");
	}
}
