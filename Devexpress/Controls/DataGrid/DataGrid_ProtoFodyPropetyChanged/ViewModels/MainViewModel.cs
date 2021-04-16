using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DevExpress.Mvvm.Native;

public partial class Customer : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged;
}

namespace DataGrid_ProtoFodyPropetyChanged.ViewModels
{

	


	public class MainViewModel : ViewModelBase
	{
		public MainViewModel()
		{
			ObservableCollection<Customer> people = new ObservableCollection<Customer>();
			people.Add(new Customer() { Name = "Gregory S. Price", City = "Hong Kong", Visits = 4, Salary = 1000 });
			people.Add(new Customer() { Name = "Irma R. Marshall", City = "Madrid", Visits = 2, Salary = 800 });
			people.Add(new Customer() { Name = "John C. Powell", City = "Los Angeles", Visits = 6, Salary = 900 });
			people.Add(new Customer() { Name = "Christian P. Laclair", City = "London", Visits = 11, Salary = 800 });
			people.Add(new Customer() { Name = "Karen J. Kelly", City = "Hong Kong", Visits = 8, Salary = 800 });
			people.Add(new Customer() { Name = "Brian C. Cowling", City = "Los Angeles", Visits = 5, Salary = 800 });
			people.Add(new Customer() { Name = "Thomas C. Dawson", City = "Madrid", Visits = 21, Salary = 500 });
			people.Add(new Customer() { Name = "Angel M. Wilson", City = "Los Angeles", Visits = 8, Salary = 800 });
			people.Add(new Customer() { Name = "Winston C. Smith", City = "London", Visits = 1, Salary = 800 });
			people.Add(new Customer() { Name = "Harold S. Brandes", City = "Bangkok", Visits = 3, Salary = 800 });
			people.Add(new Customer() { Name = "Michael S. Blevins", City = "Hong Kong", Visits = 4, Salary = 800 });
			people.Add(new Customer() { Name = "Jan K. Sisk", City = "Bangkok", Visits = 6, Salary = 800 });
			people.Add(new Customer() { Name = "Sidney L. Holder", City = "London", Visits = 19, Salary = 800 });
			Customers = people;


			//var cities = from c in people select c.City;
			var cities = people.Select(p => p.City);
			Cities = cities.Distinct().ToObservableCollection();
		}
		public ObservableCollection<string> Cities { get; private set; }
		public ObservableCollection<Customer> Customers { get; private set; }
	}

}