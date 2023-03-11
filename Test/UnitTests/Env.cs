using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;

namespace BL;

public class DummyEnv : IEnv
{
 DateTime _now;
 string _machine;
 string _user;
 public DummyEnv(DateTime now, string maschine, string user)
 {
  this._now = now;
  this._machine = maschine;
  this._user = user;
 }
 public DateTime Now { get => _now; }
 public string Machine { get => _machine; }
 public string User { get => _user; }

}
