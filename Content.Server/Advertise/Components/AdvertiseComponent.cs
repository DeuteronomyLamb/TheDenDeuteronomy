// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2024 VMSolidus <evilexecutive@gmail.com>
// SPDX-FileCopyrightText: 2024 Wrexbe (Josh) <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Shared.Advertise;
using Robust.Shared.Prototypes;

namespace Content.Server.Advertise.Components;

/// <summary>
/// Makes this entity periodically advertise by speaking a randomly selected
/// message from a specified MessagePack into local chat.
/// </summary>
[RegisterComponent]
public sealed partial class AdvertiseComponent : Component
{
    /// <summary>
    /// Minimum time in seconds to wait before saying a new ad, in seconds. Has to be larger than or equal to 1.
    /// </summary>
    [DataField]
    public int MinimumWait { get; set; } = 8 * 60;

    /// <summary>
    /// Maximum time in seconds to wait before saying a new ad, in seconds. Has to be larger than or equal
    /// to <see cref="MinimumWait"/>
    /// </summary>
    [DataField]
    public int MaximumWait { get; set; } = 10 * 60;

    /// <summary>
    /// If true, the delay before the first advertisement (at MapInit) will ignore <see cref="MinimumWait"/>
    /// and instead be rolled between 0 and <see cref="MaximumWait"/>. This only applies to the initial delay;
    /// <see cref="MinimumWait"/> will be respected after that.
    /// </summary>
    [DataField]
    public bool Prewarm = true;

    /// <summary>
    /// The identifier for the advertisements pack prototype.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<MessagePackPrototype> Pack { get; private set; }

    /// <summary>
    /// The next time an advertisement will be said.
    /// </summary>
    [DataField]
    public TimeSpan NextAdvertisementTime { get; set; } = TimeSpan.Zero;

}
