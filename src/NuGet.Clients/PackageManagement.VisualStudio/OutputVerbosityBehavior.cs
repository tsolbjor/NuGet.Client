// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;

namespace NuGet.PackageManagement.VisualStudio
{
    public class OutputVerbosityBehavior
    {
        private const string packageDiagLogsSection = "outputVerbosity";
        private const string packageDiagLogsKey = "enabled";

        private readonly Configuration.ISettings _settings;

        public OutputVerbosityBehavior(Configuration.ISettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _settings = settings;
        }

        public bool IsDiagEnabled
        {
            get
            {
                string settingsValue = _settings.GetValue(packageDiagLogsSection, packageDiagLogsKey) ?? string.Empty;

                return IsSet(settingsValue, false); // Don't show debug logs by default
            }

            set
            {
                _settings.SetValue(packageDiagLogsSection, packageDiagLogsKey, value.ToString(CultureInfo.InvariantCulture));
            }
        }

        private static bool IsSet(string value, bool defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            value = value.Trim();

            bool boolResult;
            int intResult;

            var result = ((Boolean.TryParse(value, out boolResult) && boolResult) ||
                          (Int32.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out intResult) && (intResult == 1)));

            return result;
        }
    }
}
