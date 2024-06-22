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

        public async Task<(List<string> countries, List<string> years, List<Dictionary<string, object>> datasets)> ReadDataExcelAsync(Stream fileStream)
        {
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets[0];

            var countries = new List<string>();
            var years = new List<string>();
            var datasets = new List<Dictionary<string, object>>();

            // Extract years from the first row starting from the third column
            for (int col = 3; col <= worksheet.Dimension.End.Column; col++)
            {
                years.Add(worksheet.Cells[1, col].Text);
            }

            // Extract countries and data from subsequent rows
            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var country = worksheet.Cells[row, 2].Text;
                countries.Add(country);

                if (country.ToLower() == "belgium"){
                    int x = 1;
                }

                var data = new List<double>();
                for (int col = 3; col <= worksheet.Dimension.End.Column; col++)
                {
                    if (double.TryParse(worksheet.Cells[row, col].Text, out double value))
                    {
                        data.Add(value);
                    }
                    else
                    {
                        data.Add(0); // or handle as needed
                    }
                }

                datasets.Add(new Dictionary<string, object>
                {
                    { "label", country },
                    { "data", data }
                });
            }

            return (countries, years, datasets);
        }
    }
}