using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using truckPRO_api.Data;
using truckPRO_api.DTOs;
using truckPRO_api.Models;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

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
            var user = await _context.User.Where(u => u.Id == driverId).FirstOrDefaultAsync();
            
            // egt driving records based on date range and driver id
            var records = await _context.LogEntry
                .Where(le => le.UserId == driverId && le.StartTime >= startDate && le.EndTime <= endDate)
                .ToListAsync();

            if (records.Count == 0)
            {
                throw new Exception("No driving records found for the specified date range.");
            }

            using var pdfDoc = new PdfDocument();
            pdfDoc.Info.Title = "Driving Records";

            var page = pdfDoc.AddPage();
            using var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Verdana", 12);

            int yPosition = 50;
            gfx.DrawString("Driving Records Report", font, XBrushes.Black, new XPoint(40, yPosition));
            yPosition += 30;

            foreach (var record in records)
            {
                gfx.DrawString($"Start Date: {record.StartTime.ToShortDateString()}", font, XBrushes.Black, new XPoint(40, yPosition));
                gfx.DrawString($"End Date: {record.EndTime} miles", font, XBrushes.Black, new XPoint(200, yPosition));
                yPosition += 20;
            }

            using var stream = new MemoryStream();
            pdfDoc.Save(stream, false);
            return stream.ToArray();
        }
    }
}
