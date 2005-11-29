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
using System.Text;
using System.IO;
using Nini.Config;

namespace DuffTv.Configurator
{
    public class Config 
    {
        private IConfigSource source;
        private string _Version;
        private Boolean _AutoUpdate;
        private string _ConnectionCheckURI;
        private string _XMLTVSourceURI;
        private string _Country;
        private Boolean _CreatedNewFile;
        private string _LastUpdate;
        private int _IconSize;
        private string[] _ChannelList;

        public Config()
        {
            //check if there is any config file, if not create a new one with some defaults...
            if (File.Exists(AppPath + "\\dufftv.ini"))
            {
                //declare new source ini file
                source = new IniConfigSource(AppPath + "\\dufftv.ini");

                //turn on autosaving, no need to save manually
                source.AutoSave = true;

                // Creates two Boolean aliases.
                source.Alias.AddAlias("On", true);
                source.Alias.AddAlias("Off", false);

                _Version = source.Configs["defaults"].GetString("Version");
                _AutoUpdate = source.Configs["defaults"].GetBoolean("AutoUpdate", false);
                _ConnectionCheckURI = source.Configs["defaults"].GetString("ConnectionCheckURI", "http://www.google.se");
                _LastUpdate = _Version = source.Configs["defaults"].GetString("LastUpdate");
                _IconSize =  source.Configs["defaults"].GetInt("IconSize");
                _XMLTVSourceURI = source.Configs["xmltv"].GetString("SourceURI");
                _Country = source.Configs["xmltv"].GetString("Country");
                _ChannelList = source.Configs["xmltv"].Get("ChannelList").Split('|');

                _CreatedNewFile = false;
            }
            else
            {
                CreateNewConfigFile();
            }
        }
        /// <summary>
        /// Creates a new config file with some defaults in it...
        /// </summary>
        private void CreateNewConfigFile()
        {
            IniConfigSource source = new IniConfigSource();

            IConfig config = source.AddConfig("defaults");
            config.Set("AutoUpdate", "Off");
            config.Set("ConnectionCheckURI", "http://www.google.se");
            config.Set("Version", "unknown");
            config.Set("LastUpdate", "unknown");
            config.Set("IconSize", "32");

            config = source.AddConfig("xmltv");
            config.Set("SourceURI", "http://tv.swedb.se/xmltv/");
            config.Set("Country", "Sweden");
            config.Set("ChannelList", "svt1.svt.se|svt2.svt.se|tv3.viasat.se|tv4.se|kanal5.se|ztv.se");


            source.Save(AppPath + "\\dufftv.ini");
            _CreatedNewFile = true;
        }

        public string[] ChannelList
        {
            get
            {
                for (int i = 0; i < _ChannelList.Length; i++)
                {
                    _ChannelList[i] += "_" + String.Format("{0:yyyy-MM-dd}", DateTime.Now) + ".xml.gz";
                }

                return _ChannelList;
            }
        }

        public int IconSize
        {
            get
            {
                return _IconSize;
            }
        }

        public string LastUpdate
        {
            get
            {
                return _LastUpdate;
            }
        }


        public string Version
        {
            get
            {
                return _Version;
            }
        }

        public Boolean CreatedNewConfigFile
        {
            get
            {
                return _CreatedNewFile;
            }
        }

        public Boolean AutoUpdate
        {
            get
            {
                return _AutoUpdate;
            }
            set
            {
                _AutoUpdate = value;
                source.Configs["defaults"].Set("AutoUpdate", _AutoUpdate);
            }
        }

        public string ConnectionCheckURI
        {
            get
            {
                return _ConnectionCheckURI;
            }
            set
            {
                _ConnectionCheckURI = value;
                source.Configs["defaults"].Set("ConnectionCheckURI", _ConnectionCheckURI);
            }
        }

        public string XMLTVSourceURI
        {
            get
            {
                return _XMLTVSourceURI;
            }
            set
            {
                _XMLTVSourceURI = value;
                source.Configs["xmltv"].Set("SourceURI", _XMLTVSourceURI);
            }
        }

        public string Country
        {
            get
            {
                return _Country;
            }
            set
            {
                _Country = value;
                source.Configs["xmltv"].Set("Country", _Country);
            }
        }

        public string AppPath
        {
            get
            {
                string path;
                path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                return path;

            }
        }

        public string XMLFileStoragePath
        {
            get
            {
                return this.AppPath + "\\xmltv\\";
            }
        }

        public string ImagesStoragePath
        {
            get
            {
                return this.AppPath + "\\images\\";
            }
        }

    }
}
