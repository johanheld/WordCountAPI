﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WordCountAPI.Services;

namespace WordCountAPI.Controllers
{
    [RoutePrefix("api/upload")]
    public class UploadController : ApiController
    {
        [HttpPost]
        public Task<HttpResponseMessage> PostFormData()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string path = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(path);

            var task = Request.Content.ReadAsMultipartAsync(provider).ContinueWith<HttpResponseMessage>(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                }

                string filePath = provider.FileData.First().LocalFileName;
                string result = TextProcessor.EditText(filePath, out bool success);
                System.IO.File.Delete(filePath);

                if (success)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "text/plain");
                    return response;
                }
                else
                {
                    var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    response.Content = new StringContent(result, System.Text.Encoding.UTF8, "text/plain");
                    return response;
                }
            });

            return task;
        }
    }
}