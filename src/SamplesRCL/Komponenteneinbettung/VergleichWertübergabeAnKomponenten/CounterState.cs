using System;
using System.Collections.Generic;
using System.Text;

namespace Samples.Komponenteneinbettung.VergleichWertübergabeAnKomponenten;

public class CounterState
{
 public int CurrentCount
 {
  get { return field; }
  set { field = value; NotifyStateChanged(); }
 } = 42;

 public event Action? OnChange;

 public void NotifyStateChanged() => OnChange?.Invoke();
}