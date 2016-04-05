// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.CompilerServices;
using NuGet.VisualStudio.Proxy;

namespace Proxy.Microsoft.VisualStudio.Threading
{
    [InnerType(typeof(global::Microsoft.VisualStudio.Threading.JoinableTaskFactory))]
    public class JoinableTaskFactory
    {
        //public JoinableTaskFactory(JoinableTaskContext owner);
        //public JoinableTaskFactory(JoinableTaskCollection collection);
        //public JoinableTaskContext Context { get; }
        //protected TimeSpan HangDetectionTimeout { get; set; }
        //protected SynchronizationContext UnderlyingSynchronizationContext { get; }
        //public void Run(Func<Task> asyncMethod);
        //public void Run(Func<Task> asyncMethod, JoinableTaskCreationOptions creationOptions);
        //public T Run<T>(Func<Task<T>> asyncMethod);
        //public T Run<T>(Func<Task<T>> asyncMethod, JoinableTaskCreationOptions creationOptions);
        //public JoinableTask RunAsync(Func<Task> asyncMethod);
        //public JoinableTask RunAsync(Func<Task> asyncMethod, JoinableTaskCreationOptions creationOptions);
        //public JoinableTask<T> RunAsync<T>(Func<Task<T>> asyncMethod);
        //public JoinableTask<T> RunAsync<T>(Func<Task<T>> asyncMethod, JoinableTaskCreationOptions creationOptions);
        //public MainThreadAwaitable SwitchToMainThreadAsync(CancellationToken cancellationToken = default(CancellationToken));
        //protected void Add(JoinableTask joinable);
        //protected bool IsWaitingOnLongRunningTask();
        //protected virtual void WaitSynchronouslyCore(Task task);
        //protected internal virtual void OnTransitionedToMainThread(JoinableTask joinableTask, bool canceled);
        //protected internal virtual void OnTransitioningToMainThread(JoinableTask joinableTask);
        //protected internal virtual void PostToUnderlyingSynchronizationContext(SendOrPostCallback callback, object state);
        //protected internal virtual void WaitSynchronously(Task task);

        public struct MainThreadAwaitable
        {
            private global::Microsoft.VisualStudio.Threading.JoinableTaskFactory.MainThreadAwaitable _instance;

            public MainThreadAwaitable(global::Microsoft.VisualStudio.Threading.JoinableTaskFactory.MainThreadAwaitable instance)
            {
                _instance = instance;
            }

            public MainThreadAwaiter GetAwaiter()
            {
                return new MainThreadAwaiter(_instance.GetAwaiter());
            }
        }
        public struct MainThreadAwaiter : INotifyCompletion
        {
            private global::Microsoft.VisualStudio.Threading.JoinableTaskFactory.MainThreadAwaiter _instance;

            public MainThreadAwaiter (global::Microsoft.VisualStudio.Threading.JoinableTaskFactory.MainThreadAwaiter instance)
            {
                _instance = instance;
            }

            public bool IsCompleted {
                get
                {
                    return _instance.IsCompleted;
                }
            }

            public void GetResult()
            {
                _instance.GetResult();
            }

            public void OnCompleted(Action continuation)
            {
                _instance.OnCompleted(continuation);
            }
        }
    }
}
