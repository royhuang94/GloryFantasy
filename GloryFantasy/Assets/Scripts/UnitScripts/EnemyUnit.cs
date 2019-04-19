using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameUnit
{
    public class EnemyUnit : GameUnit, IPointerDownHandler
    {
        private void Start()
        {
            hp = unitAttribute.HP;
        }

        /// <summary>
        /// 鼠标点击敌人Unit，掉血
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            UnitManager.Instance.EnemyCurUnit = transform.GetComponentInParent<BattleMap.BattleMapBlock>().GetCoordinate();
            Debug.Log("currentEnemyPositon: "+UnitManager.Instance.EnemyCurUnit);
            if (UnitManager.Instance.canAttack)
            {
                //限制攻击范围移动为单位的攻击范围
                GameUnit Attacker;
                Vector2 unitPositon;
                {
                    if (BattleMap.BattleMap.getInstance().CheckIfHasUnits(UnitManager.Instance.CurUnit))
                    {
                        unitPositon = UnitManager.Instance.CurUnit;
                    }
                    else
                    {
                        unitPositon = BattleMap.BattleMap.getInstance().curMapPos;
                    }
                }
                Attacker = BattleMap.BattleMap.getInstance().GetUnitsOnMapBlock(unitPositon);
                GameUnit AttackedUnit = BattleMap.BattleMap.getInstance().GetUnitsOnMapBlock(UnitManager.Instance.EnemyCurUnit);
                Debug.Log(Attacker);
                Debug.Log(AttackedUnit);
                UnitAttackCommand unitAtk = new UnitAttackCommand(Attacker, AttackedUnit);
                if (unitAtk.Judge() == false) return;

                //攻击成功关闭攻击染色
                Gameplay.GetInstance().gamePlayInput.HandleAtkCancel(unitPositon);
                BattleMap.BattleMap.getInstance().selcetAction_Cancel.SetActive(false);

                GFGame.UtilityHelper.Log("触发攻击", GFGame.LogColor.RED);
                hp -= 3;               
                if (!IsDead())
                {
                    Debug.Log(hp);
                    float hpDivMaxHp = (float)hp / unitAttribute.MaxHp * 100;
                    var textHp = transform.GetComponentInChildren<Text>();
                    textHp.text = string.Format("Hp: {0}%", hpDivMaxHp);

                    //TODO 反击

                }
                else
                {
                    Destroy(this.gameObject);
                    BattleMap.BattleMap.getInstance().UpDateNeighbourBlock(UnitManager.Instance.EnemyCurUnit);
                }
                UnitManager.Instance.canAttack = false;
            }
        }
    }
}


