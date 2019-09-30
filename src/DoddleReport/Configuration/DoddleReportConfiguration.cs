using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoddleReport.Configuration
{
    public class DoddleReportConfiguration
    {
        [Required]
        public WriterElementCollection Writers { get; set; }
        public ICollection<StyleElement> Styles { get; set; }
        public string DefaultWriter { get; set; } = "Html";
        public string DataRowStyleName { get; set; } = "DataRowStyle";
        public string HeaderRowStyleName { get; set; } = "HeaderRowStyle";

        public string FooterRowStyleName { get; set; } = "FooterRowStyle";
        public DoddleReportConfiguration()
        {
            Styles = new List<StyleElement>();
            Writers = new WriterElementCollection();
        }

    }
}