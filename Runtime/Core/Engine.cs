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
	/// (Bootstrap depends on whether we're in editor or standalone )
	/// and start initializing systems. Initialization gets called in both
	/// editor and runtime.
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
		// --------------------------------------------------------------------------------------- //

		/// <summary>
		/// Engine classes custom log level. We use this instead of info s  it looks
		/// nice in the console, and so they don't get stripped when building
		/// </summary>
		private const string Level = "Engine";

		/// <summary>
		/// The current game that is in session, this will
		/// be automatically created when the application launches.
		/// </summary>
		public static Game Game { get; private set; }

		// Module Callbacks
		
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

		// Runtime Game
		// --------------------------------------------------------------------------------------- //

		/// <summary>
		/// A Callback for when we have switched to play mode (in the editor)
		/// or when we have launched the application and are ready to start
		/// playing (at standalone)
		/// </summary>
		public void OnPlaying()
		{
			(Game ??= Setup()).OnReady();

			foreach ( var module in All )
			{
				(module as Game.Callbacks)?.OnPlaying();
			}

			Terminal.Log.Entry( "Entering the Game", Level );
		}

		/// <summary>
		/// A Callback for when we are exiting play mode (in the editor)
		/// or when we are shutting down the application (at standalone)
		/// </summary>
		public void OnExiting()
		{
			Game?.OnShutdown();
			
			foreach ( var module in All )
			{
				(module as Game.Callbacks)?.OnExiting();
			}
			
			Game = null;
			
			Terminal.Log.Entry( "Exiting the Game", Level );
		}

		/// <summary>
		/// Finds the right game class using Eggshell's reflection system.
		/// if none could be fine, it'll just create a instance of the game
		/// class itself. 
		/// </summary>
		private static Game Setup()
		{
			var game = Library.Database.Find<Game>()?.Create<Game>();

			if ( game?.ClassInfo == null )
			{
				Terminal.Log.Error( "Game failed to register. Cancelling game setup" );
				return null;
			}

			Terminal.Log.Entry( $"Setting up {game.ClassInfo.Title} as the Game", Level );
			return game;
		}
	}
}
