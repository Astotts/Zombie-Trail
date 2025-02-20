using Unity.Entities;

public struct AnimationSettings : IComponentData
{
    public int IdleHash;
    public int WalkLeftHash;
    public int WalkRightHash;
    public int WalkUpHash;
    public int WalkDownHash;
}
