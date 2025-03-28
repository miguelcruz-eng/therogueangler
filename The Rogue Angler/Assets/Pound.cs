using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pound : MonoBehaviour
{
    public bool inRange;
    public bool interacted;
    
    public GameObject botao;

    // [SerializeField] GameObject bob;
    // private GameObject _bob;

    // [SerializeField] Transform sideAreaRTrasnform, sideAreaLTrasnform;
    // [SerializeField] Vector2 sideAreaR, sideAreaL;

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireCube(sideAreaRTrasnform.position, sideAreaR);
    //     Gizmos.DrawWireCube(sideAreaLTrasnform.position, sideAreaL);

    // }

    private void Update()
    {
        if (inRange)
        {
            interacted = true;

            //GameManager.Instance.gameIsPaused = true;

            PlayerController.Instance.Fishing();
            
            if (PlayerController.Instance.pState.fishing)
            {
                StartCoroutine(DestroyPound());
            }
        }
        if (inRange && !PlayerController.Instance.pState.fishing)
        {
            botao.SetActive(true);
        }
    }

    IEnumerator DestroyPound()
    {
        yield return new WaitForSeconds(0.50f);
        
        Destroy(gameObject, 1f);
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
