using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieStats : MonoBehaviour
{
    [SerializeField] string zombieName;
    [SerializeField] Sprite sprite;
    [SerializeField] int health;
    [SerializeField] int damage;
    [SerializeField] int speed;

    public string ZombieName => zombieName;
    public Sprite Sprite => sprite;
    public int Health => health;
    public int Damage => damage;
    public int Speed => speed;
}
