﻿using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct Velocity : IComponentData
{
    public fix3 Value;
}
