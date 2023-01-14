using AppSettings.API.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppSettings.API.Tests
{
    public class TestBase
    {
        public string TestDir = FileUtility.FindRoot() + @"\TestFiles\";
        public string OutDir = FileUtility.FindRoot() + @"\TestOutput\";
    }
}
