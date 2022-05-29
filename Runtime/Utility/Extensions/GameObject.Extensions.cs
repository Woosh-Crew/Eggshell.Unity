using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eggshell.Unity
{
	public static class GameObjectExtensions
	{
		public static GameObject MoveTo( this GameObject self, Scene scene )
		{
			SceneManager.MoveGameObjectToScene(self, scene);
			return self;
		}
	}
}
