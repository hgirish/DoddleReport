using DoddleReport.Configuration;
using DoddleReport.iTextSharp;
using DoddleReport.OpenXml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DoddleReport.Sample.AspnetCore
{
    public class ReportResult : ActionResult
    {
        private readonly Report _report;
        private IReportWriter _writer;
        private readonly string _contentType;

        /// <summary>
        /// This property is optional. 
        /// If you don't specify a FileName then the name of the ActionResult being executed will be used. 
        /// If you do specify a FileName, you may omit the file extension. If the file extension is omitted then DoddleReport will attempt to get the extension from the URL being requested
        /// </summary>
        public string FileName { get; set; }
        private DoddleReportConfiguration ReportConfiguration;

        public ReportResult(Report report, DoddleReportConfiguration config) : this(report, report.Writer)
        {
            ReportConfiguration = config;
        }

        public ReportResult(Report report, IReportWriter writer, string contentType = null)
        {
            _report = report;
            _writer = writer;
            _contentType = contentType;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            string defaultWriter =
                ReportConfiguration.DefaultWriter;
            string defaultExtension =
                ReportConfiguration.Writers.WriterElements
                .FirstOrDefault(x => x.Format == defaultWriter)
                .FileExtension;
                

            var response = context.HttpContext.Response;

            if (_writer == null)
            {
                var writerConfig = GetWriterFromExtension(context, defaultExtension);
                response.ContentType = writerConfig.ContentType;
                _writer = writerConfig.LoadWriter();
               // _writer = new ExcelReportWriter();
                
            }
            else
            {
                response.ContentType = _contentType;
            }

            if (!string.IsNullOrEmpty(FileName))
            {
                var extension = GetDownloadFileExtension(context.HttpContext.Request, defaultExtension);
                
                if (string.IsNullOrEmpty(extension))
                {
                    extension = ".xlsx";
                }
                response.Headers["content-disposition"] = string.Format("attachment; filename={0}{1}", FileName, extension);
            }

            //response.RegisterForDispose()
           await _writer.WriteReportAsync(_report, response.Body);
            return;
        }
        protected virtual string GetDownloadFileExtension(HttpRequest request, string defaultExtension)
        {
            // Manual filename, don't override
            if (Path.HasExtension(FileName)) return "";

            // Extension passed in via in URL
            if (Path.HasExtension(request.Path))
                return Path.GetExtension(request.Path);

            return defaultExtension;
        }
        private  WriterElement GetWriterFromExtension(
            ActionContext context, string defaultExtension)
        {

            // attempt to get the report format from the extension on the URL (ex. "/action/controller.pdf" yields ".pdf")
            string extension = Path.GetExtension(context.HttpContext.Request.Path);

            if (string.IsNullOrEmpty(extension))
            {
                extension = defaultExtension;
            }
            if (ReportConfiguration == null)
            {
                ReportConfiguration = new DoddleReportConfiguration();
            }
            var writerConfig =      
                ReportConfiguration.Writers.GetWriterConfigurationForFileExtension(extension);
            if (writerConfig == null)
                throw new InvalidOperationException(
                    string.Format(
                        "Unable to locate a report writer for the extension '{0}'. Did you add this fileExtension to the web.config for DoddleReport?",
                        extension));

            return writerConfig;
        }
    }

  
}