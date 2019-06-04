using Ability.Buff;

namespace Ability.Debuff
{
    public class BlindImmune : BBlind
    {
        public override void InitialBuff()
        {
            // 设置生命周期
            SetLife(-1f);
        }

        protected override void OnDisappear()
        {
            // 就是复写一遍，避免使用到原来的逆操作
        }
    }

    public class ViscousImmune : BViscous
    {
        public override void InitialBuff()
        {
            SetLife(-1f);
        }
        
        protected override void OnDisappear()
        {
            // 就是复写一遍，避免使用到原来的逆操作
        }
    }

    public class FiringImmune : BFiring
    {
        public override void InitialBuff()
        {
            SetLife(-1f);
        }
        
        protected override void OnDisappear()
        {
            // 就是复写一遍，避免使用到原来的逆操作
        }
    }

    public class DisarmImmune : BDisarm
    {
        public override void InitialBuff()
        {
            SetLife(-1f);
        }

        protected override void OnDisappear()
        {
            // 就是复写一遍，避免使用到原来的逆操作
        }
    }
    
    
}