using CustomerCheckIn.Models;
using CustomerCheckIn.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CustomerCheckIn.ViewModels
{
    public class CustomerViewModel:BaseViewModel
    {
        public IDataStore<Customer> EmployeeDataStore => DependencyService.Get<IDataStore<Customer>>();


    }
}
