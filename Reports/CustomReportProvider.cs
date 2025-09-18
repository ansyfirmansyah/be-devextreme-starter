using be_demo_reporting;
using be_demo_reporting.Reports;
using be_devextreme_starter.Reports.OutletSummary;
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.WebUtilities;

namespace be_devextreme_starter.Reports
{
    public class CustomReportProvider : IReportProvider
    {
        // Kita tidak butuh IHttpContextAccessor lagi, jadi hapus constructor
        public CustomReportProvider()
        {
        }

        public XtraReport GetReport(string id, ReportProviderContext context)
        {
            // id yang masuk adalah "ProductReport?pCategoryID=1"
            string reportName = id;
            var queryParameters = new Dictionary<string, string>();

            // Cek apakah ada query string di dalam 'id'
            if (id.Contains("?"))
            {
                var parts = id.Split('?');
                reportName = parts[0];
                // Gunakan helper bawaan ASP.NET Core untuk mem-parsing query string
                queryParameters = QueryHelpers.ParseQuery(parts[1])
                    .ToDictionary(kv => kv.Key, kv => kv.Value.ToString());
            }
            if (reportName == "StrukPenjualanReport")
            {
                var report = new StrukPenjualanReport();

                // Ambil nilai parameter dari hasil parsing, bukan dari HttpContext
                if (queryParameters.TryGetValue("paramKode", out var categoryIdValue))
                {
                    // Jika categoryId bukan 0, baru terapkan filter
                    if (categoryIdValue != "")
                    {
                        var param = report.Parameters["paramKode"];
                        if (param != null)
                        {
                            param.Value = categoryIdValue;
                        }
                    }
                    else
                    {
                        // Jika 0, nonaktifkan filter agar semua data tampil
                        report.FilterString = "";
                    }
                }
                return report;
            }
            if (reportName == "OutletSummaryReport")
            {
                var report = new OutletList();

                // Ambil nilai parameter dari hasil parsing, bukan dari HttpContext
                if (queryParameters.TryGetValue("outletKode", out var categoryIdValue))
                {
                    // Jika categoryId bukan 0, baru terapkan filter
                    if (categoryIdValue != "")
                    {
                        var param = report.Parameters["outletKode"];
                        if (param != null)
                        {
                            param.Value = categoryIdValue != "[ALL]" ? categoryIdValue : null;
                        }
                    }
                    else
                    {
                        // Jika 0, nonaktifkan filter agar semua data tampil
                        report.FilterString = "";
                    }
                }
                return report;
            }

            return null;
        }
    }
}
