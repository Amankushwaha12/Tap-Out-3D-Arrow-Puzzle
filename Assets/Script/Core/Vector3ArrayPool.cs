using UnityEngine;
using System.Collections.Generic;

public class Vector3ArrayPool : MonoBehaviour
{
    private Dictionary<int, Queue<Vector3[]>> _pool = new Dictionary<int, Queue<Vector3[]>>();

    public Vector3[] GetArray(int size)
    {
        if (!_pool.ContainsKey(size))
        {
            _pool[size] = new Queue<Vector3[]>();
        }

        if (_pool[size].Count > 0)
        {
            return _pool[size].Dequeue();
        }

        return new Vector3[size];
    }

    public void RecycleArray(Vector3[] array)
    {
        int size = array.Length;
        if (!_pool.ContainsKey(size))
        {
            _pool[size] = new Queue<Vector3[]>();
        }
        _pool[size].Enqueue(array);
    }
}