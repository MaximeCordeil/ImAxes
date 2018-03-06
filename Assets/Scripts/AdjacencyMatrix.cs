using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AdjacencyMatrix<T> {

    T[,,] data;
    int size;

    public AdjacencyMatrix(int size)
    {
        data = new T[size, size, size];
        this.size = size;
    }

    public T this[int i, int j, int k]
    {
        get
        {
            return data[i, j, k];
        }
        set
        {
            data[j, i, k] = value;
            data[i, j, k] = value;
            data[k, i, j] = value;
            data[k, j, i] = value;
            data[i, k, j] = value;
            data[j, k, i] = value;
        }
    }

    public List<T> SliceRow(int row, int depth)
    {
        // TODO: implement a fast method for slicing
        List<T> result = new List<T>();
        for (int i = 0; i <= data.GetUpperBound(0); ++i)
        {
            result.Add(data[row, i, depth]);
        }
        return result;
    }

    public void Resize(int newSize)
    {
        if (this.size < newSize)
        {
            var newArray = new T[newSize, newSize, newSize];
            var xMin = Mathf.Min(newSize, data.GetLength(0));
            var yMin = Mathf.Min(newSize, data.GetLength(1));
            var zMin = Mathf.Min(newSize, data.GetLength(2));
            for (var x = 0; x < xMin; x++)
                for (var y = 0; y < yMin; y++)
                    for (var z = 0; z < zMin; z++)
                        newArray[x, y, z] = data[x, y, z];
            data = newArray;
        }
    }

}
