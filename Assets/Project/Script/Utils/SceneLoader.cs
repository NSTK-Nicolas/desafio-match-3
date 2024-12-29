using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening; // Biblioteca DOTween

public class SceneLoader : MonoBehaviour
{
    [Tooltip("Nome da cena que será carregada.")]
    public string sceneToLoad;
    private static bool isEventSystemPersistent = false;

    /// <summary>
    /// Carrega a nova cena e descarrega a atual.
    /// </summary>
    private void Awake()
    {
        // Garante que o EventSystem persista entre cenas
        PreserveEventSystem();
    }

    /// <summary>
    /// Preserva o EventSystem entre as cenas.
    /// </summary>
    private void PreserveEventSystem()
    {
        if (!isEventSystemPersistent)
        {
            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem != null)
            {
                DontDestroyOnLoad(eventSystem.gameObject);
                isEventSystemPersistent = true;
            }
        }
    }
    
    public void LoadNewScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            StartCoroutine(LoadAndUnloadScene());
        }
        else
        {
            Debug.LogError("O nome da cena a ser carregada não foi especificado.");
        }
    }

    private IEnumerator LoadAndUnloadScene()
    {
        // Carrega a nova cena de forma assíncrona
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        // Espera até que a cena esteja carregada
        while (!loadOperation.isDone)
        {
            yield return null;
        }

        // Mata todos os Tweens ativos antes de descarregar a cena
        KillAllTweensInScene();

        // Descarrega a cena atual
        Scene currentScene = SceneManager.GetActiveScene();
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentScene);

        // Espera até que a cena atual seja descarregada
        while (!unloadOperation.isDone)
        {
            yield return null;
        }

        // Define a nova cena como ativa
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad));
    }

    /// <summary>
    /// Mata todos os Tweens ativos na cena atual.
    /// </summary>
    private void KillAllTweensInScene()
    {
        DOTween.KillAll(); // Mata todos os Tweens ativos no projeto
    }
}
