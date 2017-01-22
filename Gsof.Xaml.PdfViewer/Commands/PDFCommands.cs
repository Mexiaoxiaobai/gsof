using System.Collections.Generic;
using System.Windows.Input;

namespace Gsof.Xaml.PdfViewer.Commands
{
    public static class PdfCommands
    {
        private static readonly Dictionary<PdfCommandsType, RoutedUICommand> Commands;

        static PdfCommands()
        {
            Commands = new Dictionary<PdfCommandsType, RoutedUICommand>();

            Commands.Add(PdfCommandsType.Open, new RoutedUICommand("Open", "Open", typeof(PdfPanel)));
            Commands.Add(PdfCommandsType.Close, new RoutedUICommand("Close", "Close", typeof(PdfPanel)));
            Commands.Add(PdfCommandsType.Zoomin, new RoutedUICommand("Zoomin", "Open", typeof(PdfPanel)));
            Commands.Add(PdfCommandsType.Zoomout, new RoutedUICommand("Zoomout", "Zoomout", typeof(PdfPanel)));
            Commands.Add(PdfCommandsType.Single, new RoutedUICommand("Single", "Single", typeof(PdfPanel)));
            Commands.Add(PdfCommandsType.Facing, new RoutedUICommand("Facing", "Facing", typeof(PdfPanel)));
            Commands.Add(PdfCommandsType.Book, new RoutedUICommand("Book", "Book", typeof(PdfPanel)));
        }

        public static RoutedUICommand Open
        {
            get { return Commands[PdfCommandsType.Open]; }
        }

        public static RoutedUICommand Close
        {
            get { return Commands[PdfCommandsType.Close]; }
        }

        public static RoutedUICommand Zoomin
        {
            get { return Commands[PdfCommandsType.Zoomin]; }
        }

        public static RoutedUICommand Zoomout
        {
            get { return Commands[PdfCommandsType.Zoomout]; }
        }

        public static RoutedUICommand Single
        {
            get { return Commands[PdfCommandsType.Single]; }
        }

        public static RoutedUICommand Facing
        {
            get { return Commands[PdfCommandsType.Facing]; }
        }

        public static RoutedUICommand Book
        {
            get { return Commands[PdfCommandsType.Book]; }
        }
    }

    public enum PdfCommandsType
    {
        Open,
        Close,
        Zoomin,
        Zoomout,
        Single,
        Facing,
        Book,
    }
}
