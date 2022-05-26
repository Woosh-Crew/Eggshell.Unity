using UnityEngine;

namespace Eggshell.Unity
{
	public class Engine : Project
	{
		// Initialization

		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
		private static void Initialize()
		{
			Crack( new Unity() );
		}
	}
}
