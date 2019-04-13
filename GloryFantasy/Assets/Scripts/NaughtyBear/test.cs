using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    UnityEvent m_MyEvent = new UnityEvent();

    public void ButtonClicked()
    {
        //TODO 此处可能为三种不同的事件
        //1.  人物部署
        //2. 单位移动   
        //3. 单位攻击 Unit攻击针对的是单位，此处可不考虑



        //TODO 进度过程中思考？
        //1.   怎么分离这些事件呢？
        //2.  如何跟，MsgDispathcer产生联系

        if (UnitManager.Instance.IsPickedUnit)
        {
            var buttonSelf = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            Debug.Log("yes, i can instantiate Unit " + buttonSelf.name);
            //检测到地图，可实例化棋子
            //UnitManager.Instance.CouldInstantiation(true, buttonSelf.name);

            //TODO 在当前类中处理事件的激活还是，全部写在对应的unit中
            //解决思路：新创建一个UnitMove脚本，用于单位实例化后挂在到单位上，通过继承IPointerDownHandler接口，实现OnPointerDown函数，持续监听消息

        }

        Debug.Log("点击事件触发");
    }

    //TODO 为啥直接取消息枚举中相同的名字，会自动调用此UnitMove函数
    //1. action与于同名函数一同使用？（会不会出问题）
    //2. 单独创建多个action使用？（会不会很麻烦）
    //public void UnitMove()
    //{
    //    //TODO 移动事件         
    //    //2. 单位移动   

    //    //TODO 此处可以调用MarkMove/AttackRange()函数处理行动力的渲染
    //   // unit.GetComponent<ShowRange>().MarkMoveRange();
    //   // unit.GetComponent<ShowRange>().MarkAttackRange();

    //    Debug.Log("单位开始准备移动");
    //}

    //TODO MsgDispather使用
    //1. 通过Awake 监听事件
    //2. eventEnter函数 中激活事件
    //3. action委托 模仿 MarkMoveRange()/MarkAttackRange()函数

    //TODO 计划顺序
    //1. 事件实现
    //2. 与朱的地图对接
    //3. 实现完整的移动功能

}
