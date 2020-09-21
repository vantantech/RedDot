using SalonMobile.Models;
using SalonMobile.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SalonMobile.ViewModels
{
    public class CustomerViewModel:BaseViewModel
    {
        public IDataStore<Customer> EmployeeDataStore => DependencyService.Get<IDataStore<Customer>>();


    }
}
