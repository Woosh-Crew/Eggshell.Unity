#define EGGSHELL

using System;
using System.Collections.Generic;
using Eggshell.Debugging.Logging;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using ILogger = Eggshell.Debugging.Logging.ILogger;

namespace Eggshell.Unity
{
	public class Unity : Bootstrap
	{
		static Unity()
		{
			Terminal.Log = new Logger();
		}

		// Logger for Unity

		private class Logger : ILogger
		{
			public IReadOnlyCollection<Entry> All => _logs;
			private readonly List<Entry> _logs = new();

			public Logger()
			{
				Application.logMessageReceived += Collected;
			}

			~Logger()
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

		// Bootstrap for Unity

		protected override void OnStart()
		{
			// Add loop and Shutdown

			Application.quitting += Shutdown;

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

		protected override void OnShutdown()
		{
			// Remove loop and Shutdown

			base.OnShutdown();

			Application.quitting -= Shutdown;

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
}
