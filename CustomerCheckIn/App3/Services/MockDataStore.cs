using CustomerCheckIn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CustomerCheckIn.Services
{
    public class MockDataStore : IDataStore<Employee>
    {
        List<Employee> items;

        public MockDataStore()
        {
            items = new List<Employee>();
            var mockItems = new List<Employee>();
       

            foreach (var item in mockItems)
            {
                items.Add(item);
            }
        }

        public async Task<bool> AddItemAsync(Employee item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Employee item)
        {
            var oldItem = items.Where((Employee arg) => arg.ID == item.ID).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Employee arg) => arg.ID.ToString() == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Employee> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.ID.ToString() == id));
        }

        public async Task<IEnumerable<Employee>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}