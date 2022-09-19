using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint
{
    public int CheckpointLevel;
    public bool CheckpointUnlocked;

    public Checkpoint(int checkpointLevel, bool checkpointUnlocked)
    {
        CheckpointLevel = checkpointLevel;
        CheckpointUnlocked = checkpointUnlocked;
    }
}
