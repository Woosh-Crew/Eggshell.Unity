// Roslyn Analyzers
#pragma warning disable IDE1006 

namespace Eggshell.Unity
{
    /// <summary>
    /// The Game class is the entry point into your unity project. Override
    /// this to provide custom logic for application wide specifics.
    /// </summary>
    [Singleton, Title("Default Game")]
    public class Game : IObject
    {
        public Library ClassInfo { get; }

        /// <summary>
        /// Creates a new game with the default tripod builder component, incredibly
        /// useful for quickly setting up a game.  
        /// </summary>
        public Game() : this(new()) { }

        /// <summary>
        /// Creates a new game with a tripod component that is used for building
        /// the tripods. This is the main constructor.
        /// </summary>
        public Game(Tripod.Builder tripods = null)
        {
            ClassInfo = Library.Register(this);
            Assert.IsNull(ClassInfo);

            Components = new(this)
            {
                tripods,
            };
        }

        // Game Registration
        // --------------------------------------------------------------------------------------- //

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

        /// <summary>
        /// Callbacks specifically made for modules, so they know what to do when we are
        /// playing or exiting (editor = playmode state change, runtime = start, shutdown)
        /// </summary>
        public interface Callbacks
        {
            /// <summary>
            /// A callback for when we actually start playing the game. In editor this means
            /// we have switched to playmode, and at runtime this means the application has
            /// started.
            /// </summary>
            void OnPlaying();

            /// <summary>
            /// A callback for when we actually have stopped playing the game. In editor this
            /// means we have left playmode, and at runtime it means the application is 
            /// shutting down.
            /// </summary>
            void OnExiting();
        }

        // Dependency Injection
        // --------------------------------------------------------------------------------------- //

        /// <summary>
        /// Dependency injection logic for the game class, some components are initialized
        /// by the Game's constructor. Watch out for those if want to try and replace a
        /// prepackaged component.
        /// </summary>
        public Components<Game> Components { get; }
    }
}
