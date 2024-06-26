using NitroxClient.Communication.Packets.Processors.Abstract;
using NitroxClient.GameLogic;
using NitroxClient.GameLogic.PlayerLogic;
using NitroxClient.MonoBehaviours;
using NitroxModel.DataStructures;
using NitroxModel.Packets;
using System.Collections.Generic;
using UnityEngine;
using static ResourceTrackerDatabase;
using static UWE.FreezeTime;

namespace NitroxClient.Communication.Packets.Processors;

public class EntityDestroyedProcessor : ClientPacketProcessor<EntityDestroyed>
{
    public const DamageType DAMAGE_TYPE_RUN_ORIGINAL = (DamageType)100;
    public TechType typeToScan;
    private readonly Entities entities;
    private readonly ResourceTrackerDatabase resourceTrackerDatabase;
    private readonly List<ResourceTrackerDatabase.ResourceInfo> resourceNodes;

    public EntityDestroyedProcessor(Entities entities, ResourceTrackerDatabase resourceTrackerDatabase, OnResourceRemoved onResourceRemoved)
    {
        this.entities = entities;
        this.resourceTrackerDatabase = resourceTrackerDatabase;
        ResourceTrackerDatabase.onResourceRemoved += onResourceRemoved;
    }

    public override void Process(EntityDestroyed packet)
    {
        entities.RemoveEntity(packet.Id);
        if (!NitroxEntity.TryGetObjectFrom(packet.Id, out GameObject gameObject))
        {
            entities.MarkForDeletion(packet.Id);
            Log.Warn($"[{nameof(EntityDestroyedProcessor)}] Could not find entity with id: {packet.Id} to destroy.");
            return;
        }

        using (PacketSuppressor<EntityDestroyed>.Suppress())
        {
            // This type of check could get out of control if there are many types with custom destroy logic.  If we get a few more, move to separate processors.
            if (gameObject.TryGetComponent(out Vehicle vehicle))
            {
                DestroyVehicle(vehicle);
            }
            else if (gameObject.TryGetComponent(out SubRoot subRoot))
            {
                DestroySubroot(subRoot);
            }
            else
            {
                DefaultDestroyAction(gameObject);
            }
        }
    }

    private void OnResourceRemoved(ResourceTrackerDatabase.ResourceInfo Info)
    {
        if(this.typeToScan == Info.techType)

        {
            this.resourceNodes.Remove(Info);
        }
    }

    private void DestroySubroot(SubRoot subRoot)
    {
        if (subRoot.live.health > 0f)
        {
            // oldHPPercent must be in the interval [0; 0.25[ because else, SubRoot.OnTakeDamage will end up in the wrong else condition
            subRoot.oldHPPercent = 0f;
            subRoot.live.health = 0f;
            subRoot.live.Kill();
        }

        // We use a specific DamageType so that the Prefix on this method will accept this call
        subRoot.OnTakeDamage(new() { type = DAMAGE_TYPE_RUN_ORIGINAL });
    }


    private void DestroyVehicle(Vehicle vehicle)
    {
        if (vehicle.GetPilotingMode()) //Check Local Object Have Player inside
        {
            vehicle.OnPilotModeEnd();

            if (!Player.main.ToNormalMode(true))
            {
                Player.main.ToNormalMode(false);
                Player.main.transform.parent = null;
            }
        }

        foreach (RemotePlayerIdentifier identifier in vehicle.GetComponentsInChildren<RemotePlayerIdentifier>(true))
        {
            identifier.RemotePlayer.ResetStates();
        }

        if (vehicle.gameObject)
        {
            if (vehicle.destructionEffect)
            {
                GameObject gameObject = Object.Instantiate(vehicle.destructionEffect);
                gameObject.transform.position = vehicle.transform.position;
                gameObject.transform.rotation = vehicle.transform.rotation;
            }

            Object.Destroy(vehicle.gameObject);
        }
    }

    private void DefaultDestroyAction(GameObject gameObject)
    {
        Object.Destroy(gameObject);
    }
}
