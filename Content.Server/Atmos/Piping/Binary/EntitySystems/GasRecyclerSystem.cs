// SPDX-FileCopyrightText: 2022 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Kevin Zheng <kevinz5000@gmail.com>
// SPDX-FileCopyrightText: 2023 faint <46868845+ficcialfaint@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+emogarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 VMSolidus <evilexecutive@gmail.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Binary.Components;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Construction;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Piping;
using Content.Shared.Audio;
using Content.Shared.Examine;
using JetBrains.Annotations;
using Robust.Server.GameObjects;

namespace Content.Server.Atmos.Piping.Binary.EntitySystems
{
    [UsedImplicitly]
    public sealed class GasReyclerSystem : EntitySystem
    {
        [Dependency] private readonly AppearanceSystem _appearance = default!;
        [Dependency] private readonly AtmosphereSystem _atmosphereSystem = default!;
        [Dependency] private readonly SharedAmbientSoundSystem _ambientSoundSystem = default!;
        [Dependency] private readonly NodeContainerSystem _nodeContainer = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<GasRecyclerComponent, AtmosDeviceEnabledEvent>(OnEnabled);
            SubscribeLocalEvent<GasRecyclerComponent, AtmosDeviceUpdateEvent>(OnUpdate);
            SubscribeLocalEvent<GasRecyclerComponent, AtmosDeviceDisabledEvent>(OnDisabled);
            SubscribeLocalEvent<GasRecyclerComponent, ExaminedEvent>(OnExamined);
            SubscribeLocalEvent<GasRecyclerComponent, RefreshPartsEvent>(OnRefreshParts);
            SubscribeLocalEvent<GasRecyclerComponent, UpgradeExamineEvent>(OnUpgradeExamine);
        }

        private void OnEnabled(EntityUid uid, GasRecyclerComponent comp, ref AtmosDeviceEnabledEvent args)
        {
            UpdateAppearance(uid, comp);
        }

        private void OnExamined(Entity<GasRecyclerComponent> ent, ref ExaminedEvent args)
        {
            var comp = ent.Comp;
            if (!EntityManager.GetComponent<TransformComponent>(ent).Anchored || !args.IsInDetailsRange) // Not anchored? Out of range? No status.
                return;

            if (!_nodeContainer.TryGetNode(ent.Owner, comp.InletName, out PipeNode? inlet))
                return;

            using (args.PushGroup(nameof(GasRecyclerComponent)))
            {
                if (comp.Reacting)
                {
                    args.PushMarkup(Loc.GetString("gas-recycler-reacting"));
                }
                else
                {
                    if (inlet.Air.Pressure < comp.MinPressure)
                    {
                        args.PushMarkup(Loc.GetString("gas-recycler-low-pressure"));
                    }

                    if (inlet.Air.Temperature < comp.MinTemp)
                    {
                        args.PushMarkup(Loc.GetString("gas-recycler-low-temperature"));
                    }
                }
            }
        }

        private void OnUpdate(Entity<GasRecyclerComponent> ent, ref AtmosDeviceUpdateEvent args)
        {
            var comp = ent.Comp;
            if (!_nodeContainer.TryGetNodes(ent.Owner, comp.InletName, comp.OutletName, out PipeNode? inlet, out PipeNode? outlet))
            {
                _ambientSoundSystem.SetAmbience(ent, false);
                return;
            }

            // The gas recycler is a passive device, so it permits gas flow even if nothing is being reacted.
            comp.Reacting = inlet.Air.Temperature >= comp.MinTemp && inlet.Air.Pressure >= comp.MinPressure;
            var removed = inlet.Air.RemoveVolume(PassiveTransferVol(inlet.Air, outlet.Air));
            if (comp.Reacting)
            {
                var nCO2 = removed.GetMoles(Gas.CarbonDioxide);
                removed.AdjustMoles(Gas.CarbonDioxide, -nCO2);
                removed.AdjustMoles(Gas.Oxygen, nCO2);
                var nN2O = removed.GetMoles(Gas.NitrousOxide);
                removed.AdjustMoles(Gas.NitrousOxide, -nN2O);
                removed.AdjustMoles(Gas.Nitrogen, nN2O);
            }

            _atmosphereSystem.Merge(outlet.Air, removed);
            UpdateAppearance(ent, comp);
            _ambientSoundSystem.SetAmbience(ent, true);
        }

        public float PassiveTransferVol(GasMixture inlet, GasMixture outlet)
        {
            if (inlet.Pressure < outlet.Pressure)
            {
                return 0;
            }
            float overPressConst = 300; // pressure difference (in atm) to get 200 L/sec transfer rate
            float alpha = Atmospherics.MaxTransferRate * _atmosphereSystem.PumpSpeedup() / (float)Math.Sqrt(overPressConst*Atmospherics.OneAtmosphere);
            return alpha * (float)Math.Sqrt(inlet.Pressure - outlet.Pressure);
        }

        private void OnDisabled(EntityUid uid, GasRecyclerComponent comp, ref AtmosDeviceDisabledEvent args)
        {
            comp.Reacting = false;
            UpdateAppearance(uid, comp);
        }

        private void UpdateAppearance(EntityUid uid, GasRecyclerComponent? comp = null)
        {
            if (!Resolve(uid, ref comp, false))
                return;

            _appearance.SetData(uid, PumpVisuals.Enabled, comp.Reacting);
        }

        private void OnRefreshParts(EntityUid uid, GasRecyclerComponent component, RefreshPartsEvent args)
        {
            var ratingTemp = args.PartRatings[component.MachinePartMinTemp];
            var ratingPressure = args.PartRatings[component.MachinePartMinPressure];

            component.MinTemp = component.BaseMinTemp * MathF.Pow(component.PartRatingMinTempMultiplier, ratingTemp - 1);
            component.MinPressure = component.BaseMinPressure * MathF.Pow(component.PartRatingMinPressureMultiplier, ratingPressure - 1);
        }

        private void OnUpgradeExamine(EntityUid uid, GasRecyclerComponent component, UpgradeExamineEvent args)
        {
            args.AddPercentageUpgrade("gas-recycler-upgrade-min-temp", component.MinTemp / component.BaseMinTemp);
            args.AddPercentageUpgrade("gas-recycler-upgrade-min-pressure", component.MinPressure / component.BaseMinPressure);
        }
    }
}
