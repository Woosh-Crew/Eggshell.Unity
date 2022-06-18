using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eggshell.Unity
{
    [Archive(Extension = "umap")]
    public class UnityMapBundle : Map.Binder
    {
        public Library ClassInfo { get; }
        public Scene Scene { get; set; }

        public UnityMapBundle()
        {
            ClassInfo = Library.Register(this);
        }

        private AssetBundle _bundle;

        public void Load(Stream stream, Action finished)
        {
            AssetBundle.LoadFromStreamAsync(stream).completed += bundleLoad =>
            {
                _bundle = ((AssetBundleCreateRequest)bundleLoad).assetBundle;
                var scenePath = _bundle.GetAllScenePaths()[0];

                SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive).completed += _ =>
                {
                    Scene = SceneManager.GetSceneByPath(scenePath);
                    finished?.Invoke();
                };
            };
        }

        public void Unload(Action finished)
        {
            Scene.Unload().completed += _ =>
            {
                _bundle.UnloadAsync(true).completed += _ => finished?.Invoke();
            };
        }
    }
}
