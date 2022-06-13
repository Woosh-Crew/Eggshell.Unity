using System;
using UnityEditor;

namespace Eggshell.Unity.Operators
{
    /// <summary>
    /// This will open a file browser. Override this and change the constructor
    /// to suit your needs of a file browser
    /// </summary>
    [Link("eggshell.ops.files.browser"), Title("Open File Browser")]
    public class FileBrowser : Operator<Action<string>>
    {
        public FileBrowser() : this("File Browser") { }

        public FileBrowser(string title = "File Browser", string extensions = "")
        {
            Title = title;
            Extensions = extensions;
        }

        public string Title { get; }
        public string Extensions { get; }

        protected virtual string OnOpen()
        {
            return EditorUtility.OpenFilePanel(Title, "", Extensions);
        }

        protected override void OnExecute(Action<string> callback)
        {
            var value = OnOpen();
            callback?.Invoke(value);
        }
    }
}
