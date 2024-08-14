using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Shared.Models.Entities;

namespace User_Management.Infrastructures.Utils.Report
{
    public class NPoiAcitivyReportProvider : IActivityReportProvider
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NPoiAcitivyReportProvider(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public Task<byte[]?> GenerateReportAsync(IEnumerable<LgActivity> activities)
        {
            byte[] bytes = null;
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "Templates", "Excels", "ActivityLogs.xlsx");
            var newPath = Path.Combine(_webHostEnvironment.WebRootPath, "Templates", "Excels", "Temp", $"{Guid.NewGuid()}.xlsx");
            var fileName = $"ActivityLogs{DateTime.Now.ToString("yyyymmddhhmm")}.xlsx";
            var tempPath = Path.Combine(_webHostEnvironment.WebRootPath, "Templates", "Excels", "Temp", fileName);
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
            }
            try
            {
                if (File.Exists(path))
                    File.Copy(path, newPath);

                if (System.IO.File.Exists(path))
                {
                    ISheet sheet;
                    using (var fileStream = new FileStream(tempPath, FileMode.OpenOrCreate))
                    {
                        using (var stream = new FileStream(path, FileMode.Open))
                        {
                            stream.Position = 0;
                            XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);
                            sheet = xssWorkbook.GetSheetAt(0);
                            IRow headerRow = sheet.GetRow(0);
                            int cellCount = headerRow.LastCellNum;
                            var i = 0;
                            foreach (var activity in activities)
                            {
                                IRow row = sheet.CreateRow(1 + i);
                                row.CreateCell(0).SetCellValue(i + 1);
                                row.CreateCell(1).SetCellValue(activity.Username);
                                row.CreateCell(2).SetCellValue(activity.Activity);
                                if (activity.Time != null)
                                    row.CreateCell(3).SetCellValue(activity.Time.Value.ToString("dd-MMM-yyyy hh:mm"));
                                i++;
                            }
                            xssWorkbook.Write(fileStream);
                        }
                    }
                    bytes = File.ReadAllBytes(tempPath);
                    return Task.FromResult(bytes);
                }
            }
            finally
            {
                if (File.Exists(newPath))
                    File.Delete(newPath);
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
            return Task.FromResult(bytes);
        }
    }
}
