using System;

namespace RN_TaskManager.Web.ViewModels
{
    public class ReportItemView
    {
        public ReportItemView()
        {
            Width = 5000;
        }

        public string Name { get; set; }
        public string Title { get; set; }
        public Type Type { get; set; }
        public int Width { get; set; }
        public string Align { get; set; }
    }
}
