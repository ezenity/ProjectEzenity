using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ezenity.API.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

        public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
            _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider ?? throw new ArgumentNullException(nameof(fileExtensionContentTypeProvider));
        }

        [HttpGet("{fileId}")]
        public async Task<ActionResult> GetFileAsync(string fileId)
        {
            var pathToFile = "";

            if(!System.IO.File.Exists(pathToFile))
                return NotFound();

            if (!_fileExtensionContentTypeProvider.TryGetContentType(pathToFile, out var contentType))
                contentType = "application/octet-stream"; // Catch-all scenerio for files we do not know the file type

            var bytes = await System.IO.File.ReadAllBytesAsync(pathToFile);

            return File(bytes, contentType, Path.GetFileName(pathToFile));
        }

        [HttpGet("{fileId}/stream")]
        public async Task<IActionResult> GetFileStreamAsync(string fileId)
        {
            var filePath = Path.Combine("path/to/your/files/folder", fileId);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            Stream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, "application/octet-stream", true);  // FileStream supports async operations internally
        }


        [HttpPost("upload", Name = "UploadFile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No File Uploaded");

            var filePath = Path.Combine("./files", file.FileName);

            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { message = "File uploaded successfully." });
        }

        [HttpDelete("{fileId}")]
        public async Task<IActionResult> DeleteFileAsync(string fileId)
        {
            return await Task.Run<IActionResult>(() =>
            {
                var filePath = Path.Combine("path/to/your/files/folder", fileId);

                if (!System.IO.File.Exists(filePath))
                    return NotFound("File not found.");

                System.IO.File.Delete(filePath);

                return Ok(new { message = "File deleted successfully." });
            });
        }


        [HttpGet("list")]
        public async Task<IActionResult> ListFilesAsync()
        {
            return await Task.Run(() =>
            {
                var files = Directory.GetFiles("./files");
                return Ok(files);
            });
        }


    }
}
