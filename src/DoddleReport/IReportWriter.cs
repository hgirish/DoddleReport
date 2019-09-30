using System.IO;
using System.Threading.Tasks;

namespace DoddleReport
{
    public interface IReportWriter
    {
        void WriteReport(Report report, Stream destination);
        Task WriteReportAsync(Report report, Stream destination);
        void AppendReport(Report source, Report destination);
    }
}