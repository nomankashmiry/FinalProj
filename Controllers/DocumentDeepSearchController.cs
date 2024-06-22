using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentDeepSearchController(DocumentDeepSearchService DocumentDSService) : ControllerBase
    {
        private readonly DocumentDeepSearchService _DocumentDSService = DocumentDSService;

        [HttpGet]
        public async Task<IActionResult> GetAll(string id)
        {
            var DocumentDSList = await _DocumentDSService.GetAllAsync();

            if (DocumentDSList == null)
            {
                return NotFound();
            }

            return Ok(DocumentDSList);
        }
    }
}