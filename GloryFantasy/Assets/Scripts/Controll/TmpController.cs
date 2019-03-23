using UnityEngine;
using System;
using MapManager;
using Random = UnityEngine.Random;
using Unit =GameUnit.GameUnit;

namespace GameControl
{
    public class TmpController: MonoBehaviour
    {
        public GameObject cursor;//存放标记的变量
        public MapManager.MapManager MapManager;
        private Vector3 coordinate;
        private Vector3 position;

        private int _MovLen = 1;
        
        public TmpController()
        {
            
            this.coordinate = new Vector3(0f, 0f, 0f);
        }

        private void Awake()
        {
            this.position = cursor.transform.position;
        }

        // 返回当前游标所指的地图坐标
        public Vector3 GetCursorCoordinate()
        {
            return this.coordinate;
        }

        private void commitChange()
        {
            //Debug.Log(string.Format("Change to {0},{1}", (int)coordinate.x, (int)coordinate.y));
            this.cursor.transform.position = this.position;
        }

        public void onClickArrow(int direction)
        {
            switch (direction)
            {
                case 0:
                    onClickUp();
                    break;
                case 1:
                    onClickDown();
                    break;
                case 2:
                    onClickLeft();
                    break;
                case 3:
                    onClickRight();
                    break;
                case 4:
                    onClickOk();
                    break;
                case 5:
                    onClickCancle();
                    break;
            }
            
        }

        public void onClickUp()
        {
            this.coordinate.y += 1;
            this.position.y += _MovLen;
            commitChange();
        }

        public void onClickDown()
        {
            this.coordinate.y -= 1;
            this.position.y -= _MovLen;
            commitChange();
        }

        public void onClickLeft()
        {
            this.coordinate.x -= 1;
            this.position.x -= _MovLen;
            commitChange();
        }

        public void onClickRight()
        {
            this.coordinate.x += 1;
            this.position.x += _MovLen;
            commitChange();
        }

        public void onClickOk()
        {
            // TODO :添加点击确定按钮事件
            //Debug.Log("Ok Cliked!");
            if (MapManager.CheckIfHasUnits(this.coordinate))
            {
                Unit unit = MapManager.GetUnitsOnMapBlock(this.coordinate)[0];
                Debug.Log("This unit belongs to :" + unit.owner);
                Debug.Log(string.Format(" {0} {1} ", unit.mapBlockBelow.x, unit.mapBlockBelow.y));
                if (unit.owner.Equals("player"))
                {
                    unit.GetComponent<ShowRange>().MarkMoveRange();
                }
            }

            Gameplay.GetInstance().gamePlayInput.HandleConfirm(this.coordinate);
        }

        public void onClickCancle()
        {
            // TODO: 添加点击取消按钮事件
            Debug.Log("Cancle clicked!");

            //下面是与指令脚本交互,输入返回
            Gameplay.GetInstance().gamePlayInput.HandleCancle();
        }
    }
}