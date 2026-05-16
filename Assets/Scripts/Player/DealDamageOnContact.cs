using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Este script se encarga de aplicar daño a otros objetos cuando el proyectil entra en contacto con ellos.
/// Verifica la propiedad del objeto para evitar daño amistoso (que un jugador se dañe a sí mismo).
/// Se usa típicamente en proyectiles, balas o cualquier objeto que inflige daño al contacto.
/// </summary>
public class DealDamageOnContact : MonoBehaviour
{
    /// <summary>
    /// Cantidad de daño que inflige este objeto cuando entra en contacto con otro.
    /// Se puede ajustar desde el Inspector de Unity para diferentes tipos de proyectiles.
    /// </summary>
    [SerializeField] private int damage = 5;

    /// <summary>
    /// ID único del cliente propietario de este proyectil.
    /// Se usa para evitar que un jugador se dañe a sí mismo o a su equipo.
    /// Se obtiene del objeto que lanzó el proyectil.
    /// </summary>
    private ulong ownerClientId;

    /// <summary>
    /// Se ejecuta para establecer quién es el propietario de este proyectil.
    /// Esto permite que el sistema de daño sepa quién disparó el proyectil.
    /// Evita daño amistoso (self-damage) durante el multiplicador.
    /// </summary>
    public void SetOwner(ulong ownerClientId)
    {
        // Guarda el ID del cliente propietario para compararlo después en OnTriggerEnter2D
        this.ownerClientId = ownerClientId;
    }

    /// <summary>
    /// Se ejecuta automáticamente por Unity cuando otro collider entra en contacto 
    /// con el collider de este objeto (configurado como trigger).
    /// Es donde se aplica el daño a objetos que tienen el componente Health.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D col)
    {
        // Si el objeto con el que chocamos no tiene Rigidbody2D, no puede recibir daño ni está configurado correctamente
        if (col.attachedRigidbody == null) { return; }

        // Verificamos si el objeto que golpeamos tiene un NetworkObject (componente de red)
        // TryGetComponent intenta obtener el componente y devuelve true si lo encuentra
        if (col.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            // Si el propietario del objeto golpeado es el mismo que el del proyectil, 
            // no aplicamos daño (evitamos daño amistoso/self-damage)
            if (ownerClientId == netObj.OwnerClientId)
            {
                return;
            }
        }

        // Intentamos obtener el componente Health del objeto golpeado
        // Si el objeto tiene este componente, significa que puede recibir daño
        if (col.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            // Aplicamos daño al objeto usando su método TakeDamage
            // Pasamos el valor de daño definido en este script
            health.TakeDamage(damage);
        }
    }
}
