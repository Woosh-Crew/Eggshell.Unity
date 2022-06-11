using UnityEngine;

#if POST_FX
using UnityEngine.Rendering.PostProcessing;
#endif

namespace Eggshell.Unity
{
    /// <summary>
    /// The Camcorder is responsible for controlling the output of the Tripod 
    /// call stack. It applies the transform to the main camera that was built
    /// by the Tripod call-stack, as well as handling any post processing.
    /// </summary>
    public sealed class Camcorder : Module, Game.Callbacks
    {
        /// <summary>
        /// The camera that the camcorder is using to translate, and change the
        /// options of. Which includes FOV, Clipping, Position, Rotation, etc.
        /// </summary>
        public Camera Camera { get; private set; }

#if POST_FX

        /// <summary>
        /// This is the Layer that gets generated / created when the Camcorder
        /// creates the Main Camera. Only appears if Post Processing is added
        /// the project.
        /// </summary>
        public PostProcessLayer Layer { get; private set; }

        /// <summary>
        /// The Debug layer for the post processing layer that gets generated
        /// when the Camcorder creates the camera. Only appears if Post 
        /// Processing is added the project.
        /// </summary>
        public PostProcessDebug Debug { get; private set; }

#endif

        // Runtime Game
        // --------------------------------------------------------------------------------------- //

        public void OnPlaying()
        {
            if (Builder == null)
            {
                // Not valid Game
                return;
            }

            Setup = Builder.Default;

            // Setup Camera
            var go = new GameObject("Main Camera");
            go.AddComponent<AudioListener>();

            Camera = go.AddComponent<Camera>();
            Camera.depth = 5;

#if POST_FX

            // Setup Post Processing (Builtin Postfx)

            Layer = go.AddComponent<PostProcessLayer>();
            Layer.Init(UnityEngine.Resources.Load<PostProcessResources>("PostProcessResources"));

            Layer.volumeTrigger = go.transform;
            Layer.volumeLayer = LayerMask.GetMask("TransparentFX", "Water");
            Layer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;

            Debug = go.AddComponent<PostProcessDebug>();
            Debug.postProcessLayer = Layer;

#endif

            Builder.Created(Camera);
            GameObject.DontDestroyOnLoad(go);
        }

        public void OnExiting()
        {
            if (Builder == null)
            {
                // Not valid Game
                return;
            }

            GameObject.Destroy(Camera.gameObject);

            Setup = default;
            Camera = null;
        }

        // Camcorder Loop
        // --------------------------------------------------------------------------------------- //

        /// <summary>
        /// The Tripod Builder this Camcorder is using for starting the initial
        /// call chain for building tripods. This gets applied automatically from
        /// your game class, Tripod.Builder is a Component on it.
        /// </summary>
        public Tripod.Builder Builder => Game.Active().Components.Get<Tripod.Builder>();

        /// <summary>
        /// The current Tripod Setup that is being processed. This is immutable.
        /// But incredibly useful for getting info about the setup, even without being
        /// inside the call chain.
        /// </summary>
        public Tripod.Setup Setup { get; private set; }

        void Game.Callbacks.OnLoop()
        {
            if (Camera == null)
            {
                // Modules run in Editor, so don't do anything.
                return;
            }

            var setup = Setup;

            // Default FOV
            setup.FieldOfView = 68;
            setup.Clipping = new(0.1f, 700);

            // Build the setup, from game.
            Builder.Build(ref setup);
            Builder.Apply(Camera, setup);

            // Reapply the setup
            Setup = setup;
        }
    }
}
