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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using DuffTv.Configurator;

namespace DuffTv.TvGuideBackend
{
    public class Programme
    {

        private Config prefs = new Config();
        private string _channel;
        private string _programDate;
        private string _programType;
        private string _title;
        private DateTime _startTime;
        private DateTime _stopTime;
        private string _language;
        private string _description;
        
        
        


	

        public Programme(string pChannel, string pProgramDate, string pProgramType,
                        string pTitle, string pLanguage, DateTime pStartTime, DateTime pEndTime,
                        string pDescription)
        {
            _channel = pChannel;
            _programDate = pProgramDate;
            _programType = pProgramType;
            _title = pTitle;
            _language = pLanguage;
            _startTime = pStartTime;
            _stopTime = pEndTime;
            _description = pDescription;

        }

        public Bitmap ChannelImage
        {
            get
            {
                Bitmap _channelImage = new Bitmap(prefs.ImagesStoragePath + "\\" + this.Channel + ".png");
                return _channelImage;
            }

        }

        public string Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        public string ProgramDate
        {
            get { return _programDate; }
            set { _programDate = value; }
        }

        public string Type
        {
            get { return _programType; }
            set { _programType = value; }
        }

        public string Caption
        {
            get { return _title; }
            set { _title = value; }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        public DateTime StopTime
        {
            get { return _stopTime; }
            set { _stopTime = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }


    }

    public class ProgrammeCollection : CollectionBase

    {

        public ProgrammeCollection GetCurrentShows()
        {
            ProgrammeCollection _onNow = new ProgrammeCollection();
            DateTime _now = DateTime.Now;

            foreach (Programme _prog in this)
            {
                //test, remove when working
                int test;
                test = _prog.StartTime.CompareTo(_now);
                string test2 = test.ToString();

                int _currentPos = this.InnerList.IndexOf(_prog);

                if (_prog.StartTime < _now && this[_currentPos + 1].StartTime > _now)
                {
                    _onNow.Add(_prog); //add show on right now
                }
                else
                {
                    //do nada
                }
            }

            return _onNow;
            
        }

        public Programme this[int index]
        {
            get { return ((Programme)(List[index])); }
            set { List[index] = value; }
        }

        public int Add(Programme oChannel)
        {
            return List.Add(oChannel);
        }

        public void Insert(int index, Programme oChannel)
        {
            List.Insert(index, oChannel);
        }

        public int IndexOf(Programme oChannel)
        {
            return List.IndexOf(oChannel);
        }

        public void Remove(Programme oChannel)
        {
            List.Remove(oChannel);
        }

        public bool Contains(Programme oChannel)
        {
            return List.Contains(oChannel);
        }

        public int Count()
        {
            return List.Count;
        }


    }
}