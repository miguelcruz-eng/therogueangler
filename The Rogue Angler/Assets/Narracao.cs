using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Narracao : MonoBehaviour
{
    public string path = "Default";
    public bool inRange;
    public bool interacted;
    
    public GameObject narracao;
    public int interacao = 1;
    public bool unique = false;

    private void Update()
    {
        if (inRange && !InputDecoder.yapping && !interacted)
        {
            interacted = true;

            InputDecoder.yapping = true;

            GameManager.Instance.gameIsPaused = true;

            InputDecoder.labels = new List<Label>();

            InputDecoder.Commands = new List<string>();
            InputDecoder.CommandLine = 0;
            InputDecoder.LastCommand = "";

            InputDecoder.readScript("Script/"+path+"/Dialogo"+interacao);

            if (unique)
            {
                narracao.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if(_other.CompareTag("Player")) inRange = true;
    }

    private void OnTriggerExit2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"));
        {
            inRange = false;
            interacted = false;
        }
    }
}
