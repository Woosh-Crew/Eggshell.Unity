using UnityEngine;

namespace Eggshell.Unity
{
	/// <summary> 
	/// A Tripod is Eggshell.Unity's camera system. It acts kinda like a wrapper for a singleton camera
	/// pattern, Which allows us to be super performant. You use the Tripod for manipulating the Main Camera
	/// every frame (Called in the Late Update Loop). You can easily swap them out on the client
	/// </summary>
	[Group( "Tripods" )]
	public class Tripod : IObject
	{
		public Library ClassInfo { get; }

		public Tripod()
		{
			ClassInfo = Library.Register( this );
		}

		~Tripod()
		{
			Library.Unregister( this );
		}

		// Tripod Callbacks
		// --------------------------------------------------------------------------------------- //

		/// <summary>
		/// Called every Tripod update. Used to control the main camera,
		/// without any overhead of complications.
		/// </summary>
		public virtual void OnBuild( ref Setup camSetup ) { }

		/// <summary>
		/// Called when the Tripod is now the active one, use this
		/// for snapping your tripod to its initial position and rotation.
		/// </summary>
		public virtual void Activated( ref Setup camSetup ) { }

		/// <summary>
		/// Called when the tripod goes out of use. Use this for cleaning
		/// up resources if need be.
		/// </summary>
		public virtual void Deactivated() { }

		// Setup Buffer
		// --------------------------------------------------------------------------------------- //

		/// <summary>
		/// A Tripod.Setup is responsible for controlling how the
		/// main camera gets manipulated in the world. Tripod setups
		/// are built every frame (In the Late Update Loop).
		/// </summary>
		public struct Setup
		{
			/// <summary> Camera's FOV </summary>
			public float FieldOfView;

			/// <summary> FieldOfView Damping </summary>
			public float Damping;

			/// <summary> The position of the camera </summary>
			public Vector3 Position;

			/// <summary> The rotation of the camera </summary>
			public Quaternion Rotation;

			/// <summary> Clipping Planes, X = Near, Y = Far </summary>
			public Vector2 Clipping;
		}

		// Builder Component
		// --------------------------------------------------------------------------------------- //

		/// <summary>
		/// A Tripod.Builder is responsible for controlling the logic
		/// of how tripods are controlled and built. Override this in your
		/// game to implement your own custom logic.
		/// </summary>
		public class Builder : IComponent<Game>
		{
			protected Tripod Current { get; set; }

			public virtual void Created( Camera camera )
			{
				camera.renderingPath = RenderingPath.DeferredShading;
				camera.gameObject.AddComponent<FlareLayer>();
			}

			public virtual void Build( ref Setup setup )
			{
				var cam = Active();

				if ( Current != cam )
				{
					Current?.Deactivated();
					Current = cam;
					Current?.Activated( ref setup );
				}

				Current?.OnBuild( ref setup );
				OnSetup( ref setup );
			}

			protected virtual Tripod Active() { return null; }
			protected virtual void OnSetup( ref Setup setup ) { }

			// IComponent			

			public void OnAttached( Game item ) { }
		}
	}
}
