using UnityEngine;

#if POST_FX
using UnityEngine.Rendering.PostProcessing;
#endif

namespace Eggshell.Unity
{
    public sealed class Camcorder : Module, Game.Callbacks
    {
        public Camera Camera { get; private set; }

#if POST_FX

        public PostProcessLayer Layer { get; private set; }
        public PostProcessDebug Debug { get; private set; }

#endif

        // Runtime Game
        // --------------------------------------------------------------------------------------- //

        public void OnPlaying()
        {
            Builder = Engine.Game.Components.Get<Tripod.Builder>();

            if (Builder == null)
            {
                // Not valid Game
                return;
            }

            // Setup Camera
            var go = new GameObject("Camera");
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
            Setup = Default;

            Camera = null;
            Builder = null;
        }

        // Camcorder Loop
        // --------------------------------------------------------------------------------------- //

        public Tripod.Builder Builder { get; private set; }

        public Tripod.Setup Setup { get; private set; } = Default;

        private static Tripod.Setup Default => new()
        {
            FieldOfView = 68,
            Rotation = Quaternion.identity,
            Position = Vector3.zero,
        };

        public override void OnUpdate()
        {
            if (Terminal.Editor || Camera == null || Builder == null)
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
            Apply(Camera, setup);

            Setup = setup;
        }

        private void Apply(Camera camera, Tripod.Setup setup)
        {
            var trans = camera.transform;
            trans.position = setup.Interpolation > 0 ? Vector3.Lerp(trans.position, setup.Position, setup.Interpolation * Time.deltaTime) : setup.Position;
            trans.rotation = setup.Interpolation > 0 ? Quaternion.Slerp(trans.rotation, setup.Rotation, setup.Interpolation * Time.deltaTime) : setup.Rotation;

            camera.fieldOfView = setup.Damping > 0 ? Mathf.Lerp(camera.fieldOfView, setup.FieldOfView, setup.Damping * Time.deltaTime) : setup.FieldOfView;

            camera.farClipPlane = setup.Clipping.y;
            camera.nearClipPlane = setup.Clipping.x;
        }
    }
}
