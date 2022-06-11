using System;
using System.Runtime.CompilerServices;
using Eggshell.Unity.Tasks;
using UnityEngine;

namespace Eggshell.Unity
{
    public static class UniAwaiters
    {
        public static AsyncOperationAwaiter GetAwaiter(this AsyncOperation operation)
        {
            return new AsyncOperationAwaiter(operation);
        }

        public static AssetbundleAwaiter GetAwaiter(this AssetBundleCreateRequest operation)
        {
            return new AssetbundleAwaiter(operation);
        }
    }
}

namespace Eggshell.Unity.Tasks
{
    // Mouth Full
    public class AsyncOperationAwaiter : INotifyCompletion
    {
        public AsyncOperation Operation { get; }

        public AsyncOperationAwaiter(AsyncOperation operation)
        {
            Operation = operation;
        }

        public bool IsCompleted => Operation.isDone;

        public void GetResult() { }

        public void OnCompleted(Action continuation)
        {
            // What we should do when its completed.
            Operation.completed += e => continuation?.Invoke();
        }
    }

    public class AssetbundleAwaiter : INotifyCompletion
    {
        public AssetBundleCreateRequest Operation { get; }

        public AssetbundleAwaiter(AssetBundleCreateRequest operation)
        {
            Operation = operation;
        }

        public bool IsCompleted => Operation.isDone;
        public AssetBundle GetResult()
        {
            return Operation.assetBundle;
        }

        public void OnCompleted(Action continuation)
        {
            Operation.completed += e => continuation?.Invoke();
        }
    }
}