using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyParser.Parsing
{
    /// <summary>
    /// contract for all the parsers to follow
    /// </summary>
    public interface IParsing
    {
        /// <summary>
        /// Parses the provided <paramref name="args"/> to be used and stored internally for later use.
        /// </summary>
        /// <param name="args"></param>
        bool Parse( string[] args );
    }
}
