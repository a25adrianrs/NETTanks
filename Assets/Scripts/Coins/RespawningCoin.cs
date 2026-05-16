using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase que representa una moneda que puede reaparecer después de ser recolectada.
/// Hereda de la clase Coin abstracta y agrega la funcionalidad específica de reaparición.
/// Las monedas siguen el ciclo: visible → recolectada → oculta → reaparece → visible.
/// </summary>
public class RespawningCoin : Coin
{
    /// <summary>
    /// Evento que se dispara cuando la moneda es recolectada por un jugador.
    /// Otros scripts (como CoinSpawner) se pueden suscribir para reaccionar a la recolección.
    /// Por ejemplo: el spawner reloca la moneda cuando es recolectada.
    /// </summary>
    public event Action<RespawningCoin> OnCollected;

    /// <summary>
    /// Almacena la posición anterior de la moneda.
    /// Se utiliza para detectar cambios de posición y mostrar la moneda cuando se mueve.
    /// Cuando el spawner reloca la moneda a una nueva posición, se detecta el cambio aquí.
    /// </summary>
    private Vector3 previousPosition;

    /// <summary>
    /// Se ejecuta cada frame.
    /// Detecta cambios de posición para mostrar la moneda cuando es reubicar (respawned).
    /// </summary>
    private void Update()
    {
        // Compara la posición actual con la posición anterior
        // Si son diferentes, significa que la moneda ha sido movida (ha reaparecido)
        if (previousPosition != transform.position)
        {
            // Muestra la moneda llamando al método protegido Show(true)
            // Activa el SpriteRenderer para que sea visible nuevamente
            Show(true);
        }

        // Guarda la posición actual como la anterior para la próxima comparación
        // Esto permite detectar cambios de posición en el siguiente frame
        previousPosition = transform.position;
    }

    /// <summary>
    /// Implementación del método abstracto Collect definido en la clase Coin.
    /// Ejecuta la lógica específica de recolección para monedas que reaparecen.
    /// Solo el servidor puede manejar la recolección de forma oficial.
    /// Los clientes muestran la moneda como oculta mientras esperan confirmación del servidor.
    /// </summary>
    public override int Collect()
    {
        // Si no es el servidor, simula la recolección mostrando la moneda como oculta
        // Esto proporciona feedback visual inmediato sin esperar al servidor
        if (!IsServer)
        {
            // Oculta la moneda en el cliente para que el jugador vea que fue recolectada
            // El servidor confirmará la recolección después
            Show(false);
            return 0;
        }

        // Si la moneda ya ha sido recolectada previamente, no hacer nada
        // Evita que se cuente varias veces si múltiples jugadores la tocan a la vez
        if (alreadyCollected) { return 0; }

        // Marca la moneda como recolectada
        // Esto evita que se procese múltiples veces antes de ser reaparición
        alreadyCollected = true;

        // Dispara el evento OnCollected para notificar a los suscriptores
        // El spawner escucha este evento y reloca la moneda a una nueva posición
        OnCollected?.Invoke(this);

        // Devuelve el valor de la moneda que se sumará al monedero del jugador
        return coinValue;
    }

    /// <summary>
    /// Reinicia el estado de la moneda después de ser recolectada.
    /// Se llama por el spawner cuando reloca la moneda a una nueva posición.
    /// Permite que la moneda vuelva a ser recolectada después de su reaparición.
    /// </summary>
    public void Reset()
    {
        // Pone la bandera alreadyCollected en false
        // Ahora la moneda puede ser recolectada nuevamente
        alreadyCollected = false;
    }
}
