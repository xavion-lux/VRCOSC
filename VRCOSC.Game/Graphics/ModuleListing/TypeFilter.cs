﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using VRCOSC.Game.Graphics.UI;
using VRCOSC.Game.Modules;

namespace VRCOSC.Game.Graphics.ModuleListing;

public sealed class TypeFilter : Container
{
    public TypeFilter()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        RelativeSizeAxes = Axes.Both;
    }

    [BackgroundDependencyLoader]
    private void load(VRCOSCGame game)
    {
        VRCOSCDropdown<Group> dropdown;
        Child = dropdown = new VRCOSCDropdown<Group>
        {
            Anchor = Anchor.TopCentre,
            Origin = Anchor.TopCentre,
            RelativeSizeAxes = Axes.X,
            Items = Enum.GetValues<Group>(),
            Current = { Value = Group.All }
        };

        dropdown.Current.BindValueChanged(group => game.TypeFilter.Value = groupToType(group.NewValue), true);
    }

    private enum Group
    {
        All = -1,
        General = 0,
        Health = 1,
        Integrations = 2,
        Accessibility = 3
    }

    private static ModuleType? groupToType(Group group) => group == Group.All ? null : (ModuleType)(int)group;
}
