using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(testePrefab))]
public class testePrefabEditor : Editor
{
    private testePrefab script;
    private bool modoEdicao = false;
    
    private void OnEnable()
    {
        script = (testePrefab)target;
    }
    
    public override void OnInspectorGUI()
    {
        // Desenhar o inspector padrão
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Ative o 'Modo de Edição' e clique na Scene View para instanciar os prefabs.\n" +
            "A área de efeito será mostrada onde você clicar.",
            MessageType.Info
        );
        
        EditorGUILayout.Space();
        
        // Botão para ativar/desativar modo de edição
        Color corOriginal = GUI.backgroundColor;
        GUI.backgroundColor = modoEdicao ? Color.green : Color.white;
        
        if (GUILayout.Button(modoEdicao ? "Modo de Edição ATIVO (Clique na Scene)" : "Ativar Modo de Edição", GUILayout.Height(100)))
        {
            modoEdicao = !modoEdicao;
            SceneView.RepaintAll();
        }
        
        GUI.backgroundColor = corOriginal;
        
        if (modoEdicao)
        {
            EditorGUILayout.HelpBox("Clique na Scene View para instanciar prefabs!", MessageType.Warning);
        }
        
        // Validação
        if (script.prefabsParaInstanciar == null || script.prefabsParaInstanciar.Length == 0)
        {
            EditorGUILayout.HelpBox("Atribua pelo menos um prefab para instanciar!", MessageType.Error);
        }
    }
    
    private void OnSceneGUI()
    {
        if (!modoEdicao)
            return;
        
        // Pegar o evento atual
        Event e = Event.current;
        
        // Desenhar um label na Scene View
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(10, 10, 300, 50));
        GUILayout.Box("MODO DE EDIÇÃO ATIVO\nClique para instanciar prefabs", GUILayout.Height(40));
        GUILayout.EndArea();
        Handles.EndGUI();
        
        // Fazer raycast a partir da câmera (onde o usuário clicar)
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, script.layerMask))
        {
            // Desenhar o círculo de área de efeito
            Handles.color = script.corCirculo;
            Quaternion orientacao = Quaternion.FromToRotation(Vector3.up, hit.normal);
            
            // Desenhar disco
            Handles.DrawSolidDisc(hit.point, hit.normal, script.raioAreaEfeito);
            
            // Desenhar círculo de contorno
            Handles.color = new Color(script.corCirculo.r, script.corCirculo.g, script.corCirculo.b, 1f);
            Handles.DrawWireDisc(hit.point, hit.normal, script.raioAreaEfeito);
            
            // Desenhar normal
            Handles.color = Color.blue;
            Handles.DrawLine(hit.point, hit.point + hit.normal * 2f);
            
            // Desenhar label com informações
            Handles.Label(hit.point + hit.normal * 2.5f, 
                $"Prefabs: {script.numeroDePrefabs}\nRaio: {script.raioAreaEfeito:F2}m",
                new GUIStyle()
                {
                    normal = new GUIStyleState() { textColor = Color.white },
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                });
            
            // Detectar clique
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                // Instanciar os prefabs
                script.InstanciarPrefabs(hit.point, hit.normal);
                
                // Consumir o evento para não selecionar objetos
                e.Use();
            }
        }
        
        // Forçar a atualização da Scene View
        if (e.type == EventType.MouseMove)
        {
            SceneView.currentDrawingSceneView.Repaint();
        }
        
        // Impedir seleção de objetos no modo de edição
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
    }
}
