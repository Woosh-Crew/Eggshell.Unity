using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eggshell.Unity
{
	public static class SceneExtensions
	{
		/// <summary> Unload this Scene. </summary>
		public static AsyncOperation Unload( this Scene self )
		{
			return self.isLoaded && self.IsValid() ? SceneManager.UnloadSceneAsync( self.name ) : null;
		}

		public static void Grab( this Scene self, GameObject gameObject )
		{
			SceneManager.MoveGameObjectToScene( gameObject, self );
		}
	}
}
