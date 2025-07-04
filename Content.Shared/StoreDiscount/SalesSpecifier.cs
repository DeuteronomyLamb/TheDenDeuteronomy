// SPDX-FileCopyrightText: 2024 VMSolidus <evilexecutive@gmail.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

namespace Content.Shared.StoreDiscount;

[DataDefinition]
public sealed partial class SalesSpecifier
{
    [DataField]
    public bool Enabled { get; private set; }

    [DataField]
    public float MinMultiplier { get; private set; }

    [DataField]
    public float MaxMultiplier { get; private set; }

    [DataField]
    public int MinItems { get; private set;  }

    [DataField]
    public int MaxItems { get; private set; }

    [DataField]
    public string SalesCategory { get; private set; } = string.Empty;

    public SalesSpecifier()
    {
    }

    public SalesSpecifier(bool enabled, float minMultiplier, float maxMultiplier, int minItems, int maxItems,
        string salesCategory)
    {
        Enabled = enabled;
        MinMultiplier = minMultiplier;
        MaxMultiplier = maxMultiplier;
        MinItems = minItems;
        MaxItems = maxItems;
        SalesCategory = salesCategory;
    }
}
