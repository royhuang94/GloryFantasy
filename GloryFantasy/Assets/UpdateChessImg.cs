using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateChessImg : MonoBehaviour {

    public void OnClicked()
    {
        SpriteRenderer spr = GameObject.Find("Player2").GetComponent<SpriteRenderer>();

        Texture2D texture2d = (Texture2D)Resources.Load("BattleMapUnitAssets/敌方棋子头像"); 
        Sprite sp = Sprite.Create(texture2d, spr.sprite.textureRect, new Vector2(0.5f, 0.5f));
        spr.sprite = sp;
    }
}
