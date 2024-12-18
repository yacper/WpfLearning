﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm;
using Neo.Api.Attributes;
using NeoTrader;
using NeoTrader.UI.ViewModels;

namespace NeoControls.View
{
    public enum TaskStatus
    {
        Waiting,            // 等待中
        Staring,            // 啟動中
        Runing,             // 任務執行中
        Ending,             // 任務結束中
        Over,               // 任務已完成
        Stoping,            // 
        Stop,
    }

    public class Task: BindableBase
    {
        public override string ToString()
        {
            return $"Task Name: {TaskName}";
        }
        public Guid TaskId { get; set; }
        [Stat]
        public string TaskName { get=>GetProperty(()=>TaskName); set=>SetProperty(()=>TaskName, value); }
        [Stat]
        public int Progress 
        { 
            get=>GetProperty(()=>Progress); 

            set 
            {
                if (value > 100 || value < 0)                
                    return;   
                
                SetProperty(() => Progress, value);
                Status = value > 0 ? TaskStatus.Runing : TaskStatus.Waiting;
                if(value == 100)
                    Status = TaskStatus.Ending;
            }
        }
        [Stat]
        public TaskStatus Status { get=>GetProperty(()=>Status); set=>SetProperty(()=>Status, value); }
        public ObservableCollection<Task> ChildTasks { get; set;}

        public Task()
        {
            Progress = 0;
            Status = TaskStatus.Waiting;            
        }

        public int StartTask()
        {
            Progress = 0;
            Status = TaskStatus.Staring;
            if(ChildTasks != null)
                ChildTasks.Select(_ => _.StartTask());
          //  Thread.Sleep(1000);
            Status = TaskStatus.Runing;
            return 1;
        }

        public int StopTask()
        {
            Status = TaskStatus.Stoping;
            if (ChildTasks != null)
                ChildTasks.Select(_ => _.StopTask());
            
            Status = TaskStatus.Stop;

            return 1;
        }

        public void AddProgress()
        {
            Progress += 10;            
        }

        public void ReduceProgress()
        {
            Progress -= 10;
        }
    }

    public class TreeListViewDemoVm: BindableBase
    {
        public ObservableCollection<Task> Tasks { get; set; }

        public string CollectionChangedInfo { get => GetProperty(() => CollectionChangedInfo); set => SetProperty(() => CollectionChangedInfo, value); }

        public ObservableCollection<CommandVm> DefaultTools { get; set; }
        public ObservableCollection<CommandVm> ChildToolsTemplate { get; set; }

        public TreeListViewDemoVm()
        {
            InitData();
        }

        private void InitData()
        {
            InitTasks();
            InitDefaultTools();
            InitChildToolsTemplate();
        }

        private void InitTasks()
        {
            Tasks = new ObservableCollection<Task>();
            for (int i = 0; i < 3; i++)
            {
                Task task = new Task() 
                {
                    TaskId = Guid.NewGuid(),
                    TaskName = $"Task {i}",
                    Progress = 0,
                    ChildTasks = new ObservableCollection<Task>()
                    {
                        new Task(){ TaskId= Guid.NewGuid(),   Progress = 0, TaskName = $"Task{i} ==> Child Task 1"},
                        new Task(){ TaskId= Guid.NewGuid(),   Progress = 0, TaskName = $"Task{i} ==> Child Task 2"},
                        //new Task(){ TaskId= Guid.NewGuid(),   Progress = 0, TaskName = $"Task{i} ==> Child Task 3"},
                        //new Task(){ TaskId= Guid.NewGuid(),   Progress = 0, TaskName = $"Task{i} ==> Child Task 4"},
                    }
                    
                };
                task.ChildTasks.CollectionChanged += ChildTasks_CollectionChanged;

                Tasks.Add(task);
            }

            Tasks.CollectionChanged += Tasks_CollectionChanged;            
        }

        private void ChildTasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CollectionChangedInfo += $"****ChildTasks**** Action:  {e.Action} --- ";
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                        CollectionChangedInfo += $"newIdx: {e.NewStartingIndex}, Data: {item} \n";
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                        CollectionChangedInfo += $"oldIdx: {e.OldStartingIndex}, Data: {item} \n";
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    CollectionChangedInfo += $"oldIdx: {e.OldStartingIndex}, Data: {e.OldItems} ---->  newIdx: {e.NewStartingIndex}, Data: {e.NewItems}\n";
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    CollectionChangedInfo += $"oldIdx: {e.OldStartingIndex}, Data: {e.OldItems[0]} ---->   newIdx: {e.NewStartingIndex}, Data: {e.NewItems[0]}\n";
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    CollectionChangedInfo += $"oldIdx: {e.OldStartingIndex}, Data: {e.OldItems} ---->   newIdx: {e.NewStartingIndex}, Data: {e.NewItems}\n";
                    break;
                default:
                    break;
            }
        }

        private void Tasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CollectionChangedInfo += $"****RootTasks**** Action:  {e.Action} --- ";
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                        CollectionChangedInfo += $"newIdx: {e.NewStartingIndex}, Data: {item} \n";
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                        CollectionChangedInfo += $"oldIdx: {e.OldStartingIndex}, Data: {item} \n";
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    CollectionChangedInfo += $"oldIdx: {e.OldStartingIndex}, Data: {e.OldItems} ---->  newIdx: {e.NewStartingIndex}, Data: {e.NewItems}\n";
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    CollectionChangedInfo += $"oldIdx: {e.OldStartingIndex}, Data: {e.OldItems[0]} ---->   newIdx: {e.NewStartingIndex}, Data: {e.NewItems[0]}\n";
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    CollectionChangedInfo += $"oldIdx: {e.OldStartingIndex}, Data: {e.OldItems} ---->   newIdx: {e.NewStartingIndex}, Data: {e.NewItems}\n";
                    break;
                default:
                    break;
            }
        }

        private void InitDefaultTools()
        {
            DefaultTools = new ObservableCollection<CommandVm>()
                {
                    new CommandVm()
                    {
                        DisplayName = "Stop",
                        Command = new DelegateCommand<FrameworkContentElement>((e) =>
                        {
                            ((e.DataContext as CommandVm).Owner as Task).StopTask();
                        })
                    }.WithPropertyBinding(T=>T.IsEnabled, S=>(S.Owner as Task).Status, (s, e) =>
                    {
                        (e.Data.Target.Source as CommandVm).IsEnabled = ((e.Data.Target.Source as CommandVm).Owner as Task).Status !=  TaskStatus.Stop;
                    } ),

                    new CommandVm()
                    {
                        DisplayName = "Start",
                        Command = new DelegateCommand<FrameworkContentElement>((e) =>
                        {
                            ((e.DataContext as CommandVm).Owner as Task).StartTask();
                        })
                    },
                    //}.WithPropertyBinding(T=>T.IsEnabled, S=>(S.Owner as Task).Status, (s, e) => 
                    //{
                    //   // (e.Data.Target.Source as CommandVm).IsEnabled = ((e.Data.Target.Source as CommandVm).Owner as Task).Status !=  TaskStatus.Runing;
                    //}),
                    new CommandVm()
                    {
                        Glyph=Images.VMore,
                        Command= new DelegateCommand(() => { }),
                        Commands = new ObservableCollection<CommandVm>()
                        {
                            new CommandVm(){ Glyph= Images.Watchlist, DisplayName="查看List", Command = new DelegateCommand(()=>{ }) },
                            new CommandVm(){ Glyph = Images.Account, DisplayName = "用户信息", Command = new DelegateCommand(()=>{ }) },
                            new CommandVm(){ Glyph = Images.Trading, DisplayName = "交易", Command = new DelegateCommand(() => { }) },
                        }
                    }
                };
        }

        private void InitChildToolsTemplate()
        {
            ChildToolsTemplate = new ObservableCollection<CommandVm>()
                {
                    new CommandVm()
                    {
                        DisplayName = "+",
                        Command = new DelegateCommand<FrameworkContentElement>((e) =>
                        {
                            ((e.DataContext as CommandVm).Owner as Task).AddProgress();
                        })
                    },
                    new CommandVm()
                    {
                        DisplayName = "-",
                        Command = new DelegateCommand<FrameworkContentElement>((e) =>
                        {
                            ((e.DataContext as CommandVm).Owner as Task).ReduceProgress();
                        })
                    },
                    //}.WithPropertyBinding(T=>T.IsEnabled, S=>(S.Owner as Task).Status, (s, e) => 
                    //{
                    //   // (e.Data.Target.Source as CommandVm).IsEnabled = ((e.Data.Target.Source as CommandVm).Owner as Task).Status !=  TaskStatus.Runing;
                    //}),
                    new CommandVm()
                    {
                        Glyph=Images.VMore,
                        Command= new DelegateCommand(() => { }),
                        Commands = new ObservableCollection<CommandVm>()
                        {
                            new CommandVm(){ Glyph= Images.Watchlist, DisplayName="查看List", Command = new DelegateCommand(()=>{ }) },
                            new CommandVm(){ Glyph = Images.Account, DisplayName = "用户信息", Command = new DelegateCommand(()=>{ }) },
                            new CommandVm(){ Glyph = Images.Trading, DisplayName = "交易", Command = new DelegateCommand(() => { }) },
                        }
                    }
                };
        }
    }
}
