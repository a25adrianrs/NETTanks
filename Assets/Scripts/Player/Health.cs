using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Este script gestiona la vida (salud) de un objeto en el juego multiplicador.
/// Sincroniza la vida a través de la red para que todos los jugadores vean el mismo estado de salud.
/// Lanza eventos cuando el objeto muere para permitir que otros scripts reaccionen (como reproducir sonidos o mostrar explosiones).
/// </summary>
public class Health : NetworkBehaviour
{
    /// <summary>
    /// Vida máxima del objeto (editable en Unity pero no desde otros scripts).
    /// [field: SerializeField] permite que sea editable en el Inspector sin permitir cambios desde código.
    /// { get; private set; } significa que cualquiera puede leer el valor pero solo esta clase puede cambiarlo.
    /// </summary>
    [field: SerializeField] public int MaxHealth { get; private set; } = 100;

    /// <summary>
    /// Vida actual del objeto sincronizada a través de la red.
    /// NetworkVariable<int> significa que todos los jugadores ven el mismo valor actualizado en tiempo real.
    /// Es esencial para multiplicador para que todos vean cuando alguien recibe daño.
    /// </summary>
    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();

    /// <summary>
    /// Bandera para rastrear si el objeto ya está muerto.
    /// Evita procesar daño adicional o eventos de muerte múltiples.
    /// </summary>
    private bool isDead;

    /// <summary>
    /// Evento que se dispara cuando el objeto muere.
    /// Otros scripts se pueden suscribir a este evento para reaccionar a la muerte.
    /// Por ejemplo: reproducir sonido de muerte, mostrar explosión, dar puntos al asesino, etc.
    /// </summary>
    public Action<Health> OnDie;

    /// <summary>
    /// Se ejecuta automáticamente cuando el objeto aparece en la red (se spawned).
    /// Es el momento correcto para inicializar valores sincronizados en red.
    /// Solo el servidor puede establecer valores iniciales.
    /// </summary>
    public override void OnNetworkSpawn()
    {
        // Solo el servidor puede establecer la vida inicial
        // Los clientes reciben automáticamente el valor actualizado del servidor
        if (!IsServer) { return; }

        // Inicializa la vida al máximo valor
        // Esto sincroniza automáticamente con todos los clientes conectados
        CurrentHealth.Value = MaxHealth;
    }

    /// <summary>
    /// Método público para aplicar daño al objeto.
    /// Se llama desde otros scripts (como DealDamageOnContact cuando un proyectil golpea).
    /// Simplemente resta el daño de la vida actual mediante ModifyHealth.
    /// </summary>
    public void TakeDamage(int damageValue)
    {
        // Usa el método ModifyHealth con un valor negativo para restar daño
        ModifyHealth(-damageValue);
    }

    /// <summary>
    /// Método público para curar el objeto y restaurar vida.
    /// Útil para items de curación, habilidades o efectos especiales.
    /// </summary>
    public void RestoreHealth(int healValue)
    {
        // Usa el método ModifyHealth con un valor positivo para sumar vida
        ModifyHealth(healValue);
    }

    /// <summary>
    /// Método central privado que modifica la vida del objeto.
    /// Consolida toda la lógica de cambio de vida en un solo lugar para mantener el código organizado.
    /// Maneja límites (mínimo 0, máximo MaxHealth) y verifica si el objeto muere.
    /// </summary>
    private void ModifyHealth(int value)
    {
        // Si ya está muerto, no procesamos más daño ni curación
        // Evita situaciones extrañas como "matar un muerto" o curar un cadáver
        if (isDead) { return; }

        // Calcula la nueva vida sumando o restando el valor modificador
        // Un valor negativo resta vida (daño), positivo suma vida (curación)
        int newHealth = CurrentHealth.Value + value;

        // Limita la vida entre 0 (mínimo) y MaxHealth (máximo)
        // Mathf.Clamp asegura que la vida nunca sea negativa ni exceda el máximo
        CurrentHealth.Value = Mathf.Clamp(newHealth, 0, MaxHealth);

        // Verifica si la vida ha llegado a 0 (el objeto ha muerto)
        if (CurrentHealth.Value == 0)
        {
            // Dispara el evento de muerte para que otros scripts reaccionen
            // Por ejemplo: enemigos cercanos pueden reproducir un sonido de muerte,
            // mostrar una explosión, eliminar el objeto del juego, etc.
            OnDie?.Invoke(this);

            // Marca el objeto como muerto para evitar procesar eventos de muerte múltiples
            isDead = true;
        }
    }
}