using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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


namespace OnTimeMeasure
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<Task> Tasks = new List<Task>();
            //Add Sample Inputs
            Tasks.Add(new Task("T1", "S1", "S2", ToolConflictType.Pathchar, 20));
            Tasks.Add(new Task("T2", "S3", "S4", ToolConflictType.Pathchar, 15));
            Tasks.Add(new Task("T3", "S2", "S3", ToolConflictType.Pathchar, 25));
            Tasks.Add(new Task("T4", "S4", "S2", ToolConflictType.Pathchar, 5));
            Tasks.Add(new Task("T5", "S1", "S5", ToolConflictType.Pathchar, 30));
            Tasks.Add(new Task("T6", "S2", "S2", ToolConflictType.Pathchar, 14));
            Tasks.Add(new Task("T7", "S6", "S4", ToolConflictType.Pathchar, 8));
            Tasks.Add(new Task("T8", "S5", "S3", ToolConflictType.Pathchar, 12));
            Tasks.Add(new Task("T9", "S4", "S6", ToolConflictType.Pathchar, 29));
            Tasks.Add(new Task("T10", "S1", "S5", ToolConflictType.Pathchar, 11));
            dataGrid.ItemsSource = Tasks;
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabItemOutput != null && tabItemOutput.IsSelected)
            {
                CalculateOutput();
            }
        }

        private void CalculateOutput()
        {
            listBoxOutput.Items.Clear();
            StringBuilder stringBuilder = new StringBuilder();
            List<Task> Tasks = (List<Task>)dataGrid.ItemsSource;
            Tasks = Tasks.OrderByDescending(r => r.Duration).ToList();


            //================================
            //--- Round-Robin MLA=1   --------
            //================================
            List<TaskSchedule> TaskSchedulesMLA1 = new List<TaskSchedule>();
            int timeMLA1 = 0;
            foreach (Task task in Tasks)
            {
                TaskSchedule taskSchedule1 = new TaskSchedule();
                taskSchedule1.TaskName = task.TaskName;
                taskSchedule1.StartTime = timeMLA1;
                timeMLA1 += task.Duration;
                taskSchedule1.FinishTime = timeMLA1;
                TaskSchedulesMLA1.Add(taskSchedule1);
            }


            //======================================
            //-- Heuristic Bin Packing     MLA=2  --
            //======================================
            int BinSize = int.Parse(textBoxBinSize.Text);
            List<TaskScheduleMLA2> TaskSchedulesMLA2 = new List<TaskScheduleMLA2>();
            //int timeMLA2_FirstQueue_StartTime = 0;
            //int timeMLA2_FirstQueue_FinishTime = 0;
            //int timeMLA2_SecondQueue_StartTime = 0;
            //int timeMLA2_SecondQueue_FinishTime = 0;


            //TimeIndex of two Queues
            int timeMLA2_FirstQueue = 0;
            int timeMLA2_SecondQueue = 0;
            int tasksCount = Tasks.Count;
            List<Task> remainingTasks = new List<Task>(Tasks);

            while (remainingTasks.Count > 0)
            {
                bool taskRemoved = false;
                for (int i = 0; i < remainingTasks.Count; i++)
                {
                    //if (CheckBinSize(remainingTasks[i].Duration, timeMLA2_FirstQueue, timeMLA2_SecondQueue, BinSize))
                    {
                        if (timeMLA2_FirstQueue <= timeMLA2_SecondQueue)
                        {
                            //Add Task to First Queue
                            if (CheckBinSize(remainingTasks[i].Duration, timeMLA2_FirstQueue, BinSize) &&
                                !TaskConflict(remainingTasks[i], 2, timeMLA2_FirstQueue, TaskSchedulesMLA2, Tasks))
                            {
                                TaskScheduleMLA2 taskSchedule = new TaskScheduleMLA2();
                                taskSchedule.TaskName = remainingTasks[i].TaskName;
                                taskSchedule.StartTime = timeMLA2_FirstQueue;
                                timeMLA2_FirstQueue += remainingTasks[i].Duration;
                                taskSchedule.FinishTime = timeMLA2_FirstQueue;
                                taskSchedule.QueueNumber = 1;
                                TaskSchedulesMLA2.Add(taskSchedule);
                                remainingTasks.RemoveAt(i);
                                taskRemoved = true;
                                break;
                            }
                            //Add Task to Second Queue
                            else if (CheckBinSize(remainingTasks[i].Duration, timeMLA2_SecondQueue, BinSize) && 
                                !TaskConflict(remainingTasks[i], 1, timeMLA2_SecondQueue, TaskSchedulesMLA2, Tasks))
                            {
                                TaskScheduleMLA2 taskSchedule = new TaskScheduleMLA2();
                                taskSchedule.TaskName = remainingTasks[i].TaskName;
                                taskSchedule.StartTime = timeMLA2_SecondQueue;
                                timeMLA2_SecondQueue += remainingTasks[i].Duration;
                                taskSchedule.FinishTime = timeMLA2_SecondQueue;
                                taskSchedule.QueueNumber = 2;
                                TaskSchedulesMLA2.Add(taskSchedule);
                                remainingTasks.RemoveAt(i);
                                taskRemoved = true;
                                break;
                            }

                        }
                        else
                        {
                            //Add Task to Second Queue
                            if (CheckBinSize(remainingTasks[i].Duration, timeMLA2_SecondQueue, BinSize) && 
                                !TaskConflict(remainingTasks[i], 1, timeMLA2_SecondQueue, TaskSchedulesMLA2, Tasks))
                            {
                                TaskScheduleMLA2 taskSchedule = new TaskScheduleMLA2();
                                taskSchedule.TaskName = remainingTasks[i].TaskName;
                                taskSchedule.StartTime = timeMLA2_SecondQueue;
                                timeMLA2_SecondQueue += remainingTasks[i].Duration;
                                taskSchedule.FinishTime = timeMLA2_SecondQueue;
                                taskSchedule.QueueNumber = 2;
                                TaskSchedulesMLA2.Add(taskSchedule);
                                remainingTasks.RemoveAt(i);
                                taskRemoved = true;
                                break;
                            }
                            //Add Task to First Queue
                            else if (CheckBinSize(remainingTasks[i].Duration, timeMLA2_FirstQueue, BinSize) && 
                                !TaskConflict(remainingTasks[i], 2, timeMLA2_FirstQueue, TaskSchedulesMLA2, Tasks))
                            {
                                TaskScheduleMLA2 taskSchedule = new TaskScheduleMLA2();
                                taskSchedule.TaskName = remainingTasks[i].TaskName;
                                taskSchedule.StartTime = timeMLA2_FirstQueue;
                                timeMLA2_FirstQueue += remainingTasks[i].Duration;
                                taskSchedule.FinishTime = timeMLA2_FirstQueue;
                                taskSchedule.QueueNumber = 1;
                                TaskSchedulesMLA2.Add(taskSchedule);
                                remainingTasks.RemoveAt(i);
                                taskRemoved = true;
                                break;
                            }
                        }
                    }
                }
                //Go to next timeTable if there is not enought free nonConflict space in current timeTable
                if (taskRemoved == false)
                {
                    timeMLA2_FirstQueue = ((timeMLA2_FirstQueue / BinSize)+1) * BinSize;
                    timeMLA2_SecondQueue = timeMLA2_FirstQueue;
                }
            }


            //Dispaly Results
            //Round-Robin     MLA=1
            stringBuilder.AppendLine("==================================");
            stringBuilder.AppendLine("Round-Robin    MLA = 1");
            stringBuilder.AppendLine("------------------------------");
            stringBuilder.AppendLine("Cycle Time: " + timeMLA1);
            foreach (TaskSchedule taskScheduleMLA1 in TaskSchedulesMLA1)
            {
                stringBuilder.AppendLine("------------------------------");
                stringBuilder.AppendLine("Task Name: " + taskScheduleMLA1.TaskName);
                stringBuilder.AppendLine("Start Time: " + taskScheduleMLA1.StartTime);
                stringBuilder.AppendLine("Finish Time: " + taskScheduleMLA1.FinishTime);
            }
            stringBuilder.AppendLine("==================================");
            stringBuilder.AppendLine("==================================");
            stringBuilder.AppendLine("==================================");
            //Heuristic Bin Packing     MLA=2
            stringBuilder.AppendLine("==================================");
            stringBuilder.AppendLine("Heuristic Bin Packing     MLA=2");
            stringBuilder.AppendLine("------------------------------");
            if (timeMLA2_FirstQueue > timeMLA2_SecondQueue)
            {
                stringBuilder.AppendLine("Cycle Time: " + timeMLA2_FirstQueue);
            }
            else
            {
                stringBuilder.AppendLine("Cycle Time: " + timeMLA2_SecondQueue);
            }
            foreach (TaskScheduleMLA2 taskScheduleMLA2 in TaskSchedulesMLA2)
            {
                stringBuilder.AppendLine("------------------------------");
                stringBuilder.AppendLine("Task Name: " + taskScheduleMLA2.TaskName);
                stringBuilder.AppendLine("Start Time: " + taskScheduleMLA2.StartTime);
                stringBuilder.AppendLine("Finish Time: " + taskScheduleMLA2.FinishTime);
                stringBuilder.AppendLine("Queue Number: " + taskScheduleMLA2.QueueNumber);
            }
            stringBuilder.AppendLine("==================================");
            textBox1.Text = stringBuilder.ToString();
            listBoxOutput.Items.Add(stringBuilder);
        }


        //Check if there is enough free space in Bin or not
        //private bool CheckBinSize(int taskDuration, int timeMLA2_FirstQueue, int timeMLA2_SecondQueue, int binSize)
        private bool CheckBinSize(int taskDuration, int timeMLA2 ,int binSize)
        {
            //int ShortestQueue = (timeMLA2_FirstQueue <= timeMLA2_SecondQueue)
            //    ? timeMLA2_FirstQueue
            //    : timeMLA2_SecondQueue;
            if (taskDuration <= (binSize - (timeMLA2 % binSize)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Check Tasks Conflict
        private bool TaskConflict(Task task1, int QueueNumber, int Queue_FinishTime, List<TaskScheduleMLA2> TaskSchedulesMLA2, List<Task> Tasks)
        {
            int taskStartTime = Queue_FinishTime;
            int taskFinishTime = Queue_FinishTime + task1.Duration;
            foreach (TaskScheduleMLA2 taskScheduleMla2 in TaskSchedulesMLA2.FindAll(r => r.QueueNumber == QueueNumber))
            {
                //Chech Time Conflict
                if ((taskStartTime <= taskScheduleMla2.StartTime && taskFinishTime > taskScheduleMla2.StartTime) ||
                     (taskStartTime >= taskScheduleMla2.StartTime && taskStartTime < taskScheduleMla2.FinishTime))
                {
                    Task concurrentTask = Tasks.FirstOrDefault(r => r.TaskName == taskScheduleMla2.TaskName);
                    //Check Conflict Diagram
                    if (task1.Source == concurrentTask.Source ||
                        task1.Source == concurrentTask.Destination ||
                        task1.Destination == concurrentTask.Source ||
                        task1.Destination == concurrentTask.Destination)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //Create Numeric TextBox
        private void textBoxBinSize_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}



