using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Array com todas as flores filhas")]
    public FlowerTarget[] flowers;

    [Header("Configurações do Jogo")]
    [Tooltip("Quantas flores devem ser acertadas para vencer")]
    public int flowersToWin = 5;

    [Tooltip("Tempo de espera antes de iniciar o ciclo")]
    public float startDelay = 2f;

    [Tooltip("Intervalo entre cada ciclo de abertura")]
    public float cycleCooldown = 1f;

    [Header("Padrões de Abertura")]
    [Tooltip("Número mínimo de flores que abrem por vez")]
    public int minFlowersPerCycle = 1;

    [Tooltip("Número máximo de flores que abrem por vez")]
    public int maxFlowersPerCycle = 3;

    [Tooltip("Se true, abre flores aleatórias. Se false, abre em sequência")]
    public bool randomPattern = true;

    [Header("Status do Jogo")]
    public int flowersHit = 0;
    public int totalAttempts = 0;
    public bool gameActive = false;

    private List<FlowerTarget> availableFlowers = new List<FlowerTarget>();
    private Coroutine cycleCoroutine;

    void Start()
    {
        if (flowers == null || flowers.Length == 0)
        {
            flowers = GetComponentsInChildren<FlowerTarget>();
            Debug.Log("[TargetController] Encontradas " + flowers.Length + " flores automaticamente.");
        }

        if (flowers.Length == 0)
        {
            Debug.LogError("[TargetController] Nenhuma flor encontrada! Adicione FlowerTarget aos objetos filhos.");
            return;
        }

        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        Debug.Log("[TargetController] Jogo iniciando em " + startDelay + " segundos...");
        yield return new WaitForSeconds(startDelay);

        gameActive = true;
        flowersHit = 0;
        totalAttempts = 0;

        Debug.Log("[TargetController] JOGO INICIADO! Acerte " + flowersToWin + " flores para vencer!");

        cycleCoroutine = StartCoroutine(FlowerCycle());
    }

    private IEnumerator FlowerCycle()
    {
        while (gameActive && flowersHit < flowersToWin)
        {
            int flowersToOpen = Random.Range(minFlowersPerCycle, maxFlowersPerCycle + 1);
            flowersToOpen = Mathf.Min(flowersToOpen, flowers.Length);

            List<FlowerTarget> selectedFlowers = SelectFlowers(flowersToOpen);

            Debug.Log("[TargetController] Abrindo " + selectedFlowers.Count + " flores...");

            foreach (FlowerTarget flower in selectedFlowers)
            {
                StartCoroutine(flower.OpenTemporarily());
            }

            float waitTime = selectedFlowers[0].openDuration + selectedFlowers[0].closedDuration + cycleCooldown;
            yield return new WaitForSeconds(waitTime);
        }

        if (flowersHit >= flowersToWin)
        {
            OnGameWon();
        }
    }

    private List<FlowerTarget> SelectFlowers(int count)
    {
        List<FlowerTarget> selected = new List<FlowerTarget>();
        List<FlowerTarget> available = new List<FlowerTarget>(flowers);

        if (randomPattern)
        {
            for (int i = 0; i < count && available.Count > 0; i++)
            {
                int randomIndex = Random.Range(0, available.Count);
                selected.Add(available[randomIndex]);
                available.RemoveAt(randomIndex);
            }
        }
        else
        {
            for (int i = 0; i < count && i < available.Count; i++)
            {
                selected.Add(available[i]);
            }
        }

        return selected;
    }

    public void OnFlowerHit(FlowerTarget flower)
    {
        flowersHit++;
        totalAttempts++;

        Debug.Log("[TargetController] Flores acertadas: " + flowersHit + "/" + flowersToWin);

        if (flowersHit >= flowersToWin)
        {
            OnGameWon();
        }
    }

    public void OnFlowerClosed(FlowerTarget flower, bool wasHit)
    {
        if (!wasHit)
        {
            totalAttempts++;
            Debug.Log("[TargetController] Flor " + flower.name + " fechou sem ser acertada.");
        }
    }

    private void OnGameWon()
    {
        gameActive = false;

        if (cycleCoroutine != null)
        {
            StopCoroutine(cycleCoroutine);
        }

        Debug.Log("╔════════════════════════════════╗");
        Debug.Log("║      PARABÉNS! VOCÊ VENCEU!    ║");
        Debug.Log("║  Flores acertadas: " + flowersHit + "/" + flowersToWin + "        ║");
        Debug.Log("║  Total de tentativas: " + totalAttempts + "      ║");
        Debug.Log("╚════════════════════════════════╝");
    }

    public void RestartGame()
    {
        if (cycleCoroutine != null)
        {
            StopCoroutine(cycleCoroutine);
        }

        foreach (FlowerTarget flower in flowers)
        {
            flower.SetFlowerState(false);
        }

        StartCoroutine(StartGame());
    }
}
