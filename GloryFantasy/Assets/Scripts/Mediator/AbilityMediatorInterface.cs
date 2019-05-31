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
        /// 获得指定坐标所在战区内所有地图快的引用的List,若参数不合法则返回null
        /// </summary>
        /// <param name="pos">有效Vector2指定需要的战区</param>
        /// <returns>若无异常情况则返回List，若有异常则返回null</returns>
        List<BattleMapBlock> GetMapBlockInSpecificBattleArea(Vector2 pos);

        /// <summary>
        /// 获得指定坐标指定范围内所有地图块上的所有单位引用的List，若参数不合法则返回null
        /// </summary>
        /// <param name="cordinate">有效Vector2坐标指定中心位置</param>
        /// <param name="range">有效int数值指定范围</param>
        /// <returns>若无异常情况则返回List，若有异常则返回null</returns>
        List<GameUnit.GameUnit> GetGameUnitsWithinRange(Vector2 cordinate, int range);


        /// <summary>
        /// 获取指定坐标所在战区的所有单位引用的List，若参数不合法则返回null
        /// </summary>
        /// <param name="pos">合法的Vector2指定需要的战区</param>
        /// <returns>若无异常情况则返回List,若有异常则返回null</returns>
        List<GameUnit.GameUnit> GetGameUnitsInBattleArea(Vector2 pos);
        
        /// <summary>
        /// 对指定的单位造成指定数额的伤害
        /// </summary>
        /// <param name="unitId">指定的单位的unitId</param>
        /// <param name="damage">要造成的伤害的数额</param>
        void CaseDamageToEnemy(string unitId, int damage);

        /// <summary>
        /// 恢复指定单位的指定血量
        /// </summary>
        /// <param name="unitId">要恢复的单位id</param>
        /// <param name="amount">血量数值</param>
        void RecoverUnitsHp(string unitId, int amount);

        /// <summary>
        /// 将指定单位从地图上抹去，也就是使之死亡
        /// </summary>
        /// <param name="unitId">要抹杀的单位</param>
        void SendUnitToDeath(string unitId);
    }
}