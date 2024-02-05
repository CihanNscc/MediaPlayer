using Microsoft.Win32;
using System.Security.Cryptography;
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
        private bool mediaPlayerIsStopped = true;

        public MainWindow()
        {
            InitializeComponent();
            playingButton.IsEnabled = false;
            tagsButton.IsEnabled = true;
            playingWindow.Visibility = Visibility.Visible;
            tagsWindow.Visibility = Visibility.Collapsed;
        }

        private void Open_File_Btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";

            if (ofd.ShowDialog() == true)
            {
                currentFile = TagLib.File.Create(ofd.FileName);

                myMediaPlayer.Source = new Uri(ofd.FileName);
                playingWindow.tagTitleDisplay.Text = ofd.FileName;
                tagsWindow.tagTitleBox.Text = currentFile.Tag.Title;
                tagsWindow.tagAlbumBox.Text = currentFile.Tag.Album;
                tagsWindow.tagYearBox.Text = currentFile.Tag.Year.ToString();
            }
        }

        private void Show_Playing_Btn_Click(object sender, RoutedEventArgs e)
        {
            playingButton.IsEnabled = false;
            tagsButton.IsEnabled = true;
            playingWindow.Visibility = Visibility.Visible;
            tagsWindow.Visibility = Visibility.Collapsed;
        }

        private void Show_Tags_Btn_Click(object sender, RoutedEventArgs e)
        {
            playingButton.IsEnabled = true;
            tagsButton.IsEnabled = false;
            playingWindow.Visibility = Visibility.Collapsed;
            tagsWindow.Visibility = Visibility.Visible;
        }

        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (myMediaPlayer.Source != null) && (mediaPlayerIsPlaying == false);
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            myMediaPlayer.Play();
            mediaPlayerIsPlaying = true;
            mediaPlayerIsStopped = false;
            playingWindow.tagDisplayMessage.Text = "Playing";
        }

        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (myMediaPlayer.Source != null) && (mediaPlayerIsPlaying == true);
        }

        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            myMediaPlayer.Pause();
            mediaPlayerIsPlaying = false;
            playingWindow.tagDisplayMessage.Text = "Paused";
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (myMediaPlayer.Source != null) && (mediaPlayerIsStopped == false);
        }

        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            myMediaPlayer.Stop();
            mediaPlayerIsPlaying = false;
            mediaPlayerIsStopped = true;
            playingWindow.tagDisplayMessage.Text = "Stopped";
        }

        public void Save_Tags()
        {
            myMediaPlayer.Close();
            currentFile.Tag.Title = tagsWindow.tagTitleBox.Text;
            currentFile.Tag.Album = tagsWindow.tagAlbumBox.Text;

            if (int.TryParse(tagsWindow.tagYearBox.Text, out int year))
            {
                currentFile.Tag.Year = (uint)year;
            }
            else
            {
                MessageBox.Show("Invalid year format. Please enter a valid number.");
            }

            currentFile.Save();
        } 
    }
}