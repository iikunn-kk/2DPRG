using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionDestination : MonoBehaviour
{
    public enum DestinationTag
    {
        ENTER, A, B, C, WoxForest, IceAndSnow, LavaDungeon, IslandPack, Mounts
    }
    public DestinationTag destinationTag;
}
