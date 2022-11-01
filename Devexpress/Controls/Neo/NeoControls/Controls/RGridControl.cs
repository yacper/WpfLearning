﻿using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using NeoTrader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NeoTrader.UI.Controls
{
    public class RGridControl : GridControl
    {
        // Tools 数据
        public static DependencyProperty RowToolsProperty = DependencyProperty.Register("RowTools", typeof(IEnumerable<CommandVm>), typeof(RGridControl), 
            new PropertyMetadata(null, (d,e) => 
            {
                //var rc = d as RGridControl;
                //rc!.RowTools = (IEnumerable<IRowTools>)e.NewValue;
            }));

        public IEnumerable<CommandVm> RowTools 
        {
            get => (IEnumerable<CommandVm>)GetValue(RowToolsProperty);
            set => SetValue(RowToolsProperty, value);            
        }

        public static DependencyProperty DefaultToolsProperty = DependencyProperty.Register("DefaultTools", typeof(IEnumerable<CommandVm>), typeof(RGridControl));

        public IEnumerable<CommandVm> DefaultTools 
        { 
            get => (IEnumerable<CommandVm>)GetValue(DefaultToolsProperty); 
            set => SetValue(DefaultToolsProperty, value);
        } 
        
        public static DependencyProperty ToolsBgBrushProperty = DependencyProperty.Register("ToolsBgBrush", typeof(Brush), typeof(RGridControl), new PropertyMetadata(Brushes.Blue));
        public Brush ToolsBgBrush
        {
            get => (Brush)GetValue(ToolsBgBrushProperty);
            set => SetValue(ToolsBgBrushProperty, value);
        }

        public static DependencyProperty ToolsBarIsFixedProperty = DependencyProperty.Register("ToolsBarIsFixed", typeof(bool), typeof(RGridControl), new PropertyMetadata(false));
        public bool ToolsBarIsFixed
        {
            get => (bool)GetValue(ToolsBarIsFixedProperty);
            set => SetValue(ToolsBarIsFixedProperty, value);
        }

        public RGridControl(): base()
        {
            // DefaultTools = new CommandVm();            
        }
    }
}
