// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System;
using System.Collections.Generic;
using Markdig.Helpers;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Platform;
using osu.Framework.Threading;
using VRCOSC.Game.Util;

namespace VRCOSC.Game.Modules;

public class ModuleManager

{
    public Dictionary<ModuleType, OrderedList<Module>> Modules { get; } = new();

    private readonly BindableBool Running = new();

    public ModuleManager(Storage storage)
    {
        IEnumerable<Module> modules = ReflectiveEnumerator.GetEnumerableOfType<Module>(storage);
        modules.ForEach(module =>
        {
            module.DataManager.LoadData();
            addModule(module);
        });
        sortModules();
    }

    private void addModule(Module? module)
    {
        if (module == null) return;

        var list = Modules.GetValueOrDefault(module.Type, new OrderedList<Module>());
        list.Add(module);
        Modules.TryAdd(module.Type, list);
    }

    private void sortModules()
    {
        Dictionary<ModuleType, OrderedList<Module>> localList = new(Modules);
        Modules.Clear();

        foreach (ModuleType moduleType in Enum.GetValues(typeof(ModuleType)))
        {
            if (!localList.TryGetValue(moduleType, out var moduleList)) return;

            Modules.Add(moduleType, moduleList);
        }
    }

    public void Start(Scheduler scheduler)
    {
        Running.Value = true;
        Modules.Values.ForEach(modules =>
        {
            modules.ForEach(module =>
            {
                if (!module.DataManager.Enabled) return;

                module.Start();

                if (double.IsPositiveInfinity(module.DeltaUpdate)) return;

                scheduler.Add(module.Update);
                scheduler.AddDelayed(module.Update, module.DeltaUpdate, true);
            });
        });
    }

    public void Stop(Scheduler scheduler)
    {
        Running.Value = false;
        scheduler.CancelDelayedTasks();
        Modules.Values.ForEach(modules =>
        {
            modules.ForEach(module =>
            {
                if (module.DataManager.Enabled) module.Stop();
            });
        });
    }
}
