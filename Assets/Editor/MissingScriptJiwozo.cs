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
			GameObject[] MissingGameObjects = GetAllGameObjectHasMissingComponent();
            if (MissingGameObjects.Length > 0) {
				Undo.IncrementCurrentGroup();
				Undo.SetCurrentGroupName("Select All Missing GameObject");
				int UndoGroupIndex = Undo.GetCurrentGroup();
				Selection.objects = MissingGameObjects;
                Debug.Log("[VRSuya] " + MissingGameObjects.Length + " GameObjects have Missing Component Selected");
				Undo.CollapseUndoOperations(UndoGroupIndex);
			} else {
				Debug.Log("[VRSuya] Not found GameObject has Missing Component");
			}
            return;
        }

		[MenuItem("Tools/VRSuya/Jiwozo/Remove All Missing Script Component")]
		/// <summary>Scene에 존재하는 Missing 컴포넌트들을 삭제합니다</summary>
		public static void RemoveMissingComponents() {
			GameObject[] MissingGameObjects = GetAllGameObjectHasMissingComponent();
			if (MissingGameObjects.Length > 0) {
				Undo.IncrementCurrentGroup();
				Undo.SetCurrentGroupName("Remove All Missing Component");
				int UndoGroupIndex = Undo.GetCurrentGroup();
				int DeletedComponentCount = 0;
				foreach (GameObject TargetGameObject in MissingGameObjects) {
					Undo.RecordObject(TargetGameObject, "Remove All Missing Component");
					int MissingComponentCount = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(TargetGameObject);
					DeletedComponentCount = DeletedComponentCount + MissingComponentCount;
					EditorUtility.SetDirty(TargetGameObject);
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
				Debug.Log("[VRSuya] " + DeletedComponentCount + " Missing Script Components Removed");
			} else {
				Debug.Log("[VRSuya] Not found Missing Script Component");
			}
			return;
		}

		/// <summary>Scene에 존재하는 Missing 컴포넌트를 가지고 있는 GameObject 배열을 반환합니다</summary>
		/// <returns>Scene에 존재하는 모든 Missing 컴포넌트를 가지고 있는 GameObject 배열</returns>
		private static GameObject[] GetAllGameObjectHasMissingComponent() {
			GameObject[] MissingGameObjects = new GameObject[0];
			Transform[] AllTransforms = SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(gameObject => gameObject.GetComponentsInChildren<Transform>(true)).ToArray();
			foreach (Transform TargetTransform in AllTransforms) {
				if (Array.Exists(TargetTransform.GetComponents<Component>(), TargetComponent => TargetComponent == null)) {
					MissingGameObjects = MissingGameObjects.Concat(new GameObject[] { TargetTransform.gameObject }).ToArray();
				}
			}
            return MissingGameObjects;
        }
    }
}