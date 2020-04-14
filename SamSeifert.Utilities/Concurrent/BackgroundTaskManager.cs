using System;
using System.Threading;

namespace SamSeifert.Utilities.Concurrent {
    public delegate void BackgroundTask(BackgroundTaskManager manager);

    public interface BackgroundToMainManager {
        /// <summary>
        /// </summary>
        /// <param name="a"></param>
        /// <returns>Whether or not that task got added to main thread.  Inheritors have the right to not enqueue tasks if they've been disposed.</returns>
        bool RunOnMainThread(Action a);
    }

    public interface BackgroundTaskManager : BackgroundToMainManager {
        CancellationToken GetCancellationToken();
        bool ShouldContinue();
        void WaitForMainThreadTasks();


    }
}
