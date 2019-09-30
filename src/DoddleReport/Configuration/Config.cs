namespace DoddleReport.Configuration
{
    public static class Config
    {
        public static DoddleReportConfiguration Report
        {
            get
            {
                // TODO: blows up on netstandard
                //var section = ConfigurationManager.GetSection("doddleReport") as DoddleReportSection;
                //return section ?? new DoddleReportSection();
                return new DoddleReportConfiguration();
            }
        }
    }
}