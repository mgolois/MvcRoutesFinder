using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvcRoutesFinder
{
    class Options
    {
        [Option('o', "output", Required = false, HelpText = "CSV Output file")]
        public string CsvOutput { get; set; }

        [Option('d', "directory", Required = true, HelpText = "Directories to scan")]
        public string Directory { get; set; }
    }
}
