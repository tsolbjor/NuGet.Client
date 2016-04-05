// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Proxy.Microsoft.VisualStudio
{
    public static class ErrorHandler
    {
        public static int CallWithCOMConvention(Func<int> method, bool reportError = false)
        {
            return global::Microsoft.VisualStudio.ErrorHandler.CallWithCOMConvention(method, reportError);
        }

        public static int CallWithCOMConvention(Action method, bool reportError = false)
        {
            return global::Microsoft.VisualStudio.ErrorHandler.CallWithCOMConvention(method, reportError);
        }

        public static bool ContainsCriticalException(AggregateException ex)
        {
            return global::Microsoft.VisualStudio.ErrorHandler.ContainsCriticalException(ex);
        }

        public static int ExceptionToHResult(Exception ex, bool reportError = false)
        {
            return global::Microsoft.VisualStudio.ErrorHandler.ExceptionToHResult(ex, reportError);
        }

        public static bool Failed(int hr)
        {
            return global::Microsoft.VisualStudio.ErrorHandler.Failed(hr);
        }

        public static bool IsCriticalException(Exception ex)
        {
            return global::Microsoft.VisualStudio.ErrorHandler.IsCriticalException(ex);
        }

        public static bool IsRejectedRpcCall(int hr)
        {
            return global::Microsoft.VisualStudio.ErrorHandler.IsRejectedRpcCall(hr);
        }

        public static bool Succeeded(int hr)
        {
            return global::Microsoft.VisualStudio.ErrorHandler.Succeeded(hr);
        }

        public static int ThrowOnFailure(int hr)
        {
            return global::Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
        }

        public static int ThrowOnFailure(int hr, params int[] expectedHRFailure)
        {
            return global::Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr, expectedHRFailure);
        }
    }
}
