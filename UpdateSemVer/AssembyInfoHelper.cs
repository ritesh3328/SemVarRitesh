﻿using System;
using System.Collections.Generic;
using System.Text;

namespace update_semver
{
    public static class AssembyInfoHelper
    {
        public static bool TryParse(this System.Reflection.CustomAttributeData attribute, out string s)
        {
            var flag = false;
            s = attribute.ToString();
            var i = s.IndexOf('"');
            if (i >= 0) { s = s.Substring(i + 1); flag = true; }
            i = s.IndexOf('"');
            if (i >= 0) { s = s.Substring(0, i); flag = true; }
            return flag;
        }
    }
}
