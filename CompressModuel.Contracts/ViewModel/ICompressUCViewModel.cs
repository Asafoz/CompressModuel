using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressModuel.Contracts.ViewModel
{
    public interface ICompressUCViewModel
    {
        event EventHandler<string> FileReadyNotify;
        event EventHandler<Tuple<long, long>> FileCompressProgressNotify;
    }
}
