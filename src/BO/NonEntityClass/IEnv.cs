using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO.NonEntityClass;

public interface IEnv
{
    DateTime Now { get; }
    string Machine { get; }
    string User { get; }
}
