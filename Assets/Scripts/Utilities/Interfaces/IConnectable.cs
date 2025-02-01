using UnityEngine;

public interface IConnectable<T> where T : class
{
    T Context { get; }

    void Connect(T obj);
    void Disconnect();
}
