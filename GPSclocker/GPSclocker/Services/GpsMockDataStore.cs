using GPSclocker.Models;
using GPSclocker.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

public class GpsMockDataStore : IDataStore<GpsItem>
{
    readonly List<GpsItem> items;

    public GpsMockDataStore()
    {
        items = new List<GpsItem>()
        {
            new GpsItem { Id = Guid.NewGuid().ToString(), Adress="no adress", Description="This is an item description." }
        };
    }

    public async Task<bool> AddItemAsync(GpsItem item)
    {
        items.Add(item);
        // Сортировка элементов по какому-то критерию, если необходимо
        return await Task.FromResult(true);
    }

    public async Task<bool> UpdateItemAsync(GpsItem item)
    {
        var oldItem = items.FirstOrDefault(arg => arg.Id == item.Id);
        items.Remove(oldItem);
        items.Add(item);
        // Вместо сортировки можете решить добавить логику обновления

        return await Task.FromResult(true);
    }

    public async Task<bool> DeleteItemAsync(string id)
    {
        var oldItem = items.FirstOrDefault(arg => arg.Id == id);
        items.Remove(oldItem);

        return await Task.FromResult(true);
    }

    public async Task<GpsItem> GetItemAsync(string id)
    {
        return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
    }

    public async Task<IEnumerable<GpsItem>> GetItemsAsync(bool forceRefresh = false)
    {
        // Вернуть все элементы
        return await Task.FromResult(items);
    }
}
