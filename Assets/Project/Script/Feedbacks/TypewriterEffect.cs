using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Texto alvo para o efeito.")]
    public TMP_Text textMeshPro;
    
    [Tooltip("Velocidade em segundos entre cada letra.")]
    public float typingSpeed = 0.05f;

    [Tooltip("Texto completo a ser exibido.")]
    [TextArea]
    public string fullText;

    [Tooltip("Inicia o efeito automaticamente ao ativar o GameObject.")]
    public bool startAutomatically = true;

    private Coroutine typewriterCoroutine;

    private void Start()
    {
        if (startAutomatically)
        {
            StartTypewriterEffect();
        }
    }

    /// <summary>
    /// Inicia o efeito de typewriter.
    /// </summary>
    public void StartTypewriterEffect()
    {
        if (textMeshPro == null)
        {
            Debug.LogError("TextMeshPro não está atribuído!");
            return;
        }

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }

        typewriterCoroutine = StartCoroutine(TypeText());
    }

    /// <summary>
    /// Interrompe o efeito de typewriter.
    /// </summary>
    public void StopTypewriterEffect()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }
    }

    private IEnumerator TypeText()
    {
        textMeshPro.text = ""; // Limpa o texto inicial

        foreach (char letter in fullText)
        {
            textMeshPro.text += letter; // Adiciona uma letra de cada vez
            yield return new WaitForSeconds(typingSpeed); // Aguarda o intervalo configurado
        }
    }
}