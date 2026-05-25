using UnityEngine;

/// <summary>
/// 传送目标点标记
/// 挂载在场景中的目标位置，供 TransitionPoint 查找
/// </summary>
public class TransitionDestination : MonoBehaviour
{
    public enum DestinationTag
    {
        ENTER, A, B, C, WoxForest, IceAndSnow, LavaDungeon, IslandPack, Mounts
    }

    public DestinationTag destinationTag;
}
