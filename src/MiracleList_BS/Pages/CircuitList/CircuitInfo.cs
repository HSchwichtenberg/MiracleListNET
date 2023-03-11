using System;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace ITVisions.Blazor.Services
{
 /// <summary>
 /// Ein Circuit kann vier Zustände haben
 /// </summary>
 public enum CircuitState
 {
  Open, ConnectionDown, ConnectionUp, Closed
 }

 /// <summary>
 /// Details zu einem Circuit
 /// </summary>
 public class CircuitInfo
 {
  static int Count { get; set; } = 0;
  public int ID { get; set; }
  public CircuitState CircuitState { get; set; } = CircuitState.Open;
  public DateTime Created { get; set; } = DateTime.Now;
  public DateTime LastStateChanged { get; set; }
  public Circuit Circuit { get; set; }
  public string ClientIP { get; set; }
  public string ClientBrowser { get; set; }
  public CircuitInfo(Circuit Circuit)
  {
   ID = ++Count;
   this.Circuit = Circuit;
  }
 }
}