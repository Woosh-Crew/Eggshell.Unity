using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Eggshell.Unity
{
    public static class TimeSpanAwaiters
    {
        public static TaskAwaiter GetAwaiter(this TimeSpan operation)
        {
            return Task.Delay(operation).GetAwaiter();
        }
    }
}