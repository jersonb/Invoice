using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;

namespace PdfGenerator
{
    public class GeneratePdf
    {
        public Uri Logo { get; }

        public GeneratePdf(Uri urlGitHub)
        {
            Logo = urlGitHub;
        }

        public MemoryStream PdfInMemoryStream => Generate();

        private MemoryStream Generate()
        {
            try
            {
                var logo = Image.GetInstance(Logo);

                var ms = new MemoryStream();
                var doc = new Document(PageSize.A4, 40, 40, 40, 40);
                var pdf = new PdfHelper(doc, ms);

                var width = doc.Right - doc.Left;
                var heigth = doc.Top - doc.Bottom;
                var padding = 5f;

                pdf.AddImage(logo, absoluteX: doc.Left + padding, absoluteY: doc.Top - (logo.Height + padding));
                pdf.Text(Labels.LEGAL_NAME, nameFont: BaseFont.TIMES_BOLD, sizeFont: 18, positionX: width * 0.5f, positionY: doc.Top - (padding * 6), align: Element.ALIGN_CENTER);
                pdf.Text(Labels.CNPJ, nameFont: BaseFont.COURIER_BOLD, sizeFont: 12, positionX: width * 0.45f, doc.Top - (padding * 9), align: Element.ALIGN_CENTER);
                pdf.Text(Labels.ADDRESS, nameFont: BaseFont.HELVETICA_BOLDOBLIQUE, sizeFont: 7, positionX: width * 0.45f, doc.Top - (padding * 11), align: Element.ALIGN_CENTER);
                pdf.Text($"{Labels.CEP} {Labels.PHONE}", nameFont: BaseFont.HELVETICA_BOLDOBLIQUE, sizeFont: 7, positionX: 250, doc.Top - (padding * 13), align: Element.ALIGN_CENTER);

                var collumnAligne = doc.Left + (width * 0.65f) + (padding * 2);
                pdf.Text(Labels.DESCRIPTION, nameFont: BaseFont.HELVETICA_BOLD, sizeFont: 8, positionX: width * 0.9f, doc.Top - (padding * 2), align: Element.ALIGN_CENTER);
                pdf.Text($"Emissão Data: {DateTime.Now:dd/MM/yyyy}", nameFont: BaseFont.HELVETICA, sizeFont: 8, positionX: collumnAligne, doc.Top - (padding * 5), align: Element.ALIGN_LEFT);
                pdf.Text($"Insc. Municipal N°: 6124836", nameFont: BaseFont.HELVETICA, sizeFont: 8, positionX: collumnAligne, doc.Top - (padding * 7), align: Element.ALIGN_LEFT);
                pdf.Text($"Natureza da Operação: Locação de Veículos", nameFont: BaseFont.HELVETICA, sizeFont: 8, positionX: collumnAligne, doc.Top - (padding * 9), align: Element.ALIGN_LEFT);
                pdf.Text($"Série: Única", nameFont: BaseFont.HELVETICA, sizeFont: 8, positionX: collumnAligne, doc.Top - (padding * 11), align: Element.ALIGN_LEFT);
                pdf.Text("N°: 00555", nameFont: BaseFont.HELVETICA_BOLD, sizeFont: 12, positionX: width * 0.9f, doc.Top - (padding * 14), align: Element.ALIGN_CENTER);

                pdf.RoundRectangle(doc.Left, doc.Top - (padding * 15), (width * 0.65f) + padding, padding * 15, radius: 2f);
                pdf.RoundRectangle(doc.Left + (width * 0.65f) + padding, doc.Top - (padding * 15), (width * 0.35f) - padding, padding * 15, radius: 2f);

                pdf.RoundRectangle(doc.Left, doc.Bottom, width, heigth, lineWidth: 1.5f, boarderColor: BaseColor.LightGray);
                doc.Close();
                var msInfo = ms.ToArray();
                ms.Write(msInfo, 0, msInfo.Length);

                ms.Position = 0;
                return ms;
            }
            catch(InvalidOperationException)
            {
                throw new InvalidOperationException($"algo de errado com o link {Logo.AbsoluteUri.Replace(".png", "")}");
            }
        }
    }

    public static class Labels
    {
        public const string LEGAL_NAME = "MB DA COSTA EIRELI-EPP";
        public const string CNPJ = "CNPJ: 11.117.014/0001-06";
        public const string ADDRESS = "RUA RIO OCEANICO, SALA 08, 422, IMBIRIBEIRA, RECIFE, PE";
        public const string PHONE = "Fone: 81-99540-2828";
        public const string CEP = "CEP: 52.200-050";
        public const string DESCRIPTION = "NOTA FISCAL DE SERVIÇO - FATURA";
    }
}