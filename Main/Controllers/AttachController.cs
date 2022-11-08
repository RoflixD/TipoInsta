using Main.Models.Attach;
using Main.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttachController : ControllerBase
    {
        private readonly AttachService _attachService;

        public AttachController(AttachService attachService)
        {
            _attachService = attachService;
        }

        #region post methods

        [HttpPost]
        public async Task<List<MetadataModel>> UploadFiles([FromForm] List<IFormFile> files) => await _attachService.UploadFiles(files);

        #endregion

        #region get methods
        #endregion

    }
}

