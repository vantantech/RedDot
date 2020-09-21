using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using CustomerCheckIn.Views;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using CustomerCheckIn.Models;
using CustomerCheckIn.Services;

namespace CustomerCheckIn.ViewModels
{
    public class EmployeeViewModel : BaseViewModel
    {

        public IDataStore<Employee> EmployeeDataStore => DependencyService.Get<IDataStore<Employee>>();

        public ObservableCollection<Employee> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public EmployeeViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<Employee>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());


            MessagingCenter.Subscribe<NewItemPage, Employee>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as Employee;
                Items.Add(newItem);
                await EmployeeDataStore.AddItemAsync(newItem);
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await EmployeeDataStore.GetItemsAsync(true);

            

                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}