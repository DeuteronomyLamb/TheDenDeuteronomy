// SPDX-FileCopyrightText: 2024 Ed <96445749+theshued@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.DeviceLinking.Systems;
using Content.Shared.DeviceLinking;
using Robust.Shared.Prototypes;

namespace Content.Server.DeviceLinking.Components;

/// <summary>
/// A system that allows you to fire GunComponent + AmmoProvider by receiving signals from DeviceLinking
/// </summary>
[RegisterComponent, Access(typeof(GunSignalControlSystem))]
public sealed partial class GunSignalControlComponent : Component
{
    [DataField]
    public ProtoId<SinkPortPrototype> TriggerPort = "Trigger";

    [DataField]
    public ProtoId<SinkPortPrototype> TogglePort = "Toggle";

    [DataField]
    public ProtoId<SinkPortPrototype> OnPort = "On";

    [DataField]
    public ProtoId<SinkPortPrototype> OffPort = "Off";
}
