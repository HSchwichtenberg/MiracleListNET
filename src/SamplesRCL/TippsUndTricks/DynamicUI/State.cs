using ITVisions;
namespace Samples.TippsUndTricks.DynamicUI;

public class State
{
 public string Value { get; set; }

 public int ValueInt
 {
  get { return Value.ToInt32(0); }
  set { Value = value.ToString(); }
 }
}