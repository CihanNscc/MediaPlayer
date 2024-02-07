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
            Tags_Editing_Enabled(false);
        }

        private void Open_File_Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (myMediaPlayer.Source != null)
                {
                    Stop_Playing();
                    Reset_Tags(false);
                }
                else
                {
                    Reset_Tags(true);
                }

                Tags_Editing_Enabled(false);

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
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while opening the file: {ex.Message}");
            }
        }

        private void Show_Playing_Btn_Click(object sender, RoutedEventArgs e)
        {
            Show_Now_Playing(true);
        }

        private void Show_Tags_Btn_Click(object sender, RoutedEventArgs e)
        {
            Show_Now_Playing(false);
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
            Stop_Playing();
        }

        public void Show_Now_Playing(bool nowPlaying)
        {
            if (nowPlaying)
            {
                playingButton.IsEnabled = false;
                tagsButton.IsEnabled = true;
                playingWindow.Visibility = Visibility.Visible;
                tagsWindow.Visibility = Visibility.Collapsed;
            }
            else
            {
                playingButton.IsEnabled = true;
                tagsButton.IsEnabled = false;
                playingWindow.Visibility = Visibility.Collapsed;
                tagsWindow.Visibility = Visibility.Visible;
            }
        }

        public void Stop_Playing()
        {
            myMediaPlayer.Stop();
            mediaPlayerIsPlaying = false;
            mediaPlayerIsStopped = true;
            playingWindow.tagDisplayMessage.Text = "Stopped";
        }

        public void Reset_Tags(bool clear)
        {
            if (clear)
            {
                tagsWindow.tagTitleBox.Text = "";
                tagsWindow.tagAlbumBox.Text = "";
                tagsWindow.tagYearBox.Text = "";
            }
            else
            {
                try
                {
                    tagsWindow.tagTitleBox.Text = currentFile.Tag.Title;
                    tagsWindow.tagAlbumBox.Text = currentFile.Tag.Album;
                    tagsWindow.tagYearBox.Text = currentFile.Tag.Year.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while saving the tags: {ex.Message}");
                }
            }
        }

        public void Tags_Editing_Enabled(bool editing)
        {
            if (editing)
            {
                tagsWindow.editButton.IsEnabled = false;
                tagsWindow.saveButton.IsEnabled = true;
                tagsWindow.cancelButton.IsEnabled = true;

                tagsWindow.tagTitleBox.IsReadOnly = false;
                tagsWindow.tagAlbumBox.IsReadOnly = false;
                tagsWindow.tagYearBox.IsReadOnly = false;
            }
            else
            {
                tagsWindow.editButton.IsEnabled = true;
                tagsWindow.saveButton.IsEnabled = false;
                tagsWindow.cancelButton.IsEnabled = false;

                tagsWindow.tagTitleBox.IsReadOnly = true;
                tagsWindow.tagAlbumBox.IsReadOnly = true;
                tagsWindow.tagYearBox.IsReadOnly = true;
            }
        }

        public void Edit_Tags()
        {
            tagsWindow.tagTitleBox.IsReadOnly = false;
            tagsWindow.tagAlbumBox.IsReadOnly = false;
            tagsWindow.tagYearBox.IsReadOnly = false;
            Tags_Editing_Enabled(true);
            Stop_Playing();
        }

        public void Save_Tags()
        {

            if (int.TryParse(tagsWindow.tagYearBox.Text, out int year))
            {
                if (myMediaPlayer.Source != null)
                {
                    try
                    {
                        myMediaPlayer.Close();
                        Tags_Editing_Enabled(false);

                        currentFile.Tag.Title = tagsWindow.tagTitleBox.Text;
                        currentFile.Tag.Album = tagsWindow.tagAlbumBox.Text;
                        currentFile.Tag.Year = (uint)year;

                        currentFile.Save();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while saving the tags: {ex.Message}");
                    }
                }
                else
                {
                    Reset_Tags(true);
                    Tags_Editing_Enabled(false);
                    MessageBox.Show("No file has been chosen to save the tags for.");
                }
            }
            else
            {
                Tags_Editing_Enabled(false);
                MessageBox.Show("Invalid year format. Please enter a valid number.");
                if (myMediaPlayer.Source != null)
                {
                    Reset_Tags(false);
                }
                else
                {
                    Reset_Tags(true);
                }
            }

        }

        public void Cancel_Tags()
        {
            if (myMediaPlayer.Source != null)
            {
                Reset_Tags(false);
            }
            else
            {
                Reset_Tags(true);
            }
            Tags_Editing_Enabled(false);
        }
    }
}