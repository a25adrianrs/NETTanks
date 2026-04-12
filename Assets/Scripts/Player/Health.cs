using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    // Vida máxima del objeto (editable en Unity pero no desde otros scripts)
    [field: SerializeField] public int MaxHealth { get; private set; } = 100;
    // Vida actual sincronizada en red (todos los jugadores ven el mismo valor)
    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();

    private bool isDead;
    // Evento que se ejecuta cuando el objeto muere
    public Action<Health> OnDie;

    public override void OnNetworkSpawn()
    {
        // Solo el servidor puede establecer la vida inicial

        if (!IsServer) { return; }

        // Inicializa la vida al máximo
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