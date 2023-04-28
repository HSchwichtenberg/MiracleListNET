using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO.NonEntityClass;

public class Env : IEnv
{
    public DateTime Now { get => DateTime.Now; }
    public string Machine { get => Environment.MachineName; }
    public string User { get => Environment.UserDomainName + @"\" + Environment.UserName; }
}