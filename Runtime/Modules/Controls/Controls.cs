using System.Collections;
using System.Collections.Generic;
using Eggshell.Unity.Inputs;
using UnityEngine;
using Event = System.Action;

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

	public class Scheme : IEnumerable<Bind>, IObject
	{
		public Library ClassInfo { get; }

		public Scheme()
		{
			ClassInfo = Library.Register( this );
		}

		~Scheme()
		{
			Library.Unregister( this );
		}

		private readonly SortedList<int, Bind> _binds = new();

		public Bind Get( string key )
		{
			return _binds.TryGetValue( key.Hash(), out var value ) ? value : null;
		}

		public T Get<T>( string key ) where T : Bind
		{
			return (T)Get( key );
		}

		public void Add( Bind bind )
		{
			var key = bind.Name.Hash();

			if ( _binds.ContainsKey( key ) )
			{
				Terminal.Log.Warning( $"Replacing Binding {key}" );
				_binds[key] = bind;
				return;
			}

			_binds.Add( key, bind );
		}

		// Enumerator

		public IEnumerator<Bind> GetEnumerator()
		{
			return _binds.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

	}

	public class Axis : Bind
	{
		public string Sampler { get; }
		public bool Raw { get; }

		public Axis( string name, string axis, bool raw = true ) : base( name )
		{
			Sampler = axis;
			Raw = raw;
		}

		public override void Sample()
		{
			Value = Raw ? Input.GetAxisRaw( Sampler ) : Input.GetAxis( Sampler );
			Held = Value > 0;
		}

		public override void Clear()
		{
			Value = 0;
		}
	}

	public class Action : Bind
	{
		public KeyCode Key { get; }

		public Event OnPressed { get; }
		public Event OnReleased { get; }

		public Action( string name, KeyCode key ) : base( name )
		{
			Key = key;
		}

		public Action( string name, KeyCode key, Event onPressed = null, Event onReleased = null ) : this( name, key )
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

	public abstract class Bind
	{
		public string Name { get; }

		public Bind( string name )
		{
			Name = name;
		}


		public abstract void Sample();
		public abstract void Clear();

		// Networking
		public virtual void Write() { }
		public virtual void Read() { }

		public float Value { get; protected set; }
		public bool Pressed { get; protected set; }
		public bool Held { get; protected set; }
		public bool Released { get; protected set; }
	}
}
