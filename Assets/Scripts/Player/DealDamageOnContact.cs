using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Aplica daño a objetos que tienen componente Health al entrar en contacto.
/// Evita dañar al propio jugador que disparó el proyectil o la entidad que generó el daño.
/// </summary>
public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int damage = 5;
    private ulong ownerClientId;

    /// <summary>
    /// Establece el identificador del cliente que originó este daño.
    /// Se usa para evitar daño propio en proyectiles o ataques.
    /// </summary>
    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.attachedRigidbody == null) { return; }

        if (col.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            if (ownerClientId == netObj.OwnerClientId)
            {
                return;
            }
        }

        if (col.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
        }
    }
}
