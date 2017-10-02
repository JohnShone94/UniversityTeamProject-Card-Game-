using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum s_Card_Type
{
    None,
    RemoveWholeRowColumn
}

public static class cardTypeUtilities
{
    public static bool includesRemoveWholeRowColumn(s_Card_Type ct)
    {
        return (ct & s_Card_Type.RemoveWholeRowColumn)
            == s_Card_Type.RemoveWholeRowColumn;
    }
}

public enum GameState
{
    None,
    SelectionStarted,
    Animating
}