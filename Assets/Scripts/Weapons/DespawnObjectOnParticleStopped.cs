using UnityEngine;

public class DespawnObjectOnParticleStopped : MonoBehaviour
{
    [SerializeField] ThrownProjectileMovement projectileMovement;
    public void OnParticleSystemStopped()
    {
        projectileMovement.Despawn();
    }
}