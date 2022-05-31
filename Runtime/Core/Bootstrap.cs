#define EGGSHELL

using System;
using System.Collections.Generic;
using Eggshell.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Eggshell.Unity.Internal
{
	internal class UnityEditor : UnityStandalone
	{
		#if UNITY_EDITOR

		protected override void OnStart()
		{
			EditorApplication.quitting += Shutdown;
			EditorApplication.playModeStateChanged += OnPlaymode;

			Hook();
		}

		protected override void OnShutdown()
		{
			foreach ( var module in Module.All )
			{
				module.OnShutdown();
			}

			EditorApplication.quitting -= Shutdown;
			EditorApplication.playModeStateChanged -= OnPlaymode;

			Unhook();
		}

		private void OnPlaymode( PlayModeStateChange state )
		{
			Terminal.Editor = !EditorApplication.isPlaying;

			switch ( state )
			{
				// Engine - Game Specific Callbacks, Could be dispatched?
				case PlayModeStateChange.EnteredPlayMode :
					Module.Get<Engine>().OnPlaying();
					break;
				case PlayModeStateChange.ExitingPlayMode :
					Module.Get<Engine>().OnExiting();
					break;
			}
		}

		#endif
	}

	internal class UnityStandalone : Bootstrap
	{
		static UnityStandalone()
		{
			Terminal.Editor = Application.isEditor;
			Terminal.Log = new UnityLogger();
		}

		// Bootstrap for Unity

		protected override void OnStart()
		{
			Application.quitting += Shutdown;
			Hook();
		}

		protected override void OnShutdown()
		{
			foreach ( var module in Module.All )
			{
				module.OnShutdown();
			}

			Application.quitting -= Shutdown;
			Unhook();
		}

		protected void Hook()
		{
			var loop = PlayerLoop.GetCurrentPlayerLoop();

			for ( var i = 0; i < loop.subSystemList.Length; ++i )
			{
				// Frame Update
				if ( loop.subSystemList[i].type == typeof( Update ) )
				{
					loop.subSystemList[i].updateDelegate += Update;
				}
			}

			PlayerLoop.SetPlayerLoop( loop );
		}

		protected void Unhook()
		{
			var loop = PlayerLoop.GetCurrentPlayerLoop();

			for ( var i = 0; i < loop.subSystemList.Length; ++i )
			{
				if ( loop.subSystemList[i].type == typeof( Update ) )
				{
					loop.subSystemList[i].updateDelegate -= Update;
				}
			}

			PlayerLoop.SetPlayerLoop( loop );
		}
	}

	internal class UnityLogger : Diagnostics.ILogger
	{
		public IReadOnlyCollection<Entry> All => _logs;
		private readonly List<Entry> _logs = new();

		public UnityLogger()
		{
			Application.logMessageReceived += Collected;
		}

		~UnityLogger()
		{
			Application.logMessageReceived -= Collected;
		}

		private void Collected( string condition, string stacktrace, LogType type )
		{
			if ( Application.isEditor )
			{
				return;
			}

			switch ( type )
			{
				case LogType.Log :
					Terminal.Log.Info( condition, stacktrace );
					break;
				case LogType.Warning :
					Terminal.Log.Warning( condition, stacktrace );
					break;
				case LogType.Assert :
				case LogType.Error :
				case LogType.Exception :
					Terminal.Log.Error( condition, stacktrace );
					break;
				default :
					throw new ArgumentOutOfRangeException( nameof( type ), type, null );
			}
		}

		public void Add( Entry entry )
		{
			entry.Time = DateTime.Now;
			entry.Message = entry.Message.IsEmpty( "n/a" );

			_logs.Add( entry );

			if ( Application.isEditor )
			{
				Report( entry );
			}
		}

		public void Report( Entry entry )
		{
			// Report back into the editor, for proper logging in editor

			if ( entry.Level.Contains( "Warn" ) )
			{
				Debug.LogWarning( entry.Message );
				return;
			}

			if ( entry.Level.Contains( "Error" ) || entry.Level.Contains( "Exception" ) )
			{
				Debug.LogError( entry.Message );
				return;
			}

			Debug.Log( entry.Message );
		}

		public void Clear()
		{
			_logs.Clear();
		}
	}
}
