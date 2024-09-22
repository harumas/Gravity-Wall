using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotater : MonoBehaviour
{
    [Header("ロールピッチの回転軸")]
    [SerializeField] private Transform pivotVertical;
    [SerializeField] private Transform pivotHorizontal;
    [SerializeField] private Transform playerPivotHorizontal, playerPivotVertical, player;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = player.rotation;
        pivotHorizontal.localEulerAngles = playerPivotHorizontal.localEulerAngles;
        pivotVertical.localEulerAngles = playerPivotVertical.localEulerAngles;
    }
}
