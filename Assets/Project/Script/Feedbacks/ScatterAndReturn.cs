using System.Collections;
using UnityEngine;
using DG.Tweening; // Biblioteca DOTween

public class ScatterAndReturn : MonoBehaviour
{
    [Header("Configurações de Dispersão")]
    [Tooltip("A força mínima aplicada radialmente para espalhar os botões.")]
    public float scatterForceMin = 3f;

    [Tooltip("A força máxima aplicada radialmente para espalhar os botões.")]
    public float scatterForceMax = 7f;

    [Tooltip("Duração antes de iniciar o retorno.")]
    public float scatterDuration = 1f;

    [Header("Configurações de Retorno")]
    [Tooltip("Velocidade da animação de retorno.")]
    public float returnSpeed = 1f;

    private bool isAnimating = false;
    private bool isSceneUnloading = false;

    /// <summary>
    /// Inicia o efeito de dispersão e retorno.
    /// </summary>
    public void Scatter()
    {
        if (isAnimating || isSceneUnloading) return;

        StopAllCoroutines();
        StartCoroutine(ScatterAndReturnCoroutine());
    }

    private IEnumerator ScatterAndReturnCoroutine()
    {
        isAnimating = true;

        int childCount = transform.childCount;
        Transform[] children = new Transform[childCount];
        Vector3[] initialPositions = new Vector3[childCount];

        // Captura os filhos e suas posições iniciais
        for (int i = 0; i < childCount; i++)
        {
            children[i] = transform.GetChild(i);
            initialPositions[i] = children[i].localPosition;
        }

        // Calcula e aplica a dispersão
        for (int i = 0; i < childCount; i++)
        {
            if (children[i] != null)
            {
                Vector3 direction = (children[i].localPosition - Vector3.zero).normalized;
                float randomForce = Random.Range(scatterForceMin, scatterForceMax);
                Vector3 scatterTarget = initialPositions[i] + direction * randomForce;

                children[i].DOLocalMove(scatterTarget, scatterDuration).SetEase(Ease.OutQuad);
                children[i].DOShakeScale(scatterDuration / 2, 0.2f, 10, 90, false).SetEase(Ease.OutBack);
            }
        }

        yield return new WaitForSeconds(scatterDuration);

        // Retorna os objetos às posições iniciais
        for (int i = 0; i < childCount; i++)
        {
            if (children[i] != null)
            {
                children[i].DOLocalMove(initialPositions[i], returnSpeed).SetEase(Ease.OutBack);
                children[i].DOScale(Vector3.one, returnSpeed / 2).SetEase(Ease.OutBack);
            }
        }

        yield return new WaitForSeconds(returnSpeed);

        isAnimating = false;
    }

    /// <summary>
    /// Método chamado antes da cena ser descarregada para evitar erros.
    /// </summary>
    private void OnDisable()
    {
        isSceneUnloading = true; // Marca que a cena está sendo descarregada
        StopAllCoroutines();    // Finaliza todas as corrotinas ativas neste script
    }
}
