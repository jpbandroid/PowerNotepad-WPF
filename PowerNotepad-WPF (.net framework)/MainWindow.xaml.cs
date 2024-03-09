using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Fluent;
using ScintillaNET.WPF;
using ScintillaNET;
using Microsoft.Win32;
using System.IO;
using AutoUpdaterDotNET;

namespace PowerNotepad_WPF__.net_framework_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : RibbonWindow
    {
        bool isDebug;
        private const string NEW_DOCUMENT_TEXT = "Untitled";
        private const int LINE_NUMBERS_MARGIN_WIDTH = 30; // TODO - don't hardcode this

        /// <summary>
        /// the background color of the text area
        /// </summary>
        private const int BACK_COLOR = 0x2A211C;

        /// <summary>
        /// default text color of the text area
        /// </summary>
        private const int FORE_COLOR = 0xB7B7B7;

        /// <summary>
        /// change this to whatever margin you want the line numbers to show in
        /// </summary>
        private const int NUMBER_MARGIN = 1;

        /// <summary>
        /// change this to whatever margin you want the bookmarks/breakpoints to show in
        /// </summary>
        private const int BOOKMARK_MARGIN = 2;

        private const int BOOKMARK_MARKER = 2;

        /// <summary>
        /// change this to whatever margin you want the code folding tree (+/-) to show in
        /// </summary>
        private const int FOLDING_MARGIN = 3;

        /// <summary>
        /// set this true to show circular buttons for code folding (the [+] and [-] buttons on the margin)
        /// </summary>
        private const bool CODEFOLDING_CIRCULAR = false;
        public MainWindow()
        {
            InitializeComponent();
            InitNumberMargin(scintillatext);
            InitSyntaxColoring(scintillatext);
            string extendedUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            string userName = Environment.UserName;
            user.Text = userName;
        }

        private void About(object sender, RoutedEventArgs e)
        {

        }

        private void OpenClick(object sender, RoutedEventArgs e)
        {
            Open();
            InitSyntaxColoring(scintillatext);
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Save();
            InitSyntaxColoring(scintillatext);
        }

        private void showinsiderinfo(object sender, RoutedEventArgs e)
        {

        }

        private void update(object sender, RoutedEventArgs e)
        {
            if (isDebug == true)
            {
                AutoUpdater.Start("https://raw.githubusercontent.com/jpbandroid/jpbOffice-Resources/main/PowerNotepad/updateinfo_debug.xml");

            }
            else
            {
                AutoUpdater.Start("https://raw.githubusercontent.com/jpbandroid/jpbOffice-Resources/main/PowerNotepad/updateinfo.xml");
            }
        }

        private void InitNumberMargin(ScintillaWPF ScintillaNet)
        {
            ScintillaNet.Styles[ScintillaNET.Style.LineNumber].BackColor = IntToColor(BACK_COLOR);
            ScintillaNet.Styles[ScintillaNET.Style.LineNumber].ForeColor = IntToColor(FORE_COLOR);
            ScintillaNet.Styles[ScintillaNET.Style.IndentGuide].ForeColor = IntToColor(FORE_COLOR);
            ScintillaNet.Styles[ScintillaNET.Style.IndentGuide].BackColor = IntToColor(BACK_COLOR);

            var nums = ScintillaNet.Margins[NUMBER_MARGIN];
            nums.Width = LINE_NUMBERS_MARGIN_WIDTH;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

            ScintillaNet.MarginClick += TextArea_MarginClick;
        }

        private void TextArea_MarginClick(object sender, MarginClickEventArgs e)
        {
        }

        public static System.Drawing.Color IntToColor(int rgb)
        {
            return System.Drawing.Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        public bool Save()
        {
            if (String.IsNullOrEmpty(_filePath))
                return SaveAs();

            return Save(_filePath);
        }

        private void Open()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool? res = openFileDialog.ShowDialog();
            if (res == null || !(bool)res)
                return;
            _filePath = openFileDialog.FileName;
            OpenFile(_filePath);
        }

        private void OpenFile(string filePath)
        {
            scintillatext.Text = File.ReadAllText(filePath);
            //doc.Scintilla.UndoRedo.EmptyUndoBuffer();
            //doc.Scintilla.Modified = false;
            Title = System.IO.Path.GetFileName(filePath) + " - PowerNotepad";
            FilePath = filePath;
            //incrementalSearcher.Scintilla = doc.Scintilla;

            return;
        }

        public bool Save(string filePath)
        {
            using (FileStream fs = File.Create(filePath))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                    bw.Write(scintillatext.Text.ToCharArray(), 0, scintillatext.Text.Length - 1); // Omit trailing NULL
            }
            this.Title = System.IO.Path.GetFileName(filePath) + " - PowerNotepad";

            scintillatext.SetSavePoint();
            return true;
        }

        public bool SaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            bool? res = saveFileDialog.ShowDialog();
            if (res != null && (bool)res)
            {
                _filePath = saveFileDialog.FileName;
                return Save(_filePath);
            }

            return false;
        }

        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        private void InitSyntaxColoring(ScintillaWPF ScintillaNet)
        {
            // Configure the default style
            ScintillaNet.StyleResetDefault();
            ScintillaNet.Styles[ScintillaNET.Style.Default].Font = "Consolas";
            ScintillaNet.Styles[ScintillaNET.Style.Default].Size = 10;
            ScintillaNet.Styles[ScintillaNET.Style.Default].BackColor = IntToColor(0x212121);
            ScintillaNet.Styles[ScintillaNET.Style.Default].ForeColor = IntToColor(0xFFFFFF);
            ScintillaNet.Styles[ScintillaNET.Style.LineNumber].BackColor = IntToColor(BACK_COLOR);
            ScintillaNet.Styles[ScintillaNET.Style.LineNumber].ForeColor = IntToColor(FORE_COLOR);
            ScintillaNet.Styles[ScintillaNET.Style.IndentGuide].ForeColor = IntToColor(FORE_COLOR);
            ScintillaNet.Styles[ScintillaNET.Style.IndentGuide].BackColor = IntToColor(BACK_COLOR);
            ScintillaNet.StyleClearAll();

            // Configure the CPP (C#) lexer styles
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.Identifier].ForeColor = IntToColor(0xD0DAE2);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.Comment].ForeColor = IntToColor(0xBD758B);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.CommentLine].ForeColor = IntToColor(0x40BF57);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.CommentDoc].ForeColor = IntToColor(0x2FAE35);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.Number].ForeColor = IntToColor(0xFFFF00);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.String].ForeColor = IntToColor(0xFFFF00);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.Character].ForeColor = IntToColor(0xE95454);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.Preprocessor].ForeColor = IntToColor(0x8AAFEE);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.Operator].ForeColor = IntToColor(0xE0E0E0);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.Regex].ForeColor = IntToColor(0xff00ff);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.CommentLineDoc].ForeColor = IntToColor(0x77A7DB);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.Word].ForeColor = IntToColor(0x48A8EE);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.Word2].ForeColor = IntToColor(0xF98906);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.CommentDocKeyword].ForeColor = IntToColor(0xB3D991);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.CommentDocKeywordError].ForeColor = IntToColor(0xFF0000);
            ScintillaNet.Styles[ScintillaNET.Style.Cpp.GlobalClass].ForeColor = IntToColor(0x48A8EE);
            ScintillaNet.Styles[ScintillaNET.Style.LineNumber].BackColor = IntToColor(BACK_COLOR);
            ScintillaNet.Styles[ScintillaNET.Style.LineNumber].ForeColor = IntToColor(FORE_COLOR);
            ScintillaNet.Styles[ScintillaNET.Style.IndentGuide].ForeColor = IntToColor(FORE_COLOR);
            ScintillaNet.Styles[ScintillaNET.Style.IndentGuide].BackColor = IntToColor(BACK_COLOR);

            ScintillaNet.Lexer = Lexer.Cpp;

            ScintillaNet.SetKeywords(0, "class extends implements import interface new case do while else if for in switch throw get set function var try catch finally while with default break continue delete return each const namespace package include use is as instanceof typeof author copy default deprecated eventType example exampleText exception haxe inheritDoc internal link mtasc mxmlc param private return see serial serialData serialField since throws usage version langversion playerversion productversion dynamic private public partial static intrinsic internal native override protected AS3 final super this arguments null Infinity NaN undefined true false abstract as base bool break by byte case catch char checked class const continue decimal default delegate do double descending explicit event extern else enum false finally fixed float for foreach from goto group if implicit in int interface internal into is lock long new null namespace object operator out override orderby params private protected public readonly ref return switch struct sbyte sealed short sizeof stackalloc static string select this throw true try typeof uint ulong unchecked unsafe ushort using var virtual volatile void while where yield");
            ScintillaNet.SetKeywords(1, "void Null ArgumentError arguments Array Boolean Class Date DefinitionError Error EvalError Function int Math Namespace Number Object RangeError ReferenceError RegExp SecurityError String SyntaxError TypeError uint XML XMLList Boolean Byte Char DateTime Decimal Double Int16 Int32 Int64 IntPtr SByte Single UInt16 UInt32 UInt64 UIntPtr Void Path File System Windows Forms ScintillaNET");
        }
    }
}
