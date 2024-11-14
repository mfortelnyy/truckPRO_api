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

        public PdfService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<byte[]> GenerateDrivingRecordsPdfAsync(int driverId, DateTime startDate, DateTime endDate)
        {
            var user = await _context.User.FindAsync(driverId);
            if (user == null)
            {
                throw new Exception("Driver not found.");
            }

            var records = await _context.LogEntry
                .Where(le => le.UserId == driverId && le.StartTime >= startDate && (le.EndTime <= endDate || le.EndTime == null))
                .ToListAsync();

            if (records.Count == 0)
            {
                throw new Exception("No driving records found for the specified date range.");
            }

            using var pdfDoc = new PdfDocument();
            pdfDoc.Info.Title = "Driving Records";

            var titleFont = new XFont("Verdana", 14, XFontStyleEx.Bold);
            var font = new XFont("Verdana", 10);

            PdfPage page = null;
            XGraphics gfx = null;
            int yPosition = 40;

            //start with the first page
            page = pdfDoc.AddPage();
            gfx = XGraphics.FromPdfPage(page);

            gfx.DrawString("Driving Records Report", titleFont, XBrushes.Black, new XPoint(40, yPosition));
            gfx.DrawString($"Driver: {user.FirstName} {user.LastName}", font, XBrushes.Black, new XPoint(40, yPosition + 20));
            gfx.DrawString($"Date Range: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}", font, XBrushes.Black, new XPoint(40, yPosition + 40));
            yPosition += 70;

            foreach (var record in records)
            {
                //add a new page is needed
                if (yPosition > page.Height - 100) 
                {
                    page = pdfDoc.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    yPosition = 40;
                }

                //log entry separator
                gfx.DrawLine(XPens.Black, 40, yPosition, page.Width - 40, yPosition);
                yPosition += 10;

                //log details: Start Time and End Time
                gfx.DrawString($"Start Time: {record.StartTime:G}", font, XBrushes.Black, new XPoint(40, yPosition));
                
                if(record.EndTime != null)
                {
                    gfx.DrawString($"End Time: {record.EndTime?.ToString("G")}", font, XBrushes.Black, new XPoint(300, yPosition));

                }
                else
                {
                    gfx.DrawString("In Progress", font, XBrushes.Black, new XPoint(300, yPosition));

                }
                yPosition += 20;

                //log Type
                gfx.DrawString($"Log Type: {record.LogEntryType}", font, XBrushes.Black, new XPoint(40, yPosition));

                //display ApprovedbyManager only for Driving type
                if (record.LogEntryType == LogEntryType.Driving)  
                {
                    gfx.DrawString($"Approved by Manager: {(record.IsApprovedByManager ? "Yes" : "No")}", font, XBrushes.Black, new XPoint(300, yPosition));
                    yPosition += 20;  
                }

                //space after each log entry 
                yPosition += 20; 
            }

            using var stream = new MemoryStream();
            pdfDoc.Save(stream, false);
            return stream.ToArray();
        }
    }
}
