// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace NuGet.VisualStudio.Proxy
{
    public static class Utility
    {
        /*        public static Task<T> AwaitTaskCast()
                {
                    ConfiguredProject configuredProject = (ConfiguredProject)await Utility.TaskCast<ProxyConfiguredProject>(
                        () => unconfiguredProject.GetSuggestedConfiguredProjectAsync());

                }*/


        // TODO: refactor these to make type-agnostic
        public static Task<T> TaskCast<T>(Func<Task<global::Microsoft.VisualStudio.ProjectSystem.ConfiguredProject>> taskFunction) where T : VsProxy, new()
        {
            var tcs = new TaskCompletionSource<T>();
            taskFunction().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    tcs.TrySetException(t.Exception.InnerException);
                }
                else if (t.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    T newTask = new T();
                    newTask._instance = t.Result;
                    tcs.TrySetResult(newTask);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }

        public static Task<T> TaskCast<T>(Func<Task<global::Microsoft.Build.Evaluation.Project>> taskFunction) where T : VsProxy, new()
        {
            var tcs = new TaskCompletionSource<T>();
            taskFunction().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    tcs.TrySetException(t.Exception.InnerException);
                }
                else if (t.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    T newTask = new T();
                    newTask._instance = t.Result;
                    tcs.TrySetResult(newTask);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }
    }
}
