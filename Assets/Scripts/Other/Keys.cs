using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keys
{
    public enum Animation
    {
        Idle,
        Walking,
        Running,
        CarryingIdle,
        CarryingWalking,
        CarryingRunning,
        Opening
    }

    public enum PlayerPrefs
    {
        Cash,
        PlayerMaxCountOnHands,
        PlayerLevel,
        GetCash,
        ProgressIndex,
        UnderProgressIndex,
    }

    public enum AIPrefs
    {
        HelperMaxCountOnHands,
    }
}
