using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using Hostel2._0.Models;

namespace Hostel2._0.Services
{
    public class PdfReceiptService
    {
        private readonly string _wkhtmltopdfPath;

        public PdfReceiptService()
        {
            // Try to find wkhtmltopdf in common installation paths
            var possiblePaths = new[]
            {
                @"C:\Program Files\wkhtmltopdf\bin\wkhtmltopdf.exe",
                @"C:\Program Files (x86)\wkhtmltopdf\bin\wkhtmltopdf.exe",
                "wkhtmltopdf" // If it's in PATH
            };

            _wkhtmltopdfPath = possiblePaths.FirstOrDefault(File.Exists) ?? "wkhtmltopdf";
        }

        public byte[] GenerateReceipt(Payment payment)
        {
            // Generate HTML content
            var htmlContent = GenerateHtmlContent(payment);

            // Create a temporary HTML file
            var tempHtmlPath = Path.GetTempFileName() + ".html";
            File.WriteAllText(tempHtmlPath, htmlContent);

            // Create a temporary PDF file
            var tempPdfPath = Path.GetTempFileName() + ".pdf";

            try
            {
                // Run wkhtmltopdf command
                var startInfo = new ProcessStartInfo
                {
                    FileName = _wkhtmltopdfPath,
                    Arguments = $"--enable-local-file-access \"{tempHtmlPath}\" \"{tempPdfPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        throw new Exception("Failed to start wkhtmltopdf process. Please ensure wkhtmltopdf is installed and accessible.");
                    }

                    var output = process.StandardOutput.ReadToEnd();
                    var error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"wkhtmltopdf failed with exit code {process.ExitCode}. Error: {error}");
                    }
                }

                // Read the generated PDF
                var pdfBytes = File.ReadAllBytes(tempPdfPath);
                return pdfBytes;
            }
            finally
            {
                // Clean up temporary files
                if (File.Exists(tempHtmlPath))
                    File.Delete(tempHtmlPath);
                if (File.Exists(tempPdfPath))
                    File.Delete(tempPdfPath);
            }
        }

        private string GenerateHtmlContent(Payment payment)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='utf-8'>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; margin: 40px; }");
            html.AppendLine(".header { text-align: center; margin-bottom: 30px; }");
            html.AppendLine(".receipt-title { font-size: 24px; font-weight: bold; color: #333; }");
            html.AppendLine(".details-table { width: 100%; border-collapse: collapse; margin: 20px 0; }");
            html.AppendLine(".details-table td { padding: 8px; border-bottom: 1px solid #ddd; }");
            html.AppendLine(".details-table td:first-child { font-weight: bold; width: 40%; }");
            html.AppendLine(".footer { text-align: center; margin-top: 40px; color: #666; }");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");

            // Header
            html.AppendLine("<div class='header'>");
            html.AppendLine("<div class='receipt-title'>PAYMENT RECEIPT</div>");
            html.AppendLine("</div>");

            // Details
            html.AppendLine("<table class='details-table'>");
            AddTableRow(html, "Receipt Number:", payment.ReceiptNumber);
            AddTableRow(html, "Date:", payment.PaymentDate.ToString("dd/MM/yyyy HH:mm"));
            AddTableRow(html, "Student Name:", payment.Student?.FullName ?? "N/A");
            AddTableRow(html, "Student ID:", payment.Student?.StudentId ?? "N/A");
            AddTableRow(html, "Room Number:", payment.Room?.RoomNumber ?? "N/A");
            AddTableRow(html, "Payment Method:", payment.PaymentMethod ?? "N/A");
            AddTableRow(html, "Amount:", $"৳{payment.Amount:N2}");
            AddTableRow(html, "Status:", payment.Status.ToString());
            html.AppendLine("</table>");

            // Footer
            html.AppendLine("<div class='footer'>");
            html.AppendLine("Thank you for your payment!");
            html.AppendLine("</div>");

            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        private void AddTableRow(StringBuilder html, string label, string value)
        {
            html.AppendLine("<tr>");
            html.AppendLine($"<td>{label}</td>");
            html.AppendLine($"<td>{value}</td>");
            html.AppendLine("</tr>");
        }
    }
} 