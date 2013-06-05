using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace HotelManagement.Util
{
  public class BinaryFileResult : ActionResult
  {
      public byte[] Content { get; set; }
    public string ContentType { get; set; }
    public string FileName { get; set; }

    public override void ExecuteResult(ControllerContext context)
    {

      context.HttpContext.Response.ClearContent();
      context.HttpContext.Response.ContentType = ContentType;
      context.HttpContext.Response.AddHeader("Content-Type", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
      context.HttpContext.Response.AddHeader("content-disposition",
      "attachment; filename=" + FileName);

      
      context.HttpContext.Response.BinaryWrite(Content);
    }
  }
}