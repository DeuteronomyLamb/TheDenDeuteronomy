// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 DEATHB4DEFEAT <77995199+DEATHB4DEFEAT@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Froffy025 <78222136+Froffy025@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Shared.Wieldable;
using Robust.Shared.GameStates;

namespace Content.Shared.Weapons.Ranged.Components;

/// <summary>
/// Applies an accuracy bonus upon wielding.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(WieldableSystem))]
public sealed partial class GunWieldBonusComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite), DataField("minAngle"), AutoNetworkedField]
    public Angle MinAngle = Angle.FromDegrees(-43);

    /// <summary>
    /// Angle bonus applied upon being wielded.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("maxAngle"), AutoNetworkedField]
    public Angle MaxAngle = Angle.FromDegrees(-43);

    /// <summary>
    /// Recoil bonuses applied upon being wielded.
    /// Higher angle decay bonus, quicker recovery.
    /// Lower angle increase bonus (negative numbers), slower buildup.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Angle AngleDecay = Angle.FromDegrees(0);

	/// <summary>
    /// Recoil bonuses applied upon being wielded.
    /// Higher angle decay bonus, quicker recovery.
    /// Lower angle increase bonus (negative numbers), slower buildup.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Angle AngleIncrease = Angle.FromDegrees(0);

    [DataField]
    public LocId? WieldBonusExamineMessage = "gunwieldbonus-component-examine";
}
