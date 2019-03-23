using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DamagePopup : MonoBehaviour {
 
    //伤害数值
    public int Value;
 
    //文本宽度
    public float ContentWidth=1000;
    //文本高度
    public float ContentHeight=500;
 
    //销毁时间
    public float FreeTime=1.5F;
 
    void Start () 
    {
        gameObject.GetComponent<Text>().text = Value.ToString();
        StartCoroutine("Free");
    }
 
    void Update()
    {
        //使文本在垂直方向山产生一个偏移
        transform.Translate(Vector3.up * 0.5F * Time.deltaTime);
    }
 
    void OnGUI()
    {
        //内部使用GUI坐标进行绘制
        
        GUI.color = Color.red;
        GUI.skin.label.fontSize = 32;
        GUI.Label(new Rect(transform.position.x,transform.position.y,ContentWidth,ContentHeight),Value.ToString());
    }
 
    IEnumerator Free()
    {
        yield return new WaitForSeconds(FreeTime);
        Destroy(this.gameObject);
    }
}