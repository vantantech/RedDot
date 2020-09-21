using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using CustomerCheckIn.Models;
using CustomerCheckIn.ViewModels;

namespace CustomerCheckIn.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemDetailPage : ContentPage
    {
        ItemDetailViewModel viewModel;

        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        public ItemDetailPage()
        {
            InitializeComponent();

            var item = new Employee
            {
                FirstName = "Item 1",
                LastName = "need last name"
            };

            viewModel = new ItemDetailViewModel(item);
            BindingContext = viewModel;
        }
    }
}