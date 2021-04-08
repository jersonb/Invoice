using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace PdfGenerator
{
    public class PdfHelper
    {
        #region Properties

        private readonly Document _doc;
        private readonly PdfContentByte _contentByte;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Estilo", "IDE0044:Adicionar modificador somente leitura", Justification = "Não ajuda")]
        private PdfWriter pdf;

        internal float CenterX
           => _doc.PageSize.Width / 2;

        internal float CenterY
           => _doc.PageSize.Height / 2;

        #endregion Properties

        internal PdfHelper(Document doc, MemoryStream ms)
        {
            _doc = doc;
            pdf = PdfWriter.GetInstance(doc, ms);
            pdf.CloseStream = false;
            _doc.Open();
            _contentByte = pdf.DirectContent;
        }

        #region Text

        private void Text(string text, string nameFont, int sizeFont, float positionX, float positionY, BaseColor color, int align)
        {
            var font = BaseFont.CreateFont(nameFont, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            _contentByte.SetColorFill(color);
            _contentByte.SetFontAndSize(font, sizeFont);
            _contentByte.BeginText();
            _contentByte.ShowTextAligned(align, text, positionX, positionY, 0);
            _contentByte.EndText();
        }

        internal void Text(string text, string nameFont = null, int? sizeFont = null, float? positionX = null, float? positionY = null, BaseColor color = null, int? align = null)
             => Text
                 (
                     text
                     , nameFont ?? BaseFont.HELVETICA
                     , sizeFont ?? 12
                     , positionX ?? CenterX
                     , positionY ?? CenterY
                     , color ?? BaseColor.Black
                     , align ?? Element.ALIGN_CENTER
                  );

        #endregion Text

        #region Image

        private void AddImage(Image image, float absoluteX, float absoluteY, float width, float height)
        {
            image.ScaleToFit(width, height);
            image.SetAbsolutePosition(absoluteX, absoluteY);

            _doc.Add(image);
        }

        internal void AddImage(Image image, float? absoluteX = null, float? absoluteY = null, float? width = null, float? height = null)
            => AddImage
            (
                image
                , absoluteX ?? CenterX
                , absoluteY ?? CenterY
                , width ?? image.Width
                , height ?? image.Height
            );

        #endregion Image

        #region Rectangle

        public void RoundRectangle(float xInit, float yInit, float width, float height, float radius = 0, float lineWidth = 1, float opacity = 0, BaseColor boarderColor = null, BaseColor insideColor = null)
        {
            var gs1 = new PdfGState
            {
                FillOpacity = opacity,
            };
            _contentByte.SetGState(gs1);
            _contentByte.SaveState();
            _contentByte.SetColorFill(insideColor ?? BaseColor.White);
            _contentByte.SetLineWidth(lineWidth);
            _contentByte.SetColorStroke(boarderColor ?? BaseColor.Black);
            _contentByte.RoundRectangle(xInit, yInit, width, height, radius);
            _contentByte.FillStroke();
            _contentByte.RestoreState();
        }

        #endregion Rectangle
    }
}