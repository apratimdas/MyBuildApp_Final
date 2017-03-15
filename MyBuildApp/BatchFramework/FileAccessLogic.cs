
using System;
using System.IO;

namespace BatchFramework
{
    public class FileAccessLogic : IFileAccessLogic
    {
        #region Property Declarations
        private bool _verbose = false;
        private bool _recursive = false;
        private bool _skipReadOnly = false;
        private bool _forceWritable = false;
        private string _filePattern = "*.*";
        private bool _cancelled = false;
        private bool _running = false;
        #endregion

        #region Event Declarations
        public event FileAccessProcessEventHandler onProcess = null;
        public event FileAccessNotifyEventHandler onNotify = null;
        #endregion

        #region IFileAccessLogic Members

        #region Property Declarations
        public bool Verbose
        {
            get { return _verbose; }
            set
            {
                if (!this._running)
                    _verbose = value;
            }
        }
        public bool Recursive
        {
            get { return _recursive; }
            set
            {
                if (!this._running)
                    _recursive = value;
            }
        }
        public bool SkipReadOnly
        {
            get { return _skipReadOnly; }
            set
            {
                if (!this._running)
                    _skipReadOnly = value;
            }
        }

        public bool ForceWriteable
        {
            get { return _forceWritable; }
            set
            {
                if (!this._running)
                    _forceWritable = value;
            }
        }
        public string FilePattern
        {
            get { return _filePattern; }
            set
            {
                if (!this._running)
                    _filePattern = value;
            }
        }

        public bool Cancelled
        {
            get { return _cancelled; }
            set { _cancelled = value; }
        }
        #endregion

        public void Execute(string fullPath)
        {
            _cancelled = false;
            _running = true;

            if (File.Exists(fullPath))
                Process(this, new FileInfo(fullPath));
            else if (Directory.Exists(fullPath))
                ProcessDirectory(fullPath);

            _running = false;
        }

        public void Cancel()
        {
            _cancelled = true;
        }

        public void Notify(string message)
        {
            if (!_verbose)
            {
                if (this.onNotify != null)
                    this.onNotify(this, new NotifyEventArgs(message));
            }
        }
        #endregion

        #region IO Functionality
        private void ProcessDirectory(string dirpath)
        {
            ProcessDirectory(new DirectoryInfo(dirpath));
        }
        private void ProcessDirectory(DirectoryInfo info)
        {
            if (_cancelled)
                return;
            ProcessFiles(info);

            if (_recursive)
            {
                foreach (DirectoryInfo subdirInfo in info.GetDirectories())
                    ProcessDirectory(subdirInfo);
            }
        }

        private void ProcessFiles(DirectoryInfo info)
        {
            foreach (FileInfo fInfo in info.GetFiles(this._filePattern))
            {
                if (_cancelled)
                    return;
                FileAttributes attributes =
                    File.GetAttributes(fInfo.FullName);
                if ((attributes & FileAttributes.ReadOnly)
                    == FileAttributes.ReadOnly)
                {
                    if (_skipReadOnly)
                        continue;
                    if (_forceWritable)
                        File.SetAttributes(fInfo.FullName,
                            FileAttributes.Normal);
                    else
                        continue;
                }

                Process(this, fInfo);
            }
        }
        #endregion

        protected virtual void Process(IFileAccessLogic logic,
                FileInfo fInfo)
        {
            if (onProcess != null)
                onProcess(this, new ProcessEventArgs(this, fInfo));
        }

    }

}