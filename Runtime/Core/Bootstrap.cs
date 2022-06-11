#define EGGSHELL

using System;
using System.Collections.Generic;
using Eggshell.Diagnostics;
using Eggshell.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Eggshell.Unity.Internal
{
    /// <summary>
    /// Bootstrap for when you are using the UnityEditor, this will only
    /// compile (the body of the class) when you are in the editor.
    /// </summary>
    public class UnityEditor : UnityStandalone
    {
#if UNITY_EDITOR

        static UnityEditor()
        {
            Terminal.Editor = Application.isEditor;
            Terminal.Log = new UnityLogger();
        }

        protected override void OnStart()
        {
            EditorApplication.quitting += Shutdown;
            EditorApplication.playModeStateChanged += OnPlaymode;
            Application.focusChanged += Focus;

            Hook();
        }

        protected override void OnShutdown()
        {
            foreach (var module in Module.All)
            {
                module.OnShutdown();
            }

            EditorApplication.quitting -= Shutdown;
            EditorApplication.playModeStateChanged -= OnPlaymode;
            Application.focusChanged -= Focus;

            Unhook();
        }

        private void OnPlaymode(PlayModeStateChange state)
        {
            Terminal.Editor = !EditorApplication.isPlaying;

            switch (state)
            {
                // Engine - Game Specific Callbacks, Could be dispatched?
                case PlayModeStateChange.EnteredPlayMode:
                    Module.Get<Engine>().OnPlaying();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    Module.Get<Engine>().OnExiting();
                    break;
            }
        }

#endif
    }

    /// <summary>
    /// A bootstrap for when you are in a standalone unity game process.
    /// This isn't the bootstrap that is used while in the editor.
    /// </summary>
    public class UnityStandalone : Bootstrap
    {
        static UnityStandalone()
        {
            // -- Game Specific
            Pathing.Add("game", Application.dataPath);
            Pathing.Add("assets", Application.isEditor ? "exports://" : "game://");

#if UNITY_EDITOR

            // -- Editor Specific
            Pathing.Add("project", $"{Application.dataPath}/../");
            Pathing.Add("exports", "project://Exports/");
            Pathing.Add("compiled", "exports://<game>/");
            Pathing.Add("editor", EditorApplication.applicationPath);

#endif
        }

        // Bootstrap for Unity

        protected override void OnStart()
        {
            Application.quitting += Shutdown;
            Application.focusChanged += Focus;

            Hook();
        }

        protected override void OnShutdown()
        {
            foreach (var module in Module.All)
            {
                module.OnShutdown();
            }

            Application.quitting -= Shutdown;
            Application.focusChanged -= Focus;

            Unhook();
        }

        protected void Hook()
        {
            var loop = PlayerLoop.GetCurrentPlayerLoop();

            for (var i = 0; i < loop.subSystemList.Length; ++i)
            {
                // Frame Update
                if (loop.subSystemList[i].type == typeof(Update))
                {
                    loop.subSystemList[i].updateDelegate += Update;
                }
            }

            PlayerLoop.SetPlayerLoop(loop);
        }

        protected void Unhook()
        {
            var loop = PlayerLoop.GetCurrentPlayerLoop();

            for (var i = 0; i < loop.subSystemList.Length; ++i)
            {
                if (loop.subSystemList[i].type == typeof(Update))
                {
                    loop.subSystemList[i].updateDelegate -= Update;
                }
            }

            PlayerLoop.SetPlayerLoop(loop);
        }
    }

    internal class UnityLogger : Diagnostics.ILogger
    {
        public IReadOnlyCollection<Entry> All => _logs;
        private readonly List<Entry> _logs = new();

        public void Add(Entry entry)
        {
            entry.Time = DateTime.Now;
            entry.Message = entry.Message.IsEmpty("n/a");

            _logs.Add(entry);
            Report(entry);
        }

        public void Report(Entry entry)
        {
            // Report back into the editor, for proper logging in editor

            if (entry.Level.Contains("Warn"))
            {
                Debug.LogWarning(entry.Message);
                return;
            }

            if (entry.Level.Contains("Error") || entry.Level.Contains("Exception"))
            {
                Debug.LogError($"{entry.Message}\n\n{entry.Trace}\n\n");
                return;
            }

            if (!entry.Level.Contains("Info"))
            {
                entry.Message = $"[<color=yellow>{entry.Level}</color>] {entry.Message}";
            }

            Debug.Log(entry.Message);
        }

        public void Clear()
        {
            _logs.Clear();
        }
    }
}
