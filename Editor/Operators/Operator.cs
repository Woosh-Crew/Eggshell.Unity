using System;
using UnityEditor;
using UnityEngine;

namespace Eggshell.Unity.Tools
{
	public class Search : EditorWindow
	{
		[MenuItem( "Eggshell/Search" )]
		public static void Open()
		{
			GetWindow<Search>().Show();
		}

		private string _input = string.Empty;

		private void OnGUI()
		{
			_input = EditorGUILayout.TextField( "Input", _input );

			GUI.enabled = !string.IsNullOrEmpty( _input );

			if ( GUILayout.Button( "Submit" ) )
			{
				Operator.Run<Action<string>>( _input, s => s.Log() );
			}

			GUI.enabled = true;
		}
	}

	/// <summary>
	/// This will save all open and modified scenes without asking if it
	/// should or not.
	/// </summary>
	[Library( "eggshell.ops.scenes.save" ), Title( "Save Modified Scenes" )]
	public class Save : Operator
	{
		protected override void OnExecute()
		{
			UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
		}
	}

	/// <summary>
	/// This will save all open and modified scenes without asking if it
	/// should or not.
	/// </summary>
	[Library( "eggshell.ops.files.browser" ), Title( "Save Modified Scenes" )]
	public class FileBrowser : Operator<Action<string>>
	{
		protected override void OnExecute( Action<string> callback )
		{
			var value = EditorUtility.OpenFilePanel( "File Browser", "", "" );
			callback?.Invoke( value );
		}
	}

	/// <summary>
	/// An operator is a operation that is performed in the editor. Operators
	/// can be searched for in the editor is well. It is also recommended you
	/// use operators for tools that do actions / operations. This operator
	/// can have a callback.
	/// </summary>
	[Library( "tools.operator_generic" )]
	public abstract class Operator<T> : Operator where T : Delegate
	{
		public void Execute( T callback )
		{
			OnExecute( callback );
		}

		protected abstract void OnExecute( T callback );
		protected sealed override void OnExecute() { Execute( null ); }
	}

	/// <summary>
	/// An operator is a operation that is performed in the editor. Operators
	/// can be searched for in the editor is well. It is also recommended you
	/// use operators for tools that do actions / operations.
	/// </summary>
	[Library( "tools.operator" )]
	public abstract class Operator : IObject
	{
		public static void Run( string name )
		{
			Library.Database[name]?.Create<Operator>().Execute();
		}

		public static void Run<T>( string name, T callback ) where T : Delegate
		{
			Library.Database[name]?.Create<Operator<T>>().Execute( callback );
		}

		public Library ClassInfo => GetType();

		// Operator

		public virtual bool Valid() { return true; }

		public void Execute()
		{
			OnExecute();
		}

		protected abstract void OnExecute();
	}
}
