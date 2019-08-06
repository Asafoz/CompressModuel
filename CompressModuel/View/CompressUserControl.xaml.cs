using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CompressModuel.Contracts.View;
using CompressModuel.Contracts.ViewModel;

namespace CompressModuel.View
{
    /// <summary>
    /// Interaction logic for CompressUserControl.xaml
    /// </summary>
    [Export(typeof(ICompressUserControl))]
    public partial class CompressUserControl : UserControl , ICompressUserControl
    {
        public event EventHandler<string> FileReadyNotify;
        public event EventHandler<Tuple<long, long>> FileCompressProgressNotify;

        [ImportingConstructor]
        public CompressUserControl([Import(AllowDefault = true)]ICompressUCViewModel viewModel)
        {
            InitializeComponent();

            this.Loaded += (s, e) =>
            {
                this.DataContext = viewModel;
                viewModel.FileReadyNotify += new EventHandler<string>(OnFileReadyNotify);
                viewModel.FileCompressProgressNotify += new EventHandler<Tuple<long, long>>(OnFileCompressProgressNotify);
            };
        }

        private void OnFileCompressProgressNotify(object sender, Tuple<long, long> e)
        {
            if (FileCompressProgressNotify != null)
            {
                FileCompressProgressNotify(this, e);
            }
        }

        private void OnFileReadyNotify(object sender, string e)
        {
            if (FileReadyNotify != null)
            {
                FileReadyNotify(this, e);
            }
        }
    }
}
