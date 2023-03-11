using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;

namespace BL;

public class Env : IEnv
{
 public DateTime Now { get => DateTime.Now; }
 public string Machine { get => System.Environment.MachineName; }
 public string User { get => System.Environment.UserDomainName + @"\" + System.Environment.UserName ; }

}
