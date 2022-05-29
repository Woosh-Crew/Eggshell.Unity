using UnityEngine;

namespace Eggshell.Unity
{
	/// <summary>
	/// The Eggshell.Unity initialization point, Creates the Unity Bootstrap
	/// and start initializing systems.
	/// </summary>
	[Order( -10 )]
	public sealed class Engine : Project
	{
		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
		private static void Initialize()
		{
			Crack( new Unity() );
		}

		// Engine

		/// <summary>
		/// The current game that is in session, this will
		/// be automatically created when the application launches.
		/// </summary>
		public static Game Game { get; private set; }

		// Logic

		public override void OnReady()
		{
			Game = Setup();
			
			// Initialize
			
			Game?.OnReady();
		}

		public override void OnUpdate()
		{
			Game?.OnUpdate();
		}

		public override void OnShutdown()
		{
			Game?.OnShutdown();
			Game = null;
		}

		private Game Setup()
		{
			var game = Library.Database.Find<Game>()?.Create<Game>();

			if ( game?.ClassInfo == null )
			{
				Terminal.Log.Error( "Game failed to register. Cancelling game setup" );
				return null;
			}

			Terminal.Log.Info( $"Setting up {game.ClassInfo.Title} as the Game" );
			return game;
		}
	}
}
