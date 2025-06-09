using UnityEngine;

public interface IConsumable
{
    bool Consume(GameObject gameObject);
    bool CanConsume();
}
