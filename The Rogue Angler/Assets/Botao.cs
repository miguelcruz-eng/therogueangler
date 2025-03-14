using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Botao : MonoBehaviour
{
    public bool inRange;
    public bool interacted;

    public GameObject botao;
    public GameObject button;
    private Animator buttonAnimator;

    void Start()
    {
        // Obtém o componente Animator do GameObject do botão
        buttonAnimator = button.GetComponent<Animator>();

        if (buttonAnimator == null)
        {
            Debug.LogError("Animator component not found on the button GameObject.");
        }
    }

    private void Update()
    {
        if (inRange && Input.GetButtonDown("Interact"))
        {
            interacted = true;

            botao.SetActive(false);

            buttonAnimator.SetBool("Open", true);
        }
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if(_other.CompareTag("Player")) inRange = true;
        botao.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"));
        {
            inRange = false;
            interacted = false;
            botao.SetActive(false);
        }
    }
}
