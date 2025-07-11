// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <metalgearsloth@gmail.com>
// SPDX-FileCopyrightText: 2023 Checkraze <71046427+Cheackraze@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 VMSolidus <evilexecutive@gmail.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Shared.Cargo;
using Content.Shared.Cargo.Components;
using JetBrains.Annotations;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;

namespace Content.Client.Cargo.Systems;

public sealed partial class CargoSystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    private static readonly Animation CargoTelepadBeamAnimation = new()
    {
        Length = TimeSpan.FromSeconds(0.5),
        AnimationTracks =
        {
            new AnimationTrackSpriteFlick
            {
                LayerKey = CargoTelepadLayers.Beam,
                KeyFrames =
                {
                    new AnimationTrackSpriteFlick.KeyFrame(new RSI.StateId("beam"), 0f)
                }
            }
        }
    };

    private static readonly Animation CargoTelepadIdleAnimation = new()
    {
        Length = TimeSpan.FromSeconds(0.8),
        AnimationTracks =
        {
            new AnimationTrackSpriteFlick
            {
                LayerKey = CargoTelepadLayers.Beam,
                KeyFrames =
                {
                    new AnimationTrackSpriteFlick.KeyFrame(new RSI.StateId("idle"), 0f)
                }
            }
        }
    };

    private const string TelepadBeamKey = "cargo-telepad-beam";
    private const string TelepadIdleKey = "cargo-telepad-idle";

    private void InitializeCargoTelepad()
    {
        SubscribeLocalEvent<CargoTelepadComponent, AppearanceChangeEvent>(OnCargoAppChange);
        SubscribeLocalEvent<CargoTelepadComponent, AnimationCompletedEvent>(OnCargoAnimComplete);
    }

    private void OnCargoAppChange(EntityUid uid, CargoTelepadComponent component, ref AppearanceChangeEvent args)
    {
        OnChangeData(uid, args.Sprite);
    }

    private void OnCargoAnimComplete(EntityUid uid, CargoTelepadComponent component, AnimationCompletedEvent args)
    {
        OnChangeData(uid);
    }

    private void OnChangeData(EntityUid uid, SpriteComponent? sprite = null)
    {
        if (!Resolve(uid, ref sprite)
            || !EntityManager.TryGetComponent(uid, out AnimationPlayerComponent? animation))
            return;

        var entity = new Entity<AnimationPlayerComponent>(uid, animation);
        _appearance.TryGetData<CargoTelepadState?>(uid, CargoTelepadVisuals.State, out var state);

        switch (state)
        {
            case CargoTelepadState.Teleporting:
                if (_player.HasRunningAnimation(uid, animation, TelepadBeamKey))
                    return;
                _player.Stop(entity, animation, TelepadIdleKey);
                _player.Play(entity, CargoTelepadBeamAnimation, TelepadBeamKey);
                break;
            case CargoTelepadState.Unpowered:
                sprite.LayerSetVisible(CargoTelepadLayers.Beam, false);
                _player.Stop(uid, animation, TelepadBeamKey);
                _player.Stop(uid, animation, TelepadIdleKey);
                break;
            default:
                sprite.LayerSetVisible(CargoTelepadLayers.Beam, true);

                if (_player.HasRunningAnimation(uid, animation, TelepadIdleKey) ||
                    _player.HasRunningAnimation(uid, animation, TelepadBeamKey))
                    return;

                _player.Play(entity, CargoTelepadIdleAnimation, TelepadIdleKey);
                break;
        }
    }

    [UsedImplicitly]
    private enum CargoTelepadLayers : byte
    {
        Base = 0,
        Beam = 1,
    }
}
