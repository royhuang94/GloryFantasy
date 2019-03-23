using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DamagePopup : MonoBehaviour {
 
    //伤害数值
    public int Value;
 
    //销毁时间
    public float FreeTime=1.5F;

    private void Awake()
    {
        gameObject.GetComponent<Text>().text = Value.ToString();
    }

    void Start () 
    {
        
        //StartCoroutine("Free");
    }
 
    void Update()
    {
        //使文本在垂直方向山产生一个偏移
        //transform.Translate(Vector3.up * 0.5F * Time.deltaTime);
    }
 
    IEnumerator Free()
    {
        yield return new WaitForSeconds(FreeTime);
        Destroy(this.gameObject);
    }
}