#if UNITY_EDITOR
using Entry = UnityEditor.InitializeOnLoadMethodAttribute;
using Startup = Eggshell.Unity.Internal.UnityEditor;

#else
using Entry = UnityEngine.RuntimeInitializeOnLoadMethodAttribute;
using Startup = Eggshell.Unity.Internal.UnityStandalone;
#endif

namespace Eggshell.Unity
{
	/// <summary>
	/// The Eggshell.Unity initialization point, Creates the Unity Bootstrap
	/// and start initializing systems.
	/// </summary>
	[Order( -10 )]
	public sealed class Engine : Project
	{
		[Entry]
		private static void Initialize()
		{
			Crack( new Startup() );
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

			#if !UNITY_EDITOR
			OnPlaying();
			#endif
		}

		public override void OnUpdate()
		{
			Game?.OnUpdate();
		}

		public override void OnShutdown()
		{
			#if !UNITY_EDITOR
			OnExiting();
			#endif
		}

		public void OnPlaying()
		{
			(Game ??= Setup()).OnReady();

			Terminal.Log.Info( "Playing" );
		}

		public void OnExiting()
		{
			Game?.OnShutdown();
			Game = null;

			Terminal.Log.Info( "Exiting" );
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
