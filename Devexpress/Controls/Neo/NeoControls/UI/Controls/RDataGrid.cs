﻿using DevExpress.Mvvm;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using Neo.Api.Attributes;
using NeoControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using NeoTrader.UI.ViewModels;
using System.Windows.Controls.Primitives;

namespace NeoTrader.UI.Controls
{
    public class RDataGrid : GridControl
    {
        public static DependencyProperty AlwaysShowToolBarProperty = DependencyProperty.Register(nameof(AlwaysShowToolBar), typeof(bool), typeof(RDataGrid), new PropertyMetadata(false));
        public bool AlwaysShowToolBar
        {
            get => (bool)GetValue(AlwaysShowToolBarProperty);
            set => SetValue(AlwaysShowToolBarProperty, value);
        }

        public static DependencyProperty ToolCommandsTemplateProperty = DependencyProperty.Register(nameof(ToolCommandsTemplate), typeof(ObservableCollection<CommandVm>), typeof(RDataGrid));

        public ObservableCollection<CommandVm> ToolCommandsTemplate 
        { 
            get => (ObservableCollection<CommandVm>)GetValue(ToolCommandsTemplateProperty); 
            set => SetValue(ToolCommandsTemplateProperty, value);
        }

        public static DependencyProperty CellTemplateSelectorProperty = DependencyProperty.Register(nameof(CellTemplateSelector), typeof(DataTemplateSelector), typeof(RDataGrid),
            new PropertyMetadata(null, (d, e) => 
            {

            }));

        public DataTemplateSelector CellTemplateSelector
        {
            get=>(DataTemplateSelector)GetValue(CellTemplateSelectorProperty);
            set=>SetValue(CellTemplateSelectorProperty, value);
        }

        protected ObservableCollection<UI.ViewModels.CommandVm> ContextMenuVms { get; set; }           // 存储右键的VMS

        public ICommand RowControlLoadCommand { get; private set; }
        public ICommand RMouseDoubleClickCommand { get; private set; }
        public ICommand CellsControlParentPannelLoadCommand { get; private set; }
        public ICommand RMouseRightClickCommand { get; private set; }

        public RDataGrid(): base()
        {
            SelectionMode = MultiSelectMode.Row;
            ToolCommandsTemplate  = new();
            if (View == null)
                View = CreateTreeListView();

            InitCommand();
            Loaded += RGridControl_Loaded;
        }

        private void InitCommand()
        {
            InitRowControlLoadCommand();
            InitRMouseRightClickCommand();
            InitRMouseDoubleClickCommand();            
            InitCellsControlParentPannelLoadCommand();
        }

        private void RGridControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(Columns.Count() == 0 && ColumnsSource == null)            
                ColumnsSource = CreateColumns();                
            
            if(ColumnGeneratorTemplate == null && ColumnsSource != null)
                ColumnGeneratorTemplate = (DataTemplate)App.Current.FindResource("DefaultColumnTemplate");          // 指定默认Column模板


            

        }

        private RTreeListView CreateTreeListView()
        {
            var tlv = new RTreeListView();            
            tlv.ShowIndicator = false;            
            tlv.AllowDragDrop = true;

            tlv.ShowRootIndent = false;
            tlv.HighlightItemOnHover = true;            
            tlv.DropLimtEnum = TLVDragDropLimtEnum.TableView;
            tlv.ColumnSortClearMode = ColumnSortClearMode.Click;

            tlv.RowStyle = new Style(typeof(RowControl));
            tlv.RowStyle.Setters.Add(new Setter()
            {
                Property = RowControl.TemplateProperty,
                Value = App.Current.FindResource("RDataGrid_RowControlTemplate")
            });

            return tlv;
        }

        private IEnumerable<RColumnItemData> CreateColumns()
        {
            List<RColumnItemData> columns = new List<RColumnItemData>();
            Type type = (ItemsSource as IList)[0].GetType();
            foreach (var p in type.GetProperties().Where(_=> { return _.GetCustomAttribute(typeof(StatAttribute)) != null; }))
            {
                RColumnItemData rcid = new RColumnItemData(p.Name);                
                columns.Add(rcid);
            }

            return columns;
        }

        private void InitCellsControlParentPannelLoadCommand()
        {
            CellsControlParentPannelLoadCommand = new DelegateCommand<Grid>((g) =>
            {
                return;
                var rd = UiUtils.UIUtils.GetParentObject<RDataGrid>(g);
                if (rd.View is TableView)
                    return;

                DXImage dXImage = new DXImage();                
                dXImage.Source = Images.SortDsec;
                dXImage.Width = 10;
                dXImage.Height = 10;
                dXImage.Margin = new Thickness(10, 0, 0, 0);

                var border = UiUtils.UIUtils.GetChildObject<RowFitBorder>(g, typeof(RowFitBorder));

                int c = Grid.GetColumn(border);
                g.ColumnDefinitions.Insert(c, new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(30, GridUnitType.Pixel) });

                Grid.SetColumn(dXImage, c);
                g.Children.Add(dXImage);

                MultiBinding multiBinding = new MultiBinding();
                multiBinding.Bindings.Add(new Binding() 
                {
                    Path =new PropertyPath("DataContext.IsExpanded", null), 
                    RelativeSource = new RelativeSource() { Mode = RelativeSourceMode.FindAncestor, AncestorLevel = 1, AncestorType = typeof(RowControl) },
                });
                multiBinding.Bindings.Add(new Binding() 
                {
                    Path = new PropertyPath("DataContext.RowLevel", null),
                    RelativeSource = new RelativeSource() { Mode = RelativeSourceMode.FindAncestor, AncestorLevel = 1, AncestorType = typeof(RowControl) },
                });

                multiBinding.Bindings.Add(new Binding()
                {
                    Path = new PropertyPath("DataContext", null),
                    RelativeSource = new RelativeSource() { Mode = RelativeSourceMode.FindAncestor, AncestorLevel = 1, AncestorType = typeof(RowControl) },
                });

                multiBinding.Converter = new TreeListRowControlExpandedTipsVisibleConverter();

                dXImage.SetBinding(DXImage.VisibilityProperty, multiBinding);

            });
        }

        private void InitRowControlLoadCommand()
        {
            RowControlLoadCommand = new DelegateCommand<RowControl>(rc =>
            {
                TreeListRowData rd = rc.DataContext as TreeListRowData;
                if (rd == null)
                    return;

                var rdg = UiUtils.UIUtils.GetParentObject<RDataGrid>(rc);
                var toolBarControl = UiUtils.UIUtils.GetChildObject<ToolBarControl>(rc, typeof(ToolBarControl));
                
                int level = rd.Node.ActualLevel;    // level  只能 為 0/1
                var ChildToolCommandsTemplate = (rd.View as RTreeListView).ChildToolCommandsTemplate;
                if (level == 0 || ChildToolCommandsTemplate == null)
                {
                    if (rdg.ToolCommandsTemplate == null)
                        return;

                    toolBarControl.ItemsSource = rdg.ToolCommandsTemplate.Select(x => x.Clone(rd.Row));
                }
                else
                {
                    toolBarControl.ItemsSource = ChildToolCommandsTemplate.Select(x => x.Clone(rd.Row));
                }


                rd.PropertyChanged += (s, e) =>
                {
                    // System.Diagnostics.Debug.WriteLine(e.PropertyName);
                    if (e.PropertyName == nameof(RowData.Row))
                    {
                        if (rd.Row == null)
                            return;

                        int newLevel = rd.Node.ActualLevel;
                        if(newLevel == level || ChildToolCommandsTemplate == null)  // ChildToolCommandsTemplate 很少在運行是發生變化
                        {
                            var barItems = UiUtils.UIUtils.GetChildObjects<LightweightBarItemLinkControl>(rc, typeof(LightweightBarItemLinkControl));
                            barItems.ForEach(x =>
                            {
                                if (x.DataContext is CommandVm)
                                    (x.DataContext as CommandVm).SetOwner(rd.Row);
                            });

                            return;
                        }

                        if(toolBarControl.Tag == null)
                        {
                            toolBarControl.Tag = toolBarControl.ItemsSource;
                            if (newLevel == 0 )
                                toolBarControl.ItemsSource = rdg.ToolCommandsTemplate.Select(x => x.Clone(rd.Row));                            
                            else                                                            
                                toolBarControl.ItemsSource = ChildToolCommandsTemplate.Select(x => x.Clone(rd.Row));                            

                            return;
                        }

                        var vms = toolBarControl.Tag as IEnumerable<CommandVm>;
                        toolBarControl.Tag = toolBarControl.ItemsSource;
                        toolBarControl.ItemsSource = vms.Select(x => x.SetOwner(rd.Row));

                    }                    
                };
            });
        }

        private void InitRMouseDoubleClickCommand()
        {
            RMouseDoubleClickCommand = new DelegateCommand<RowControl>((rc) =>
            {
                if (!(rc.DataContext is TreeListRowData))
                    return;

                var rowData = rc.DataContext as TreeListRowData;
                rowData.Node.IsExpanded = !rowData.IsExpanded;
            });
        }

        private void InitRMouseRightClickCommand()
        {
            RMouseRightClickCommand = new DelegateCommand<RowControl>((rc) =>
            {
                if (!(rc.DataContext is TreeListRowData))
                    return;

                var rdg = UiUtils.UIUtils.GetParentObject<RDataGrid>(rc);
                var toolBarControl = UiUtils.UIUtils.GetChildObject<ToolBarControl>(rc, typeof(ToolBarControl));
                var vms = toolBarControl.ItemsSource as IEnumerable<UI.ViewModels.CommandVm>;


                PopupMenu popup = new PopupMenu();
                popup.StaysOpen = false;
                popup.PlacementTarget = rc;
                popup.Placement = PlacementMode.MousePoint;

                popup.ItemLinksSource = vms;
                popup.ItemTemplateSelector = (DataTemplateSelector)App.Current.FindResource("BarItemDataTemplateSelector");
                

                popup.IsOpen = true;

            });
        }
    }
}
