using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField] public PlayerInventoriesSO Inventory { get; private set; }
    [field: SerializeField] public PlayerMovement Movement { get; private set; }
    [field: SerializeField] public SpriteRenderer ItemOnHandSprite { get; private set; }

    public Stat Strength { get; private set; } // Carry more weight, melee harder
    public Stat Endurance { get; private set; } // More health, Run for longer
    public Stat Speed { get; private set; } // Walk faster, pickup faster, do things faster
}