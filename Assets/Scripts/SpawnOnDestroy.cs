using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Este script crea una copia (instancia) de un prefab en la posición actual del objeto 
/// cada vez que el objeto es destruido.
/// Útil para crear efectos de explosión, dejar rastros, o generar nuevos objetos al destruirse.
/// </summary>
public class SpawnOnDestroy : MonoBehaviour
{
    /// <summary>
    /// El prefab que se instanciará cuando este objeto sea destruido.
    /// Un prefab es una plantilla reutilizable de un GameObject configurada previamente.
    /// Se asigna desde el Inspector de Unity.
    /// </summary>
    [SerializeField] private GameObject prefab;

    /// <summary>
    /// Se ejecuta automáticamente cuando el objeto está a punto de ser destruido.
    /// Es el momento perfecto para realizar acciones finales como generar efectos o spawneando nuevos objetos.
    /// </summary>
    private void OnDestroy()
    {
        // Mediante la función Instantiate, se crea una nueva instancia del prefab 
        // en la posición del objeto actual (transform.position) 
        // y con una rotación de identidad (Quaternion.identity).
        // transform.position = ubicación en el mundo donde se spawneará el prefab
        // Quaternion.identity = rotación neutra (0,0,0) - sin rotación aplicada
        // Esto significa que cada vez que este objeto sea destruido, 
        // se generará una nueva instancia del prefab en su lugar.
        Instantiate(prefab, transform.position, Quaternion.identity);
    }
}
