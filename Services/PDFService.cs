using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using truckPRO_api.Data;
using truckPRO_api.Models;
using PdfSharp.UniversalAccessibility.Drawing;
using PdfSharp.Pdf.Annotations;


namespace truckPRO_api.Services
{
    public class PdfService : IPdfService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IManagerService _managerService;

        public PdfService(ApplicationDbContext context, IMapper mapper, IManagerService managerService)
        {
            _context = context;
            _mapper = mapper;
            _managerService = managerService;
        }

        public async Task<byte[]> GenerateDrivingRecordsPdfAsync(int driverId, DateTime startDate, DateTime endDate, List<LogEntryType> selectedLogTypes)
        {
            var user = await _context.User.FindAsync(driverId);
            if (user == null)
            {
                throw new Exception("Driver not found.");
            }

            var logs = await _managerService.GetLogsByDriver(driverId);
            var filteredLogs = logs
                .Where(log => log != null && log.StartTime >= startDate 
                            && (log.EndTime <= endDate || log.EndTime == null)
                            && selectedLogTypes.Contains(log.LogEntryType))
                .ToList();

            if (filteredLogs.Count == 0)
            {
                throw new Exception("No driving records found for the specified date range.");
            }

            using var pdfDoc = new PdfDocument();
            pdfDoc.Info.Title = "Driving Records";

            var titleFont = new XFont("Arial", 16, XFontStyleEx.Bold);
            var sectionFont = new XFont("Arial", 14, XFontStyleEx.Bold);
            var font = new XFont("Arial", 12, XFontStyleEx.Regular);

            var grayBrush = new XSolidBrush(XColor.FromGrayScale(0.7));
            var lineColor = XPens.Gray;

            PdfPage page = pdfDoc.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            int yPosition = 40;

            // Add Title Section
            gfx.DrawString("Driving Records Report", titleFont, XBrushes.Black, new XPoint(40, yPosition));
            gfx.DrawString($"Driver: {user.FirstName} {user.LastName}", font, XBrushes.Black, new XPoint(40, yPosition + 20));
            gfx.DrawString($"Date Range: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}", font, XBrushes.Black, new XPoint(40, yPosition + 40));

            yPosition += 70;

            foreach (var parentLog in filteredLogs)
            {
                // Add a new page if needed
                if (yPosition > page.Height - 100)
                {
                    page = pdfDoc.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    yPosition = 40;
                }

                // Parent Log Entry Header
                gfx.DrawLine(lineColor, 40, yPosition, page.Width - 40, yPosition);
                yPosition += 10;
                gfx.DrawString($"Log Type: {parentLog.LogEntryType}", sectionFont, grayBrush, new XPoint(40, yPosition));
                gfx.DrawString($"Start Time: {parentLog.StartTime:G}", font, XBrushes.Black, new XPoint(40, yPosition + 20));
                if (parentLog.EndTime != null)
                {
                    gfx.DrawString($"End Time: {parentLog.EndTime:G}", font, XBrushes.Black, new XPoint(300, yPosition + 20));
                }
                else
                {
                    gfx.DrawString("In Progress", font, XBrushes.Black, new XPoint(300, yPosition + 20));
                }
                if (parentLog.LogEntryType == LogEntryType.Driving)
                {
                    gfx.DrawString($"Approved by Manager: {parentLog.IsApprovedByManager}", font, XBrushes.Black, new XPoint(40, yPosition + 40)); 
                }
                yPosition += 50;
                gfx.DrawString($"Events", titleFont, XBrushes.Black, new XPoint(50, yPosition));


                // Child Logs for Parent Log
                foreach (var childLog in parentLog.ChildLogEntries)
                {
                    if (yPosition > page.Height - 100)
                    {
                        page = pdfDoc.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        yPosition = 40;
                    }

                    gfx.DrawLine(lineColor, 50, yPosition, page.Width - 50, yPosition);
                    yPosition += 10;
                    gfx.DrawString($"{childLog.LogEntryType}", font, XBrushes.Black, new XPoint(50, yPosition));
                    gfx.DrawString($"Start Time: {childLog.StartTime:G}", font, XBrushes.Black, new XPoint(50, yPosition + 20));
                    if (childLog.EndTime != null)
                    {
                        gfx.DrawString($"End Time: {childLog.EndTime:G}", font, XBrushes.Black, new XPoint(300, yPosition + 20));
                    }
                    else
                    {
                        gfx.DrawString("In Progress", font, XBrushes.Black, new XPoint(300, yPosition + 20));
                    }
                    yPosition += 40;
                }
            }

            using var stream = new MemoryStream();
            pdfDoc.Save(stream, false);
            return stream.ToArray();
        }
            
    }
}
