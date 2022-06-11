using System;
using System.IO;
using System.Linq;
using Eggshell.IO;
using Eggshell.Resources;

// Internal Scene Management
using Internal = UnityEngine.SceneManagement.SceneManager;
using Scene = UnityEngine.SceneManagement.Scene;

// Roslyn Analyzers
#pragma warning disable IDE1006 

namespace Eggshell.Unity
{
    [Archive(Extension = "ubundle")]
    public class Bundle : Map.File
    {
        public Library ClassInfo { get; }
        public Scene Scene { get; }

        public Bundle()
        {
            ClassInfo = Library.Register(this);
        }

        public void Load(Stream stream)
        {
            var bundle = UnityEngine.AssetBundle.LoadFromStream(stream);
        }

        public void Unload()
        {
        }
    }

    [Group("Maps")]
    public class Map : IAsset
    {
        public static Archive[] Compatible { get; } = Library.Database.With<Archive>(e => e.Inherits<File>()).ToArray();

        public Library ClassInfo { get; }
        public Components<Map> Components { get; }

        private File Bundle { get; set; }

        public Map()
        {
            ClassInfo = Library.Register(this);
            Assert.IsNull(ClassInfo);

            Components = new(this);
        }

        [Group("Maps")]
        public interface File : IObject
        {
            Scene Scene { get; }

            void Load(Stream stream);
            void Unload();
        }

        // IAsset - Resource
        // --------------------------------------------------------------------------------------- //

        public Resource Resource { get; set; }

        public bool Setup(string extension)
        {
            // Get the correct binder using reflection
            Bundle = Array.Find(Compatible, e => e.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase))?
                            .Attached.Create<File>();

            return Bundle != null;
        }

        void IAsset.Load(Stream stream)
        {
            // Tell Provider to load Map
            Bundle.Load(stream);
            OnLoad();
        }

        private void OnLoad()
        {
            // Invoke Events
            foreach (var item in Components)
            {
                (item as Callbacks)?.OnLoad(this);
            }

            foreach (var item in Module.All)
            {
                (item as Callbacks)?.OnLoad(this);
            }
        }

        void IAsset.Unload()
        {
            // Tell Provider to unload Map
            Bundle.Unload();
            OnUnload();
        }

        private void OnUnload()
        {
            // Invoke Events
            foreach (var item in Components)
            {
                (item as Callbacks)?.OnUnload(this);
            }

            foreach (var item in Module.All)
            {
                (item as Callbacks)?.OnUnload(this);
            }
        }

        public IAsset Clone()
        {
            // Single Instance Asset
            return this;
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