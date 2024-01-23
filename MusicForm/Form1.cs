using Azure.Core;
using Siticone.Desktop.UI.Designer;
using Siticone.Desktop.UI.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Speech.Synthesis.TtsEngine;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; 

namespace MusicForm
{

    public partial class Form1 : Form
    {
        private List<string> musicFiles;
        private List<string> favoriteFiles ;
        public List<string>[] albums;
        private List<string> currentList;
        private string currentSong;
        private int currentPosition;
        private bool isPause;
        private bool isChangePosition;
        private bool mouseDown;
        private bool chooseAllFile;
        private Point lastLocation;
        private bool continuePlay;
        public Form1()
        {
            InitializeComponent();
            currentPosition = 0;
            albums = new List<string>[10];
            musicFiles = new List<string>();
            favoriteFiles = new List<string>();
            currentList = new List<string>();
            currentList = null;
            albums[0] = favoriteFiles;
            isPause = false;
            chooseAllFile = true;
            isChangePosition = false;
            siticoneTrackBar1.Value = 0;
            albumsTabControl.SelectedIndex = 0;
            continuePlay = false;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X,
                    (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            indicator.Top = btnPlaying.Top;
            siticoneTabControl1.SelectTab(tabPage1);
        }

        private void siticoneButton3_Click(object sender, EventArgs e)
        {
            indicator.Top = btnExplore.Top;
            siticoneTabControl1.SelectTab(tabPage2);
        }

        private void btnAlbums_Click(object sender, EventArgs e)
        {
            indicator.Top = btnAlbums.Top;
            listBox3.Items.Clear();
            siticoneTabControl1.SelectTab(tabPage4);
            if (albums[albumsTabControl.SelectedIndex] != null)
            {
                foreach (string file in albums[albumsTabControl.SelectedIndex])
                    listBox3.Items.Add(chuyenDoiUI(Path.GetFileName(file)));
                if (isPause == false && chooseAllFile == false && currentList == albums[albumsTabControl.SelectedIndex] && currentList.Count != 0)
                    listBox3.SelectedIndex = currentPosition;
            }
        }

        private void siticoneButton4_Click(object sender, EventArgs e)
        {
            indicator.Top = btnPlaylists.Top;
            siticoneTabControl1.SelectTab(tabPage3);
            if (currentList != null)
            {
                indicator.Top = btnPlaylists.Top;
                listBox2.Items.Clear();
                if (currentList.Count == 0)
                    return;
                foreach (string file in currentList)
                    listBox2.Items.Add(chuyenDoiUI(Path.GetFileName(file)));
                listBox2.SelectedIndex = currentPosition;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void siticoneImageButton2_Click(object sender, EventArgs e)
        {
            
        }
        public string chuyenDoiUI(string str)// xoa duoi .mp3
        {

            if (str[str.Length - 4] == '.')
            {
                char[] MyChar = { '.', 'm', 'p', '3' };
                str = str.TrimEnd(MyChar);
            }
            return str;
        }
        private void importButton_Click(object sender, EventArgs e)
        {
            // mo file
            OpenFileDialog openFileDialog= new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "MP3 Files | *.mp3";
            // them goi y
            AutoCompleteStringCollection auto1 = new AutoCompleteStringCollection();
            addSongTB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            addSongTB.AutoCompleteSource = AutoCompleteSource.CustomSource;
            addSongPlaylistTB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            addSongPlaylistTB.AutoCompleteSource = AutoCompleteSource.CustomSource;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {   
                foreach(string file in openFileDialog.FileNames) 
                {
                    //musicFiles.Add(file);
                    musicFiles.Add(file);
                    string musicName = chuyenDoiUI(Path.GetFileName(file));
                    listBox1.Items.Add(musicName);
                    auto1.Add(musicName);
                    //listBox1.Items.Add(Path.GetFileName(file));
                }
                addSongTB.AutoCompleteCustomSource=auto1;
                addSongPlaylistTB.AutoCompleteCustomSource = auto1;
                if(musicFiles.Count > 0)
                {
                    StartButton.Enabled = true;
                }
            }
            if(chooseAllFile)
            {
                currentList = musicFiles;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timerPlayBack_Tick(object sender, EventArgs e)
        {
            if(continuePlay)
            {
                musicPlayer.Ctlcontrols.play();
                continuePlay = false;
            }
            if(!isChangePosition&&musicPlayer!=null)
            {
                if(musicPlayer.currentMedia.duration!=0)
                    siticoneTrackBar1.Value = ((int)musicPlayer.Ctlcontrols.currentPosition* siticoneTrackBar1.Maximum / (int) musicPlayer.currentMedia.duration);
                label6.Text = "Length: " + FormatTime(musicPlayer.Ctlcontrols.currentPosition) + " / " + FormatTime(musicPlayer.currentMedia.duration);
            }
        }

        private void PauseButton_Click(object sender, EventArgs e)//play pause 
        {
            if(isPause==false) 
            {
                musicPlayer.Ctlcontrols.pause();
                isPause = true;
                label3.Text = chuyenDoiUI(Path.GetFileName(currentList[currentPosition]));
                if (currentList == albums[albumsTabControl.SelectedIndex])
                    listBox3.SelectedIndex = currentPosition;
            }
            else
            {
                musicPlayer.Ctlcontrols.play();
                isPause = false;
                if (currentList == albums[albumsTabControl.SelectedIndex])
                    listBox3.SelectedIndex = currentPosition;
            }
        }
        private string FormatTime(double seconds)
        {
            TimeSpan time= TimeSpan.FromSeconds(seconds);
            return time.ToString(@"mm\:ss");
        }
        private void musicPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)//phat lien tuc
        {
            if (e.newState == 8 && musicPlayer.currentMedia.duration != 0)
            {
                if (currentList.Count - 1 == currentPosition)
                    currentPosition = 0;
                else currentPosition++;
                if (chooseAllFile)
                    listBox1.SelectedIndex = currentPosition;
                else if (currentList == albums[albumsTabControl.SelectedIndex] && listBox3.Items.Count - 1 >= currentPosition)
                    listBox3.SelectedIndex = currentPosition;
                currentSong = currentList[currentPosition];
                musicPlayer.URL = currentSong;
                musicPlayer.Ctlcontrols.play();
                isPause = false;
                label3.Text = chuyenDoiUI(Path.GetFileName(currentList[currentPosition]));
                listBox2.Items.Clear();
                foreach (string file in currentList)
                    listBox2.Items.Add(chuyenDoiUI(Path.GetFileName(file)));
                listBox2.SelectedIndex = currentPosition;
                listBox2.SelectedIndex = currentPosition;
                continuePlay = true;
            }
        }

        private void siticoneTrackBar1_Scroll(object sender, ScrollEventArgs e)
        {
        }

        private void siticoneTrackBar2_Scroll(object sender, ScrollEventArgs e)// chinh vollume
        {
            musicPlayer.settings.volume = siticoneTrackBar2.Value;
            label_track_vol.Text= siticoneTrackBar2.Value.ToString()+"%";
        }

        private void siticoneButton1_Click_1(object sender, EventArgs e)//play pause song
        {
            chooseAllFile = true;
            currentList = musicFiles;
            currentPosition = listBox1.SelectedIndex;
            if (currentPosition >= 0)
            {
                if (isPause)
                {
                    musicPlayer.Ctlcontrols.play();
                    isPause = false;
                }
                else
                {
                    currentSong = musicFiles[currentPosition];
                    musicPlayer.URL = currentSong;
                    musicPlayer.Ctlcontrols.play();
                }
                label3.Text = chuyenDoiUI(Path.GetFileName(currentList[currentPosition]));
                timerPlayBack.Enabled = true;
                AlbumLb.Text = "";
            }
        }

/*        public void nextSong()
        {
            if (listBox1.Items.Count - 1 == listBox1.SelectedIndex)
                listBox1.SelectedIndex = 0;
            else listBox1.SelectedIndex++;
            currentSong = musicFiles[listBox1.SelectedIndex];
            musicPlayer.URL = currentSong;
            //musicPlayer.Ctlcontrols.play();
            isPause = false;
            label3.Text = listBox1.Text;
        }*/

        private void NextButton_Click(object sender, EventArgs e)//phat bai hat ke tiep
        {
            if (currentList.Count-1 == currentPosition)
                currentPosition = 0;
            else currentPosition++;
            if (chooseAllFile)
            {
                listBox1.SelectedIndex = currentPosition;
            }
            else if (currentList == albums[albumsTabControl.SelectedIndex] && listBox3.Items.Count - 1 >= currentPosition)
                listBox3.SelectedIndex = currentPosition;
            currentSong = currentList[currentPosition];
            musicPlayer.URL = currentSong;
            musicPlayer.Ctlcontrols.play();
            isPause = false;
            label3.Text = chuyenDoiUI(Path.GetFileName(currentList[currentPosition]));
            listBox2.Items.Clear();
            foreach (string file in currentList)
                listBox2.Items.Add(chuyenDoiUI(Path.GetFileName(file)));
            listBox2.SelectedIndex = currentPosition;
            listBox2.SelectedIndex = currentPosition;

        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            if (currentPosition ==0)
                currentPosition=currentList.Count-1;
                //currentPosition = listBox2.Items.Count-1;
            else currentPosition--;
            if (chooseAllFile)
                listBox1.SelectedIndex = currentPosition;
            else if (currentList == albums[albumsTabControl.SelectedIndex] && listBox3.Items.Count - 1 >= currentPosition)
                listBox3.SelectedIndex = currentPosition;
            currentSong = currentList[currentPosition];
            musicPlayer.URL = currentSong;
            musicPlayer.Ctlcontrols.play();
            isPause = false;
            label3.Text = chuyenDoiUI(Path.GetFileName(currentList[currentPosition]));
            listBox2.SelectedIndex = currentPosition;

        }

        // chinh mute vollume
        private void siticoneImageButton5_Click(object sender, EventArgs e)
        {
            siticoneTrackBar2.Value = 0;
            musicPlayer.settings.volume = siticoneTrackBar2.Value;
            label_track_vol.Text = siticoneTrackBar2.Value.ToString() + "%";
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        //tua nhac
        private void siticoneTrackBar1_MouseDown(object sender, MouseEventArgs e)
        {
            if(currentList==null)
                return;
             musicPlayer.Ctlcontrols.currentPosition= musicPlayer.currentMedia.duration * e.X / siticoneTrackBar1.Width;
        }

        //chuyen tab
        private void siticoneTabControl1_Click(object sender, EventArgs e)
        {
            if (siticoneTabControl1.SelectedTab.Text == "Playing")
                indicator.Top = btnPlaying.Top;
            else if (siticoneTabControl1.SelectedTab.Text == "Explore")
                indicator.Top = btnExplore.Top;
            else if (siticoneTabControl1.SelectedTab.Text == "Albums")
            {
                indicator.Top = btnAlbums.Top;
                listBox3.Items.Clear();
                if (albums[albumsTabControl.SelectedIndex] != null)
                {
                    foreach (string file in albums[albumsTabControl.SelectedIndex])
                        listBox3.Items.Add(chuyenDoiUI(Path.GetFileName(file)));
                    if (isPause == false && chooseAllFile == false && currentList == albums[albumsTabControl.SelectedIndex]&&currentList.Count!=0)
                        listBox3.SelectedIndex = currentPosition;
                }
            }
            else if (siticoneTabControl1.SelectedTab.Text == "Playlists")
            {
                if(currentList!=null)
                {
                    indicator.Top = btnPlaylists.Top;
                    listBox2.Items.Clear();
                    if (currentList.Count == 0)
                        return;
                    foreach (string file in currentList)
                        listBox2.Items.Add(chuyenDoiUI(Path.GetFileName(file)));
                    listBox2.SelectedIndex = currentPosition;
                }
            }
        }
        // chinh max vollume
        private void siticoneImageButton6_Click(object sender, EventArgs e)
        {
            siticoneTrackBar2.Value = 100;
            musicPlayer.settings.volume = siticoneTrackBar2.Value;
            label_track_vol.Text = siticoneTrackBar2.Value.ToString() + "%";
        }

        // add to favorite album
        private void siticoneButton1_Click_2(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != null && listBox1.SelectedIndex!=-1)
            {
                string file = musicFiles[listBox1.SelectedIndex];
                favoriteFiles.Add(file);
                //listBox2.Items.Add(listBox1.Text);
            }

        }

        private void siticoneTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void siticoneButton6_Click(object sender, EventArgs e)
        {

        }

        private void siticoneButton4_Click_1(object sender, EventArgs e)
        {

        }

        private void siticoneButton3_Click_1(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            musicPlayer.Ctlcontrols.play();

        }
        // delete song
        private void siticoneButton8_Click(object sender, EventArgs e)
        {
            if (albums[albumsTabControl.SelectedIndex].Count==0)
            {
                MessageBox.Show("album nay khong co bai hat nao de xoa");
                return;
            }
            //tiMessageBox.Show(listBox3.SelectedIndex.ToString());
            if (listBox3.SelectedIndex==-1)
            {
                MessageBox.Show("hay chon bai hat de xoa");
                return;
            }
            int x = listBox3.SelectedIndex;
            if (listBox3.SelectedIndex == listBox3.Items.Count-1)
                x--;
            if (currentPosition == listBox3.SelectedIndex)
            {
                albums[albumsTabControl.SelectedIndex].RemoveAt(listBox3.SelectedIndex);
                listBox3.Items.RemoveAt(listBox3.SelectedIndex);
                if (albums[albumsTabControl.SelectedIndex].Count == 0)
                {
                    musicPlayer.Ctlcontrols.stop();
                    musicPlayer.URL = "";
                    albumsTabControl.SelectedIndex = 0;
                    label3.Text = "Song Title - Playing";
                    AlbumLb.Text = "";
                    return;
                }
                listBox3.SelectedIndex = x;
                currentPosition = listBox3.SelectedIndex;
                currentSong = currentList[listBox3.SelectedIndex];
                musicPlayer.URL = currentSong;
                musicPlayer.Ctlcontrols.play();
                isPause = false;
                listBox2.SelectedIndex = listBox3.SelectedIndex;
                label3.Text = chuyenDoiUI(Path.GetFileName(currentList[currentPosition]));
                timerPlayBack.Enabled = true;
            }
            else
            {
                albums[albumsTabControl.SelectedIndex].RemoveAt(listBox3.SelectedIndex);
                listBox3.Items.RemoveAt(listBox3.SelectedIndex);
            }
        }
        // add album
        private void addAlbumsBtn_Click(object sender, EventArgs e)
        {
            string s = addAlbumTB.Text;
            if (s != "")
            {
                addAlbumTB.Clear();
                albumsTabControl.TabPages.Add(s);
            }
        }
        //delete album
        private void deleteAlbumsBtn_Click(object sender, EventArgs e)
        {
            if (albumsTabControl.SelectedIndex == 0)
            {
                MessageBox.Show("Khong the xoa muc ua thich.");
                return;
            }
            if (currentList == albums[albumsTabControl.SelectedIndex])
            {
                currentPosition = 0;
                musicPlayer.Ctlcontrols.stop();
                musicPlayer.URL = "";
                //musicFiles.Clear();
                currentList.Clear();
                isPause = true;
            }
            albumsTabControl.TabPages.RemoveAt(albumsTabControl.SelectedIndex);
            albumsTabControl.SelectedIndex = 0;
            label3.Text = "Song Title - Playing";
            AlbumLb.Text = "";
        }
        //chuyen tab album
        private void albumsTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentList == albums[albumsTabControl.SelectedIndex] &&listBox3.Items.Count-1>=currentPosition)
                listBox3.SelectedIndex = currentPosition;
            listBox3.Items.Clear();
            if (albums[albumsTabControl.SelectedIndex]== null)
                albums[albumsTabControl.SelectedIndex]= new List<string>();
            foreach (string file in albums[albumsTabControl.SelectedIndex])
                listBox3.Items.Add(chuyenDoiUI(Path.GetFileName(file)));
            if (currentList == albums[albumsTabControl.SelectedIndex]&&currentList.Count>0)
                listBox3.SelectedIndex = currentPosition;
        }
        
        public  bool CheckNhac(string s1, string s2) //ktra 2 string nhac giong nhau hay kh
        {
            s1=s1.ToUpper();
            s2=s2.ToUpper();
            if (s1 == s2)
                return true;
            else return false;
        }
        public string ChuyenDoi(string str) //chuyen chuoi string theo dinh dang kieu bai hat
        {
            int i = 0;
            while (i < str.Length)
                if (str[i] == ' ')
                {
                    str=str.Remove(i, 1);
                }
                else i++;
            return str;
        }
        private void siticoneButton6_Click_1(object sender, EventArgs e)//add song
        {
            string s = addSongTB.Text;
            s = ChuyenDoi(s);
            if (s != "")
            {
                bool check = false;
                addAlbumTB.Clear();
                for(int i=0;i<listBox1.Items.Count;i++)
                    if (CheckNhac(s, listBox1.Items[i].ToString()))
                    {
                        albums[albumsTabControl.SelectedIndex].Add(musicFiles[i]);
                        listBox3.Items.Add(listBox1.Items[i].ToString());
                        check= true;
                    }
                if (!check)
                    MessageBox.Show("Khong co bai hat nay trong danh sach.");
            }
            addSongTB.Clear();
        }
        public void PrintMusicList(List<string> s) // xuat dsach bai hat
        {
            string st = "";
            foreach(string file in s)
            {
                st += "file\n";
            }
            MessageBox.Show(st);
        }
        private void playAlbumsBtn_Click(object sender, EventArgs e) // chon album va phat
        {
            if (albums[albumsTabControl.SelectedIndex] ==null )
            {
                MessageBox.Show("Album rong");
                return;
            }
            if (albums[albumsTabControl.SelectedIndex].Count == 0)
            {
                MessageBox.Show("Album rong");
                return;
            }
            if (listBox3.SelectedIndex == -1)
            {
                currentPosition = 0;
                listBox3.SelectedIndex= 0;
            }
            else
                currentPosition = listBox3.SelectedIndex;
            currentList = albums[albumsTabControl.SelectedIndex];
            foreach (string file in currentList)
                listBox2.Items.Add(chuyenDoiUI(Path.GetFileName(file)));
            listBox2.SelectedIndex = currentPosition;
            chooseAllFile = false;
            if (currentPosition == -1 || currentPosition == null)
                currentPosition = 0;
            if (currentPosition >= 0)
            {
                currentSong = currentList[currentPosition];
                musicPlayer.URL = currentSong;
                musicPlayer.Ctlcontrols.play();
                isPause = false;
                // neu list box 2 rong ma them thi sao...
                //listBox2 = listBox3;
                listBox2.SelectedIndex = currentPosition;
                label3.Text = chuyenDoiUI(Path.GetFileName(currentList[currentPosition]));
                timerPlayBack.Enabled = true;
                AlbumLb.Text= "from " + albumsTabControl.SelectedTab.Text;
            }
        }

        private void listBox1_ControlAdded(object sender, ControlEventArgs e)
        {
            
        }

        private void siticoneButton3_Click_2(object sender, EventArgs e)
        {

        }

        private void AddSongBtnPlaylist_Click(object sender, EventArgs e)//them bai hat vao danh sach dang phat
        {
            string s = addSongPlaylistTB.Text;
            /*if(chooseAllFile)
            {
                addSongPlaylistTB.Clear();
                MessageBox.Show("Khong them bai hat vao danh sach goc\nHay tao album cho rieng ban");
                return;
            }*/
            s = ChuyenDoi(s);
            if (s != "")
            {
                bool check = false;
                for (int i = 0; i < listBox1.Items.Count; i++)
                    if (CheckNhac(s, listBox1.Items[i].ToString()))
                    {
                        currentList.Add(musicFiles[i]);
                        listBox2.Items.Add(listBox1.Items[i].ToString());
                        check = true;
                    }
                if (!check)
                    MessageBox.Show("Khong co bai hat nay trong danh sach.");
            }
            addSongPlaylistTB.Clear();
        }

        private void deleteSongBtnPlaylist_Click(object sender, EventArgs e)//xoa bai hat danh sach dang phat
        {
            if (currentList.Count == 0)
            {
                MessageBox.Show("album nay khong co bai hat nao de xoa");
                return;
            }
            if (listBox2.SelectedIndex == -1)
            {
                MessageBox.Show("hay chon bai hat de xoa");
                return;
            }
            int x = listBox2.SelectedIndex;
            if (listBox2.SelectedIndex == listBox2.Items.Count - 1)
                x--;
            if (currentPosition == listBox2.SelectedIndex)
            {
                currentList.RemoveAt(listBox2.SelectedIndex);
                listBox2.Items.RemoveAt(listBox2.SelectedIndex);
                if (currentList.Count == 0)
                {
                    musicPlayer.Ctlcontrols.stop();
                    musicPlayer.URL = "";
                    //albumsTabControl.SelectedIndex = 0;
                    label3.Text = "Song Title - Playing";
                    return;
                }
                listBox2.SelectedIndex = x;
                currentPosition = listBox2.SelectedIndex;
                currentSong = currentList[listBox2.SelectedIndex];
                musicPlayer.URL = currentSong;
                musicPlayer.Ctlcontrols.play();
                isPause = false;
                //listBox2.SelectedIndex = listBox3.SelectedIndex;
                label3.Text = chuyenDoiUI(Path.GetFileName(currentList[currentPosition]));
                timerPlayBack.Enabled = true;
            }
            else
            {
                currentList.RemoveAt(listBox2.SelectedIndex);
                listBox2.Items.RemoveAt(listBox2.SelectedIndex);
            }
        }
    }
}