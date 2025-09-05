using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using parc_App.Models;
using System.IO;
using System.Linq;

namespace parc_App.Controllers
{
    public class PDFController : Controller
    {
        private readonly Appdatacontext _context;

        public PDFController(Appdatacontext context)
        {
            _context = context;
        }

        public IActionResult ExportPdf()
        {
            // Récupérer les données
            var data = _context.Materiels
                .Include(m => m.Preneur)
                .Select(m => new
                {
                    m.Id,
                    m.TypeMateriel,
                    m.Marque,
                    m.Modele,
                    m.Etat,
                    Preneur = m.Preneur != null ? m.Preneur.Nom + " " + m.Preneur.Prenom : "Non attribué"
                })
                .ToList();

            var stream = new MemoryStream();
            PdfWriter writer = new PdfWriter(stream);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf, PageSize.A4);

            // Polices
            PdfFont boldFont = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
            PdfFont regularFont = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);

            // Titre
            Paragraph title = new Paragraph("Liste des matériels")
                .SetFont(boldFont)
                .SetFontSize(18)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20);
            document.Add(title);

            // Header avec logo uniquement
            Table headerTable = new Table(1).UseAllAvailableWidth();

            // Logo
            string logoPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "img", "tib002.jpg");
            if (System.IO.File.Exists(logoPath))
            {
                Image logo = new Image(ImageDataFactory.Create(logoPath))
                    .ScaleToFit(80, 80)
                    .SetHorizontalAlignment(HorizontalAlignment.RIGHT);
                headerTable.AddCell(new Cell()
                    .Add(logo)
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                    .SetTextAlignment(TextAlignment.RIGHT));
            }
            else
            {
                headerTable.AddCell(new Cell().SetBorder(iText.Layout.Borders.Border.NO_BORDER));
            }

            document.Add(headerTable);
            document.Add(new Paragraph("\n")); // petit espace

            // Tableau principal
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 5, 15, 20, 20, 15, 25 })).UseAllAvailableWidth();

            // Couleur entêtes
            Color headerBgColor = ColorConstants.LIGHT_GRAY;
            table.AddHeaderCell(new Cell().Add(new Paragraph("Id").SetFont(boldFont)).SetBackgroundColor(headerBgColor));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Type").SetFont(boldFont)).SetBackgroundColor(headerBgColor));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Marque").SetFont(boldFont)).SetBackgroundColor(headerBgColor));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Modèle").SetFont(boldFont)).SetBackgroundColor(headerBgColor));
            table.AddHeaderCell(new Cell().Add(new Paragraph("État").SetFont(boldFont)).SetBackgroundColor(headerBgColor));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Preneur").SetFont(boldFont)).SetBackgroundColor(headerBgColor));

            // Remplissage lignes
            foreach (var m in data)
            {
                table.AddCell(new Cell().Add(new Paragraph(m.Id.ToString()).SetFont(regularFont)));
                table.AddCell(new Cell().Add(new Paragraph(m.TypeMateriel ?? "").SetFont(regularFont)));
                table.AddCell(new Cell().Add(new Paragraph(m.Marque ?? "").SetFont(regularFont)));
                table.AddCell(new Cell().Add(new Paragraph(m.Modele ?? "").SetFont(regularFont)));
                table.AddCell(new Cell().Add(new Paragraph(m.Etat ?? "").SetFont(regularFont)));
                table.AddCell(new Cell().Add(new Paragraph(m.Preneur ?? "Non attribué").SetFont(regularFont)));
            }

            document.Add(table);
            document.Close();

            return File(stream.ToArray(), "application/pdf", "Materiels.pdf");
        }
    }
}
