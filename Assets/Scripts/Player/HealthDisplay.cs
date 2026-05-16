using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Este script se encarga de mostrar la vida del objeto en una barra visual (UI)
/// y actualizarla automáticamente cuando cambia la vida en red.
/// Convierte el valor numérico de la vida en un visual que el jugador puede entender fácilmente.
/// </summary>
public class HealthDisplay : NetworkBehaviour
{
    /// <summary>
    /// Referencia al script Health que contiene la vida del objeto y los eventos.
    /// Se asigna desde el Inspector y se usa para suscribirse a cambios de vida.
    /// </summary>
    [SerializeField] private Health health;

    /// <summary>
    /// Referencia a la componente Image de la barra de vida en la UI.
    /// Se ajusta su propiedad 'fillAmount' para mostrar la vida visualmente (0 = vacío, 1 = lleno).
    /// </summary>
    [SerializeField] private Image healthBarImage;

    /// <summary>
    /// Se ejecuta cuando el objeto aparece en la red (se spawned).
    /// Es cuando configuramos las suscripciones a eventos para mantener la UI sincronizada.
    /// </summary>
    public override void OnNetworkSpawn()
    {
        // Solo los clientes necesitan mostrar la UI (no el servidor)
        // El servidor gestiona la lógica pero no necesita mostrar gráficos localmente
        if (!IsClient) { return; }

        // Nos suscribimos al evento de cambio de vida del objeto Health
        // Cada vez que cambia CurrentHealth, se llama a HandleHealthChanged automáticamente
        // OnValueChanged es un evento específico de NetworkVariable que se dispara cuando el valor cambia
        health.CurrentHealth.OnValueChanged += HandleHealthChanged;

        // Actualizamos la barra inmediatamente con la vida actual
        // Esto evita que la barra muestre un valor incorrecto al spawnearse el objeto
        HandleHealthChanged(0, health.CurrentHealth.Value);
    }

    /// <summary>
    /// Se ejecuta cuando el objeto desaparece de la red (se despawned).
    /// Es importante desuscribirse de eventos para evitar fugas de memoria.
    /// Las fugas de memoria ocurren cuando los objetos se destruyen pero los eventos siguen activos.
    /// </summary>
    public override void OnNetworkDespawn()
    {
        if (!IsClient) { return; }

        // Nos desuscribimos del evento de cambio de vida
        // Esto evita que HandleHealthChanged se llame después de que el objeto ya no existe
        health.CurrentHealth.OnValueChanged -= HandleHealthChanged;
    }

    /// <summary>
    /// Este método se llama automáticamente cada vez que cambia la vida.
    /// Es el intermediario entre los datos de vida (Health.cs) y la visualización (barra de vida).
    /// Recibe la vida anterior (oldHealth) y la nueva vida (newHealth) para comparar.
    /// </summary>
    private void HandleHealthChanged(int oldHealth, int newHealth)
    {
        // Ajusta el fill (relleno) de la barra de salud
        // fillAmount va de 0 (vacío) a 1 (completamente lleno)
        // Dividimos la vida actual entre la vida máxima para obtener un valor entre 0 y 1
        // Ejemplo: Si vida actual es 50 y máxima es 100, fillAmount = 0.5 (barra media llena)
        healthBarImage.fillAmount = (float)newHealth / health.MaxHealth;
    }
}
