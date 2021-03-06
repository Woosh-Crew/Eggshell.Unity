using System.Collections.Generic;

namespace Eggshell.Unity
{
    /// <summary>
    /// The world is responsible for keeping track of all loaded levels as
    /// well as providing network channels for entities. World > Map > Entities
    /// </summary>
    public class World : Module, Map.Callbacks
    {
        // Map Management
        // --------------------------------------------------------------------------------------- //

        private readonly List<Map> _loaded = new();

        /// <summary>
        /// A Collection of all currently loaded maps in the world. Use this
        /// for getting maps. Instead of from a straight reference to it.
        /// </summary>
        public IEnumerable<Map> Loaded => _loaded;

        void Map.Callbacks.OnLoading(Map map)
        {
            // Unload - Load if we want single or additive
        }

        void Map.Callbacks.OnLoad(Map map)
        {
            // Map Loaded, add it to world
            _loaded.Add(map);
        }

        void Map.Callbacks.OnUnload(Map map)
        {
            // Map Unloaded, remove it from world
            _loaded.Remove(map);
        }
    }
}
