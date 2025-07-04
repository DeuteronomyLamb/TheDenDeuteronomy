// SPDX-FileCopyrightText: 2025 Solaris <60526456+SolarisBirb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Shared.Access;
using Content.Shared.Turrets;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.TurretController;

/// <summary>
/// Attached to entities that can set data on linked turret-based entities
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedDeployableTurretControllerSystem))]
public sealed partial class DeployableTurretControllerComponent : Component
{
    /// <summary>
    /// The states of the turrets linked to this entity, indexed by their device address.
    /// </summary>
    [ViewVariables]
    public Dictionary<string, DeployableTurretState> LinkedTurrets = new();

    /// <summary>
    /// The current armament state of the linked turrets.
    /// [-1: Inactive, 0: weapon mode A, 1: weapon mode B, etc]
    /// </summary>
    [DataField, AutoNetworkedField]
    public int ArmamentState = -1;

    /// <summary>
    /// Access levels that are known to the entity.
    /// </summary>
    [DataField]
    public HashSet<ProtoId<AccessLevelPrototype>> AccessLevels = new();

    /// <summary>
    ///Access groups that are known to the entity.
    /// </summary>
    [DataField]
    public HashSet<ProtoId<AccessGroupPrototype>> AccessGroups = new();

    /// <summary>
    /// Sound to play when denied access.
    /// </summary>
    [DataField]
    public SoundSpecifier AccessDeniedSound = new SoundPathSpecifier("/Audio/Machines/custom_deny.ogg");
}

[Serializable, NetSerializable]
public sealed class DeployableTurretControllerBoundInterfaceMessage : BoundUserInterfaceMessage
{
    public List<(string, string)> TurretStates;

    public DeployableTurretControllerBoundInterfaceMessage(List<(string, string)> turretStates)
    {
        TurretStates = turretStates;
    }
}

[Serializable, NetSerializable]
public sealed class DeployableTurretControllerBoundInterfaceState : BoundUserInterfaceState
{
    public List<(string, string)> TurretStates;

    public DeployableTurretControllerBoundInterfaceState(List<(string, string)> turretStates)
    {
        TurretStates = turretStates;
    }
}

[Serializable, NetSerializable]
public sealed class DeployableTurretArmamentSettingChangedMessage : BoundUserInterfaceMessage
{
    public int ArmamentState;

    public DeployableTurretArmamentSettingChangedMessage(int armamentState)
    {
        ArmamentState = armamentState;
    }
}

[Serializable, NetSerializable]
public sealed class DeployableTurretExemptAccessLevelChangedMessage : BoundUserInterfaceMessage
{
    public HashSet<ProtoId<AccessLevelPrototype>> AccessLevels;
    public bool Enabled;

    public DeployableTurretExemptAccessLevelChangedMessage(HashSet<ProtoId<AccessLevelPrototype>> accessLevels, bool enabled)
    {
        AccessLevels = accessLevels;
        Enabled = enabled;
    }
}

[Serializable, NetSerializable]
public enum TurretControllerVisuals : byte
{
    ControlPanel,
}


[Serializable, NetSerializable]
public enum DeployableTurretControllerUiKey : byte
{
    Key,
}
