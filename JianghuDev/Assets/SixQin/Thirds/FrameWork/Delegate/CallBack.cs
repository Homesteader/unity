using UnityEngine;
using System.Collections;

public delegate void CallBack();
public delegate void CallBack<T>(T t);
public delegate void CallBack<T1, T2>(T1 t1, T2 t2);
public delegate void CallBack<T1, T2, T3>(T1 t1, T2 t2, T3 t3);
