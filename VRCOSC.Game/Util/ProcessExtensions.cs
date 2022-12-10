﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Extensions.IEnumerableExtensions;
using VRCOSC.Game.Modules;

namespace VRCOSC.Game.Util;

public static class ProcessExtensions
{
    #region Keys

    public static async Task PressKeys(IEnumerable<WindowsVKey> keys, int pressTime)
    {
        var windowsVKeys = keys as WindowsVKey[] ?? keys.ToArray();
        windowsVKeys.ForEach(key => ProcessKey.HoldKey((int)key));
        await Task.Delay(pressTime);
        windowsVKeys.ForEach(key => ProcessKey.ReleaseKey((int)key));
    }

    #endregion

    #region Window

    public static void ShowMainWindow(this Process process, ShowWindowEnum showWindowEnum)
    {
        ProcessWindow.ShowMainWindow(process, showWindowEnum);
    }

    public static void SetMainWindowForeground(this Process process)
    {
        ProcessWindow.SetMainWindowForeground(process);
    }

    #endregion

    #region Volume

    public static float RetrieveProcessVolume(string processName)
    {
        return ProcessVolume.GetApplicationVolume(processName) ?? 0f;
    }

    public static bool IsProcessMuted(string processName)
    {
        return ProcessVolume.GetApplicationMute(processName) ?? false;
    }

    public static void SetProcessVolume(string processName, float percentage)
    {
        ProcessVolume.SetApplicationVolume(processName, percentage);
    }

    public static void SetProcessMuted(string processName, bool muted)
    {
        ProcessVolume.SetApplicationMute(processName, muted);
    }

    #endregion
}
