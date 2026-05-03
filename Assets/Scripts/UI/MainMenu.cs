using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Campo serializado para el campo de entrada del código de unión
    [SerializeField] private TMP_InputField joinCodeField;

    public async void StartHost()
    {
        //await HostSingleton.GaneManager.StartHostAsync();

    }

    public async void StartClient()
    {
        //await ClientSingleton.GameManager.StartClientAsync(joinCodeField.text);
    }
}