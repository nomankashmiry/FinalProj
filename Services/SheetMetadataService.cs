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
        private readonly IMongoCollection<SheetMetadata> _sheetMetadata;
        private readonly GridFSBucket _gridFSBucket;    
        public SheetMetadataService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDb"));
            var database = client.GetDatabase(config.GetConnectionString("DatabaseName"));
            _sheetMetadata = database.GetCollection<SheetMetadata>("Sheet_Metadata");
            _gridFSBucket = new GridFSBucket(database);
        }

        public async Task<List<SheetMetadata>> GetAsync() =>
            await _sheetMetadata.Find(item => true).ToListAsync();

        public async Task<SheetMetadata> GetAsync(string id) =>
            await _sheetMetadata.Find(item => item.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(SheetMetadata item) =>
            await _sheetMetadata.InsertOneAsync(item);

        public async Task UpdateAsync(string id, SheetMetadata itemIn) =>
            await _sheetMetadata.ReplaceOneAsync(item => item.Id == id, itemIn);

        public async Task RemoveAsync(string id) =>
            await _sheetMetadata.DeleteOneAsync(item => item.Id == id);

        public async Task<SheetMetadata> CreateFileAsync(SheetMetadata metadata, IFormFile file)
        {
            // Store the file in GridFS
            using (var stream = file.OpenReadStream())
            {
                var fileId = await _gridFSBucket.UploadFromStreamAsync(file.FileName, stream);

                // Update metadata with the GridFS file ID
                metadata.FileId = fileId.ToString();
                metadata.UploadDate = DateTime.UtcNow;

                await _sheetMetadata.InsertOneAsync(metadata);
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