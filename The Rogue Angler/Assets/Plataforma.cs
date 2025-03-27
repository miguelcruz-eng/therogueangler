using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Plataforma : MonoBehaviour
{
    public bool inRange;
    public bool interacted;
    
    public GameObject botao;

    [SerializeField] GameObject bob;
    private GameObject _bob;

    [SerializeField] Transform sideAreaRTrasnform, sideAreaLTrasnform;
    [SerializeField] Vector2 sideAreaR, sideAreaL;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(sideAreaRTrasnform.position, sideAreaR);
        Gizmos.DrawWireCube(sideAreaLTrasnform.position, sideAreaL);

    }

    private void Update()
    {
        if (inRange && Input.GetButtonDown("Interact") && !PlayerController.Instance.pState.bobing)
        {
            interacted = true;

            GameManager.Instance.gameIsPaused = true;

            botao.SetActive(false);

            StartCoroutine(ThrowBob());
        }
        if (inRange && !PlayerController.Instance.pState.bobing)
        {
            botao.SetActive(true);
        }
    }

    IEnumerator ThrowBob()
    {
        // Aguarda o momento certo para instanciar o fireball
        PlayerController.Instance.pState.bobing = true;
        yield return new WaitForSeconds(0.50f);
        GameObject _bob;
        if(PlayerController.Instance.pState.lookingRight)
        {
            _bob = Instantiate(bob, sideAreaRTrasnform.position, Quaternion.identity);
        } 
        else
        {
            _bob = Instantiate(bob, sideAreaLTrasnform.position, Quaternion.identity);
        }   

        yield return new WaitForSeconds(0.40f);
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
