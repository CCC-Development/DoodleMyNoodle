using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SpawnLocationTag : IComponentData 
{
    public int OrderPosition;
}