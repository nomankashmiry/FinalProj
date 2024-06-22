using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SheetMetadataController(SheetMetadataService sheetMetadataService, ExcelService excelService) : ControllerBase
    {
        private readonly SheetMetadataService _sheetMetadataService = sheetMetadataService;
        private readonly ExcelService _excelService = excelService;

        [HttpGet("Hello")]
        public IActionResult GetHelloMessage()
        {
            return Ok("Hello, world!");
        }

        [HttpGet("{id}", Name = "GetSheetMetadata")]
        public async Task<IActionResult> GetSheetMetadata(string id)
        {
            var item = await _sheetMetadataService.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPost("UploadExcel")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var metadata = new SheetMetadataModel
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length
            };

            var createdMetadata = await _sheetMetadataService.CreateFileAsync(metadata, file);

            return CreatedAtRoute("GetSheetMetadata", new { id = createdMetadata.Id.ToString() }, createdMetadata);
        }

        [HttpGet("DownloadExcel/{id:length(24)}")]
        public async Task<IActionResult> DownloadExcel(string id)
        {
            var metadata = await _sheetMetadataService.GetAsync(id);

            if (metadata == null)
            {
                return NotFound();
            }

            var fileStream = await _sheetMetadataService.GetFileAsync(metadata.FileId);

            return File(fileStream, metadata.ContentType, metadata.FileName);
        }

        [HttpPost("ReadExcel/{id:length(24)}")]
        public async Task<IActionResult> ReadExcel(string id)
        {
            var metadata = await _sheetMetadataService.GetAsync(id);

            if (metadata == null)
            {
                return NotFound();
            }

            var fileStream = await _sheetMetadataService.GetFileAsync(metadata.FileId);

            // Use ExcelService to read data from the Excel file
            try
            {
                var data = await _excelService.ReadExcelAsync(fileStream);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error reading Excel file: {ex.Message}");
            }
        }

        [HttpPost("ReadDataByCountries/{id:length(24)}")]
        public async Task<IActionResult> ReadDataByCountries(string id)
        {
            var metadata = await _sheetMetadataService.GetAsync(id);

            if (metadata == null)
            {
                return NotFound();
            }

            var fileStream = await _sheetMetadataService.GetFileAsync(metadata.FileId);

            // Use ExcelService to read data from the Excel file
            try
            {
                var (countries, years, datasets) = await _excelService.ReadDataExcelAsync(fileStream);

                var result = new
                {
                    Countries = string.Join(",", countries),
                    Years = string.Join(",", years),
                    Datasets = datasets
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error reading Excel file: {ex.Message}");
            }
        }
    }
}