using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TagLib.File currentFile;

        private bool mediaPlayerIsPlaying = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Open_File_Btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";

            if (ofd.ShowDialog() == true) {
                fileNameBox.Text = ofd.FileName;
            }

            currentFile = TagLib.File.Create(ofd.FileName);

            myMediaPlayer.Source = new Uri(ofd.FileName);
        }

        private void Show_Tags_Btn_Click(object sender, RoutedEventArgs e)
        {
            var year = currentFile.Tag.Year;
            var title = currentFile.Tag.Title;

            tagNameBox.Text = title;
        }

        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (myMediaPlayer.Source != null) && (mediaPlayerIsPlaying == false);
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            myMediaPlayer.Play();
            mediaPlayerIsPlaying = true;
        }

        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (myMediaPlayer.Source != null) && (mediaPlayerIsPlaying == true);
        }

        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            myMediaPlayer.Pause();
            mediaPlayerIsPlaying = false;
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (myMediaPlayer.Source != null) && (mediaPlayerIsPlaying == true);
        }

        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            myMediaPlayer.Stop();
            mediaPlayerIsPlaying = false;
        }
    }
}