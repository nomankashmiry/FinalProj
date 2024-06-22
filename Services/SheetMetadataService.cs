using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace backend.Services
{
    public class SheetMetadataService
    {
        private readonly IMongoCollection<SheetMetadataModel> _SheetMetadataModel;
        private readonly GridFSBucket _gridFSBucket;    
        public SheetMetadataService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDb"));
            var database = client.GetDatabase(config.GetConnectionString("DatabaseName"));
            _SheetMetadataModel = database.GetCollection<SheetMetadataModel>("Sheet_Metadata");
            _gridFSBucket = new GridFSBucket(database);
        }

        public async Task<List<SheetMetadataModel>> GetAsync() =>
            await _SheetMetadataModel.Find(item => true).ToListAsync();

        public async Task<SheetMetadataModel> GetAsync(string id) =>
            await _SheetMetadataModel.Find(item => item.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(SheetMetadataModel item) =>
            await _SheetMetadataModel.InsertOneAsync(item);

        public async Task UpdateAsync(string id, SheetMetadataModel itemIn) =>
            await _SheetMetadataModel.ReplaceOneAsync(item => item.Id == id, itemIn);

        public async Task RemoveAsync(string id) =>
            await _SheetMetadataModel.DeleteOneAsync(item => item.Id == id);

        public async Task<SheetMetadataModel> CreateFileAsync(SheetMetadataModel metadata, IFormFile file)
        {
            // Store the file in GridFS
            using (var stream = file.OpenReadStream())
            {
                var fileId = await _gridFSBucket.UploadFromStreamAsync(file.FileName, stream);

                // Update metadata with the GridFS file ID
                metadata.FileId = fileId.ToString();
                metadata.UploadDate = DateTime.UtcNow;

                await _SheetMetadataModel.InsertOneAsync(metadata);
            }

            return metadata;
        }
        
        public async Task<Stream> GetFileAsync(string fileId)
        {
            var objectId = new ObjectId(fileId);
            var stream = await _gridFSBucket.OpenDownloadStreamAsync(objectId);
            return stream;
        }
    }

}