using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontCheck
{
    class ErrorFont
    {
        private string lineNumber;

        private string font;

        private string columnNumber;

        private string path;
        private string fileName;

        private string logFileName;

        public string LineNumber { get => lineNumber; set => lineNumber = value; }
        public string Font { get => font; set => font = value; }
        public string ColumnNumber { get => columnNumber; set => columnNumber = value; }
        public string Path { get => path; set => path = value; }
        public string FileName { get => fileName; set => fileName = value; }
        public string LogFileName { get => logFileName; set => logFileName = value; }
    }
}
