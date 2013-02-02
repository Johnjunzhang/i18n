using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;

namespace i18n.Core
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class PoFileWatcher : IDisposable
    {
        private static readonly TimeSpan CHECK_INTERVAL = new TimeSpan(0, 0, 2);

        public TimeSpan ChangeWindow { get; set; }
        private readonly string workingDir;
        private FileSystemWatcher fileWatcher;
        private DateTime lastChangeTime = DateTime.MaxValue;
        private readonly Thread watchThread;
        public event EventHandler<FileChangeEventArgs> OnChange;
        private readonly HashSet<string> changeList = new HashSet<string>();

        public PoFileWatcher(string workingDir)
        {
            ChangeWindow = new TimeSpan(0, 0, 2);
            this.workingDir = workingDir;
            if (!Directory.Exists(workingDir))
            {
                Directory.CreateDirectory(workingDir);
            }
            watchThread = new Thread(BeginWatch) {IsBackground = true};
        }

        public void Begin()
        {
            if (OnChange == null)
            {
                throw new ApplicationException("OnChange is empty!");
            }

            SetupWatcher();
            watchThread.Start();
            evt = new AutoResetEvent(false);
        }

        private AutoResetEvent evt;

        private void BeginWatch()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(CHECK_INTERVAL);

                    if (ExceedWindow())
                    {
                        lock (changeList)
                        {
                            OnChange(this, new FileChangeEventArgs(changeList.ToArray()));
                            Restore();
                        }
                    }

                    if (Unchanged())
                    {
                        evt.Set();
                    }
                }
            }
            catch (ThreadAbortException)
            {
                TearDownWatcher();
            }
        }

        private void Restore()
        {
            changeList.Clear();
            TearDownWatcher();
            SetupWatcher();
            lastChangeTime = DateTime.MaxValue;
        }

        private bool ExceedWindow()
        {
            return DateTime.Now - lastChangeTime >= ChangeWindow;
        }

        private bool Unchanged()
        {
            return lastChangeTime == DateTime.MaxValue;
        }

        private void TearDownWatcher()
        {
            TearDownWather(fileWatcher);
        }

        private void TearDownWather(FileSystemWatcher watcher)
        {
            watcher.Changed -= MarkChanged;
            watcher.Created -= MarkChanged;
            watcher.Error -= MarkChangedWhenError;
            watcher.Dispose();
        }

        private void SetupWatcher()
        {
            fileWatcher = SetupChangeWatcher("*.po");
            fileWatcher.EnableRaisingEvents = true;
        }

        private FileSystemWatcher SetupChangeWatcher(string filter)
        {
            var watcher = new FileSystemWatcher(workingDir, filter)
                {
                    IncludeSubdirectories = true,
                    EnableRaisingEvents = false
                };
            watcher.Changed += MarkChanged;
            watcher.Created += MarkChanged;
            watcher.Error += MarkChangedWhenError;
            return watcher;
        }

        private void MarkChanged(object sender, FileSystemEventArgs e)
        {
            evt.Reset();
            lock (changeList)
            {
                changeList.Add(e.FullPath);
            }

            lastChangeTime = DateTime.Now;
        }

        private void MarkChangedWhenError(object sender, EventArgs e)
        {
            evt.Reset();
            lastChangeTime = DateTime.Now;
        }

        public void Wait()
        {
            evt.WaitOne();
        }

        public void Dispose()
        {
            watchThread.Abort();
            watchThread.Join();
            evt.Dispose();
        }
    }

    public class FileChangeEventArgs : EventArgs
    {
        public string[] ChangeList { get; private set; }

        public FileChangeEventArgs(string[] changeList)
        {
            ChangeList = changeList;
        }
    }
}