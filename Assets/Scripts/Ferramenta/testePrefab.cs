using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class testePrefab : MonoBehaviour
{
    [Header("Configurações de Prefab")]
    [Tooltip("Prefabs a serem instanciados")]
    public GameObject[] prefabsParaInstanciar;
    
    [Header("Configurações de Área")]
    [Tooltip("Raio do círculo de área de efeito")]
    [Range(0.5f, 20f)]
    public float raioAreaEfeito = 5f;
    
    [Tooltip("Número de prefabs a serem instanciados")]
    [Range(1, 100)]
    public int numeroDePrefabs = 10;
    
    [Header("Configurações de Alinhamento")]
    [Tooltip("Alinhar os prefabs com a normal da superfície")]
    public bool alinharComNormal = true;
    
    [Tooltip("Offset vertical dos prefabs")]
    public float offsetVertical = 0f;
    
    [Header("Configurações de Rotação")]
    [Tooltip("Rotação aleatória no eixo Y")]
    public bool rotacaoAleatoriaY = true;
    
    [Tooltip("Escala aleatória")]
    public bool escalaAleatoria = false;
    
    [Range(0.5f, 2f)]
    public float escalaMinima = 0.8f;
    
    [Range(0.5f, 2f)]
    public float escalaMaxima = 1.2f;
    
    [Header("Configurações de Layer")]
    [Tooltip("Layer do chão para detectar")]
    public LayerMask layerMask = -1;
    
    [Header("Visualização")]
    [Tooltip("Cor do círculo de área de efeito")]
    public Color corCirculo = new Color(0f, 1f, 0f, 0.3f);
    
    // Método chamado pelo editor para instanciar os prefabs
    public void InstanciarPrefabs(Vector3 pontoClique, Vector3 normalSuperficie)
    {
        if (prefabsParaInstanciar == null || prefabsParaInstanciar.Length == 0)
        {
            Debug.LogWarning("Nenhum prefab atribuído para instanciar!");
            return;
        }
        
        // Criar um grupo para organizar os prefabs instanciados
        GameObject grupo = new GameObject("GrupoPrefabs_" + System.DateTime.Now.ToString("HHmmss"));
        #if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(grupo, "Criar grupo de prefabs");
        #endif
        
        // Orientar o círculo de acordo com a normal do ponto de colisão
        Quaternion orientacaoCirculo = Quaternion.FromToRotation(Vector3.up, normalSuperficie);
        
        // Instanciar os prefabs
        for (int i = 0; i < numeroDePrefabs; i++)
        {
            // Pegar ponto aleatório dentro do círculo
            Vector2 pontoAleatorio2D = Random.insideUnitCircle * raioAreaEfeito;
            
            // Converter para 3D considerando a orientação do círculo
            Vector3 offsetLocal = new Vector3(pontoAleatorio2D.x, 0f, pontoAleatorio2D.y);
            Vector3 pontoAleatorio3D = pontoClique + orientacaoCirculo * offsetLocal;
            
            // Raycast para baixo para encontrar o chão
            Vector3 origemRay = pontoAleatorio3D + normalSuperficie * 10f; // Começar acima
            RaycastHit hit;
            
            if (Physics.Raycast(origemRay, -normalSuperficie, out hit, 20f, layerMask))
            {
                // Posição final com offset
                Vector3 posicaoFinal = hit.point + hit.normal * offsetVertical;
                
                // Calcular rotação baseada na normal
                Quaternion rotacao = Quaternion.identity;
                
                if (alinharComNormal)
                {
                    rotacao = Quaternion.FromToRotation(Vector3.up, hit.normal);
                }
                
                // Adicionar rotação aleatória no eixo Y
                if (rotacaoAleatoriaY)
                {
                    float anguloAleatorio = Random.Range(0f, 360f);
                    rotacao *= Quaternion.Euler(0f, anguloAleatorio, 0f);
                }
                
                // Escolher um prefab aleatório
                GameObject prefabEscolhido = prefabsParaInstanciar[Random.Range(0, prefabsParaInstanciar.Length)];
                
                // Instanciar o prefab
                GameObject instancia = Instantiate(prefabEscolhido, posicaoFinal, rotacao, grupo.transform);
                
                // Aplicar escala aleatória se habilitado
                if (escalaAleatoria)
                {
                    float escala = Random.Range(escalaMinima, escalaMaxima);
                    instancia.transform.localScale = Vector3.one * escala;
                }
                
                // Registrar no sistema de Undo
                #if UNITY_EDITOR
                Undo.RegisterCreatedObjectUndo(instancia, "Instanciar prefab");
                #endif
            }
        }
    }
    
    // Método para desenhar o círculo de área de efeito no editor
    public void DesenharCirculoAreaEfeito(Vector3 centro, Vector3 normal)
    {
        Quaternion orientacao = Quaternion.FromToRotation(Vector3.up, normal);
        
        // Desenhar círculo
        int segmentos = 64;
        Vector3 pontoAnterior = Vector3.zero;
        
        for (int i = 0; i <= segmentos; i++)
        {
            float angulo = (i / (float)segmentos) * Mathf.PI * 2f;
            Vector3 pontoLocal = new Vector3(Mathf.Cos(angulo) * raioAreaEfeito, 0f, Mathf.Sin(angulo) * raioAreaEfeito);
            Vector3 pontoMundo = centro + orientacao * pontoLocal;
            
            if (i > 0)
            {
                Debug.DrawLine(pontoAnterior, pontoMundo, corCirculo, 0.1f);
            }
            
            pontoAnterior = pontoMundo;
        }
        
        // Desenhar linhas cruzadas
        Vector3 direita = orientacao * Vector3.right * raioAreaEfeito;
        Vector3 frente = orientacao * Vector3.forward * raioAreaEfeito;
        
        Debug.DrawLine(centro - direita, centro + direita, corCirculo, 0.1f);
        Debug.DrawLine(centro - frente, centro + frente, corCirculo, 0.1f);
    }
}
