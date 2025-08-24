using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlowerController : MonoBehaviour
{
    public PetalController[] petals;
    public int initialSequenceLength = 1;
    private List<int> sequence = new List<int>();
    private int currentStep = 0;
    private int round = 0;
    private bool playerTurn = false;

    public SwitchCamera switchCamera;

    private void Start()
    {
        StartCoroutine(StartRound());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switchCamera.ManagerCamera(0); // Troca para visão por cima
        }
    }

    private IEnumerator StartRound()
    {
        round++;
        playerTurn = false;
        // Adiciona um novo índice aleatório à sequência
        sequence.Add(Random.Range(0, petals.Length));
        yield return ShowSequence();
        currentStep = 0;
        playerTurn = true;
    }

    private IEnumerator ShowSequence()
    {
        for (int i = 0; i < sequence.Count; i++)
        {
            petals[sequence[i]].SetColor(Color.yellow); // Exemplo: cor de destaque
            // TODO: tocar som
            yield return new WaitForSeconds(0.5f);
            petals[sequence[i]].SetColor(Color.white); // Exemplo: cor normal
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void OnPetalStepped(int petalIndex)
    {
        if (!playerTurn) return;

        if (petalIndex == sequence[currentStep])
        {
            currentStep++;
            if (currentStep >= sequence.Count)
            {
                Debug.Log("Rodada completa!");
                StartCoroutine(StartRound());
            }
        }
        else
        {
            Debug.Log("Errou! Reiniciando jogo.");
            sequence.Clear();
            round = 0;
            StartCoroutine(StartRound());
        }
    }
}