using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontCheck.lib
{
    public class ProgressInfo
    {
        private int i;
        private string filePath;
        private string fileCount;

        public int I { get => i; set => i = value; }
        public string FilePath { get => filePath; set => filePath = value; }
        public string FileCount { get => fileCount; set => fileCount = value; }
    }
}
