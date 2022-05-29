using System;

namespace Eggshell.Unity
{
	/// <summary>
	/// An ILoadable is a loadable item that needs a UI representing it (Such as loading a map,
	/// connecting to a networked game, downloading an item while loading a map from the workshop).
	/// </summary>
	public interface ILoadable
	{
		/// <summary>
		/// The progress from 0 to 1 on how far done this load request is.
		/// </summary>
		float Progress { get; }

		/// <summary>
		/// The text that should appear in the UI for when this request  is loading.
		/// </summary>
		string Text { get; }

		/// <summary>
		/// Load this item through the loader. Which means it should have a UI representing
		/// its current loading state.
		/// </summary>
		void Load( Action loaded );

		/// <summary>
		/// Allows the injection of instructions before this instruction happens. Useful for preloading
		/// files, downloading maps, etc before actually loading the map
		/// </summary>
		ILoadable[] Inject() { return null; }
	}
}
