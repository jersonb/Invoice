using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;

namespace PdfGenerator
{
    public class GeneratePdf
    {
        public Uri GitHubAvatar { get; }

        public GeneratePdf(Uri urlGitHub)
        {
            GitHubAvatar = urlGitHub;
        }

        public MemoryStream PdfInMemoryStream => Generate();

        private MemoryStream Generate()
        {
            try
            {
                var gitHubAvata = Image.GetInstance(GitHubAvatar);

                var ms = new MemoryStream();
                var doc = new Document(new Rectangle(urx: gitHubAvata.Width, ury: gitHubAvata.Height));
                var pdf = new PdfHelper(doc, ms);

                pdf.AddImage(gitHubAvata, absoluteX: 0, absoluteY: 0);
                pdf.Text("teste", sizeFont: 20, positionX: 20, positionY: 60, nameFont: BaseFont.HELVETICA_BOLD, align: Element.ALIGN_LEFT, color: BaseColor.White);

                doc.Close();
                var msInfo = ms.ToArray();
                ms.Write(msInfo, 0, msInfo.Length);

                ms.Position = 0;
                return ms;
            }
            catch(InvalidOperationException)
            {
                throw new InvalidOperationException($"algo de errado com o link {GitHubAvatar.AbsoluteUri.Replace(".png", "")}");
            }
        }
    }
}