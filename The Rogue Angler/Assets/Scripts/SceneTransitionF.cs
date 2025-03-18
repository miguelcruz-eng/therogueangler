using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionF : MonoBehaviour
{
    [SerializeField] private string transitionTo;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Vector2 exitDirection;
    [SerializeField] private float exitTime;

    public bool inRange;
    public bool interacted;
    
    public GameObject botao;

    private void Start()
    {
        if (transitionTo == GameManager.Instance.transitionedFromScene)
        {
            PlayerController.Instance.transform.position = startPoint.position;

            StartCoroutine(PlayerController.Instance.walkIntoNewScene(exitDirection, exitTime));
        }
        
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
    }

    private void Update()
    {
        if (inRange && Input.GetButtonDown("Interact"))
        {
            interacted = true;

            botao.SetActive(false);

            GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;

            PlayerController.Instance.pState.cutscene = true;
            PlayerController.Instance.pState.invincible = true;
            
            StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, transitionTo));
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
