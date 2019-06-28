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
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

        if (GUILayout.Button("ant_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("ant_1", null);
        }
        if (GUILayout.Button("ant_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("ant_2", null);
        }
        if (GUILayout.Button("ant_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("ant_3", null);
        }
        if (GUILayout.Button("ant_4"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("ant_4", null);
        }
        if (GUILayout.Button("gargoyle_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("gargoyle_1", null);
        }
        if (GUILayout.Button("gargoyle_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("gargoyle_2", null);
        }
        if (GUILayout.Button("gargoyle_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("gargoyle_3", null);
        }
        if (GUILayout.Button("gargoyle_4"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("gargoyle_4", null);
        }
        if (GUILayout.Button("desertshadow_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("desertshadow_1", null);
        }
        if (GUILayout.Button("desertshadow_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("desertshadow_2", null);
        }
        if (GUILayout.Button("desertshadow_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("desertshadow_3", null);
        }
        if (GUILayout.Button("desertshadow_4"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("desertshadow_4", null);
        }
        if (GUILayout.Button("Desert_ShadowFire_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Desert_ShadowFire_1", null);
        }
        if (GUILayout.Button("dk_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("dk_1", null);
        }
        if (GUILayout.Button("dk_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("dk_2", null);
        }
        if (GUILayout.Button("Forest_Hunter_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Forest_Hunter_1", null);
        }
        if (GUILayout.Button("Forest_Hunter_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Forest_Hunter_2", null);
        }
        if (GUILayout.Button("Forest_Hunter_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Forest_Hunter_3", null);
        }
        if (GUILayout.Button("ooze_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("ooze_1", null);
        }
        if (GUILayout.Button("ooze_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("ooze_2", null);
        }
        if (GUILayout.Button("ooze_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("ooze_3", null);
        }
        if (GUILayout.Button("ooze_4"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("ooze_4", null);
        }
        if (GUILayout.Button("chomper_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("chomper_1", null);
        }
        if (GUILayout.Button("chomper_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("chomper_2", null);
        }
        if (GUILayout.Button("Liberia_Boss_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Liberia_Boss_1", null);
        }
        if (GUILayout.Button("Liberia_Boss_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Liberia_Boss_2", null);
        }
        if (GUILayout.Button("Liberia_Boss_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Liberia_Boss_3", null);
        }
        if (GUILayout.Button("Liberia_Boss_4"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Liberia_Boss_4", null);
        }
        if (GUILayout.Button("reader_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("reader_1", null);
        }
        if (GUILayout.Button("reader_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("reader_2", null);
        }
        if (GUILayout.Button("reader_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("reader_3", null);
        }
        if (GUILayout.Button("tentacle_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("tentacle_1", null);
        }
        if (GUILayout.Button("tentacle_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("tentacle_2", null);
        }
        if (GUILayout.Button("tentacle_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("tentacle_3", null);
        }
        if (GUILayout.Button("hunter_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("hunter_1", null);
        }
        if (GUILayout.Button("hunter_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("hunter_2", null);
        }
        if (GUILayout.Button("planeshadow_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("planeshadow_1", null);
        }
        if (GUILayout.Button("planeshadow_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("planeshadow_2", null);
        }
        if (GUILayout.Button("planeshadow_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("planeshadow_3", null);
        }
        if (GUILayout.Button("Devil_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Devil_1", null);
        }
        if (GUILayout.Button("Devil_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Devil_2", null);
        }
        if (GUILayout.Button("bumblebee_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("bumblebee_1", null);
        }
        if (GUILayout.Button("bumblebee_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("bumblebee_2", null);
        }
        if (GUILayout.Button("bumblebee_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("bumblebee_3", null);
        }
        if (GUILayout.Button("bumblebee_4"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("bumblebee_4", null);
        }
        if (GUILayout.Button("sandworm_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("sandworm_1", null);
        }
        if (GUILayout.Button("sandworm_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("sandworm_2", null);
        }
        GUILayout.EndScrollView();

        GUILayout.Label("现在输入状态为：" + GamePlay.Gameplay.Instance().gamePlayInput.InputFSM.CurrentState.ToString());
        GUILayout.Label("现在TargetList已选择目标有：" + GamePlay.Gameplay.Instance().gamePlayInput.InputFSM.TargetList.Count + "个");
    }
}