using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ShowIssueTracker.Models;
using Microsoft.Azure.Documents;

namespace ShowIssueTracker
{
    public interface IDocumentDBRepository<T> where T : Item
    {
        Task<Document> CreateItemAsync(T item);
        Task DeleteItemAsync(string id);
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate);
        Task<Document> UpdateItemAsync(string id, T item);

       
    }
}
