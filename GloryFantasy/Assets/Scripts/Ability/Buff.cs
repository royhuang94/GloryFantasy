﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GamePlay;
using System.IO;

namespace Ability.Buff
{
    public class BuffManager
    {
        List<Buff> BuffList = new List<Buff>();

        /// <summary>
        /// 将传入的Buff加入到管理器中管理声明周期
        /// </summary>
        /// <param name="buff"></param>
        public void AddBuff(Buff buff)
        {
            BuffList.Add(buff);
        }

        /// <summary>
        /// 将所有Buff的生命周期减去0.5，并删除生命周期为0的Buff
        /// </summary>
        public void SubtractBuffLife()
        {
            foreach (Buff buff in BuffList)
            {
                //触发buff减少cd时的效果
                buff.OnSubtractBuffLife();
                //非负生命周期
                if (!buff.isParmament)
                {
                    //减少buff持续时间
                    buff.SetLife(buff.Life - 0.5f);
                    //如果buff时间小于等于0，删除并触发删除效果
                    if (buff.Life <= 0)
                    {
                        buff.OnComplete();
                        BuffList.Remove(buff);
                        GameObject.Destroy(buff);
                    }
                }
            }
        }
    }

    public class Buff : MonoBehaviour, GameplayTool
    {
        //从异能获取到的变量集合
        protected AbilityVariable _buffVariable = new AbilityVariable();
        /// <summary>
        /// 填充变量集合
        /// </summary>
        /// <param name="otherAbilityVariable"></param>
        private void FillBuffVariable(AbilityVariable otherAbilityVariable)
        {
            //用序列化拷贝AbilityVariable
            Stream stream = GameUtility.Serializer.InstanceDataToMemory(otherAbilityVariable);
            stream.Position = 0;
            this._buffVariable = (AbilityVariable)GameUtility.Serializer.MemoryToInstanceData(stream);
        }


        /// <summary>
        /// Buff的声明周期，0.5等于半个回合，1等于自己+PC完整的一个回合
        /// </summary>
        public float Life { get; private set; }
        /// <summary>
        /// Buff的声明周期，0.5等于半个回合，1等于自己+PC完整的一个回合
        /// </summary>
        public bool isParmament { get; private set; }
        /// <summary>
        /// 仅用作增加错误信息，无实际用途
        /// </summary>
        public string BuffName = "Buff:NoDefine";

        private void Start()
        {
            //将自己加入到BuffManager
            Gameplay.Instance().buffManager.AddBuff(this);
        }

        private void OnDestroy()
        {
            //调用被删除
            this.OnDisappear();
        }

        /// <summary>
        /// 设定Buff生命周期，0.5等于半个回合，1等于自己+PC完整的一个回合
        /// </summary>
        /// <param name="Life"></param>
        public void SetLife(float life)
        {
            this.Life = life;
            if (life < 0)
                this.isParmament = true;
            else
                this.isParmament = false;
            
        }

        /// <summary>
        /// 设定Buff被赋予时要做的事情
        /// </summary>
        virtual public void InitialBuff() { }

        /// <summary>
        /// 设定buff的CD在回合结束根据规则减少时触发的效果
        /// </summary>
        virtual public void OnSubtractBuffLife() { }

        /// <summary>
        /// 设定Buff消失时要做的事情（暂时不区分被净化和达到时限的区别）
        /// </summary>
        virtual protected void OnDisappear() { }

        /// <summary>
        /// 设定Buff按照回合数读完发生的事情（eg 吟唱）
        /// </summary>
        virtual public void OnComplete() { }

        /// <summary>
        /// 对于需要使用异能变量的buff重载这个方法来进行书写。参数AbilityVariable。
        /// </summary>
        virtual public void setVariable(AbilityVariable variable)
        {
            FillBuffVariable(variable);
            InitialBuff();
        }
    }
}
