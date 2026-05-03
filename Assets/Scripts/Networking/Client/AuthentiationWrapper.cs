using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper
{
    // Esta clase es un wrapper para el sistema de autenticación de Unity, 
    // para manejar el estado de autenticación y los reintentos de forma más sencilla.
    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;


    // Método principal para realizar la autenticación, maneja los estados y reintentos.
    public static async Task<AuthState> DoAuth(int maxRetries = 5)
    {
        // Si ya estamos autenticados, simplemente devolvemos el estado.
        if (AuthState == AuthState.Authenticated)
        {
            return AuthState;
        }
        // Si ya estamos en proceso de autenticación, esperamos a que termine y devolvemos el estado final.
        if (AuthState == AuthState.Authenticating)
        {
            Debug.LogWarning("Already authenticating!");
            await Authenticating();
            return AuthState;
        }

        await SignInAnonymouslyAsync(maxRetries);

        return AuthState;
    }

    // Método privado para esperar a que termine el proceso de autenticación, devolviendo el estado final.
    private static async Task<AuthState> Authenticating()
    {
        // Esperamos a que el proceso de autenticación termine, verificando el estado cada 200ms.
        while (AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
        {
            await Task.Delay(200);
        }

        return AuthState;
    }


    // Método privado para realizar la autenticación anónima, manejando los reintentos y actualizando el estado.
    private static async Task SignInAnonymouslyAsync(int maxRetries)
    {
        AuthState = AuthState.Authenticating;

        int retries = 0;
        // Intentamos autenticarnos hasta que tengamos éxito o superemos el número máximo de reintentos.
        while (AuthState == AuthState.Authenticating && retries < maxRetries)
        {
            // Intentamos autenticarnos de forma anónima utilizando el servicio de autenticación de Unity.
            try
            {   // Si la autenticación fue exitosa, actualizamos el estado a Authenticated y salimos del bucle.
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                // Si la autenticación fue exitosa, actualizamos el estado a Authenticated y salimos del bucle.
                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    // Debug.Log($"Player signed in successfully with PlayerId: {AuthenticationService.Instance.PlayerId}");
                    AuthState = AuthState.Authenticated;
                    break;
                }
            }// Si ocurre una excepción durante la autenticación, la registramos y actualizamos el estado a Error.
            catch (AuthenticationException ex)
            {
                Debug.LogError(ex);
                AuthState = AuthState.Error;
            }// Si ocurre una excepción de tipo RequestFailedException, la registramos y actualizamos el estado a Error.
            catch (RequestFailedException exeption)
            {
                Debug.LogError(exeption);
                AuthState = AuthState.Error;
            }

            retries++;
            await Task.Delay(1000);
        }
        // Si después de los reintentos no hemos logrado autenticarnos, actualizamos el estado a TimeOut y registramos una advertencia.
        if (AuthState != AuthState.Authenticated)
        {
            Debug.LogWarning($"Player was not signed in successfully after {retries} retries");
            AuthState = AuthState.TimeOut;
        }
    }
}
// Enum para representar los diferentes estados de autenticación.
public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}