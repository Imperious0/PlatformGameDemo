using UnityEngine;

public abstract class Obstacle : MonoBehaviour, IContactable
{
    [SerializeField]
    protected bool isCauseDeath = false;
    public virtual bool isKiller() { return isCauseDeath; }
}
