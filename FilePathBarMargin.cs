using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace FilePathBar;

internal sealed class FilePathBarMargin : IWpfTextViewMargin
{
    public const string MarginName = "FilePathBar";

    private readonly IWpfTextView textView;
    private readonly ITextDocumentFactoryService textDocumentFactoryService;
    private readonly TextBox pathTextBox;
    private readonly Border root;
    private bool isDisposed;
    private ITextDocument? textDocument;

    public FilePathBarMargin(IWpfTextView textView, ITextDocumentFactoryService textDocumentFactoryService)
    {
        this.textView = textView;
        this.textDocumentFactoryService = textDocumentFactoryService;

        pathTextBox = CreatePathTextBox();
        root = CreateRoot(pathTextBox);

        textView.Closed += OnTextViewClosed;
        AttachDocument();
        UpdatePathText();
    }

    public FrameworkElement VisualElement => root;

    public double MarginSize => root.ActualHeight;

    public bool Enabled => !isDisposed;

    public ITextViewMargin? GetTextViewMargin(string marginName)
    {
        return string.Equals(marginName, MarginName, StringComparison.OrdinalIgnoreCase) ? this : null;
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        isDisposed = true;
        textView.Closed -= OnTextViewClosed;
        DetachDocument();
    }

    private static Border CreateRoot(UIElement child)
    {
        var root = new Border
        {
            MinHeight = 22,
            Padding = new Thickness(7, 1, 7, 1),
            BorderThickness = FilePathBarPlacement.IsTop ? new Thickness(0, 0, 0, 1) : new Thickness(0, 1, 0, 0),
            Child = child
        };

        root.SetResourceReference(Control.BackgroundProperty, VsBrushes.WindowKey);
        root.SetResourceReference(Border.BorderBrushProperty, VsBrushes.CommandBarBorderKey);

        return root;
    }

    private TextBox CreatePathTextBox()
    {
        var textBox = new TextBox
        {
            IsReadOnly = true,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(0),
            MinHeight = 18,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
            TextWrapping = TextWrapping.NoWrap,
            Cursor = Cursors.IBeam,
            ContextMenu = CreateContextMenu()
        };

        textBox.SetResourceReference(Control.BackgroundProperty, VsBrushes.WindowKey);
        textBox.SetResourceReference(Control.ForegroundProperty, VsBrushes.WindowTextKey);

        return textBox;
    }

    private ContextMenu CreateContextMenu()
    {
        var copyFullPathMenuItem = new MenuItem { Header = "Copy Full Path" };
        copyFullPathMenuItem.Click += (_, _) => CopyFullPath();

        var contextMenu = new ContextMenu();
        contextMenu.Items.Add(copyFullPathMenuItem);
        return contextMenu;
    }

    private void AttachDocument()
    {
        if (textDocumentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out var document))
        {
            textDocument = document;
            textDocument.FileActionOccurred += OnFileActionOccurred;
        }
    }

    private void DetachDocument()
    {
        if (textDocument is not null)
        {
            textDocument.FileActionOccurred -= OnFileActionOccurred;
            textDocument = null;
        }
    }

    private void OnTextViewClosed(object sender, EventArgs e)
    {
        Dispose();
    }

    private void OnFileActionOccurred(object sender, TextDocumentFileActionEventArgs e)
    {
        UpdatePathText();
    }

    private void UpdatePathText()
    {
        pathTextBox.Text = GetDisplayPath();
    }

    private string GetDisplayPath()
    {
        var path = textDocument?.FilePath;
        if (string.IsNullOrWhiteSpace(path))
        {
            return "Unsaved file";
        }

        return Path.GetFullPath(path);
    }

    private void CopyFullPath()
    {
        var path = textDocument?.FilePath;
        if (!string.IsNullOrWhiteSpace(path))
        {
            Clipboard.SetText(Path.GetFullPath(path));
        }
    }
}
