using System;
using System.IO;
using System.Linq;
using Eggshell.Resources;
using UnityEngine.SceneManagement;

// Roslyn Analyzers
#pragma warning disable IDE1006

namespace Eggshell.Unity
{
    public class Listing
    {
        // Skybox / 360 Ball [Just a Map]
        public Resource<Skybox> Skybox { get; set; }

        // Models
        public Resource<Map> Interior { get; set; }
        public Resource<Map> Exterior { get; set; }

        public string Identifier { get; }
        public string Name { get; }
    }

    public class Skybox : Map { }

    [Group("Maps")]
    public class Map : IAsset
    {
        public static Archive[] Compatible { get; } = Library.Database.With<Archive>(e => e.Inherits<Binder>()).ToArray();

        #if UNITY_EDITOR

        [UnityEditor.MenuItem("Eggshell/Load Map")]
        private static void Load()
        {
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                Terminal.Log.Warning("Go into playmode, in order to load a map");
                return;
            }

            Assets.Load<Map>(UnityEditor.EditorUtility.OpenFilePanel("Select Map", "", "")).Request(_ => { });
        }

        #endif

        public Library ClassInfo { get; }
        protected Binder Bundle { get; set; }

        public Map()
        {
            ClassInfo = Library.Register(this);
            Assert.IsNull(ClassInfo);
        }

        [Group("Maps")]
        public interface Binder : IObject
        {
            Scene Scene { get; }

            void Load(Stream stream, Action finished);
            void Unload(Action finished);
        }

        // IAsset - Resource
        // --------------------------------------------------------------------------------------- //

        public Resource Resource { get; set; }

        bool IAsset.Setup(string extension)
        {
            // Get the correct binder using reflection
            Bundle = Array.Find(Compatible, e => e.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase))?.Attached.Create<Binder>();
            return Bundle != null;
        }

        void IAsset.Load(Stream stream, Action finished)
        {
            OnLoading();
            finished += OnLoad;

            // Tell Provider to load Map
            Bundle.Load(stream, finished);
        }

        protected virtual void OnLoad()
        {
            SceneManager.SetActiveScene(Bundle.Scene);

            foreach ( var module in Module.All )
            {
                (module as Callbacks)?.OnLoad(this);
            }
        }

        protected virtual void OnLoading()
        {
            foreach ( var module in Module.All )
            {
                (module as Callbacks)?.OnLoading(this);
            }
        }

        void IAsset.Unload(Action finished)
        {
            finished += OnUnload;

            // Tell Provider to unload Map
            Bundle.Unload(finished);
        }

        protected virtual void OnUnload()
        {
            foreach ( var module in Module.All )
            {
                (module as Callbacks)?.OnUnload(this);
            }
        }

        // IAsset - Resource
        // --------------------------------------------------------------------------------------- //

        /// <summary>
        /// Callbacks that get called for both Modules and Map Components. Use 
        /// this to implement OOP events.
        /// </summary>
        public interface Callbacks
        {
            /// <summary>
            /// A Callback for when a map starts to load, usable in Modules and
            /// Map components. Use this for unloading old maps.
            /// </summary>
            void OnLoading(Map map);

            /// <summary>
            /// A Callback for when a map finishes loaded, usable in Modules and
            /// Map components.
            /// </summary>
            void OnLoad(Map map);

            /// <summary>
            /// A Callback for when a map finishes unloading, usable in Modules and
            /// Map components.
            /// </summary>
            void OnUnload(Map map);
        }

    }

}
