using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerController : MonoBehaviour
{
    [Header("Pétalas")]
    public PetalController[] petals;

    [Header("Configurações do Jogo")]
    public int maxRounds = 10; // Número máximo de rodadas
    public float sequenceDisplayDelay = 0.5f; // Tempo que cada pétala fica acesa
    public float timeBetweenPetals = 0.3f; // Tempo entre cada pétala na sequência
    public float delayBeforeNextRound = 1.5f; // Delay antes de iniciar próxima rodada
    public float initialStartDelay = 3f; // Delay inicial antes de começar o jogo

    [Header("Sons das Pétalas")]
    public AudioClip[] petalSounds; // Um som para cada pétala (mesmo índice)
    public AudioClip errorSound; // Som de erro
    public AudioClip successSound; // Som de acerto de rodada
    public AudioClip victorySound; // Som de vitória final

    [Header("Câmera")]
    public SwitchCamera switchCamera;

    [Header("UI Feedback")]
    public Text feedbackText; // Texto para exibir mensagens ao jogador (UI Text Legacy)
    public Text sequenceText; // Texto para exibir a sequência correta

    [Header("Parede de Contenção")]
    public GameObject puzzleWall; // Parede que impede o jogador de sair da área

    [Header("Animator do Espinho")]
    public Animator spikeAnimator; // Animator para controlar da vitoria

    public Collider spikeCollider; // Collider do espinho para ativar/desativar
    public string spikeVictoryTrigger = "Victory"; // Nome do trigger para animação de vitória

    [Header("Estado do Jogo")]
    // Estado do jogo
    private List<int> fullSequence = new List<int>(); // Sequência completa sorteada no início
    private int currentRoundLength = 1; // Quantas pétalas mostrar nesta rodada
    private int currentStep = 0;
    private int round = 0;
    private bool playerTurn = false;
    private bool isPlaying = false;
    private bool gameStarted = false;
    private bool gameCompleted = false; // Flag para indicar que o jogo foi completado
    private bool processingInput = false; // Evita múltiplos inputs simultâneos

    private void Start()
    {
        // Inicializa as pétalas com suas cores normais
        ResetAllPetals();
        UpdateFeedbackText("");

        // Garante que a parede está desativada no início
        if (puzzleWall != null)
        {
            puzzleWall.SetActive(false);
        }
    }

    // Chamado quando o player entra no centro da flor
    public void StartGame()
    {
        // Só permite iniciar se não foi iniciado e não foi completado
        if (!gameStarted && !gameCompleted)
        {
            gameStarted = true;

            // Ativa a parede para impedir fuga
            if (puzzleWall != null)
            {
                puzzleWall.SetActive(true);
            }

            GenerateFullSequence(); // Sorteia a sequência completa
            StartCoroutine(InitialDelay());
        }
    }

    private void GenerateFullSequence()
    {
        fullSequence.Clear();

        for (int i = 0; i < maxRounds; i++)
        {
            fullSequence.Add(Random.Range(0, petals.Length));
        }

        UpdateFeedbackText("Sequência sorteada!");
        DisplayFullSequence(); // Mostra a sequência COMPLETA
    }

    private IEnumerator InitialDelay()
    {
        UpdateFeedbackText($"Jogo iniciará em {initialStartDelay:0} segundos...");

        // Aguarda 3 segundos antes de começar
        yield return new WaitForSeconds(initialStartDelay);

        UpdateFeedbackText("Iniciando jogo!");
        StartCoroutine(StartRound());
    }

    private IEnumerator StartRound()
    {
        // Verifica se já completou o jogo
        if (round >= maxRounds)
        {
            yield return GameComplete();
            yield break;
        }

        round++;
        currentRoundLength = round; // Mostra 1 pétala na rodada 1, 2 na rodada 2, etc.
        playerTurn = false;
        isPlaying = true;
        processingInput = false; // Reseta o bloqueio de input

        // IMPORTANTE: Reseta todas as pétalas antes de começar a nova rodada
        ResetAllPetals();

        UpdateFeedbackText($"Rodada {round}");

        // Aguarda um momento antes de mostrar a sequência
        yield return new WaitForSeconds(0.5f);

        // Mostra a sequência (até a rodada atual)
        yield return ShowSequence();

        // Reseta o passo atual e libera para o jogador
        currentStep = 0;
        playerTurn = true;
        isPlaying = false;

        UpdateFeedbackText("Sua vez!");
    }

    private IEnumerator ShowSequence()
    {
        UpdateFeedbackText($"Observe a sequência ({currentRoundLength} pétala{(currentRoundLength > 1 ? "s" : "")})");

        // Mostra apenas as pétalas até a rodada atual
        for (int i = 0; i < currentRoundLength; i++)
        {
            int petalIndex = fullSequence[i];

            // Acende a pétala
            petals[petalIndex].Highlight();

            // Toca o som da pétala
            PlayPetalSound(petalIndex);

            // Aguarda o tempo de display
            yield return new WaitForSeconds(sequenceDisplayDelay);

            // Apaga a pétala
            petals[petalIndex].ResetColor();

            // Aguarda entre pétalas
            yield return new WaitForSeconds(timeBetweenPetals);
        }
    }

    public void OnPetalStepped(int petalIndex)
    {
        // Ignora se não for turno do jogador, está mostrando sequência ou já está processando um input
        if (!playerTurn || isPlaying || processingInput) return;

        // Bloqueia novos inputs temporariamente
        processingInput = true;

        // Toca o som da pétala
        PlayPetalSound(petalIndex);

        // Verifica se acertou (compara com a sequência completa)
        if (petalIndex == fullSequence[currentStep])
        {
            // Acendeu a pétala correta em verde
            petals[petalIndex].SetColor(Color.green);

            currentStep++;

            // Completou a rodada
            if (currentStep >= currentRoundLength)
            {
                UpdateFeedbackText($"Rodada {round} completa! ✓");
                playerTurn = false;
                StartCoroutine(RoundComplete());
            }
            else
            {
                // Continua jogando - libera para próxima pétala após um pequeno delay
                UpdateFeedbackText($"Correto! ({currentStep}/{currentRoundLength})");
                StartCoroutine(UnlockInputAfterDelay(0.3f));
            }
        }
        else
        {
            // Errou! Marca a pétala errada em vermelho
            petals[petalIndex].SetColor(Color.red);

            UpdateFeedbackText("Erro! Reiniciando jogo... ✗");
            playerTurn = false;
            StartCoroutine(GameOver());
        }
    }

    private IEnumerator UnlockInputAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        processingInput = false;
    }

    private IEnumerator RoundComplete()
    {
        // Toca som de sucesso
        PlaySound(successSound);

        // Efeito visual de sucesso - todas as pétalas ficam VERDES
        yield return FlashAllPetals(1, 1f, Color.green);

        // Aguarda antes da próxima rodada
        yield return new WaitForSeconds(delayBeforeNextRound);

        // Inicia próxima rodada
        StartCoroutine(StartRound());
    }

    private IEnumerator GameOver()
    {
        playerTurn = false;
        isPlaying = true;

        // Toca som de erro
        PlaySound(errorSound);

        // Efeito visual de erro - todas as pétalas ficam VERMELHAS
        yield return FlashAllPetals(1, 1f, Color.red);

        // Reseta o jogo
        fullSequence.Clear();
        round = 0;
        currentStep = 0;
        currentRoundLength = 1;
        processingInput = false;

        UpdateFeedbackText("Prepare-se para recomeçar...");

        // Aguarda e reinicia
        yield return new WaitForSeconds(1.5f);

        // Gera uma nova sequência completa
        GenerateFullSequence();
        StartCoroutine(StartRound());
    }

    private IEnumerator GameComplete()
    {
        UpdateFeedbackText("PARABÉNS! Você venceu! 🎉");

        playerTurn = false;
        isPlaying = true;
        gameCompleted = true; // Marca o jogo como completado

        // Dispara a animação do espinho
        if (spikeAnimator != null && !string.IsNullOrEmpty(spikeVictoryTrigger))
        {
            spikeAnimator.SetTrigger(spikeVictoryTrigger);
        }

        // Desativa o collider (box) do espinho
        if (spikeCollider != null)
        {
            spikeCollider.enabled = false;
        }

        // Toca som de vitória
        PlaySound(victorySound);

        // Efeito visual de vitória (pétalas piscam várias cores)
        for (int i = 0; i < 5; i++)
        {
            foreach (var petal in petals)
            {
                petal.SetColor(Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f));
            }
            yield return new WaitForSeconds(0.2f);
        }

        ResetAllPetals();

        // Desativa a parede para liberar o jogador
        if (puzzleWall != null)
        {
            puzzleWall.SetActive(false);
        }

        // Volta para a câmera de terceira pessoa
        if (switchCamera != null)
        {
            switchCamera.ManagerCamera(1); // Troca para visão de terceira pessoa
        }

        
        
    }

    private IEnumerator FlashAllPetals(int times, float interval, Color? color = null)
    {
        Color flashColor = color ?? Color.yellow;

        for (int i = 0; i < times; i++)
        {
            foreach (var petal in petals)
            {
                petal.SetColor(flashColor);
            }
            yield return new WaitForSeconds(interval);

            ResetAllPetals();
            yield return new WaitForSeconds(interval);
        }
    }

    private void ResetAllPetals()
    {
        foreach (var petal in petals)
        {
            petal.ResetColor();
        }
    }

    private void PlayPetalSound(int petalIndex)
    {
        if (petalSounds != null && petalIndex < petalSounds.Length && petalSounds[petalIndex] != null)
        {
            AudioManager audioManager = AudioManager.GetInstance();
            if (audioManager != null)
            {
                audioManager.PlaySFX(petalSounds[petalIndex]);
            }
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioManager audioManager = AudioManager.GetInstance();
            if (audioManager != null)
            {
                audioManager.PlaySFX(clip);
            }
        }
    }

    private void UpdateFeedbackText(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
    }

    private void DisplayFullSequence()
    {

        if (sequenceText == null) return;

        sequenceText.text = "";

        for (int i = 0; i < fullSequence.Count; i++)
        {
            sequenceText.text += (fullSequence[i]).ToString();

            // Adiciona separador se não for o último
            if (i < fullSequence.Count - 1)
            {
                sequenceText.text += " → ";
            }
        }
    }

    // Método chamado pelo Player quando entra no centro da flor
    public void OnPlayerEnterCenter()
    {
        // Só inicia o jogo se ainda não foi iniciado e não foi completado
        if (!gameStarted && !gameCompleted)
        {
            if (switchCamera != null)
            {
                switchCamera.ManagerCamera(0); // Troca para visão por cima
            }

            StartGame();
        }
    }
}