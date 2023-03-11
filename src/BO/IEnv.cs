using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO;

public interface IEnv
{
 DateTime Now { get;  }
 String Machine { get;  }
 String User { get;  }
}
