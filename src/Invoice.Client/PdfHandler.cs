using Invoice.Client.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfGenerator;
using System;
using System.IO;

namespace Invoice.Client
{
    public class PdfHandler
    {
        private readonly InvoiceModel _invoice;
        public Uri Logo { get; }

        public PdfHandler(Uri urlGitHub, InvoiceModel invoice)
        {
            Logo = urlGitHub;
            _invoice = invoice;
        }

        public MemoryStream PdfInMemoryStream
            => Generate(_invoice);

        private MemoryStream Generate(InvoiceModel invoice)
        {
            try
            {
                var logo = Image.GetInstance(Logo);

                var ms = new MemoryStream();
                var doc = new Document(PageSize.A4, 40, 40, 40, 40);
                var pdf = new PdfHelper(doc, ms);

                var width = doc.Right - doc.Left;
                var padding = 5f;

                #region Header

                pdf.AddImage(logo, absoluteX: doc.Left + padding, absoluteY: doc.Top - 65f);
                pdf.Text(Labels.LEGAL_NAME, nameFont: BaseFont.TIMES_BOLD, sizeFont: 18, positionX: width * 0.5f, positionY: doc.Top - (padding * 6), align: Element.ALIGN_CENTER);
                pdf.Text($"CNPJ: {Labels.LEGAL_NUMBER}", nameFont: BaseFont.COURIER_BOLD, sizeFont: 12, positionX: 260f, doc.Top - (padding * 9.5f), align: Element.ALIGN_CENTER);
                pdf.Text(Labels.ADDRESS, nameFont: BaseFont.HELVETICA_BOLDOBLIQUE, sizeFont: 7, positionX: 260f, positionY: doc.Top - (padding * 12), align: Element.ALIGN_CENTER);
                pdf.Text($"CEP: {Labels.CEP} Fone: {Labels.PHONE}", nameFont: BaseFont.HELVETICA_BOLDOBLIQUE, sizeFont: 7, positionX: 260f, positionY: doc.Top - (padding * 14), align: Element.ALIGN_CENTER);

                var collumnAligne = doc.Left + (width * 0.65f) + (padding * 2);
                pdf.Text(Labels.DESCRIPTION.ToUpper(), nameFont: BaseFont.HELVETICA_BOLD, sizeFont: 8, positionX: width * 0.9f, positionY: doc.Top - (padding * 2.5f), align: Element.ALIGN_CENTER);
                pdf.Text($"Emissão Data: {invoice.Date ?? DateTime.Now:dd/MM/yyyy}", nameFont: BaseFont.HELVETICA, sizeFont: 8, positionX: collumnAligne, doc.Top - (padding * 5), align: Element.ALIGN_LEFT);
                pdf.Text($"Insc. Municipal N°: {Labels.LEGAL_REGIONAL_NUMBER}", nameFont: BaseFont.HELVETICA, sizeFont: 8, positionX: collumnAligne, doc.Top - (padding * 7), align: Element.ALIGN_LEFT);
                pdf.Text($"Natureza da Operação: Locação de Veículos", nameFont: BaseFont.HELVETICA, sizeFont: 8, positionX: collumnAligne, doc.Top - (padding * 9), align: Element.ALIGN_LEFT);
                pdf.Text($"Série: ÚNICA", nameFont: BaseFont.HELVETICA, sizeFont: 8, positionX: collumnAligne, doc.Top - (padding * 11), align: Element.ALIGN_LEFT);
                pdf.Text($"N°: {invoice.Number.PadLeft(5, '0')}", nameFont: BaseFont.HELVETICA_BOLD, sizeFont: 12, positionX: width * 0.9f, doc.Top - (padding * 14), align: Element.ALIGN_CENTER);

                #endregion Header

                #region Client

                var aligneLeft = doc.Left + 10;
                pdf.Text($"Nome do Sacado: {invoice.Client.Name}", BaseFont.HELVETICA, 10, aligneLeft, doc.Top - 95, align: Element.ALIGN_LEFT);
                pdf.Text($"Endereço: {invoice.Client.Address.Street}, {invoice.Client.Address.Number}", BaseFont.HELVETICA, 10, aligneLeft, doc.Top - 110, align: Element.ALIGN_LEFT);
                pdf.Text($"Município: {invoice.Client.Address.Region}", BaseFont.HELVETICA, 10, aligneLeft, doc.Top - 125, align: Element.ALIGN_LEFT);
                pdf.Text($"Estado: {invoice.Client.Address.State}", BaseFont.HELVETICA, 10, width * 0.55f, doc.Top - 125, align: Element.ALIGN_LEFT);
                pdf.Text($"CEP: {invoice.Client.Address.ZipCode}", BaseFont.HELVETICA, 10, width * 0.75f, doc.Top - 125, align: Element.ALIGN_LEFT);
                pdf.Text($"CNPJ: {invoice.Client.LegalNumber}", BaseFont.HELVETICA, 10, aligneLeft, doc.Top - 140, align: Element.ALIGN_LEFT);
                pdf.Text($"Insc. Municipal N°: {invoice.Client.RegionalLegalNumber}", BaseFont.HELVETICA, 10, width * 0.45f, doc.Top - 140, align: Element.ALIGN_LEFT);

                #endregion Client

                pdf.Text("VALOR REFERENTE POR EXTENSO:", BaseFont.HELVETICA_BOLD, 10, aligneLeft, doc.Top - 162, align: Element.ALIGN_LEFT);
                pdf.BigTextLeftColumn(invoice.Total.DecimalToExtenso(), doc.Left + padding, doc.Top - 230, width, 50f, nameFont: BaseFont.HELVETICA, sizeFont: 10, align: Element.ALIGN_CENTER);

                pdf.BigTextLeftColumn(string.Format(Labels.MESSAGE, Labels.LEGAL_NAME), doc.Left + padding, doc.Top - 275, width, 50f, nameFont: BaseFont.HELVETICA, sizeFont: 12);

                var col1 = doc.Left + (width * 0.25f);
                var col2 = doc.Left + (width * 0.65f);
                var col3 = doc.Left + (width * 0.75f);
                var col4 = doc.Left + (width * 0.9f);

                var header = doc.Top - 280;

                pdf.Text("Item Descrição", BaseFont.HELVETICA_BOLD, 10, col1, header, align: Element.ALIGN_CENTER);
                pdf.Text("Quant", BaseFont.HELVETICA_BOLD, 10, col2, header, align: Element.ALIGN_CENTER);
                pdf.Text("Vl.Unitário", BaseFont.HELVETICA_BOLD, 10, col3, header, align: Element.ALIGN_CENTER);
                pdf.Text("Valor", BaseFont.HELVETICA_BOLD, 10, col4, header, align: Element.ALIGN_CENTER);

                #region Items

                header -= 15;
                foreach(var item in invoice.Products)
                {
                    header -= 15;
                    pdf.Text(item.Description, BaseFont.COURIER, 10, aligneLeft, header, align: Element.ALIGN_LEFT);
                    pdf.Text(item.Quantity.ToString(), BaseFont.HELVETICA, 10, col2, header, align: Element.ALIGN_CENTER);
                    pdf.Text(item.UnitaryValue.FormatMoneyPtBr(), BaseFont.HELVETICA, 10, col3, header, align: Element.ALIGN_CENTER);
                    pdf.Text(item.TotalValue.FormatMoneyPtBr(), BaseFont.HELVETICA, 10, col4, header, align: Element.ALIGN_CENTER);
                }

                #endregion Items

                #region Bancary

                pdf.Text($"Período do Serviço:", BaseFont.COURIER_BOLD, 10, aligneLeft, doc.Bottom + 340, align: Element.ALIGN_LEFT);
                pdf.Text($"{invoice.ServicePeriod}", BaseFont.COURIER_BOLD, 10, aligneLeft + 40, doc.Bottom + 330, align: Element.ALIGN_LEFT);
                pdf.Text($"Dados do Banco", BaseFont.COURIER_BOLD, 10, aligneLeft, doc.Bottom + 320, align: Element.ALIGN_LEFT);
                pdf.Text($"Banco: ITAU", BaseFont.COURIER_BOLD, 10, aligneLeft, doc.Bottom + 310, align: Element.ALIGN_LEFT);
                pdf.Text($"Agência: 6385", BaseFont.COURIER_BOLD, 10, aligneLeft, doc.Bottom + 300, align: Element.ALIGN_LEFT);
                pdf.Text($"Conta Corrente: 56230-8", BaseFont.COURIER_BOLD, 10, aligneLeft, doc.Bottom + 290, align: Element.ALIGN_LEFT);
                pdf.Text($"N° do Empenho: {invoice.Commitment}", BaseFont.COURIER_BOLD, 10, aligneLeft, doc.Bottom + 280, align: Element.ALIGN_LEFT);

                pdf.Text($"Observações:", BaseFont.COURIER_BOLD, 10, doc.Left + (width * 0.45f) + padding, doc.Bottom + 335, align: Element.ALIGN_LEFT);
                pdf.BigTextLeftColumn(invoice.Observation, doc.Left + (width * 0.45f) + padding, doc.Bottom + 265, width * 0.50f, 60f, sizeFont: 10);

                #endregion Bancary

                #region Legal Description

                pdf.Text(Labels.INVOICE_FUNCTION, BaseFont.HELVETICA_BOLD, 15, doc.Left + (width * 0.75f) / 2, doc.Bottom + 220, align: Element.ALIGN_CENTER);
                pdf.Text(Labels.LEGAL_DESCRIPTION, BaseFont.HELVETICA_OBLIQUE, 11, doc.Left + (width * 0.75f) / 2, doc.Bottom + 190, align: Element.ALIGN_CENTER);

                pdf.Text("VALOR TOTAL", BaseFont.HELVETICA_BOLD, 15, doc.Left + (width * 0.875f), doc.Bottom + 220, align: Element.ALIGN_CENTER);
                pdf.Text($"{invoice.Total.FormatMoneyPtBr()}", BaseFont.HELVETICA, 11, doc.Left + (width * 0.875f), doc.Bottom + 190, align: Element.ALIGN_CENTER);

                #endregion Legal Description

                #region Footer

                var line = doc.Bottom + 10;
                collumnAligne = doc.Left + 5;
                pdf.Text($"Recebi(emos) da {Labels.LEGAL_NAME} os serviços constantes nesta", BaseFont.HELVETICA, 15, collumnAligne, line + 95, align: Element.ALIGN_LEFT);
                pdf.Text($"{Labels.DESCRIPTION} - Série ÚNICA N°: {invoice.Number.PadLeft(5, '0')}", BaseFont.HELVETICA, 15, collumnAligne, line + 75, align: Element.ALIGN_LEFT);
                pdf.Text("Data: _____/_____/__________", BaseFont.HELVETICA, 15, pdf.CenterX, line + 40, align: Element.ALIGN_CENTER);
                pdf.Text("Assinatura: _______________________________", BaseFont.HELVETICA, 15, pdf.CenterX, line + 10, align: Element.ALIGN_CENTER);

                #endregion Footer

                #region Rectaggles

                var heigthHeader = 80f;
                pdf.RoundRectangle(xInit: doc.Left, yInit: doc.Top - heigthHeader, width: (width * 0.65f) + padding, height: heigthHeader, radius: 2f);
                pdf.RoundRectangle(xInit: doc.Left + (width * 0.65f) + padding, yInit: doc.Top - heigthHeader, width: (width * 0.35f) - padding, height: heigthHeader, radius: 2f);

                var heigthClient = 65f;
                pdf.RoundRectangle(xInit: doc.Left, yInit: doc.Top - (heigthClient + heigthHeader) - 2, width: width, height: heigthClient, radius: 2f);

                var heigthExtension = 60f;
                pdf.RoundRectangle(xInit: doc.Left, yInit: doc.Top - (heigthClient + heigthHeader + heigthExtension) - 4, width: width, height: heigthExtension, radius: 2f);

                var heigthMessage = 50f;
                pdf.RoundRectangle(xInit: doc.Left, yInit: doc.Top - (heigthClient + heigthHeader + heigthExtension + heigthMessage) - 6, width: width, height: heigthMessage, radius: 2f);

                var heigthItems = 145f;
                pdf.RoundRectangle(xInit: doc.Left, yInit: doc.Top - (heigthClient + heigthHeader + heigthExtension + heigthMessage + heigthItems) - 8, width: width, height: heigthItems, radius: 2f);

                var heigthBancary = 80f;
                pdf.RoundRectangle(xInit: doc.Left, yInit: doc.Bottom + 272, width: width * 0.45f, height: heigthBancary, radius: 2f);
                pdf.RoundRectangle(xInit: doc.Left + (width * 0.45f), yInit: doc.Bottom + 272, width: width * 0.55f, height: heigthBancary, radius: 2f);

                var heigthLegalDescription = 99f;
                pdf.RoundRectangle(xInit: doc.Left, yInit: doc.Bottom + 170, width: width * 0.75f, height: heigthLegalDescription, radius: 2f);
                pdf.RoundRectangle(xInit: width * 0.75f + doc.LeftMargin, yInit: doc.Bottom + 170, width: width * 0.25f, height: heigthLegalDescription, radius: 2f);

                pdf.Line(doc.Bottom + 150);
                pdf.Line(doc.Top - 290);

                pdf.RoundRectangle(xInit: doc.Left, yInit: doc.Bottom, width: width, height: doc.Bottom + 92, radius: 2f);

                #endregion Rectaggles

                doc.Close();
                var msInfo = ms.ToArray();
                ms.Write(msInfo, 0, msInfo.Length);

                ms.Position = 0;
                return ms;
            }
            catch(InvalidOperationException)
            {
                throw new InvalidOperationException($"");
            }
        }
    }
}