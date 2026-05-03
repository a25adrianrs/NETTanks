using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    private void OnDestroy()
    {
        // Mediante la función Instantiate, se crea una nueva instancia del prefab 
        // en la posición del objeto actual (transform.position) 
        // y con una rotación de identidad (Quaternion.identity).
        // Esto significa que cada vez que este objeto sea destruido, 
        // se generará una nueva instancia del prefab en su lugar.
        Instantiate(prefab, transform.position, Quaternion.identity);
    }
}
