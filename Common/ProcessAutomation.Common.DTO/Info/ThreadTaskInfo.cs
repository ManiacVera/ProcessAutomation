namespace ProcessAutomation.Common.DTO.Info
{
    public class ThreadTaskInfo
    {
        public int ThreadTaskId { get; private set; }
        public int ParentTaskId { get; private set; }
        public int VirtualThreadTaskId { get; private set; }
        public int VirtualParentThreadTaskId { get; private set; }
        public bool IsDeleted { get; set; }

        public ThreadTaskInfo(int threadTaskId, int parentTaskId, int virtualThreadTaskId, int virtualParentThreadTaskId)
        {
            this.ThreadTaskId = threadTaskId;
            this.ParentTaskId = parentTaskId;   
            this.VirtualThreadTaskId = virtualThreadTaskId; 
            this.VirtualParentThreadTaskId = virtualParentThreadTaskId;
            this.IsDeleted = false;
        }
    }
}
