using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressModuel.Contracts.View
{

    public interface ICompressUserControl
    {
        event EventHandler<string> FileReadyNotify;
        event EventHandler<Tuple<long, long>> FileCompressProgressNotify;
    }
}
