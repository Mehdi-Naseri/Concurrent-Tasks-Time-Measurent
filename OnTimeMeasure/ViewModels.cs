using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTimeMeasure
{
    //Classes
    public class Task
    {
        //public int Id { get; set; }
        public string TaskName { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public ToolConflictType ToolConflict { get; set; }
        public int Duration { get; set; }
        public Task(string TaskName, string Source, string Destination, ToolConflictType ToolConflict, int Duration)
        {
            this.TaskName = TaskName;
            this.Source = Source;
            this.Destination = Destination;
            this.ToolConflict = ToolConflict;
            this.Duration = Duration;
        }
    }

    public enum ToolConflictType
    {
        Pathchar,
        Iperf,
        H323Beacon,
        Ping
    }

    public class TaskSchedule
    {
        //public int Id { get; set; }
        public string TaskName { get; set; }
        public int StartTime { get; set; }
        public int FinishTime { get; set; }
    }

    public class TaskScheduleMLA2
    {
        //public int Id { get; set; }
        public string TaskName { get; set; }
        public int StartTime { get; set; }
        public int FinishTime { get; set; }

        public int QueueNumber { get; set; }
    }

    public class RunningTask
    {
        //public int Id { get; set; }
        public string TaskName { get; set; }
        public bool Executed { get; set; }
    }
}
