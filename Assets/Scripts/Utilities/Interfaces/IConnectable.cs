using UnityEngine;

public interface IConnectable<T> where T : class
{
    void Connect(T obj);
    void Disconnect();
}
