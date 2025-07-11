// SPDX-FileCopyrightText: 2025 Lyndomen <49795619+Lyndomen@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Shared._CD.Records;

namespace Content.Server._CD.Records;

/// <summary>
/// The component on the station that stores records after the round starts.
/// </summary>
[RegisterComponent]
[Access(typeof(CharacterRecordsSystem))]
public sealed partial class CharacterRecordsComponent : Component
{
    [ViewVariables(VVAccess.ReadOnly)]
    public Dictionary<uint, FullCharacterRecords> Records = new();

    [ViewVariables(VVAccess.ReadOnly)]
    private uint _nextKey = 1;

    /// <summary>
    /// Creates a key has never been used previously
    /// </summary>
    public uint CreateNewKey()
    {
        return _nextKey++;
    }
}

public sealed record CharacterRecordKey
{
    public EntityUid Station { get; init; }
    public uint Index { get; init; }
}
