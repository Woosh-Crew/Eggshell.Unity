using UnityEditor.SceneManagement;

namespace Eggshell.Unity.Operators
{
	/// <summary>
	/// This will save all open and modified scenes without asking if it
	/// should or not.
	/// </summary>
	[Library( "eggshell.ops.scenes.save" ), Title( "Save Modified Scenes" )]
	public class SaveScenes : Operator
	{
		public override bool Valid()
		{
			return EditorSceneManager.sceneCount != 0;
		}

		protected override void OnExecute()
		{
			EditorSceneManager.SaveOpenScenes();
		}
	}
}
