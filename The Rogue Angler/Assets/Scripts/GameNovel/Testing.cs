using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Testing : MonoBehaviour
{
    void Awake()
    {
        // Inicialize outros objetos no InputDecoder
        InputDecoder.InterfaceElements = GameObject.Find("UI_Elements");
        InputDecoder.canvas = GameObject.Find("ImageLayers");
        InputDecoder.ImageInst = Resources.Load("Prefabs/Background") as GameObject;
        InputDecoder.PI = Resources.Load("Prefabs/PersonagemI") as GameObject;
        InputDecoder.PII = Resources.Load("Prefabs/PersonagemII") as GameObject;
        InputDecoder.DialogueTextObject = GameObject.Find("Dialogue_Text");
        InputDecoder.NamePlateTextObject = GameObject.Find("NamePlate_Text");
        InputDecoder.CharImage = GameObject.Find("Personagem");
        InputDecoder.labels = new List<Label>();

        InputDecoder.Commands = new List<string>();
        InputDecoder.CommandLine = 0;
        InputDecoder.LastCommand = "";
    }

    public bool seta = false;
    // Start is called before the first frame update
    void Start()
    {
        int numeroAleatorio = Random.Range(1, 10);
        InputDecoder.InterfaceElements.SetActive(false);
        InputDecoder.readScript("Script/Enredo"+numeroAleatorio);
        PlayerPrefs.SetString("QuizData", "Quiz"+numeroAleatorio);
        PlayerPrefs.SetString("AtencaoData", "atencao"+numeroAleatorio);
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKeyDown("h"))
        {
            if(InputDecoder.InterfaceElements.activeInHierarchy)
            {
                InputDecoder.InterfaceElements.SetActive(false);
            }
            else
            {
                InputDecoder.InterfaceElements.SetActive(true);
            }
        }*/

        if(InputDecoder.Commands[InputDecoder.CommandLine] != InputDecoder.LastCommand)
        {
            InputDecoder.LastCommand = InputDecoder.Commands[InputDecoder.CommandLine];
            InputDecoder.PausedHere = false;
            InputDecoder.ParseInputLine(InputDecoder.Commands[InputDecoder.CommandLine]);
        }else
        {
            InputDecoder.PausedHere = true;
        }

        if(!InputDecoder.PausedHere && InputDecoder.CommandLine < InputDecoder.Commands.Count - 1)
        {
            InputDecoder.CommandLine ++;
        }

        if(InputDecoder.PausedHere && seta && InputDecoder.CommandLine < InputDecoder.Commands.Count - 1)
        {
            InputDecoder.CommandLine ++;
            seta = false;
        }
    }

    public void setinha()
    {
        seta = true;
    }
}
