using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EDFSharp;

namespace LibEDF_CSharpCOM
{
    public class LibEDF_Com
    {
        //TODO: expose EDF read/write functions via COM interface
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
        Guid("bd8a1e95-8931-44d4-9af8-425616be3aac")]
    public interface IComClassExample
    {
        [DispId(1)]
        int AddTheseUp(int adder1, int adder2);
    }

    [ClassInterface(ClassInterfaceType.None),
        Guid("4f941303-123f-4c5f-8a50-6a6ef01e11c7")]
    public class ComClassExample : IComClassExample
    {
        // constructor - does nothing in this example
        public ComClassExample() { }

        // a method that returns an int
        public int AddTheseUp(int adder1, int adder2)
        {
            return adder1 + adder2;
        }
    }
}
