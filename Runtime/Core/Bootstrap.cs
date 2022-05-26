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
		// Logger for Unity

		private class Logger : ILogger
		{
			public IReadOnlyCollection<Entry> All => _logs;
			private readonly List<Entry> _logs = new();

			public void Add( Entry entry )
			{
				entry.Time = DateTime.Now;
				entry.Message = entry.Message.IsEmpty( "n/a" );

				Debug.Log( entry.Message );
				_logs.Add( entry );
			}

			public void Clear()
			{
				_logs.Clear();
			}
		}

		// Bootstrap for Unity

		static Unity()
		{
			Terminal.Log = new Logger();
		}

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
