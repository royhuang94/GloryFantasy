using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit = GameUnit.GameUnit;
using UnityEngine.UI;

/// <summary>
/// 处理异常地图块，例如灼烧
/// </summary>
namespace BattleMap
{
    public class DebuffBattleMapBlock
    {
        /// <summary>
        ///创建烧灼块，通过坐标
        /// </summary>
        /// <param name="vector2"></param>
        public void SetBattleMapBlockBurning(Vector2 vector2)
        {
            BattleMapBlock bm = null;
            bm = BattleMap.Instance().GetSpecificMapBlock(vector2);
            SetBattleMapBlockBurning(bm);
        }

        /// <summary>
        /// 创造烧灼块，直接通过地图快
        /// </summary>
        /// <param name="bm"></param>
        public void SetBattleMapBlockBurning(BattleMapBlock bm)
        {
            bm.blockType = EMapBlockType.Burnning;
            SpriteRenderer upLayer = bm.transform.Find("upLayer").GetComponent<SpriteRenderer>();
            upLayer.sprite = BattleMap.Instance().firing;
            upLayer.color = new Color(255, 255, 255, 255);
        }
        /// <summary>
        /// 创造滞留块，通过坐标
        /// </summary>
        /// <param name="vector2s"></param>
        public void SetBattleMapBlockRetrad(Vector2 vector2)
        {
            BattleMapBlock bm = null;
            bm = BattleMap.Instance().GetSpecificMapBlock(vector2);
            SetBattleMapBlockRetrad(bm);
        }
        /// <summary>
        /// 创造滞留块，直接通过地图块
        /// </summary>
        /// <param name="bm"></param>
        public void SetBattleMapBlockRetrad(BattleMapBlock bm)
        {
            bm.blockType = EMapBlockType.Retire;
            SpriteRenderer upLayer = bm.transform.Find("upLayer").GetComponent<SpriteRenderer>();
            upLayer.sprite = BattleMap.Instance().viscous;
            upLayer.color = new Color(255, 255, 255, 255);
        }
        /// <summary>
        /// 异常地图块消失
        /// </summary>
        /// <param name="vector2"></param>
        public void SetBattleMapBlockNormal(Vector2 vector2)
        {
            BattleMapBlock bm = null;
            bm = BattleMap.Instance().GetSpecificMapBlock(vector2);
            SetBattleMapBlockNormal(bm);
        }
        public void SetBattleMapBlockNormal(BattleMapBlock bm)
        {
            bm.blockType = EMapBlockType.normal;
            SpriteRenderer upLayer = bm.transform.Find("upLayer").GetComponent<SpriteRenderer>();
            upLayer.color = new Color(255, 255, 255, 0);
        }
        ////单位进入灼烧块
        //public void UnitEnterBurning(Vector2 vector2)
        //{
        //    if (BattleMap.Instance().CheckIfHasUnits(vector2)){
        //        Unit unit = BattleMap.Instance().GetUnitsOnMapBlock(vector2);
        //        unit.hp -= 1;
        //        GamePlay.Gameplay.Instance().gamePlayInput.UpdateHp(unit);
        //    }
        //}

        ////单位进入滞留块
        //public void UnitEnterRetire(Unit unit,BattleMapBlock battleMapBlock)
        //{
        //    unit.mapBlockBelow = battleMapBlock;
        //}
    }
}

