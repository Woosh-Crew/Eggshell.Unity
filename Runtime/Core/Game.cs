namespace Eggshell.Unity
{
	public class Phoenix : Game
	{
		public override void OnReady() { }
	}

	/// <summary>
	/// The Game class is the entry point into your unity project. Override
	/// this to provide custom logic for application wide specifics.
	/// </summary>
	[Singleton]
	public class Game : IObject
	{
		public Library ClassInfo { get; }

		public Game()
		{
			ClassInfo = Library.Register( this );
			Assert.IsNull( ClassInfo );
		}

		// Registration

		/// <summary>
		/// OnReady is called by the Engine module when we are entering playmode
		/// (in editor) or when the application starts (in standalone)
		/// </summary>
		public virtual void OnReady() { }

		/// <summary>
		/// OnUpdate is a direct hook to unity's update loop. Allows us to use the
		/// update call-chain without MonoBehaviour (yay!)
		/// </summary>
		public virtual void OnUpdate() { }

		/// <summary>
		/// OnShutdown gets called by the Engine module when we are exiting playmode
		/// (in editor) or when the application is shutting down (in standalone)
		/// </summary>
		public virtual void OnShutdown() { }

		// Components

		/// <summary>
		/// Dependency injection logic for the game class, some components are initialized
		/// by the Game's constructor. Watch out for those if want to try and replace a
		/// prepackaged component.
		/// </summary>
		public Components<Game> Components { get; }
	}
}
