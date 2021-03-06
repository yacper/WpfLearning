using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.ModuleInjection;
using DevExpress.Mvvm.ModuleInjection.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core.MvvmSample;
using DotEx.Common;
using MIFRestoreState2.ViewModels;
using MIFRestoreState2.Views;
using Module = DevExpress.Mvvm.ModuleInjection.Module;
using ModuleView = MIFRestoreState2.Views.ModuleView;

namespace MIFRestoreState2
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			ApplicationThemeHelper.UpdateApplicationThemeName();
			new Bootstrapper().Run();
		}
		protected override void OnExit(ExitEventArgs e)
		{
			ApplicationThemeHelper.SaveApplicationThemeName();
			base.OnExit(e);
		}

	}
	public class StateConfig
	{
		public string LogicalState { get; set; }
		public string VisualState { get; set; }

	}

	public class Bootstrapper
	{
		const string StateVersion = "1.4";
		public virtual void Run()
		{
			ConfigureTypeLocators();
			if (!RestoreState())
			{
				RegisterModules();
				InjectModules();
			}
			ShowMainWindow();
			ConfigureNavigation();
		}

		protected IModuleManager Manager { get { return ModuleManager.DefaultManager; } }
		protected virtual void ConfigureTypeLocators()
		{
			var mainAssembly = typeof(MainViewModel).Assembly;
			var modulesAssembly = typeof(ModuleViewModel).Assembly;
			var assemblies = new[] { mainAssembly, modulesAssembly };
			ViewModelLocator.Default = new ViewModelLocator(assemblies);
			ViewLocator.Default = new ViewLocator(assemblies);
		}
		protected virtual void RegisterModules()
		{
			Manager.Register(Regions.MainWindow, new Module(Modules.Main, MainViewModel.Create, typeof(MainView)));
			//Manager.Register(Regions.Navigation, new Module(Modules.Module1, () => new NavigationItem("Module1")));
			//Manager.Register(Regions.Navigation, new Module(Modules.Module2, () => new NavigationItem("Module2")));
			Manager.Register(Regions.Documents, new Module(Modules.Module1, () => ModuleViewModel.Create("Module1", "Module1 Content"), typeof(ModuleView)));
			Manager.Register(Regions.Documents, new Module(Modules.Module2, () => ModuleViewModel.Create("Module2", "Module2 Content"), typeof(ModuleView)));
		}
		protected virtual bool RestoreState()
		{
			StateConfig con = ("StateConfig.json").FileToJsonObj<StateConfig>();
			if (con == null)
				return false;

			var logicalInfo = LogicalInfo.Deserialize(con.LogicalState);
			foreach (var region in logicalInfo.Regions)
			{
				foreach (var regionItem in region.Items)
				{
					if (Manager.GetModule(region.RegionName, regionItem.Key) != null)
						continue;
					Manager.Register(region.RegionName, new Module(regionItem.Key, regionItem.ViewModelName, regionItem.ViewName));
				}
			}
			return Manager.Restore(con.LogicalState, con.VisualState);
		}
		protected virtual void InjectModules()
		{
			Manager.Inject(Regions.MainWindow, Modules.Main);
			//Manager.Inject(Regions.Documents, Modules.Module1);
            Manager.InjectOrNavigate(Regions.Documents, Modules.Module1);
//			Manager.Inject(Regions.Navigation, Modules.Module2);
		}
		protected virtual void ConfigureNavigation()
		{
			Manager.GetEvents(Regions.Navigation).Navigation += OnNavigation;
			Manager.GetEvents(Regions.Documents).Navigation += OnDocumentsNavigation;
		}
		protected virtual void ShowMainWindow()
		{
			App.Current.MainWindow = new MainWindow();
			App.Current.MainWindow.Show();
			App.Current.MainWindow.Closing += OnClosing;
		}
		void OnNavigation(object sender, NavigationEventArgs e)
		{
			if (e.NewViewModelKey == null) return;
			Manager.InjectOrNavigate(Regions.Documents, e.NewViewModelKey);
		}
		void OnDocumentsNavigation(object sender, NavigationEventArgs e)
		{
			Manager.Navigate(Regions.Navigation, e.NewViewModelKey);
		}
		void OnClosing(object sender, CancelEventArgs e)
		{
			string logicalState;
			string visualState;
			Manager.Save(out logicalState, out visualState);

			StateConfig con = new StateConfig();
			con.LogicalState = logicalState;
			con.VisualState = visualState;

			con.ToJsonFile("StateConfig.json");


			//Settings.Default.StateVersion = StateVersion;
			//Settings.Default.LogicalState = logicalState;
			//Settings.Default.VisualState = visualState;
			//Settings.Default.Save();
		}
	}
}
