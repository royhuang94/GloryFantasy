namespace Ability.Debuff
{
    public class BDisarm :Buff.Buff
    {
        public override void InitialBuff()
        {
            base.InitialBuff();
            // 暂时定为两回合
            SetLife(2f);
            
            // 这个Debuff就是让单位无法攻击
            GetComponent<GameUnit.GameUnit>().canNotAttack = true;
        }

        protected override void OnDisappear()
        {
            base.OnDisappear();
            // 取消无法攻击的设定
            GetComponent<GameUnit.GameUnit>().canNotAttack = false;
        }
    }
}