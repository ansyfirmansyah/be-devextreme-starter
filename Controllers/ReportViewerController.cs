using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native.Services;
using Microsoft.AspNetCore.Mvc;

namespace be_devextreme_starter.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ReportViewerController : WebDocumentViewerController
    {
        public ReportViewerController(IWebDocumentViewerMvcControllerService controllerService)
            : base(controllerService)
        {
        }
    }
}