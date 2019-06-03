namespace Ability.Debuff
{
    public class BDizziness :Buff.Buff
    {
        public override void InitialBuff()
        {
            base.InitialBuff();
            SetLife(2f);
            
            // 设置不能移动，不能攻击，就是不能行动的意思了吧
            GetComponent<GameUnit.GameUnit>().canNotAttack = true;
            GetComponent<GameUnit.GameUnit>().canNotMove = true;
        }

        protected override void OnDisappear()
        {
            base.OnDisappear();
            
            // 恢复行动力
            GetComponent<GameUnit.GameUnit>().canNotAttack = false;
            GetComponent<GameUnit.GameUnit>().canNotMove = false;
        }
    }
}