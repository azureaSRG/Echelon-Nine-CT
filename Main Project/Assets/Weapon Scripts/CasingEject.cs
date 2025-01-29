using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasingEject : MonoBehaviour
{
    public Rigidbody myRigidbody;
    float forceApplied;
    float caseLifetime = 4;
    float caseFadetime = 2;

    // Start is called before the first frame update
    void Start()
    {
        //Applies ejection force
        float forceApplied = Random.Range(70, 100);
        myRigidbody.AddForce(transform.right * forceApplied);
        myRigidbody.AddTorque(Random.insideUnitSphere * forceApplied);

        //Initilizes Fade Function
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(caseLifetime);

        float percent = 0;
        float fadeSpeed = 1 / caseFadetime;
        Material mat = GetComponent<Renderer>().material;
        Color initalColor = mat.color;

        // While object is seeable, fade
        while (percent < 1)
        {
            percent += fadeSpeed * Time.deltaTime;
            mat.color = Color.Lerp(initalColor, Color.clear, percent);
            yield return null;
        }

        //Destorys object
        Destroy(gameObject);
    }
}
