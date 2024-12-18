﻿using DevExpress.Xpf.Bars;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using NeoControls;
using NeoTrader.UI.ViewModels;

namespace  NeoTrader.UI.Controls
{
    public class RToolBar : ToolBarControl
    {
        public RToolBar()
        {
            // 提供默认值
            UseWholeRow             = true;
            AllowCustomizationMenu  = false;
            AllowQuickCustomization = false;
            RotateWhenVertical      = true;
            ShowDragWidget          = false;
            ItemTemplateSelector    = (DataTemplateSelector)App.Current.FindResource("BarItemDataTemplateSelector");
        }
    }

  public class BarItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BarCheckItemTemplate { get; set; }
        public DataTemplate BarButtonItemTemplate { get; set; }
        public DataTemplate BarSubItemTemplate { get; set; }
        public DataTemplate LinkItemTemplate { get; set; }
        public DataTemplate BarItemSeparatorTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null || !(item is CommandVm))
                return base.SelectTemplate(item, container);

            var vm = item as CommandVm;

            if (vm.IsSeparator)
                return BarItemSeparatorTemplate;


            if (vm.IsCheckBox)
                return BarCheckItemTemplate;

            if (vm.IsLink)
                return LinkItemTemplate;

            return vm.Commands == null ? BarButtonItemTemplate : BarSubItemTemplate;
        }
    }


public class LinkStatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is ELinkStatus))
                return null;

            var status = (ELinkStatus)value;
            switch (status)
            {
                case ELinkStatus.None:
                    break;
                case ELinkStatus.One:
                    return Brushes.AliceBlue;                    
                case ELinkStatus.Two:
                    return Brushes.Aquamarine;
                case ELinkStatus.Three:
                    return Brushes.Beige;
                case ELinkStatus.Four:
                    return Brushes.Blue;
                case ELinkStatus.Five:
                    return Brushes.BurlyWood;
                case ELinkStatus.Six:
                    return Brushes.Chocolate;
                case ELinkStatus.Seven:
                    return Brushes.Cyan;
                default:
                    break;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ELinkStatusToColorTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is ELinkStatus))
                return null;

            var status = (ELinkStatus)value;
            switch (status)
            {
                case ELinkStatus.None:
                    break;
                case ELinkStatus.One:
                    return "绿色";
                case ELinkStatus.Two:
                    return "橘绿";
                case ELinkStatus.Three:
                    return "品红";
                case ELinkStatus.Four:
                    return "蓝色";
                case ELinkStatus.Five:
                    return "褐色";
                case ELinkStatus.Six:
                    return "灰色";
                case ELinkStatus.Seven:
                    return "红色";
                default:
                    break;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RowControlToolsDataContentConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2 || !(values[0] is DevExpress.Xpf.Grid.RowData || values[0] is TreeListRowData) || !(values[1] is RDataGrid))
                return null;

            var rgc = values[1] as RDataGrid;
            //if (values[0] is DevExpress.Xpf.Grid.RowData)
            //{
            //    if (rgc!.RowTools == null || rgc!.RowTools.Count() == 0)
            //        return rgc!.DefaultTools;

            //    return rgc!.RowTools.First();
            //}

            //int level = (values[0] as TreeListRowData)!.Node.ActualLevel;
            //if (rgc!.RowTools == null || rgc!.RowTools.Count() == 0)
            //    return rgc!.DefaultTools;

            //var tools = rgc.RowTools.Where(_ => { return _.Level == level; }).FirstOrDefault();
            return rgc.ToolCommandsTemplate;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class TableRowToolsVisibleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2 || !(values[0] is bool) || !(values[1] is bool))
                return Visibility.Collapsed;

            bool IsFixed = (bool)values[0];
            bool IsMuseMove = (bool)values[1];
            if (IsFixed)
                return Visibility.Visible;

            return IsMuseMove ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RGridControlToVmValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values.Length != 3 || !(values[0] is RowData) || !(values[1] is RDataGrid) || !(values[2] is Visibility))
                return null;
            

            var rowData = (RowData)values[0];
            var rdg = (RDataGrid)values[1];
            var visibility = (Visibility)values[2];            
            if (visibility == Visibility.Hidden)            
                return null;
            
            if(rdg.View is TableView)
                return rdg.ToolCommandsTemplate.Select(_ => _.Clone(rowData.Row));

            var tlrd = rowData as TreeListRowData;
            if(tlrd.Node.ParentNode == null)  // 根
                return rdg.ToolCommandsTemplate.Select(_ => _.Clone(rowData.Row));

            //if(tlrd.Node.HasChildren || rdg.ChildToolCommandsTemplate == null)       // has  child 
            //    return rdg.ToolCommandsTemplate.Select(_ => _.Clone(rowData.Row));

            //return rdg.ChildToolCommandsTemplate.Select(_=>_.Clone(rowData.Row)); 
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToFixedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return FixedStyle.None;

            bool newVal = (bool)value;
            if (newVal)
                return FixedStyle.Left;

            return FixedStyle.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    }

    public class TreeListRowControlExpandedTipsVisibleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values.Length != 3 || !(values[0] is bool) || !(values[1] is int) || !(values[2] is RowData))
                return Visibility.Collapsed;

            bool IsExpanded = (bool)values[0];
            int level = (int)values[1];
            TreeListRowData data = (TreeListRowData)values[2];
            return (!IsExpanded && data.Node.HasChildren) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
