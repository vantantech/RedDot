using System;

using SalonMobile.Models;

namespace SalonMobile.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public Employee Item { get; set; }
        public ItemDetailViewModel(Employee item = null)
        {
            Title = item?.DisplayName;
            Item = item;
        }
    }
}
