using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Player/Movement", fileName = "New Player Move Stats")]
public class PlayerMoveStats : ScriptableObject
{
    [field: SerializeField] public float MoveSpeed { get; private set; }
}