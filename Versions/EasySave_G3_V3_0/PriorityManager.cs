using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasySave_G3_V3_0
{

    public class PriorityManager
    {
        private readonly HashSet<string> _priorityExts;
        private int _pendingCount;

        // TaskCompletionSource signale la fin des fichiers prioritaires
        private readonly TaskCompletionSource<bool> _priorityDrained =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public PriorityManager(IEnumerable<string> priorityExtensions)
        {
            _priorityExts = new HashSet<string>(
                priorityExtensions.Select(e => e.StartsWith(".") ? e.ToLower() : "." + e.ToLower())
            );
        }


        public void RegisterPendingFiles(IEnumerable<string> allFilePaths)
        {
            _pendingCount = allFilePaths.Count(path =>
                _priorityExts.Contains(Path.GetExtension(path).ToLower()));
            if (_pendingCount == 0)
                _priorityDrained.TrySetResult(true);
        }


        public Task WaitIfNonPriorityAsync(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLower();
            if (!_priorityExts.Contains(ext) && Volatile.Read(ref _pendingCount) > 0)
                return _priorityDrained.Task;
            return Task.CompletedTask;
        }


        public void SignalPriorityFileDone(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLower();
            if (_priorityExts.Contains(ext) &&
                Interlocked.Decrement(ref _pendingCount) == 0)
            {
                _priorityDrained.TrySetResult(true);
            }
        }
    }
}
