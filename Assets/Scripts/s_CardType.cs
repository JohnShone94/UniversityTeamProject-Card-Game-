using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum s_CardType
{
    None,
    RemoveWholeRowColumn
}

public static class s_Card_Utilities
{
    public static bool Includes_Remove_Whole_Row_Column(s_CardType ct)
    {
        return (ct & s_CardType.RemoveWholeRowColumn)
            == s_CardType.RemoveWholeRowColumn;
    }
}

public enum GameState
{
    None,
    SelectionStarted,
    Animating
}