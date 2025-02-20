using Unity.Netcode;

public class CarUpgrader : NetworkBehaviour
{
    public void UpgradeHealth()
    {
        if (!IsHost)
            return;

        CarManager.Instance.UpgradeRpc(ECarStatName.HEALTH);
    }
    public void UpgradeDamage()
    {
        if (!IsHost)
            return;

        CarManager.Instance.UpgradeRpc(ECarStatName.DAMAGE);
    }
    public void UpgradeSpeed()
    {
        if (!IsHost)
            return;

        CarManager.Instance.UpgradeRpc(ECarStatName.SPEED);
    }
    public void UpgradeCapacity()
    {
        if (!IsHost)
            return;

        CarManager.Instance.UpgradeRpc(ECarStatName.CAPACITY);
    }
}