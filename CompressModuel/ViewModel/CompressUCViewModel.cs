using CompressModuel.Contracts.ViewModel;
using GongSolutions.Wpf.DragDrop;
using Prism.Commands;
using Prism.Mvvm;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Writers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Application = System.Windows.Application;

namespace CompressModuel.ViewModel
{
    [Export(typeof(ICompressUCViewModel))]
    class CompressUCViewModel : BindableBase , ICompressUCViewModel, GongSolutions.Wpf.DragDrop.IDropTarget
    {
        public event EventHandler<string> FileReadyNotify;
        public event EventHandler<Tuple<long, long>> FileCompressProgressNotify;

        private string m_sourceFolderLocation;
        private string m_compressFileName;

        private int m_minimumProgress;
        private int m_maximumProgress;
        private int m_valueProgress;
        private bool m_showTotalProcess;

        public DelegateCommand SourceFolderLocationCommand { get; set; }
        public DelegateCommand CompressCommand { get; set; }



        public CompressUCViewModel()
        {
            CompressCommand = new DelegateCommand(CompressExecute, CompressCanExecute);
            SourceFolderLocationCommand = new DelegateCommand(SourceFolderLocationExecute, SourceFolderLocationCanExecute);
            ValueProgress = 0;
            ShowTotalProcess = false;
        }



        public string CompressFileName
        {
            get { return m_compressFileName; }
            set
            {
                SetProperty(ref m_compressFileName, value);
                CompressCommand.RaiseCanExecuteChanged();
            }
        }

        public string SourceFolderLocation
        {
            get { return m_sourceFolderLocation; }
            set
            {
                SetProperty(ref m_sourceFolderLocation, value);
                CompressCommand.RaiseCanExecuteChanged();
            }
        }

        public int MinimumProgress
        {
            get { return m_minimumProgress; }
            set
            {
                SetProperty(ref m_minimumProgress, value);
            }
        }

        public int MaximumProgress
        {
            get { return m_maximumProgress; }
            set
            {
                SetProperty(ref m_maximumProgress, value);
            }
        }

        public int ValueProgress
        {
            get { return m_valueProgress; }
            set
            {
                SetProperty(ref m_valueProgress, value);
            }
        }

        public bool ShowTotalProcess
        {
            get { return m_showTotalProcess; }
            set
            {
                SetProperty(ref m_showTotalProcess, value);
            }
        }

        private bool SourceFolderLocationCanExecute()
        {
            return true;
        }

        private void SourceFolderLocationExecute()
        {
            string location = string.Empty;
            GetFolderLocation(ref location);
            SourceFolderLocation = location;
        }

        private bool CompressCanExecute()
        {

            //bool b;
            //b = (!string.IsNullOrEmpty(CompressFileName));
            //b = (SourceFolderLocation != null);
            //b = (File.GetAttributes(SourceFolderLocation).HasFlag(FileAttributes.Directory));
            return ((!string.IsNullOrEmpty(CompressFileName)) && (SourceFolderLocation != null) && (File.GetAttributes(SourceFolderLocation).HasFlag(FileAttributes.Directory)));
        }

        private void CompressExecute()
        {
            //IArchive rar = SharpCompress.Archives.Rar.RarArchive. Open(new FileInfo("ze.rar"), SharpCompress.Common.Options.None);
            //string directory = Path.Combine(Directory.GetCurrentDirectory(), "DATA");

            //// Calculate the total extraction size.
            //double totalSize = rar.Entries.Where(e => !e.IsDirectory).Sum(e => e.Size);
            //long completed = 0;

            //// Use the same logic for extracting each file like IArchive.WriteToDirectory extension.
            //foreach (var entry in rar.Entries.Where(e => !e.IsDirectory))
            //{
            //    entry.WriteToDirectory(directory, ExtractOptions.Overwrite);

            //    // Track each individual extraction.
            //    completed += entry.Size;
            //    var percentage = completed / totalSize;
            //    // TODO do something with percentage.
            //}
            /////////////////////////////////////////////////////////////////////
            //DirectoryInfo di = new DirectoryInfo(SourceFolderLocation);
            //using (var rar = File.OpenWrite(CompressFileName))
            //using (var rarWriter = WriterFactory.Open(rar, ArchiveType.Rar, CompressionType.Rar))
            //{
            //    HandleZipDirectories(di, rarWriter);
            //}
            /////////////////////////////////////////////////////////////////////
            ///

            DoCompress();

            //compressData.Wait();
            //DirectoryInfo di = new DirectoryInfo(SourceFolderLocation);

            //MinimumProgress = 0;
            //MaximumProgress = GetTotalNumberOfFiles(di);
            //ValueProgress = 0;

            //using (var rar = File.OpenWrite(CompressFileName))
            //using (var rarWriter = WriterFactory.Open(rar, ArchiveType.Zip, CompressionType.Deflate))
            //{
            //    HandleZipDirectories(di, @".\", rarWriter);
            //}
        }

        private void DoCompress()
        {
            Task compressData = Task.Factory.StartNew(() =>
            {
                ShowTotalProcess = true;

                DirectoryInfo di = new DirectoryInfo(SourceFolderLocation);

                MinimumProgress = 0;
                MaximumProgress = GetTotalNumberOfFiles(di);
                ValueProgress = 0;

                if (FileCompressProgressNotify != null)
                {
                    FileCompressProgressNotify(this, Tuple.Create<long, long>(MinimumProgress, MaximumProgress));
                }

                try
                {
                    using (var rar = File.OpenWrite(CompressFileName))
                    using (var rarWriter = WriterFactory.Open(rar, ArchiveType.Zip, CompressionType.Deflate))
                    {
                        HandleZipDirectories(di, @".\", rarWriter);
                    }

                }
                catch (Exception)
                {

                }

                if (FileReadyNotify != null)
                {
                    FileReadyNotify(this, CompressFileName);
                }
                ShowTotalProcess = false;
            });
        }

        private int GetTotalNumberOfFiles(DirectoryInfo di)
        {
            int nOfFiles = 0;
            foreach (var item in di.GetDirectories())
            {
                nOfFiles += GetTotalNumberOfFiles(item);
            }
            nOfFiles += GetTotalNumberOfFilesInDirectory(di);
            return nOfFiles;
        }
        private int GetTotalNumberOfFilesInDirectory(DirectoryInfo di)
        {
            return di.GetFiles().Length;
        }

        private void HandleZipDirectories(DirectoryInfo di, string strFolder , IWriter rarWriter)
        {
            foreach (var dirPath in di.GetDirectories())
            {                
                HandleZipDirectories(dirPath, strFolder + dirPath.Name + @"\" , rarWriter);                
            }
            HandleZipFiles(di, strFolder, rarWriter);
        }
        private void HandleZipFiles(DirectoryInfo di, string strFolder , IWriter rarWriter)
        {
            foreach (var filePath in di.GetFiles())
            {
                rarWriter.Write(strFolder + filePath.Name, filePath);
                ValueProgress = ValueProgress + 1;

                if (FileCompressProgressNotify != null)
                {
                    FileCompressProgressNotify(this, Tuple.Create<long, long>(ValueProgress, MaximumProgress));
                }

            }
        }



        void GongSolutions.Wpf.DragDrop.IDropTarget.DragOver(IDropInfo dropInfo)
        {
            var dragFileList = ((System.Windows.DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = dragFileList.Any(item =>
            {
                return File.GetAttributes(item).HasFlag(FileAttributes.Directory);
            }) ? System.Windows.DragDropEffects.Copy : System.Windows.DragDropEffects.None;           
        }
        void GongSolutions.Wpf.DragDrop.IDropTarget.Drop(IDropInfo dropInfo)
        {
            var dragFileList = ((System.Windows.DataObject)dropInfo.Data).GetFileDropList().Cast<string>();

            foreach (var item in dragFileList)
            {
                if (File.GetAttributes(item).HasFlag(FileAttributes.Directory))
                {
                    SourceFolderLocation = item;

                    //http://www.daveoncsharp.com/2009/09/how-to-use-temporary-files-in-csharp/

                    // Get the full name of the newly created Temporary file. 
                    // Note that the GetTempFileName() method actually creates
                    // a 0-byte file and returns the name of the created file.
                    string fileName = Path.GetTempFileName();

                    // Craete a FileInfo object to set the file's attributes
                    FileInfo fileInfo = new FileInfo(fileName);
                    CompressFileName = Path.ChangeExtension(fileInfo.FullName, "rar");

                    DoCompress();
                }

                if (File.GetAttributes(item).HasFlag(FileAttributes.Normal))
                {
                    SourceFolderLocation = item;
                    CompressFileName = item;
                    if (FileReadyNotify != null)
                    {
                        FileReadyNotify(this, SourceFolderLocation);
                    }
                }

            }
        }

        private bool GetFolderLocation(ref string strSelection)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    strSelection = fbd.SelectedPath;
                }
            }

            return true;
        }
    }
}
