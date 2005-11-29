/*
 *  Copyright 2005 GotCode Team <http://gotcode.jelica.se>
 * 
 *  This file is part of DuffTV.
 *
 *  DuffTV is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  DuffTV is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *
 *  You should have received a copy of the GNU General Public License
 *  along with DuffTV; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using DuffTv.TvGuideBackend;
using System.Collections;
using OwnerDrawnListFWProject;
using DuffTv.Configurator;


namespace DuffTv.Presentation
{
    public partial class MainForm : Form
    {
        private Config prefs;

        public MainForm()
        {
            InitializeComponent();
             prefs = new Config();
             if (prefs.CreatedNewConfigFile)
             {
                 MessageBox.Show("New configuration file created. You are advised to look thru Options menu."
                 , "Info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                                
             }

             CreateOnNowListing();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void btnGetListings_Click(object sender, EventArgs e)
        {
            //do nada

        }
        private ProgrammeCollection GetProgrammes()
        {
            const String nl = "\r\n";

            XMLTVParser parser;
            String strXMLFileContents = "";
            String tvdbURI = prefs.XMLTVSourceURI;
            ProgrammeCollection parsedObj = new ProgrammeCollection();

            string[] kanaler = prefs.ChannelList;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                for (int i = 0; i < kanaler.Length ;i++ )
                {
                    string uri = tvdbURI + kanaler[i].ToString();
                    string strFileName;

                    Leecher _leech = new Leecher(uri);
                    //LOG: String.Format("Starting download of file: {0}", uri) + nl+ nl ;

                    _leech.GetListings(false);

                    strFileName = _leech.PathToFile;

                    if (strFileName.EndsWith(".xml.gz"))
                    {
                        //LOG: this
                        // decompress the gziped file to string, and send content to xmltv parser
                        GzipDecompressor gzPack = new GzipDecompressor();
                        strXMLFileContents = gzPack.Decompress(strFileName);
                        
                    }
                    else
                    {
                        if (strFileName.EndsWith(".xml"))
                        {
                            //LOG: this
                            //no need for decompression, read filestream into string at once...
                            StreamReader st = new StreamReader(strFileName);
                            strXMLFileContents = st.ReadToEnd();
                        }
                        else
                            throw new Exception("Unknow filetype. Must be XML or XML.GZ");

                    }

                    //LOG: this "File downloaded: " + Path.GetFileName(strFileName) + nl + nl;

                    parser = new XMLTVParser(strXMLFileContents);
                    foreach (Programme oProg in parser.ParsedProgrammes)
                    {
                        parsedObj.Add(oProg);
                    }
                    

                }
               

            }
            catch (Exception downloadExcepetion)
            {
                MessageBox.Show(downloadExcepetion.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
            Cursor.Current = Cursors.Default;

            return parsedObj;
        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox1.BackColor = Color.Blue;

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox1.BackColor = Color.White;

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            CreateOnNowListing();
        }

        public void CreateOnNowListing()
        {
            ProgrammeCollection _tvToday = GetProgrammes();
            ProgrammeCollection _tvNow = _tvToday.GetCurrentShows();


            CustomListBox custListBox = new CustomListBox(prefs.IconSize);
            custListBox.BackColor = Color.White;
            //custListBox.ForeColor = Color.White;

            custListBox.Width = 234;
            custListBox.Height = 230;
            custListBox.Location = new Point(3, 3);



            ImageList _channelImageList = new ImageList();
            _channelImageList.ImageSize = new Size(prefs.IconSize, prefs.IconSize);
            int i = 0;

            //Populate items
            foreach (Programme prog in _tvNow)
            {
                int _currentPos = _tvToday.IndexOf(prog);
                Programme _nextShow = _tvToday[_currentPos + 1];

                ListItem item1 = new ListItem();

                item1.Text = String.Format("{0:HH:mm}", prog.StartTime) + " " + prog.Caption;
                item1.Text += "\r\n" + String.Format("{0:HH:mm}", _nextShow.StartTime) + " " + _nextShow.Caption;
                _channelImageList.Images.Add(prog.ChannelImage);
                item1.ImageIndex = i;
                custListBox.Items.Add(item1);
                i++;
            }

            //Assign ImageList
            custListBox.ImageList = _channelImageList;

            this.Controls.Add(custListBox);

        }


    }
}