using ProcessAutomation.Common.DTO.Info;
using System.Collections.Generic;
using System.Linq;

namespace ProcessAutomation.Common.Service
{
    public class ThreadTaskBC
    {
        List<ThreadTaskInfo> threadTaskList = null;
        object threadTaskLock = new object();

        public ThreadTaskBC()
        {
            threadTaskList = new List<ThreadTaskInfo>();
        }

        public void AddThreadTask(int threadTaskId, int parentTaskId, int virtualThreadTaskId, int virtualParentThreadTaskId)
        {
            lock (threadTaskLock)
            {
                threadTaskList.Add(new ThreadTaskInfo(threadTaskId, parentTaskId, virtualThreadTaskId, virtualParentThreadTaskId));
            }
        }

        public ThreadTaskInfo GetThreadTaskById(int threadTaskId, bool addDeleted = true)
        {
            if (addDeleted)
                return threadTaskList.FirstOrDefault(a => a.ThreadTaskId == threadTaskId);
            else
                return threadTaskList.FirstOrDefault(a => a.ThreadTaskId == threadTaskId && !a.IsDeleted);
        }

        public ThreadTaskInfo GetThreadTaskByVirtualId(int virtualThreadTaskId, bool addDeleted = true)
        {
            if (addDeleted)
                return threadTaskList.FirstOrDefault(a => a.VirtualThreadTaskId == virtualThreadTaskId);
            else
                return threadTaskList.FirstOrDefault(a => a.VirtualThreadTaskId == virtualThreadTaskId && !a.IsDeleted);
        }

        public bool DeleteThreadTask(int threadTaskId)
        {
            bool result = false;
            ThreadTaskInfo threadTaskInfo = null;
            if ((threadTaskInfo = threadTaskList.FirstOrDefault(a => a.ThreadTaskId == threadTaskId)) != null)
            {
                //threadTaskList.Remove(threadTaskInfo);
                threadTaskInfo.IsDeleted = true;
                result = true;
            }
            return result;
        }
    }
}
