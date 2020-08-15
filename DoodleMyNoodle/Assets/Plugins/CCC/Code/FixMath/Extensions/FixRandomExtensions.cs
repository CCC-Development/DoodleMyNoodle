﻿using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class FixRandomExtensions
{
    public static bool2 NextBool2(this FixRandom random)                                           => new bool2(random.NextBool(), random.NextBool());
    public static bool3 NextBool3(this FixRandom random)                                           => new bool3(random.NextBool(), random.NextBool(), random.NextBool());
    public static bool4 NextBool4(this FixRandom random)                                           => new bool4(random.NextBool(), random.NextBool(), random.NextBool(), random.NextBool());
    public static fix2 NextFixVector2(this FixRandom random)                                 => new fix2(random.NextFix64(), random.NextFix64());
    public static fix2 NextFixVector2(this FixRandom random, fix2 max)                 => new fix2(random.NextFix64(max.x), random.NextFix64(max.y));
    public static fix2 NextFixVector2(this FixRandom random, fix2 min, fix2 max) => new fix2(random.NextFix64(min.x, max.x), random.NextFix64(min.y, max.y));
    public static fix3 NextFixVector3(this FixRandom random)                                 => new fix3(random.NextFix64(), random.NextFix64(), random.NextFix64());
    public static fix3 NextFixVector3(this FixRandom random, fix3 max)                 => new fix3(random.NextFix64(max.x), random.NextFix64(max.y), random.NextFix64(max.z));
    public static fix3 NextFixVector3(this FixRandom random, fix3 min, fix3 max) => new fix3(random.NextFix64(min.x, max.x), random.NextFix64(min.y, max.y), random.NextFix64(min.z, max.z));
    public static fix4 NextFixVector4(this FixRandom random)                                 => new fix4(random.NextFix64(), random.NextFix64(), random.NextFix64(), random.NextFix64());
    public static fix4 NextFixVector4(this FixRandom random, fix4 max)                 => new fix4(random.NextFix64(max.x), random.NextFix64(max.y), random.NextFix64(max.z), random.NextFix64(max.w));
    public static fix4 NextFixVector4(this FixRandom random, fix4 min, fix4 max) => new fix4(random.NextFix64(min.x, max.x), random.NextFix64(min.y, max.y), random.NextFix64(min.z, max.z), random.NextFix64(min.w, max.w));
    public static int2 NextInt2(this FixRandom random)                                             => new int2(random.NextInt(), random.NextInt());
    public static int2 NextInt2(this FixRandom random, int2 max)                                   => new int2(random.NextInt(max.x), random.NextInt(max.y));
    public static int2 NextInt2(this FixRandom random, int2 min, int2 max)                         => new int2(random.NextInt(min.x, max.x), random.NextInt(min.y, max.y));
    public static int3 NextInt3(this FixRandom random)                                             => new int3(random.NextInt(), random.NextInt(), random.NextInt());
    public static int3 NextInt3(this FixRandom random, int3 max)                                   => new int3(random.NextInt(max.x), random.NextInt(max.y), random.NextInt(max.z));
    public static int3 NextInt3(this FixRandom random, int3 min, int3 max)                         => new int3(random.NextInt(min.x, max.x), random.NextInt(min.y, max.y), random.NextInt(min.z, max.z));
    public static int4 NextInt4(this FixRandom random)                                             => new int4(random.NextInt(), random.NextInt(), random.NextInt(), random.NextInt());
    public static int4 NextInt4(this FixRandom random, int4 max)                                   => new int4(random.NextInt(max.x), random.NextInt(max.y), random.NextInt(max.z), random.NextInt(max.w));
    public static int4 NextInt4(this FixRandom random, int4 min, int4 max)                         => new int4(random.NextInt(min.x, max.x), random.NextInt(min.y, max.y), random.NextInt(min.z, max.z), random.NextInt(min.w, max.w));
    public static uint2 NextUInt2(this FixRandom random)                                           => new uint2(random.NextUInt(), random.NextUInt());
    public static uint3 NextUInt3(this FixRandom random)                                           => new uint3(random.NextUInt(), random.NextUInt(), random.NextUInt());
    public static uint4 NextUInt4(this FixRandom random)                                           => new uint4(random.NextUInt(), random.NextUInt(), random.NextUInt(), random.NextUInt());
    
    // methods that were not yet ported from Unity.Mathematic.Random
    //public static uint2 NextUInt2(this FixRandom random, uint2 max)                                => new uint2(random.NextUInt(max.x), random.NextUInt(max.y));
    //public static uint3 NextUInt3(this FixRandom random, uint3 max)                                => new uint3(random.NextUInt(max.x), random.NextUInt(max.y), random.NextUInt(max.z));
    //public static uint4 NextUInt4(this FixRandom random, uint4 max)                                => new uint4(random.NextUInt(max.x), random.NextUInt(max.y), random.NextUInt(max.z), random.NextUInt(max.w));
    //public static uint2 NextUInt2(this FixRandom random, uint2 min, uint2 max)                     => new uint2(random.NextUInt(), random.NextUInt());
    //public static uint3 NextUInt3(this FixRandom random, uint3 min, uint3 max)                     => new uint3(random.NextUInt(), random.NextUInt());
    //public static uint4 NextUInt4(this FixRandom random, uint4 min, uint4 max)                     => new uint4(random.NextUInt(), random.NextUInt());
    //public static quaternion NextQuaternionRotation(this FixRandom random);

    /// <summary>
    /// Vector will be normalized
    /// </summary>
    public static fix2 NextFixVector2Direction(this FixRandom random)
    {
        fix angle = random.NextFix64Ratio() * fix.PiTimes2;

        return new fix2(
            fix.Cos(angle),   // x
            fix.Sin(angle));  // y
    }

    /// <summary>
    /// Vector will be normalized
    /// </summary>
    public static fix3 NextFixVector3Direction(this FixRandom random)
    {
        fix phi = random.NextFix64Ratio() * fix.PiTimes2;
        fix costheta = random.NextFix64(-1, 1);
        fix theta = fix.Acos(costheta);

        return new fix3(
            fix.Sin(theta) * fix.Cos(phi), // x
            fix.Sin(theta) * fix.Sin(phi), // y
            fix.Cos(theta));                 // z
    }

    /// <summary>
    /// Shuffle your list with simulation random
    /// </summary>
    public static List<T> Shuffle<T>(this ref FixRandom random, List<T> list)
    {
        T temp;
        for (int i = list.Count - 1; i >= 1; i--)
        {
            int chosen = random.NextInt(0, i + 1);
            if (chosen == i)
                continue;

            temp = list[chosen];
            list[chosen] = list[i];
            list[i] = temp;
        }

        return list;
    }
}
