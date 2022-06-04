using UnityEngine;

namespace Eggshell.Unity.Inputs
{
	public sealed class Mouse
	{
		// Control

		public bool Visible { get; set; }
		public bool Locked { get; set; }
		public bool Confined { get; set; }

		// Input

		public Vector3 Position { get; private set; }
		public Vector2 Delta { get; private set; }
		public float Wheel { get; private set; }

		internal void Sample()
		{
			Position = Input.mousePosition;
			Delta = new( Input.GetAxis( "Mouse X" ), Input.GetAxis( "Mouse Y" ) );
			Wheel = Input.mouseScrollDelta.y;
		}

		public void Clear()
		{
			Delta = Vector2.zero;
			Wheel = 0;
		}
	}
}
