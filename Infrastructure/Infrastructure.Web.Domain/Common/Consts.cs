﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Web.Domain.Common
{
    public static class Consts {
        public static string DepartmentConfigKey = "DepartmentSetting";
        public static string DefaultDepartments = "[{\"key\":1,\"value\":\"中铁山桥\"},{\"key\":2, \"value\":\"中铁宝桥\"}, {\"key\":3, \"value\":\"中铁九桥\"}]";
    }
   public class ApiReturnCode
    {
        public const string Success = "SYSTEM_SUCCESS";
        public const string Fail = "failed";
    }
}
