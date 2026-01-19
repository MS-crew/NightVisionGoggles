using System.Collections.Generic;

using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;

using static NightVisionGoggles.NightVisionGoggles;

using Light = Exiled.API.Features.Toys.Light;
using MapEvent = Exiled.Events.Handlers.Map;
using PlayerEvent = Exiled.Events.Handlers.Player;
using ServerEvent = Exiled.Events.Handlers.Server;

namespace NightVisionGoggles
{
    public class EventHandlers
    {
        private readonly HashSet<ushort> dirtyPickupSerials = [];

        public HashSet<Player> DirtyPlayers { get; set; } = [];

        public void Subscribe()
        {
            ServerEvent.WaitingForPlayers += OnWaitingforPlayers;

            PlayerEvent.Verified += OnVerified;
            PlayerEvent.ChangingRole += OnChangingRole;
            PlayerEvent.ChangingSpectatedPlayer += OnChangingSpectatedPlayer;

            MapEvent.PickupAdded += OnPickupAdded;
            MapEvent.PickupDestroyed += OnPickupDestroyed;
        }

        public void Unsubscribe()
        {
            ServerEvent.WaitingForPlayers -= OnWaitingforPlayers;

            PlayerEvent.Verified -= OnVerified;
            PlayerEvent.ChangingRole -= OnChangingRole;
            PlayerEvent.ChangingSpectatedPlayer -= OnChangingSpectatedPlayer;

            MapEvent.PickupAdded -= OnPickupAdded;
            MapEvent.PickupDestroyed -= OnPickupDestroyed;
        }

        private void OnWaitingforPlayers()
        {
            DirtyPlayers.Clear();
            dirtyPickupSerials.Clear();
        }

        private void OnVerified(VerifiedEventArgs ev)
        {
            foreach (Light light in NVG.Lights.Values)
            {
                ev.Player.HideNetworkIdentity(light.Base.netIdentity);
            }
        }

        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (NVG.Lights.ContainsKey(ev.Player))
                WearOffNightVision(ev.Player.ReferenceHub);

            if (DirtyPlayers.Contains(ev.Player))
            {
                foreach (Light light in NVG.Lights.Values)
                {
                    ev.Player.HideNetworkIdentity(light.Base.netIdentity);
                }

                DirtyPlayers.Remove(ev.Player);
            }
        }

        private void OnChangingSpectatedPlayer(ChangingSpectatedPlayerEventArgs ev)
        {
            if (ev.OldTarget != null && NVG.Lights.ContainsKey(ev.OldTarget))
            {
                ev.Player.HideNetworkIdentity(NVG.Lights[ev.OldTarget]?.Base?.netIdentity);
                DirtyPlayers.Remove(ev.Player);
            }  

            if (ev.NewTarget != null && NVG.Lights.ContainsKey(ev.NewTarget))
            {
                ev.Player.ShowHidedNetworkIdentity(NVG.Lights[ev.NewTarget]?.Base?.netIdentity);
                DirtyPlayers.Add(ev.Player);
            }
        }

        private void OnPickupAdded(PickupAddedEventArgs ev)
        {
            if (!NVG.Check(ev.Pickup))
                return;

            if (dirtyPickupSerials.Contains(ev.Pickup.Serial))
            {
                ev.Pickup.Destroy();
                return;
            }

            dirtyPickupSerials.Add(ev.Pickup.Serial);
        }

        private void OnPickupDestroyed(PickupDestroyedEventArgs ev)
        {
            if (dirtyPickupSerials.Contains(ev.Pickup.Serial))
                dirtyPickupSerials.Remove(ev.Pickup.Serial);
        }
    }
}
