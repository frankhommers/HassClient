﻿using System.Collections.Generic;

namespace HassClient.Core.Helpers
{
  internal class Map<T1, T2>
  {
    private readonly Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
    private readonly Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

    public IReadOnlyDictionary<T1, T2> Forward => _forward;

    public IReadOnlyDictionary<T2, T1> Reverse => _reverse;

    public void Add(T1 t1, T2 t2)
    {
      _forward.Add(t1, t2);
      _reverse.Add(t2, t1);
    }
  }
}