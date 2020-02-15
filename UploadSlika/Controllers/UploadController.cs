using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UploadImages.Api.Data;
using UploadImages.Api.Models;

namespace UploadImages.Controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        private Context _context;

        public UploadController( Context context)
        {
            _context = context;
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> GetImages()
        {
            var slike = _context.Images.ToList();
            var list = new List<ImageModel>();
            try
            {
                foreach (var slika in slike)
                {

                    var path = slika.Path;
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(path, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    var slikaByteArray = memory.ToArray();

                    var base64String = "data:" + System.Net.Mime.MediaTypeNames.Image.Jpeg + ";base64," + System.Convert.ToBase64String(slikaByteArray);

                    var res = new ImageModel
                    {
                        Src = base64String,
                        Thumbnail = base64String,
                        Caption = slika.StartDate.HasValue && slika.EndDate.HasValue ?
                                        "Processing time: " + (slika.EndDate.Value - slika.StartDate.Value).TotalMilliseconds.ToString() + " ms" : "Processing time: 0 ms",
                        Id = slika.Id
                    };
                    list.Add(res);
                }
            }
            catch
            {
                ModelState.AddModelError("", "Error.");
            }
            return Ok(list);
        }

        [HttpPost("[action]")]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> PostImages()
        {
            try
            {
                var httpRequest = HttpContext.Request;
                if (httpRequest.Form.Files.Count > 0)
                {
                    foreach (var file in httpRequest.Form.Files)
                    {
                        var image = new Image();
                        image.StartDate = DateTime.Now;
                        var fileName = String.Format("{0}_{1}", Guid.NewGuid().ToString().Substring(0, 5), file.FileName);
                        var folderPath = "Images";
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);
                        var filePath = Path.Combine(folderPath, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        image.Name = fileName;
                        image.Path = filePath;
                        image.EndDate = DateTime.Now;
                        _context.Images.Add(image);
                    }
                    _context.SaveChanges();
                }
            }
            catch
            {
                ModelState.AddModelError("", "Upload error.");
            }
            return Ok();
        }

        [HttpDelete("[action]/{Id}")]
        public IActionResult DeleteImage(int Id)
        {
            var image = _context.Images.Where(x => x.Id == Id).FirstOrDefault();

            if (System.IO.File.Exists(image.Path))
            {
                try
                {
                    System.IO.File.Delete(image.Path);
                    _context.Images.Remove(image);
                    _context.SaveChanges();
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return Ok();
        }
    }
}
