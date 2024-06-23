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
            var DocumentDSList = await _DocumentDSService.GetAllAsync();

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
    }
}