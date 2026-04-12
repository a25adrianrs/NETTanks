using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int damage = 5;

    private ulong ownerClientId;

    // Este método se llama para establecer el propietario del daño, lo que permite evitar que un jugador se dañe a sí mismo
    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }

    // Este método se llama automáticamente por Unity cuando otro collider entra en contacto con el collider de este objeto
    private void OnTriggerEnter2D(Collider2D col)
    {
        // Si el objeto no tiene Rigidbody, no puede recibir daño
        if (col.attachedRigidbody == null) { return; }

        // Verificamos si el objeto tiene un NetworkObject para comparar el propietario
        if (col.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            // Si el propietario del objeto es el mismo que el del daño, no hacemos nada (evitamos daño propio)
            if (ownerClientId == netObj.OwnerClientId)
            {
                return;
            }
        }
        // Si el objeto tiene un componente Health, le aplicamos daño
        if (col.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            // Aplicamos daño al objeto usando su método TakeDamage, pasando el valor de daño definido en este script
            health.TakeDamage(damage);
        }
    }
}
