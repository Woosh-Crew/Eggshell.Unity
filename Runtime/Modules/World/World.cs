namespace Eggshell.Unity
{
    /// <summary>
    /// The world is responsible for keeping track of all loaded levels as
    /// well as providing network channels for entities. World > Map > Entities
    /// </summary>
    public class World : Module, Game.Callbacks
    {
        public void OnPlaying() { }
        public void OnExiting() { }
    }
}