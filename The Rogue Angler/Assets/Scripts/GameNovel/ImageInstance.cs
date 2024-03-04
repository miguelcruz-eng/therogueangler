using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ImageInstance : MonoBehaviour
{
    public float FadeInSpeed = 1f;
    public bool FadeIn, FadeOut;
    private GameObject gObject;
    private Image thisImage;

    // Start is called before the first frame update
    void Start()
    {
        gObject = this.gameObject;
        thisImage = this.gameObject.GetComponent<Image>();

        if(FadeIn)
        {
            Color c = thisImage.color;
            c.a = 0;
            thisImage.color = c;
            StartCoroutine(FadeInRoutine(FadeInSpeed));
        }    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator FadeInRoutine(float imageDeleay)
    {
        Color c = thisImage.color;
        for(float alpha = 0f; alpha <= 1f; alpha += 0.05f)
        {
            c.a = alpha;
            thisImage.color = c;
            yield return new WaitForSeconds(imageDeleay / 20);
        }
        c.a = 1.0f;
        thisImage.color = c;
    }

    IEnumerator FadeOutRoutine(float imageDeleay)
    {
        Color c = thisImage.color;
        for(float alpha = 1f; alpha >= 1f; alpha -= 0.05f)
        {
            c.a = alpha;
            thisImage.color = c;
            yield return new WaitForSeconds(imageDeleay / 20);
        }
        Destroy(gObject);
    }

    public void KillMe()
    {
        StartCoroutine("FadeOutRoutine");
    }
}
