using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using MongoDB.Driver;

namespace backend.Services
{
    public class DocumentDeepSearchService
    {
         private readonly IMongoCollection<DocumentDSModel> _DocumentDSModel;
        public DocumentDeepSearchService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDb"));
            var database = client.GetDatabase(config.GetConnectionString("DatabaseName"));
            _DocumentDSModel = database.GetCollection<DocumentDSModel>("DcoumentDS");
        }

        public async Task<List<DocumentDSModel>> GetAllAsync() => 
            await _DocumentDSModel.Find(item => true).ToListAsync();

        public async Task<DocumentDSModel> GetAsync(string id) => 
            await _DocumentDSModel.Find(item => item.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(DocumentDSModel item) =>
            await _DocumentDSModel.InsertOneAsync(item);

        public async Task UpdateAsync(string id, DocumentDSModel itemIn) =>
            await _DocumentDSModel.ReplaceOneAsync(item => item.Id == id, itemIn);

        public async Task RemoveAsync(string id) =>
            await _DocumentDSModel.DeleteOneAsync(item => item.Id == id);
    }
}