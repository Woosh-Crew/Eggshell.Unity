using System;
using System.IO;
using System.Linq;
using Eggshell.Resources;

// Internal Scene Management
using Internal = UnityEngine.SceneManagement.SceneManager;
using Scene = UnityEngine.SceneManagement.Scene;

// Roslyn Analyzers
#pragma warning disable IDE1006 

namespace Eggshell.Unity
{
    public class Bundle : Map.File
    {
        public override void Load(Stream stream)
        {
        }

        public override void Unload()
        {
        }
    }

    [Group("Maps"), Path("maps", "assets://Maps/")]
    public class Map : IAsset
    {
        public static Archive[] Compatible { get; } = Library.Database.With<Archive>(e => e.Inherits<File>()).ToArray();

        public Library ClassInfo { get; }
        public Components<Map> Components { get; }

        private File Binder { get; set; }

        public Map()
        {
            ClassInfo = Library.Register(this);
            Assert.IsNull(ClassInfo);

            Components = new(this);
        }

        [Group("Maps")]
        public abstract class File : IObject
        {
            public Library ClassInfo { get; }

            public File()
            {
                ClassInfo = Library.Register(this);
            }

            public abstract void Load(Stream stream);
            public abstract void Unload();
        }

        // IAsset - Resource
        // --------------------------------------------------------------------------------------- //

        public Resource Resource { get; set; }

        public bool Setup(string extension)
        {
            // Get the correct binder using reflection
            Binder = Array.Find(Compatible, e => e.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase))?
                            .Attached.Create<File>();

            return Binder != null;
        }

        void IAsset.Load(Stream stream)
        {
            // Tell Provider to load Map
            Binder.Load(stream);
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
            Binder.Unload();
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