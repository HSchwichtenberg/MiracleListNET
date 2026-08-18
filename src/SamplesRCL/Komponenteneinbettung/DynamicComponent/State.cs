using ITVisions;
namespace Samples.NET6;

public class State
{
 public string Value { get; set; }

 public int ValueInt
 {
  get { return Value.ToInt32(0); }
  set { Value = value.ToString(); }
 }
}