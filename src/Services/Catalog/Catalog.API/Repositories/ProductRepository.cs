using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
  public class ProductRepository : IProductRepository
  {
    private readonly ICatalogContext _catalogContext;

    public ProductRepository(ICatalogContext catalogContext)
    {
      this._catalogContext = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
    }

    public async Task<Product> GetProductById(string id) =>
      await _catalogContext.Products.Find(p => p.Id == id).FirstOrDefaultAsync();

    public async Task<IEnumerable<Product>> GetProducts() =>
      await _catalogContext.Products.Find(p => true).ToListAsync();

    public async Task<IEnumerable<Product>> GetProductsByCategory(string categoryName)
    {
      var filter = Builders<Product>.Filter.Eq(p => p.Category, categoryName);

      return await _catalogContext.Products.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByName(string name)
    {
      var filter = Builders<Product>.Filter.ElemMatch(p => p.Name, name);

      return await _catalogContext.Products.Find(filter).ToListAsync();
    }
    

    public async Task CreateProduct(Product product) => await _catalogContext.Products.InsertOneAsync(product);
    public async Task<bool> DeleteProduct(string id)
    {
      var deleteResult = await _catalogContext.Products.DeleteOneAsync(filter: p => p.Id == id);

      return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0; 
    }
    public async Task<bool> UpdateProduct(Product product)
    {
     var updateResult = await _catalogContext.Products.ReplaceOneAsync(filter: g => g.Id == product.Id, replacement: product);
      return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
    }
  }
}
