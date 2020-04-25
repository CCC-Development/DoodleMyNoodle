// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class ArrayNetSerializer_System_Byte
{
    public static int GetNetBitSize(ref System.Byte[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(UInt32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_System_Byte.GetNetBitSize(ref obj[i]);
        }
        return result;
    }

    public static void NetSerialize(ref System.Byte[] obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        writer.WriteUInt32((UInt32)obj.Length);
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_System_Byte.NetSerialize(ref obj[i], writer);
        }
    }

    public static void NetDeserialize(ref System.Byte[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new System.Byte[reader.ReadUInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_System_Byte.NetDeserialize(ref obj[i], reader);
        }
    }
}
