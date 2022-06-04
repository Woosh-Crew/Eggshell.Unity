using UnityEngine;

namespace Eggshell.Unity.Inputs
{
	public class Action : Bind
	{
		public KeyCode Key { get; }

		public System.Action OnPressed { get; }
		public System.Action OnReleased { get; }

		public Action( string name, KeyCode key ) : base( name )
		{
			Key = key;
		}

		public Action( string name, KeyCode key, System.Action onPressed = null, System.Action onReleased = null ) : this( name, key )
		{
			OnPressed = onPressed;
			OnReleased = onReleased;
		}

		public override void Sample()
		{
			Pressed = Input.GetKeyDown( Key );
			Held = Input.GetKey( Key );
			Released = Input.GetKeyUp( Key );

			if ( Pressed )
				OnPressed?.Invoke();

			if ( Released )
				OnReleased?.Invoke();
		}

		public override void Clear()
		{
			Pressed = false;
			Held = false;
			Released = false;
		}
	}
}
