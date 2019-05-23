using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;

namespace Ability
{
    public class immue : Ability
    {
        Trigger trigger;
        List<Ability> immueAbilities = new List<Ability>();

        private void Awake()
        {
            //TODO 等策划更新JSON时 解注释
            //InitialAbility("immue");
        }

        private void Start()
        {

        }

        //这个技能被删除时要做反向操作
        private void OnDestroy()
        {

        }

    }
}