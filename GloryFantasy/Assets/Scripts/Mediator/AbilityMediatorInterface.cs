using BattleMap;
using System.Collections.Generic;
using UnityEngine;

namespace Mediator
{
    public interface AbilityMediatorInterface
    {
        /// <summary>
        /// 根据指定的Unit的Id，查询相应Id是否还活着，死亡则true
        /// </summary>
        /// <param name="unitId">要查询的unit的id</param>
        /// <returns>若单位死亡，返回true，否则返回false</returns>
        bool CheckUnitDeathById(string unitId);

        /// <summary>
        /// 减少指定指定使用者id使用的卡牌的cd指定回合数
        /// </summary>
        /// <param name="unitId">使用者id</param>
        /// <param name="amount">要减少的回合数</param>
        void ReduceSpecificCardCd(string unitId, int amount);

        /// <summary>
        /// 获得指定坐标指定范围内所有地图块的引用的List，若参数不合法则返回null
        /// </summary>
        /// <param name="cordinate">有效Vector2坐标指定中心位置</param>
        /// <param name="range">有效int数值指定范围</param>
        /// <returns>若无异常情况则返回List，若有异常则返回null</returns>
        List<BattleMapBlock> GetMapBlocksWithinRange(Vector2 cordinate, int range);

        /// <summary>
        /// 获得指定坐标指定范围内所有地图块上的所有单位引用的List，若参数不合法则返回null
        /// </summary>
        /// <param name="cordinate">有效Vector2坐标指定中心位置</param>
        /// <param name="range">有效int数值指定范围</param>
        /// <returns>若无异常情况则返回List，若有异常则返回null</returns>
        List<GameUnit.GameUnit> GetGameUnitsWithinRange(Vector2 cordinate, int range);

        
        /// <summary>
        /// 对指定的单位造成指定数额的伤害
        /// </summary>
        /// <param name="unitId">指定的单位的unitId</param>
        /// <param name="damage">要造成的伤害的数额</param>
        void CaseDamageToEnemy(string unitId, int damage);
    }
}