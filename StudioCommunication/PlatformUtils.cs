﻿using System;
using System.IO;

namespace StudioCommunication;

public static class PlatformUtils {
    private static bool? runningOnWine;

    public static bool Wine {
        get {
            runningOnWine ??= File.Exists("/proc/self/exe") && Environment.OSVersion.Platform.HasFlag(PlatformID.Win32NT);
            return runningOnWine.Value;
        }
    }

    public static bool NonWindows => !Environment.OSVersion.Platform.HasFlag(PlatformID.Win32NT);
}