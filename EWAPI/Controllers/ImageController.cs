using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EWAPI.Models;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using EWAPI.Tool;
using System.Data;

namespace EWAPI.Controllers
{
    public class Cambling
    {
      
        public int id { get; set; }
      
        public string periods { get; set; }
       
        public string awardNum { get; set; }
      
        public int awardSum { get; set; }
      
        public string awardResult { get; set; }
      
        public DateTime awardTime { get; set; }
    }
    public class ImageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        DBDapper db = null;
        private  IHostingEnvironment hostingEnv;
        public ImageController(IHostingEnvironment env)
        {
            this.hostingEnv = env;
            db = new DBDapper(env);
        }
        [HttpGet]
        [Route("Get")]
        public IActionResult Get()
        {
   
               var data = new DapperAsync(hostingEnv).GetList<Cambling>(" select * from Camblings ");
            return  Json(new { data = data.Result });
        }
        [HttpPost]
        [Route("GetImage")]
        public IActionResult GetImage(int id, string ip, string cookie)
        {
            var data = db.GetInfoList<Images>("SELECT * FROM Images where id = " + id).FirstOrDefault();
            if (data != null)
            {
                var num = db.Insert(string.Format(" insert into ImagesInfo values({0},'{1}','{2}','{3}'); ",
                            id,
                            ip,
                            cookie,
                            DateTime.Now));
                if (db.UpdateSql("  Update Images SET num+=1 where id= " + id) && num > 0)
                {
                    return Json(new { Url = data.epath });
                }
                else
                {
                    return Json(new { state = "图片信息保存失败！" });
                }
            }
            else
            {
                return Json(new { state = "图片不存在！" });
            }
        }
        [HttpPost]
        [Route("Upload")]
        public IActionResult Upload()
        {
            if (Request.ContentLength > 0)
            {
                var imgFile = Request.Form.Files[0];
                if (imgFile != null && !string.IsNullOrEmpty(imgFile.FileName))
                {
                    long size = 0;
                    string tempname = "";
                    var filename = ContentDispositionHeaderValue
                                    .Parse(imgFile.ContentDisposition)
                                    .FileName
                                    .Trim('"');
                    var extname = filename.Substring(filename.LastIndexOf("."), filename.Length - filename.LastIndexOf("."));
                    var filename1 = System.Guid.NewGuid().ToString().Substring(0, 6) + extname;
                    tempname = filename1;
                    var path = hostingEnv.WebRootPath;
                    string dir = DateTime.Now.ToString("yyyyMMdd");
                    if (!System.IO.Directory.Exists(hostingEnv.WebRootPath + $@"\upload\{dir}"))
                    {
                        System.IO.Directory.CreateDirectory(hostingEnv.WebRootPath + $@"\upload\{dir}");
                    }
                    filename = hostingEnv.WebRootPath + $@"\upload\{dir}\{filename1}";
                    size += imgFile.Length;
                    using (FileStream fs = System.IO.File.Create(filename))
                    {
                        imgFile.CopyTo(fs);
                        fs.Flush();
                    }
                    var num = db.Insert(string.Format(" insert into Images values('{0}','{1}','{2}','{3}',{4}) ",
                        Time.GetTimeStamp(),
                        filename1,
                        $"/upload/{dir}/{filename1}",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        -1));
                    if (num > 0)
                    {
                        if (db.UpdateSql("  Update Images SET num+=1 where id= " + num))
                        {
                            return Json(new { state = GetStateMessage(UploadState.Success), url = $"/upload/{dir}/{filename1}", id = num });
                        }
                    }
                    else
                    {
                        return Json(new { state = GetStateMessage(UploadState.Unknown) });
                    }
                }
                return Json(new { state = GetStateMessage(UploadState.FileAccessError) });
            }
            return Json(new { state = GetStateMessage(UploadState.NetworkError) });
        }
        private string GetStateMessage(UploadState state)
        {
            switch (state)
            {
                case UploadState.Success:
                    return "上传成功！";
                case UploadState.FileAccessError:
                    return "上传失败！";
                case UploadState.Unknown:
                    return "文件路径保存失败！";
                case UploadState.NetworkError:
                    return "请选择要上传的文件！";
            }
            return "未知错误";
        }
    }
    public enum UploadState
    {
        Success = 0,
        SizeLimitExceed = -1,
        TypeNotAllow = -2,
        FileAccessError = -3,
        NetworkError = -4,
        Unknown = 1,
    }
}

