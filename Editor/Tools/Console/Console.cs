using UnityEngine;

namespace Eggshell.Unity.Tools
{
	/// <summary>
	/// Eggshells custom terminal for invoking commands and seeing logs.
	/// (by type, time, etc). Probably better than the default one.
	/// </summary>
	public class Console : Tool
	{
		public override	void OnGUI()
		{
			GUILayout.Label( "Hello" );
		}
	}
}
