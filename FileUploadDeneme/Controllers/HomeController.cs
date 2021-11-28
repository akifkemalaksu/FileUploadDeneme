using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FileUploadDeneme.Controllers
{
    public class HomeController : Controller
    {
        private const string FileDir = @"C:\Users\akif_\Desktop\FileDeneme";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public JsonResult AllFileLoad()
        {
            List<string> AllFiles = new List<string>();
            GetAllFiles(ref AllFiles, FileDir);
            return Json(AllFiles.Select(x =>
            {
                var fileInfo = new FileInfo(x);
                return new
                {
                    fileInfo.Name,
                    fileInfo.FullName,
                    fileInfo.Length,
                    Extension = fileInfo.Extension.Replace(".", ""),
                    fileInfo.CreationTime
                };
            }));
        }

        private void GetAllFiles(ref List<string> AllFiles, string path)
        {
            if (Directory.Exists(path))
            {
                AllFiles.AddRange(Directory.GetFiles(path));
                string[] SubDirs = Directory.GetDirectories(path);
                foreach (string subdir in SubDirs)
                    GetAllFiles(ref AllFiles, subdir);
            }
        }

        [HttpPost]
        public JsonResult FileUpload(IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var fileExtension = Path.GetExtension(fileName);
                    var fileDirectory = FileDir + @"\" + fileExtension.Replace(".", "");

                    if (!Directory.Exists(fileDirectory))
                    {
                        Directory.CreateDirectory(fileDirectory);
                    }

                    file.SaveAs(fileDirectory + @"\" + fileName);
                }

                return Json(new { result = "1" });
            }
            catch (Exception ex)
            {
                return Json(new { result = "-1", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult FileDelete(string key)
        {
            try
            {
                if (System.IO.File.Exists(key))
                {
                    System.IO.File.Delete(key);
                    return Json("");
                }
                return Json(new { error = "File is not found." });

            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public FileResult FileDownload(string key)
        {
            var fileInfo = new FileInfo(key);
            byte[] fileBytes = System.IO.File.ReadAllBytes(key);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileInfo.Name);
        }
    }
}