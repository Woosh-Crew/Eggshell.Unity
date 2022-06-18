/*

using UnityEditor;
using UnityEngine;

namespace Eggshell.Unity.Editors
{
    [Title("Terminal")]
    public class TerminalEditorWindow : EditorWindow, IObject
    {
        public static TerminalEditorWindow Active { get; private set; }

        [MenuItem("Eggshell/Console %`")]
        public static void Open()
        {
            if (Active != null)
            {
                Active.Close();
                Active = null;

                return;
            }

            Active = CreateInstance<TerminalEditorWindow>();

            var width = 450;
            var height = 350;

            Active.position = new Rect((Screen.currentResolution.width / 2) - (width / 2), (Screen.currentResolution.height / 2) - (height / 2), width, height);
            Active.ShowPopup();
        }

        public Library ClassInfo { get; set; }

        private void Awake()
        {
            ClassInfo ??= Library.Register(this);
        }

        private void OnEnable()
        {
            titleContent = new GUIContent(ClassInfo.Title, ClassInfo.Help);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("This is an example of EditorWindow.ShowPopup", EditorStyles.wordWrappedLabel);

            if (GUILayout.Button("Agree!"))
            {
                Close();
            }
        }

        private void OnDestory()
        {
            Library.Unregister(this);
        }
    }
}

*/