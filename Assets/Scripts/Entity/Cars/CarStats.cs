using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[CreateAssetMenu(menuName = "Car/Stats", fileName = "New Car Stats")]
public class CarStats : ScriptableObject
{
    // These will show up in inspector for entering base stats
    [SerializeField] private string _carName;
    [SerializeField] private string _carDescription;
    [SerializeField] private int _health;
    [SerializeField] private int _capacity;
    [SerializeField] private float _speed;

    // Other scripts can read these but can't modify
    public string CarName => _carName;
    public string CarDescription => _carDescription;
    public int Health => _health;
    public int Capacity => _capacity;
    public float Speed => _speed;
}