using System;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace com.vrsuya.jiwozo {

    public class MissingScriptJiwozo : EditorWindow {

        [MenuItem("Tools/VRSuya/Jiwozo/Select All GameObject of Missing Script")]
		/// <summary>Scene에 존재하는 Missing 컴포넌트를 포함한 GameObject를 선택합니다</summary>
		public static void SelectMissingGameObjects() {
            Component[] MissingComponents = GetAllMissingComponents();
            if (MissingComponents.Length > 0) {
				Undo.IncrementCurrentGroup();
				Undo.SetCurrentGroupName("Select All Missing GameObject");
				int UndoGroupIndex = Undo.GetCurrentGroup();
				GameObject[] MissingGameObjects = MissingComponents.Select(TargetComponent => TargetComponent.gameObject).ToArray();
				Selection.objects = MissingGameObjects;
                Debug.Log("[VRSuya] " + MissingComponents.Length + " GameObjects have Missing Component Selected");
				Undo.CollapseUndoOperations(UndoGroupIndex);
			} else {
				Debug.Log("[VRSuya] Not found GameObject has Missing Component");
			}
            return;
        }

		[MenuItem("Tools/VRSuya/Jiwozo/Remove All Missing Script Component")]
		/// <summary>Scene에 존재하는 Missing 컴포넌트를 포함한 GameObject를 선택합니다</summary>
		public static void RemoveMissingComponents() {
			Component[] MissingComponents = GetAllMissingComponents();
			if (MissingComponents.Length > 0) {
				Undo.IncrementCurrentGroup();
				Undo.SetCurrentGroupName("Remove All Missing Component");
				int UndoGroupIndex = Undo.GetCurrentGroup();
				for (int Index = 0; Index < MissingComponents.Length; Index++) {
					GameObject DirtyGameObject = MissingComponents[Index].gameObject;
					Undo.RecordObject(DirtyGameObject, "Remove All Missing Component");
					DestroyImmediate(MissingComponents[Index]);
					EditorUtility.SetDirty(DirtyGameObject);
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
				Debug.Log("[VRSuya] " + MissingComponents.Length + " Missing Script Components Removed");
			} else {
				Debug.Log("[VRSuya] Not found Missing Script Component");
			}
			return;
		}

		/// <summary>Scene에 존재하는 Missing 컴포넌트 배열을 반환합니다</summary>
		/// <returns>Scene에 존재하는 모든 Missing 컴포넌트 배열</returns>
		private static Component[] GetAllMissingComponents() {
			Component[] AllComponents = SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(gameObject => gameObject.GetComponentsInChildren<Component>(true)).ToArray();
            Component[] MissingComponents = Array.FindAll(AllComponents, TargetComponent => TargetComponent == null).ToArray();
            return MissingComponents;
        }
    }
}