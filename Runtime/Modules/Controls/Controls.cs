using Eggshell.Unity.Inputs;
using UnityEngine;

namespace Eggshell.Unity
{
	/// <summary>
	/// Controls is Eggshell.Unity's input system. It allows you to easily create
	/// new binds that can be changed at anytime, and are automatically serialized.
	/// You use these binds in an object oriented way, instead of using an Enum
	/// going to a bunch of unity internal calls.
	/// </summary>
	public sealed class Controls : Module, Game.Callbacks
	{
		// Public Static API
		// --------------------------------------------------------------------------------------- //

		/// <summary>
		/// The current sheet that is currently being sampled and processed, this happens
		/// per client. So each client can have its own input.
		/// </summary>
		public static Sheet Processing { get; private set; }

		/// <summary>
		/// The current mouse data that has been sampled this frame. (Gets sampled
		/// every frame). Mouse is how you change cursor state, and get mouse delta
		/// </summary>
		public static Mouse Mouse => Processing.Mouse;

		/// <summary>
		/// The current bindings data that has been sampled this frame. (Gets
		/// sampled every frame). Scheme is how you get binds, (axis or action)
		/// </summary>
		public static Scheme Scheme => Processing.Scheme;

		/// <summary>
		/// Is the inputted name, currently being pressed on the processing client?
		/// (Gets the bind from the scheme using the id)
		/// </summary>
		public static bool Pressed( string name )
		{
			return Scheme.Get( name )?.Pressed ?? false;
		}

		/// <summary>
		/// Is the inputted name, currently being held on the processing client?
		/// (Gets the bind from the scheme using the id)
		/// </summary>
		public static bool Held( string name )
		{
			return Scheme.Get( name )?.Held ?? false;
		}

		/// <summary>
		/// Is the inputted name, currently being released on the processing client?
		/// (Gets the bind from the scheme using the id)
		/// </summary>
		public static bool Released( string name )
		{
			return Scheme.Get( name )?.Released ?? false;
		}

		// Controls
		// --------------------------------------------------------------------------------------- //

		public override void OnUpdate()
		{
			if ( Terminal.Editor || Processing?.Scheme == null )
			{
				// Modules run in Editor, so don't do anything.
				return;
			}

			Processing.Sample();

			// Applying
			Cursor.visible = Processing.Mouse.Visible;
			Cursor.lockState = Processing.Mouse.Locked ? CursorLockMode.Locked : CursorLockMode.None;

			if ( !Processing.Mouse.Locked && Processing.Mouse.Confined )
			{
				Cursor.lockState = CursorLockMode.Confined;
			}
		}

		public void OnPlaying()
		{
			// Get Scheme from Game
			Processing = new( new() { Locked = true, Visible = false }, null );
		}

		public void OnExiting()
		{
			// Revert everything, so we can quick load in Editor
			Processing = null;
		}

		/// <summary>
		/// An input sheet is a 
		/// </summary>
		public class Sheet
		{
			public Sheet( Mouse mouse, Scheme scheme )
			{
				Mouse = mouse;
				Scheme = scheme;
			}

			/// <summary> Mouse Data, gets reset every Input Sample </summary>
			public Mouse Mouse { get; }

			/// <summary> You're Games keybindings </summary>
			public Scheme Scheme { get; }

			/// <summary> Where a pawns Eyes should be facing (ViewAngles) </summary>
			public Vector3 View { get; set; }

			/// <summary> Clears the Input Setup </summary>
			public void Clear()
			{
				Mouse?.Clear();

				foreach ( var binding in Scheme )
				{
					binding.Clear();
				}
			}

			public void Sample()
			{
				Mouse?.Sample();

				foreach ( var binding in Scheme )
				{
					binding.Sample();
				}
			}
		}
	}
}