using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Core.Events
{
    public enum ApplicationEvents
    {
        OnApplication_PreInitialize,
        OnApplication_InitializeComplete,
        BeginRequest,
        EndRequest,
        Error,
        PostResolveRequestCache
    }
}
