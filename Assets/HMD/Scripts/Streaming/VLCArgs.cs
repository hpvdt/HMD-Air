using System.Collections.Generic;
using System.Linq;
using LibVLCSharp;

namespace HMD.Scripts.Streaming
{

    public class VLCArgs
    {
        public List<string> Lines;
        public FromType FromType;

        public VLCArgs(List<string> lines, FromType fromType)
        {
            this.Lines = lines;
            this.FromType = fromType;
        }

        public string Location
        {
            get
            {
                var str = Lines[0];
                var trimmedPath = str.Trim(new[] { '"' });
                //Windows likes to copy paths with quotes but Uri does not like to open them

                return trimmedPath;
            }
        }

        public string[] Parameters
        {
            get
            {
                return Lines.Skip(1).Select(v => v.Trim()).ToArray();
            }
        }
    }
}
