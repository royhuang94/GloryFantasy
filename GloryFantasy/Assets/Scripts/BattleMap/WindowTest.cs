using UnityEngine;

using System.Collections;
using UnityEditor;//需要使用到UnityEditor
public class WindowTest : MonoBehaviour
{
    [MenuItem("MyMenu/ReInitMap")] 
    static void ReInitMap()          
    {
        BattleMap.BattleMap.Instance().RestatInitMap("Plain_Shadow_2");
    }
}