using UnityEngine;

public class Dash : MonoBehaviour
{
    public float dashSpeed = 15f;       // velocidade do dash
    public float dashDuration = 0.2f;   // quanto tempo dura
    public float dashCooldown = 1f;     // tempo até poder usar de novo

    private CharacterController controller; // Referência ao CharacterController
    private bool isDashing = false; // Indica se o player está usando o dash
    private bool canDash = true; //Indica que o player pode usar o dash

    void Start()
    {
        // Atribui o componente CharacterController que está no mesmo GameObject do player
        controller = GetComponent<CharacterController>();
    }

    

    //void DoDash() //Sistema de corrotina para melhorar a performance do dash
    //{
    //    canDash = false; //Faz o dash entrar em cooldown
    //    isDashing = true; //Marca que o dash está sendo utilizado

    //    float startTime = Time.time; //Armazena o momento em que o dash começou

    //    // Enquanto o tempo atual for menor que o tempo inicial + duração configurada, o dash continua
    //    while (Time.time < startTime + dashDuration)
    //    {
    //        // move o player pra frente, sem mexer no Y (estava bugando com a camera)
    //        Vector3 dashMove = transform.forward * dashSpeed;
    //        controller.Move(dashMove * Time.deltaTime);

    //        // Espera até o próximo frame antes de continuar o loop
    //        yield return null;
    //    }

    //    isDashing = false; //Finaliza o dash

    //    //Espera o tempo do cooldown antes de deixar o próximo dash ser utilzado
    //    yield return new WaitForSeconds(dashCooldown);
    //    canDash = true; //Libera o próximo dash
    //}
}
