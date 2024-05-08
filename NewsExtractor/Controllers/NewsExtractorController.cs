using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsExtractor.Models;
using NewsExtractor.Services;

namespace NewsExtractor.Controllers
{
    [Route("/")]
    [ApiController]
    public class NewsExtractorContreller : ControllerBase
    {
        private readonly INewsExtractor _context;
        public NewsExtractorContreller(INewsExtractor context)
        {
            _context = context;
        }

        [HttpGet("GetInfo")]
        public async Task<ActionResult<InfoTable>> GetInfo()
        {
            var result = await _context.InfoReturner();
            if (result.Status == false)
            {
                return Ok(result);
            }
            return Ok(result);
        }


        [HttpPost("SendInfo")]
        public async Task<ActionResult<PostMessage>> SendInfo(InfoTable info)
        {
            PostMessage pm = new();
            var result = await _context.InfoSender(info);
            if (result == null)
            {
                pm.Status = false;
                return Ok(pm);
            }
            return Ok(result);
        }

        [HttpGet("GetAllInfo")]
        public async Task<ActionResult<List<InfoTable>>> GetAllInfo()
        {
            var result = await _context.ExtractAllInfo();
            if (result == null)
            {
                return Ok(null);
            }
            return Ok(result);
        }
    }
}
