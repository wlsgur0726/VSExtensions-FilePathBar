using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace FilePathBar;

[Export(typeof(IWpfTextViewMarginProvider))]
[Name(FilePathBarMargin.MarginName)]
[MarginContainer(FilePathBarPlacement.Current)]
[ContentType("text")]
[TextViewRole(PredefinedTextViewRoles.Document)]
internal sealed class FilePathBarMarginProvider : IWpfTextViewMarginProvider
{
    [Import]
    internal ITextDocumentFactoryService TextDocumentFactoryService { get; set; } = null!;

    public IWpfTextViewMargin? CreateMargin(IWpfTextViewHost textViewHost, IWpfTextViewMargin containerMargin)
    {
        return new FilePathBarMargin(textViewHost.TextView, TextDocumentFactoryService);
    }
}
