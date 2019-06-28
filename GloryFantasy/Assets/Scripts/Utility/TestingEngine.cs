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
            BattleMap.BattleMap.Instance().RestatInitMap("ant_1");
        }
        if (GUILayout.Button("ant_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("ant_2");
        }
        if (GUILayout.Button("ant_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("ant_3");
        }
        if (GUILayout.Button("gargoyle_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("gargoyle_1");
        }
        if (GUILayout.Button("gargoyle_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("gargoyle_2");
        }
        if (GUILayout.Button("gargoyle_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("gargoyle_3");
        }
        if (GUILayout.Button("desertshadow_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("desertshadow_1");
        }
        if (GUILayout.Button("desertshadow_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("desertshadow_2");
        }
        if (GUILayout.Button("desertshadow_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("desertshadow_3");
        }
        /*if (GUILayout.Button("Desert_ShadowFire_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Desert_ShadowFire_1");
        }*/
        if (GUILayout.Button("dk_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("dk_1");
        }
        if (GUILayout.Button("dk_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("dk_2");
        }
        /*if (GUILayout.Button("Forest_Hunter_1"))
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
        }*/
        if (GUILayout.Button("ooze_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("ooze_1");
        }
        if (GUILayout.Button("ooze_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("ooze_2");
        }
        if (GUILayout.Button("ooze_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("ooze_3");
        }
        if (GUILayout.Button("chomper_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("chomper_1");
        }
        if (GUILayout.Button("chomper_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("chomper_2");
        }
        /*
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
        }*/
        if (GUILayout.Button("reader_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("reader_1");
        }
        if (GUILayout.Button("reader_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("reader_2");
        }
        if (GUILayout.Button("reader_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("reader_3");
        }
        if (GUILayout.Button("tentacle_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("tentacle_1");
        }
        if (GUILayout.Button("tentacle_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("tentacle_2");
        }
        if (GUILayout.Button("tentacle_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("tentacle_3");
        }
        if (GUILayout.Button("hunter_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("hunter_1");
        }
        if (GUILayout.Button("hunter_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("hunter_2");
        }
        if (GUILayout.Button("planeshadow_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("planeshadow_1");
        }
        if (GUILayout.Button("planeshadow_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("planeshadow_2");
        }
        if (GUILayout.Button("planeshadow_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("planeshadow_3");
        }
        if (GUILayout.Button("Devil_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Devil_1");
        }
        if (GUILayout.Button("Devil_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("Devil_2");
        }
        if (GUILayout.Button("bumblebee_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("bumblebee_1");
        }
        if (GUILayout.Button("bumblebee_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("bumblebee_2");
        }
        if (GUILayout.Button("bumblebee_3"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("bumblebee_3");
        }
        if (GUILayout.Button("bumblebee_4"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("bumblebee_4");
        }
        if (GUILayout.Button("sandworm_1"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("sandworm_1");
        }
        if (GUILayout.Button("sandworm_2"))
        {
            BattleMap.BattleMap.Instance().RestatInitMap("sandworm_2");
        }
        GUILayout.EndScrollView();

        GUILayout.Label("现在输入状态为：" + GamePlay.Gameplay.Instance().gamePlayInput.InputFSM.CurrentState.ToString());
        GUILayout.Label("现在TargetList已选择目标有：" + GamePlay.Gameplay.Instance().gamePlayInput.InputFSM.TargetList.Count + "个");
    }
}