using System;
using System.IO;

namespace BatchFramework
{
    /// <summary>
    /// This is our main interface for file accessing.
    /// All tools are going to use this.
    /// </summary>
    public interface IFileAccessLogic
    {
        #region Property Accessors
        bool Recursive
        {
            get;
            set;
        }
        bool SkipReadOnly
        {
            get;
            set;
        }
        bool ForceWriteable
        {
            get;
            set;
        }
        string FilePattern
        {
            get;
            set;
        }
        bool Cancelled
        {
            get;
            set;
        }
        #endregion

        void Execute(string fullPath);

        void Cancel();

        void Notify(string message);

        event FileAccessProcessEventHandler onProcess;

        event FileAccessNotifyEventHandler onNotify;
    }

    public delegate void FileAccessProcessEventHandler(
        object sender, ProcessEventArgs e);

    public delegate void FileAccessNotifyEventHandler(
        object sender, NotifyEventArgs e);
    public class ProcessEventArgs : EventArgs
    {
        private IFileAccessLogic _logic;

        private FileInfo _fileInfo;

        /// <summary>
        /// Accessor for _logic
        /// </summary>
        public IFileAccessLogic Logic
        {
            get { return _logic; }
        }

        /// <summary>
        /// accessor for FileInfo
        /// </summary>
        public FileInfo FileInfo
        {
            get { return _fileInfo; }
        }

        public ProcessEventArgs(IFileAccessLogic
            logic, FileInfo fileInfo)
        {
            _logic = logic;
            _fileInfo = fileInfo;
        }
    }

    public class NotifyEventArgs : EventArgs
    {
        private string _message;
        public string Message
        {
            get { return _message; }
        }

        public NotifyEventArgs(string message)
        {
            _message = message;
        }
    }
}
