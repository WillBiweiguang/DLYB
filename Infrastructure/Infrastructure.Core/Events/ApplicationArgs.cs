using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Core.Events
{
    public class ApplicationArgs : EventArgs
    {
        public ApplicationContextBase ApplicationContext { get; set; }
    }
}
