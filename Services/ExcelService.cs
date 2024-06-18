using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace backend.Services
{
    public class ExcelService
    {
        public async Task<List<string>> ReadExcelAsync(Stream fileStream)
        {
            var result = new List<string>();

            using (var package = new ExcelPackage(fileStream))
            {
                // Assuming there's only one worksheet in the Excel file
                var worksheet = package.Workbook.Worksheets[0];

                // Loop through rows and read data
                for (int row = 1; row <= worksheet.Dimension.Rows; row++)
                {
                    // Assuming data is in the first column (A)
                    var cellValue = worksheet.Cells[row, 1].Value?.ToString();
                    
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        result.Add(cellValue);
                    }
                }
            }

            return result;
        }
    }
}