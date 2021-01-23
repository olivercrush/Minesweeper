using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver<T>
{
    void UpdateFromObservable(T observable);
}
