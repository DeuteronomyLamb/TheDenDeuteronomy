// SPDX-FileCopyrightText: 2024 beck-thompson <107373427+beck-thompson@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 sleepyyapril <flyingkarii@gmail.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Shared.Inventory;

namespace Content.Shared.Medical;

public abstract class BeforeDefibrillatorZapsEvent : CancellableEntityEventArgs, IInventoryRelayEvent
{
    public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;
    public EntityUid EntityUsingDefib;
    public readonly EntityUid Defib;
    public EntityUid DefibTarget;

    public BeforeDefibrillatorZapsEvent(EntityUid entityUsingDefib, EntityUid defib, EntityUid defibTarget)
    {
        EntityUsingDefib = entityUsingDefib;
        Defib = defib;
        DefibTarget = defibTarget;
    }
}

/// <summary>
///     This event is raised on the user using the defibrillator before is actually zaps someone.
///     The event is triggered on the user and all their clothing.
/// </summary>
public sealed class SelfBeforeDefibrillatorZapsEvent : BeforeDefibrillatorZapsEvent
{
    public SelfBeforeDefibrillatorZapsEvent(EntityUid entityUsingDefib, EntityUid defib, EntityUid defibtarget) : base(entityUsingDefib, defib, defibtarget) { }
}

/// <summary>
///     This event is raised on the target before it gets zapped with the defibrillator.
///     The event is triggered on the target itself and all its clothing.
/// </summary>
public sealed class TargetBeforeDefibrillatorZapsEvent : BeforeDefibrillatorZapsEvent
{
    public TargetBeforeDefibrillatorZapsEvent(EntityUid entityUsingDefib, EntityUid defib, EntityUid defibtarget) : base(entityUsingDefib, defib, defibtarget) { }
}
