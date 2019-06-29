using System.Collections;
using System.Collections.Generic;
using BattleMap;
using UnityEngine;

using IMessage;
using GamePlay;
using GamePlay.Input;
using GameGUI;

namespace Ability
{
    
    /// <summary>
    /// 要求携带者：弓手。使用者：携带者。消耗动作次数。选择4/5/5格以内的格子。使用者对其周围3级爆发范围的敌方单位造成4/4/6点伤害。
    /// </summary>
    public class Arrowrain : Ability
    {
        Trigger trigger;
        
        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            trigger = 
                new TArrowrain(this.GetCardReceiver(this), AbilityVariable.Damage.Value, AbilityVariable.Range.Value, abilityId);
            MsgDispatcher.RegisterMsg(trigger, abilityId);
            MyTargetConstraintList[0] = Range_0;
            MyTargetConstraintList[1] = Range_1;
        }

        public bool Range_0(object target)
        {
            // 如果目标类型不是GameUnit，直接返回false，为了防止后续强转出错
            if (!target.GetType().ToString().Equals("GameUnit.FriendlyUnit"))
            {
                return false;
            }

            GameUnit.GameUnit _unit = (target as GameUnit.GameUnit);
            // 检测使用者是不是携带者
            return _unit.tag.Contains("弓手");
        }

        public bool Range_1(object target)
        {
            return GameplayToolExtend.distanceBetween(
                       Gameplay.Instance().gamePlayInput.InputFSM.TargetList[0], 
                       target) <= AbilityVariable.Range.Value;
        }

    }

    public class TArrowrain : Trigger
    {

        private int _damage;
        private int _range;
        private string _abilityId;
        
        public TArrowrain(MsgReceiver speller, int damage, int area, string abilityId)
        {
            _damage = damage;
            _range = area;
            _abilityId = abilityId;
            register = speller;
            //初始化响应时点,为卡片使用时
            msgName = (int)MessageType.CastCard;
            //初始化条件函数和行为函数
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            //判断发动的卡是不是这个技能的注册者，并且这张卡是不是箭雨
            if (this.GetCastingCard().GetMsgReceiver() == register && this.GetCastingCard().ability_id.Contains(_abilityId))
                return true;
            else
                return false;
        }

        private void Action()
        {
//            GameUnit.GameUnit unit = (GameUnit.GameUnit)this.GetSelectingUnits()[0];
//            Vector3 unitCoordinate =  BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
//            AbilityFormat ability = AbilityDatabase.GetInstance().GetAbilityFormat("Arrowrain");
//            Gameplay.Instance().gamePlayInput.HandleSkillConfim(unitCoordinate,(int)ability.AbilityVariable.Range);//显示技能可释放范围（攻击范围)
            //GamePlay.Gameplay.Instance().gamePlayInput.isSkill = true;//可以发动技能
            List<BattleMapBlock> affectedBlocks =
                GameplayToolExtend.getAreaByPos(_range, Gameplay.Instance().gamePlayInput.InputFSM.TargetList[1]);
            
            foreach (BattleMapBlock affectedBlock in affectedBlocks)
            {
                if(affectedBlock.unit == null)
                    continue;
                
                GameplayToolExtend.DealDamage(
                    this.GetAbilitySpeller(),
                    affectedBlock.unit,
                    new Damage(_damage)
                    );
            }
        }
        
//        public void ReleaseSkill(GameUnit.GameUnit skillMaker,int skillRange,Vector2 skillMakerPosition, Vector2 targetPositon)
//        {
//            ReleaseSkillCommand releaseSkillCommand = new ReleaseSkillCommand(skillMaker, skillRange, skillMakerPosition, targetPositon);
//            if (releaseSkillCommand.Judge())
//            {
//                Debug.Log("箭雨发动");
//                //TODO技能伤害范围染色
//                Gameplay.Instance().gamePlayInput.HandleSkillCancel(BattleMap.BattleMap.Instance().GetUnitCoordinate(skillMaker), skillRange);//取消技能可释放范围染色
//
//                List<Vector2> vector2s = new List<Vector2>();
//                GameUnit.GameUnit unit = (GameUnit.GameUnit)this.GetSelectingUnits()[0];
//                Vector3 unitCoordinate = BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
//                vector2s = ShowRange.Instance().GetSkillRnage(unitCoordinate, _range);
//                foreach(Vector2 vector2 in vector2s)
//                {
//                    if (BattleMap.BattleMap.Instance().CheckIfHasUnits(vector2))
//                    {
//                        GameUnit.GameUnit gameUnit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(vector2);
//                        gameUnit.hp -= _damage;
//                    }
//                }
//                //TODO取消技能伤害范围染色
//                //GamePlay.Gameplay.Instance().gamePlayInput.isSkill = false;//箭雨释放完毕;
//            }
//        }
    }
}
