using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase que representa una moneda que puede reaparecer después de ser recolectada. 
// Hereda de la clase Coin y agrega la funcionalidad de reaparición.
public class RespawningCoin : Coin
{
    // Evento que se lanza cuando la moneda es recolectada, permitiendo que otros scripts reaccionen a esta acción.
    public event Action<RespawningCoin> OnCollected;

    // Variable para almacenar la posición anterior de la moneda, 
    // utilizada para detectar cambios de posición y mostrar la moneda si se mueve.
    private Vector3 previousPosition;

    private void Update()
    {
        // Si la posición de la moneda ha cambiado desde el último frame, la mostramos.
        if (previousPosition != transform.position)
        {
            Show(true);
        }
        // Actualizamos la posición anterior para la próxima comparación.
        previousPosition = transform.position;
    }

    // Implementación del método Collect, que se llama cuando la moneda es recolectada por un jugador.
    public override int Collect()
    {
        // Solo el servidor puede manejar la lógica de recolección y reaparecer la moneda,
        if (!IsServer)
        {
            Show(false);
            return 0;
        }

        // Si la moneda ya ha sido recolectada, no hace nada y devuelve 0.
        if (alreadyCollected) { return 0; }
        // Marca la moneda como recolectada para evitar que se recoja varias veces antes de reaparecer.
        alreadyCollected = true;
        // Oculta la moneda visualmente para indicar que ha sido recolectada.
        OnCollected?.Invoke(this);

        return coinValue;
    }

    // Método para reiniciar el estado de la moneda, permitiendo que vuelva a ser recolectada después de haber sido recogida.
    public void Reset()
    {
        alreadyCollected = false;
    }
}
