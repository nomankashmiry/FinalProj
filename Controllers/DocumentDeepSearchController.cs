using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentDeepSearchController(DocumentDeepSearchService DocumentDSService) : ControllerBase
    {
        private readonly DocumentDeepSearchService _DocumentDSService = DocumentDSService;

        [HttpGet("GetAllDocuments")]
        public async Task<IActionResult> GetAllDocuments()
        {
            var DocumentDSList = await _DocumentDSService.GetAllAsync();

            if (DocumentDSList == null)
            {
                return NotFound();
            }

            return Ok(DocumentDSList);
        }

        [HttpGet("{id}", Name = "GetDocumentData")]
        public async Task<IActionResult> GetDocumentData(string id)
        {
            var DocumentDSList = await _DocumentDSService.GetAsync(id);

            if (DocumentDSList == null)
            {
                return NotFound();
            }

            return Ok(DocumentDSList);
        }

        [HttpPost("CreateDocumentData")]
        public async Task<IActionResult> CreateDocumentData([FromBody] DocumentDSModel DocumentData)
        {
             await _DocumentDSService.CreateAsync(DocumentData);

            return CreatedAtRoute("GetDocumentData", new { id = DocumentData.Id.ToString() }, DocumentData);
        }

        [HttpPost("UpdateDocumentData")]
        public async Task<IActionResult> UpdateDocumentData([FromBody] DocumentDSModel updatedDocumentData)
        {
            if (updatedDocumentData == null || string.IsNullOrEmpty(updatedDocumentData.Id.ToString()))
            {
                return BadRequest("Invalid document data.");
            }

            var existingDocument = await _DocumentDSService.GetAsync(updatedDocumentData.Id.ToString());
            if (existingDocument == null)
            {
                return NotFound("Document not found.");
            }

            await _DocumentDSService.UpdateAsync(updatedDocumentData.Id.ToString(), updatedDocumentData);

            return Ok(updatedDocumentData);
        }

    }
}