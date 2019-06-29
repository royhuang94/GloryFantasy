using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBox : UnitySingleton<MessageBox>
{

    public GameObject Title;
    private TextMesh textMesh;
    public bool isShow;

    // Use this for initialization
    void Start()
    {
        isShow = false;
        textMesh = Title.GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isShow == true)
            StartCoroutine(show());
    }

    IEnumerator show()
    {
        textMesh.color = new Color(255, 0, 0, 255);
        yield return new WaitForSeconds(1f);
        textMesh.color = new Color(255, 0, 0, 0);
        isShow = false;
    }
}