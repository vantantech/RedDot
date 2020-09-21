using System;

using CustomerCheckIn.Models;

namespace CustomerCheckIn.ViewModels
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
