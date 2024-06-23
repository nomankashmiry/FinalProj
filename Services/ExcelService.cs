using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace backend.Services
{
    public class ExcelService
    {
        public async Task<(List<string> countries, List<string> years, List<Dictionary<string, object>> datasets)> ReadTypedExcel(Stream fileStream, string TypeID)
        {
            List<string> countries = new List<string>();
            List<string> years = new List<string>();
            List<Dictionary<string, object>> datasets = new List<Dictionary<string, object>>();

            if (TypeID == "1")
            {
                (countries, years, datasets) = await ReadCapacityData(fileStream);
            }
            else if (TypeID == "2")
            {
                (countries, years, datasets) = await ReadSegmentedData(fileStream);
            }

            return (countries, years, datasets);
        }

        public async Task<(List<string> countries, List<string> years, List<Dictionary<string, object>> datasets)> ReadCapacityData(Stream fileStream)
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

         private async Task<(List<string> countries, List<string> years, List<Dictionary<string, object>> datasets)> ReadSegmentedData(Stream fileStream)
        {
            var countries = new List<string>();
            var years = new List<string>();
            var datasets = new List<Dictionary<string, object>>();

            using var package = new ExcelPackage(fileStream);
            
                var worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first sheet
                var rows = worksheet.Dimension?.Rows ?? 0;
                var cols = worksheet.Dimension?.Columns ?? 0;

                if (rows == 0 || cols == 0)
                {
                    return (countries, years, datasets);
                }

                for (int row = 2; row <= rows; row++)
                {
                    var country = worksheet.Cells[row, 1].Text;
                    if (!countries.Contains(country))
                    {
                        countries.Add(country);
                    }

                    for (int col = 2; col <= cols; col += 3)
                    {
                        if (worksheet.Cells[1, col].Text.Contains("Y"))
                        {
                            var headerText =  worksheet.Cells[1, col].Text;
                            var year = headerText.Contains("Rooftop") ? headerText.Split(' ')[1]: headerText.Split(' ')[2];
                            if (!years.Contains(year))
                            {
                                years.Add(year);
                            }

                            var rooftop = worksheet.Cells[row, col]?.Text ?? "0";
                            var utilityScale = worksheet.Cells[row, col + 1]?.Text ?? "0";

                            var rooftopData = datasets.FirstOrDefault(d => d["country"].ToString() == country && d["label"].ToString() == "Rooftop");
                            var utilityScaleData = datasets.FirstOrDefault(d => d["country"].ToString() == country && d["label"].ToString() == "Utility Scale");

                            if (rooftopData == null)
                            {
                                rooftopData = new Dictionary<string, object>
                                {
                                    { "country", country },
                                    { "label", "Rooftop" },
                                    { "data", new List<string>() }
                                };
                                datasets.Add(rooftopData);
                            }
                            ((List<string>)rooftopData["data"]).Add(rooftop);

                            if (utilityScaleData == null)
                            {
                                utilityScaleData = new Dictionary<string, object>
                                {
                                    { "country", country },
                                    { "label", "Utility Scale" },
                                    { "data", new List<string>() }
                                };
                                datasets.Add(utilityScaleData);
                            }
                            ((List<string>)utilityScaleData["data"]).Add(utilityScale);
                        }
                    }
            }

            return (countries, years, datasets);
        }
    }
}