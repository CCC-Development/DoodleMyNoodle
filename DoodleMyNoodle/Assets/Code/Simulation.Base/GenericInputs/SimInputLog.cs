﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetSerializable]
public class SimInputLog : SimInput
{
    public string message;

    public override void Execute(SimWorld world)
    {
        DebugService.Log(message);
    }
}
