using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleMap;

public class TestingEngine : UnitySingleton<TestingEngine>
{
    Vector2 scrollPosition;

    private void OnGUI()
    {
        GUILayout.Label("生成关卡：");
        #region 遭遇选择器
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

        if (GUILayout.Button("Desert_Ant_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Desert_Ant_1");
        }
        if (GUILayout.Button("Desert_Ant_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Desert_Ant_2");
        }
        if (GUILayout.Button("Desert_Ant_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Desert_Ant_3");
        }
        if (GUILayout.Button("Desert_Ant_4"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Desert_Ant_4");
        }
        if (GUILayout.Button("Desert_Gargoyle_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Desert_Gargoyle_1");
        }
        if (GUILayout.Button("Desert_Gargoyle_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Desert_Gargoyle_2");
        }
        if (GUILayout.Button("Desert_Gargoyle_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Desert_Gargoyle_3");
        }
        if (GUILayout.Button("Desert_Gargoyle_4"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Desert_Gargoyle_4");
        }
        if (GUILayout.Button("Desert_Shadow_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Desert_Shadow_1");
        }
        if (GUILayout.Button("Desert_Shadow_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Desert_Shadow_2");
        }
        if (GUILayout.Button("Desert_Shadow_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Desert_Shadow_3");
        }
        if (GUILayout.Button("Desert_Shadow_4"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Desert_Shadow_4");
        }
        if (GUILayout.Button("Desert_ShadowFire_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Desert_ShadowFire_1");
        }
        if (GUILayout.Button("Forest_Ooze_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Forest_Ooze_1");
        }
        if (GUILayout.Button("Forest_Ooze_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Forest_Ooze_2");
        }
        if (GUILayout.Button("Forest_Ooze_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Forest_Ooze_3");
        }
        if (GUILayout.Button("Forest_Ooze_4"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Forest_Ooze_4");
        }
        if (GUILayout.Button("Forest_DK_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Forest_DK_1");
        }
        if (GUILayout.Button("Forest_Hunter_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Forest_Hunter_1");
        }
        if (GUILayout.Button("Forest_Hunter_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Forest_Hunter_2");
        }
        if (GUILayout.Button("Forest_Hunter_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Forest_Hunter_3");
        }
        if (GUILayout.Button("Forest_Ooze_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Forest_Ooze_1");
        }
        if (GUILayout.Button("Forest_Ooze_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Forest_Ooze_2");
        }
        if (GUILayout.Button("Forest_Ooze_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Forest_Ooze_3");
        }
        if (GUILayout.Button("Forest_Ooze_4"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Forest_Ooze_4");
        }
        if (GUILayout.Button("Liberia_Boss_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Liberia_Boss_1");
        }
        if (GUILayout.Button("Liberia_Boss_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Liberia_Boss_2");
        }
        if (GUILayout.Button("Liberia_Boss_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Liberia_Boss_3");
        }
        if (GUILayout.Button("Liberia_Boss_4"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Liberia_Boss_4");
        }
        if (GUILayout.Button("Liberia_Reader_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Liberia_Reader_1");
        }
        if (GUILayout.Button("Liberia_Reader_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Liberia_Reader_2");
        }
        if (GUILayout.Button("Liberia_Reader_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Liberia_Reader_3");
        }
        if (GUILayout.Button("Liberia_Tentacle_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Liberia_Tentacle_1");
        }
        if (GUILayout.Button("Liberia_Tentacle_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Liberia_Tentacle_2");
        }
        if (GUILayout.Button("Liberia_Tentacle_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Liberia_Tentacle_3");
        }
        if (GUILayout.Button("Plain_Hunter_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Plain_Hunter_1");
        }
        if (GUILayout.Button("Plain_Hunter_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Plain_Hunter_2");
        }
        if (GUILayout.Button("Plain_Shadow_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Plain_Shadow_1");
        }
        if (GUILayout.Button("Plain_Shadow_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Plain_Shadow_2");
        }
        if (GUILayout.Button("Plain_Shadow_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Plain_Shadow_3");
        }
        if (GUILayout.Button("Boss_Final_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Boss_Final_1");
        }
        if (GUILayout.Button("Boss_Final_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Boss_Final_2");
        }
        GUILayout.EndScrollView();
        #endregion
        GUILayout.Label("现在输入状态为：" + GamePlay.Gameplay.Instance().gamePlayInput.InputFSM.CurrentState.ToString());
    }
}