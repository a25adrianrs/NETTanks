using System;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Controla la vida de un objeto con sincronización de red.
/// Solo el servidor inicializa y modifica la salud; los clientes reciben el valor actualizado.
/// </summary>
public class Health : NetworkBehaviour
{
    [field: SerializeField] public int MaxHealth { get; private set; } = 100;
    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();

    private bool isDead;
    public Action<Health> OnDie;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        CurrentHealth.Value = MaxHealth;
    }
    // Método para aplicar daño 
    public void TakeDamage(int damageValue)
    {
        ModifyHealth(-damageValue);
    }
    // Método para curar
    public void RestoreHealth(int healValue)
    {
        ModifyHealth(healValue);
    }
    // Método central que modifica la vida
    private void ModifyHealth(int value)
    {
        // Si ya está muerto, no hace nada
        if (isDead) { return; }
        // Calcula la nueva vida sumando o restando el valor
        int newHealth = CurrentHealth.Value + value;
        // Limita la vida entre 0 y la vida máxima
        CurrentHealth.Value = Mathf.Clamp(newHealth, 0, MaxHealth);
        // Si la vida llega a 0, el objeto muere
        if (CurrentHealth.Value == 0)
        {
            // Lanza el evento de muerte para que otros scripts(jugadores)  reaccionen

            OnDie?.Invoke(this);
            // Marca el objeto como muerto
            isDead = true;
        }
    }
}