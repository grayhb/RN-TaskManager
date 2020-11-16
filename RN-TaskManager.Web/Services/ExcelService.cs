using System;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using RN_TaskManager.Web.ViewModels;

namespace RN_TaskManager.Web.Services
{
    public class ExcelService : IExcelService
    {

        public string Report(List<object> items)
        {
            List<ReportItemView> fields = new List<ReportItemView>();

            ProjectTaskViewModel tmp;

            fields.Add(new ReportItemView()
            {
                Name = nameof(tmp.Important),
                Title = "Ключевой проект",
                Type = typeof(bool)
            });

            fields.Add(new ReportItemView()
            {
                Name = nameof(tmp.TaskStatusName),
                Title = "Статус"
            });

            fields.Add(new ReportItemView()
            {
                Name = nameof(tmp.ProjectName),
                Title = "Проект"
            });

            fields.Add(new ReportItemView()
            {
                Name = nameof(tmp.Details),
                Title = "Описание задачи",
                Width = 15000,
                Align = "Left"
            });

            fields.Add(new ReportItemView()
            {
                Name = nameof(tmp.StartPlan),
                Title = "Начало план",
                Type = typeof(DateTime)
            });

            fields.Add(new ReportItemView()
            {
                Name = nameof(tmp.EndPlan),
                Title = "Окончание план",
                Type = typeof(DateTime)
            });

            fields.Add(new ReportItemView()
            {
                Name = nameof(tmp.StartFact),
                Title = "Начало факт",
                Type = typeof(DateTime)
            });

            fields.Add(new ReportItemView()
            {
                Name = nameof(tmp.EndFact),
                Title = "Окончание факт",
                Type = typeof(DateTime)
            });

            fields.Add(new ReportItemView()
            {
                Name = nameof(tmp.Performers),
                Title = "Исполнитель",
                Align = "Left"
            });

            fields.Add(new ReportItemView()
            {
                Name = nameof(tmp.GroupName),
                Title = "Группа"
            });

            fields.Add(new ReportItemView()
            {
                Name = nameof(tmp.Note),
                Title = "Комментарий исполнителя",
                Align = "Left",
                Width = 15000
            });

            fields.Add(new ReportItemView()
            {
                Name = nameof(tmp.BlockName),
                Title = "Блок"
            });

            fields.Add(new ReportItemView()
            {
                Name = nameof(tmp.EffectAfterHours),
                Title = "Трудоемкость после автоматизации",
                Type = typeof(double)
            });

            return CreateReport("Отчет по задачам", items, fields);
        }

        private string CreateReport(string sheetName, List<object> data, List<ReportItemView> fields)
        {
            XSSFWorkbook xssfwb = new XSSFWorkbook();

            //основной шрифт для стилей
            var defaultFont = xssfwb.CreateFont();
            defaultFont.FontHeightInPoints = 9;
            defaultFont.FontName = "Arial";
            //бледный шрифт для стилей
            var paleFont = xssfwb.CreateFont();
            paleFont.FontHeightInPoints = 9;
            paleFont.FontName = "Arial";
            paleFont.Color = NPOI.HSSF.Util.HSSFColor.Grey80Percent.Index;

            ISheet sheet = xssfwb.CreateSheet(sheetName);

            ICellStyle headStyle = xssfwb.CreateCellStyle();
            headStyle.SetFont(defaultFont);
            headStyle.BorderTop = BorderStyle.Thin;
            headStyle.BorderLeft = BorderStyle.Thin;
            headStyle.BorderRight = BorderStyle.Thin;
            headStyle.BorderBottom = BorderStyle.Thin;
            headStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
            headStyle.FillPattern = FillPattern.SolidForeground;
            headStyle.Alignment = HorizontalAlignment.Center;
            headStyle.WrapText = true;
            headStyle.VerticalAlignment = VerticalAlignment.Center;

            //пропишем заголовки колонок (строки и колонки начинаются с нулевой!)
            IRow headline = sheet.CreateRow(0);

            for (int i = 0; i < fields.Count; i++)
            {
                headline.CreateCell(i).SetCellValue(fields[i].Title);
                sheet.SetColumnWidth(i, fields[i].Width);
                headline.GetCell(i).CellStyle = headStyle;
            }

            //фильтр:
            sheet.SetAutoFilter(new CellRangeAddress(0, 0, 0, fields.Count - 1));

            //закрепление шапки
            sheet.CreateFreezePane(0, 1);

            //стиль для остальных ячеек
            ICellStyle mainStyle = xssfwb.CreateCellStyle();
            mainStyle.SetFont(defaultFont);
            mainStyle.BorderTop = BorderStyle.Thin;
            mainStyle.BorderLeft = BorderStyle.Thin;
            mainStyle.BorderRight = BorderStyle.Thin;
            mainStyle.BorderBottom = BorderStyle.Thin;
            mainStyle.VerticalAlignment = VerticalAlignment.Center;
            mainStyle.Alignment = HorizontalAlignment.Center;
            mainStyle.WrapText = true;

            IDataFormat dataFormatCustom = xssfwb.CreateDataFormat();

            ICellStyle dateStyle = xssfwb.CreateCellStyle();
            dateStyle.CloneStyleFrom(mainStyle);
            dateStyle.DataFormat = dataFormatCustom.GetFormat("dd/MM/yyyy");

            ICellStyle doubleStyle = xssfwb.CreateCellStyle();
            doubleStyle.CloneStyleFrom(mainStyle);
            doubleStyle.DataFormat = dataFormatCustom.GetFormat("0.00");

            ICellStyle mainLeftStyle = xssfwb.CreateCellStyle();
            mainLeftStyle.CloneStyleFrom(mainStyle);
            mainLeftStyle.Alignment = HorizontalAlignment.Left;

            int rowIndex = 0;

            foreach (var item in data)
            {
                IRow row = sheet.CreateRow(++rowIndex);
                for (int i = 0; i < fields.Count; i++)
                {
                    var value = GetPropValue(item, fields[i].Name);
                    var valueStr = value != null ? Convert.ToString(value) : "";

                    if (fields[i].Type == typeof(DateTime) && valueStr != "")
                    {
                        row.CreateCell(i).SetCellValue(DateTime.Parse(valueStr ?? string.Empty));
                        row.GetCell(i).CellStyle = dateStyle;
                    }
                    else if (fields[i].Type == typeof(double) && valueStr != "")
                    {
                        //valueStr = valueStr.Replace("%", "");
                        row.CreateCell(i).SetCellValue(double.Parse(valueStr ?? string.Empty));
                        row.GetCell(i).CellStyle = doubleStyle;
                    }
                    else
                    {
                        row.CreateCell(i).SetCellValue(valueStr);

                        row.GetCell(i).CellStyle = fields[i].Align == "Left" ? mainLeftStyle : mainStyle;
                    }
                }
            }

            string fileNameExcel = Path.GetTempFileName();
            FileStream sw = File.Create(fileNameExcel);
            xssfwb.Write(sw);
            sw.Close();

            return fileNameExcel;
        }

        private object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName)?.GetValue(src, null);
        }

    }
}
