using FishSlop;
using Mirror;
using UnityEngine;
public class PLAYTEST_PlayerRepairShip : NetworkBehaviour
{
    private PlayerGrab pg;
    private PlayerMovementHandler pmh;
    private PLAYTEST_RepairingObject ro;

    [SerializeField] private float maxRepairDistance = 15f;

    public bool canAttemptRepair { get; private set; }

    public override void OnStartLocalPlayer()
    {
        canAttemptRepair = false;
        pg = GetComponent<PlayerGrab>();
        pmh = GetComponent<PlayerMovementHandler>();
    }
    private void Update()
    {
        canAttemptRepair = false;
        ro = null;

        if (!isLocalPlayer || pmh == null || pg == null) return;
        if (pmh.ship == null || pg.held == null) return;

        if (pg.held.TryGetComponent(out ro)) CheckCanRepair(pmh.ship.GetComponent<ShipHealth>());
    }

    private void CheckCanRepair(ShipHealth sH)
    {
        if (sH == null || ro == null) return;

        if (sH.currentHealth < sH.maxHealth)
        {
            canAttemptRepair = true;
            InteractableUIHandler.Instance.ShowInteractableText($"[E] Repair Ship ({ro.repairAmount})");
        }
        else
        {
            canAttemptRepair = false;
        }
    }
    public void AttemptRepair()
    {
        if (!canAttemptRepair || pmh.ship == null) return;

        var shipIdentity = pmh.ship.GetComponent<NetworkIdentity>();
        if (shipIdentity == null) return;

        CmdRepair(shipIdentity);
    }

    [Command]
    private void CmdRepair(NetworkIdentity shipIdentity)
    {
        if (shipIdentity == null) return;

        if (pg.held == null || !pg.held.TryGetComponent(out PLAYTEST_RepairingObject obj)) return;

        if (!shipIdentity.TryGetComponent(out ShipHealth health)) return;
        if (Vector3.Distance(transform.position, shipIdentity.transform.position) > maxRepairDistance) return;

        if (health.currentHealth >= health.maxHealth) return;

        health.Repair(obj.repairAmount);

        pg.CmdDrop();

        NetworkServer.Destroy(pg.held.gameObject);
    }
}
