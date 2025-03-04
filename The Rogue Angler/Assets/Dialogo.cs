using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dialogo : MonoBehaviour
{
    public bool inRange;
    public bool interacted;
    public GameObject botao;

    int interacao = 1;

    private void Update()
    {
        if (inRange && Input.GetButtonDown("Interact") && !InputDecoder.yapping)
        {
            interacted = true;

            InputDecoder.yapping = true;

            GameManager.Instance.gameIsPaused = true;

            botao.SetActive(false);

            InputDecoder.labels = new List<Label>();

            InputDecoder.Commands = new List<string>();
            InputDecoder.CommandLine = 0;
            InputDecoder.LastCommand = "";

            InputDecoder.readScript("Script/Verdinho/Dialogo"+interacao);

            interacao = 2;

            Debug.Log("Falando");
        }
        if (inRange && !InputDecoder.yapping)
        {
            botao.SetActive(true);
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
