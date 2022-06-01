﻿using UnityEngine;

namespace Eggshell.Unity
{
	public sealed class Camcorder : Module, Game.Callbacks
	{
		public Camera Camera { get; private set; }

		// Runtime Game
		// --------------------------------------------------------------------------------------- //

		public void OnPlaying()
		{
			Builder = Engine.Game.Components.Get<Tripod.Builder>();

			// Setup Camera
			var go = new GameObject( "Camera" );
			go.AddComponent<AudioListener>();

			Camera = go.AddComponent<Camera>();
			Camera.depth = 5;

			GameObject.DontDestroyOnLoad( go );
		}

		public void OnExiting()
		{
			GameObject.Destroy( Camera );
			
			Camera = null;
			Builder = null;
		}

		// Camcorder Loop
		// --------------------------------------------------------------------------------------- //

		public Tripod.Builder Builder { get; private set; }

		public Tripod.Setup Setup { get; private set; } = new()
		{
			FieldOfView = 68,
			Rotation = Quaternion.identity,
			Position = Vector3.zero,
		};

		public override void OnUpdate()
		{
			if ( Terminal.Editor || Camera == null )
			{
				// Modules run in Editor, so don't do anything.
				return;
			}

			var setup = Setup;

			// Default FOV
			setup.FieldOfView = 68;
			setup.Clipping = new( 0.1f, 700 );

			// Build the setup, from game.
			Builder.Build( ref setup );
			Apply( Camera, setup );

			Setup = setup;
		}

		private void Apply( Camera camera, Tripod.Setup setup )
		{
			var trans = camera.transform;
			trans.localPosition = setup.Position;
			trans.localRotation = setup.Rotation;

			camera.fieldOfView = setup.Damping > 0 ? Mathf.Lerp( camera.fieldOfView, setup.FieldOfView, setup.Damping * Time.deltaTime ) : setup.FieldOfView;
			camera.farClipPlane = setup.Clipping.y;
			camera.nearClipPlane = setup.Clipping.x;
		}
	}
}
