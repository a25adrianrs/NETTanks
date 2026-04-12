using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

// Este script se encarga de mostrar la vida del objeto en una barra (UI)
// y actualizarla automáticamente cuando cambia la vida en red
public class HealthDisplay : NetworkBehaviour
{
    [Header("References")]
    // Referencia al script Health
    [SerializeField] private Health health;
    // Referencia a la imagen de la barra de vida
    [SerializeField] private Image healthBarImage;

    // Se ejecuta cuando el objeto aparece en la red
    public override void OnNetworkSpawn()
    {
        // Solo los clientes necesitan mostrar la UI (no el servidor)
        if (!IsClient) { return; }

        // Nos suscribimos al evento que se lanza cuando cambia la vida
        health.CurrentHealth.OnValueChanged += HandleHealthChanged;

        // Actualizamos la barra inmediatamente con la vida actual
        HandleHealthChanged(0, health.CurrentHealth.Value);
    }

    // Se ejecuta cuando el objeto desaparece de la red
    public override void OnNetworkDespawn()
    {
        if (!IsClient) { return; }
        // Nos desuscribimos del evento para evitar errores o fugas de memoria
        health.CurrentHealth.OnValueChanged -= HandleHealthChanged;
    }

    // Este método se llama cada vez que cambia la vida
    private void HandleHealthChanged(int oldHealth, int newHealth)
    {
        // Ajusta el fill de la barra (0 = vacío, 1 = lleno)
        // Dividimos la vida actual entre la vida máxima para obtener un valor entre 0 y 1
        healthBarImage.fillAmount = (float)newHealth / health.MaxHealth;
    }
}
