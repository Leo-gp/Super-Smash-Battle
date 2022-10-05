using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorAnimacoes : MonoBehaviour
{
    Animator animadorPlayer, animadorP2;
    float eixoHorizontal, eixoVertical;
    bool atirou, pulou;
    Player player;
    private void Awake()
    {
        player = GetComponent<Player>();
        animadorPlayer = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animadorPlayer.SetBool("NoChao", player.Grounded());
        ConfigurarEixos();
        Correr();
        Atirar();
        Pular();
    }

    void ConfigurarEixos()
    {
        if (player.isPlayer1)
        {
            eixoHorizontal = Input.GetAxis("HorizontalP1");
            eixoVertical = Input.GetAxis("VerticalP1");
            atirou = Input.GetButton("ShootP1");
            pulou = Input.GetButton("JumpP1");
            animadorPlayer.SetFloat("EixoHorizontal", eixoHorizontal);
            animadorPlayer.SetFloat("EixoVertical", eixoVertical);
        }
        else
        {
            eixoHorizontal = Input.GetAxis("HorizontalP2");
            eixoVertical = Input.GetAxis("VerticalP2");
            atirou = Input.GetButton("ShootP2");
            pulou = Input.GetButton("JumpP2");
            animadorPlayer.SetFloat("EixoHorizontal", eixoHorizontal);
            animadorPlayer.SetFloat("EixoVertical", eixoVertical);
        }
    }
    void Correr()
    {
        if ((eixoHorizontal > 0.1 || eixoHorizontal < -0.1) && animadorPlayer.GetBool("NoChao"))
            animadorPlayer.SetBool("Correndo", true);
        if (eixoHorizontal == 0 || !animadorPlayer.GetBool("NoChao"))
            animadorPlayer.SetBool("Correndo", false);
        if (animadorPlayer.GetBool("Correndo") && atirou)
        {
            animadorPlayer.SetBool("Atirar", false);
            animadorPlayer.SetBool("AtirarCorrendo", true);
            animadorPlayer.SetLayerWeight(1, 1);
        }
        else
        {
            animadorPlayer.SetBool("AtirarCorrendo", false);
            animadorPlayer.SetLayerWeight(1, 0);
        }
        if (animadorPlayer.GetBool("Correndo") && pulou)
        {
            animadorPlayer.SetBool("Atirar", false);
            animadorPlayer.SetBool("PularCorrendo", true);
            animadorPlayer.SetLayerWeight(3, 1);
        }
        else
        {
            animadorPlayer.SetBool("PularCorrendo", false);
            animadorPlayer.SetLayerWeight(3, 0);
        }
    }
    void Atirar()
    {
        if (atirou && !animadorPlayer.GetBool("AtirarCorrendo") && !animadorPlayer.GetBool("AtirarPulando"))
            animadorPlayer.SetBool("Atirar", true);
        if (!atirou)
            animadorPlayer.SetBool("Atirar", false);
    }
    void Pular()
    {
        if (pulou)
            animadorPlayer.SetBool("Pular", true);
        else
            animadorPlayer.SetBool("Pular", false);
        if (!animadorPlayer.GetBool("NoChao") && atirou)
        {
            animadorPlayer.SetBool("Atirar", false);
            animadorPlayer.SetBool("PularCorrendo", false);
            animadorPlayer.SetLayerWeight(3, 0);
            animadorPlayer.SetBool("AtirarPulando", true);
            animadorPlayer.SetLayerWeight(2, 1);
        }
        else
        {
            animadorPlayer.SetBool("AtirarPulando", false);
            animadorPlayer.SetLayerWeight(2, 0);
        }
    }
}
