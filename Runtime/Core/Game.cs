namespace Eggshell.Unity
{
	/// <summary>
	/// The Game class is the entry point into your unity project. Override
	/// this to provide custom logic for application wide specifics.
	/// </summary>
	[Singleton]
	public class Game : IObject
	{
		public Library ClassInfo { get; }

		// Constructors

		public Game()
		{
			ClassInfo = Library.Register( this );
			Assert.IsNull( ClassInfo );
		}

		// Registration

		// protected virtual void OnSetup( ref Scheme scheme ) { }

		public virtual void OnReady() { }
		public virtual void OnUpdate() { }
		public virtual void OnShutdown() { }

		// Components

		public Components<Game> Components { get; }
	}
}
