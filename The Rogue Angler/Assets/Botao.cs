using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Botao : MonoBehaviour
{
    public bool inRange;
    public bool interacted;

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
        if (inRange)
        {
            interacted = true;
            buttonAnimator.SetBool("Open", true);
        }
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if(_other.CompareTag("Bob")) inRange = true;
    }

    private void OnTriggerExit2D(Collider2D _other)
    {
        if (_other.CompareTag("Bob"));
        {
            inRange = false;
            interacted = false;
        }
    }
}
