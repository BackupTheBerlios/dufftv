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
using System.Xml;
using System.Globalization;

namespace DuffTv.TvGuideBackend
{
    public class XMLTVParser
    {
        private string m_Content;
        private ProgrammeCollection parsedProgs;


        public XMLTVParser(string content)
        {
            Load(content);
            
        }

        private void Load(string content)
        {
            //make sure we convert everything to lower-case, for string-matching purpose
            m_Content = content;

            try
            {
                //Validate against the schema
                //Validate(content);

                //Parse document
                ParseDoc();
            }
            catch (XmlException xmlExc)
            {
                throw new Exception("Error parsing XML file. Msg: " + xmlExc.Message);
            }
            catch (Exception ex1)
            {
                throw new Exception("Undefined error. Msg: " + ex1.Message);
            }
     

        }


        public ProgrammeCollection ParsedProgrammes
        {
            get
            {
                return parsedProgs;
            }
            set
            {
                parsedProgs = value;
            }
        }
        /// <summary>
        /// Convert time from xml file to something we can type as DateTime
        /// </summary>
        /// <param name="xmlTime">time from xml file in format:YYYYMMDDhhmmss ZONE</param>
        /// <returns>DateTime</returns>
        private DateTime ConvertTime(string xmlTime)
        {
            DateTime convertedTime = DateTime.Now;

            try
            {

                //parse date, wanted format:YYYY-MM-DD hh:mm:ss
                //xmltv format: YYYYMMDDhhmmss ZONE
                xmlTime = xmlTime.Insert(4, "-");
                xmlTime = xmlTime.Insert(7, "-");
                xmlTime = xmlTime.Insert(10, " ");
                xmlTime = xmlTime.Insert(13, ":");
                xmlTime = xmlTime.Insert(16, ":");
                xmlTime = xmlTime.Insert(19, " ");
                xmlTime = xmlTime.Remove(19, (xmlTime.Length - 19));

                convertedTime = DateTime.Parse(xmlTime);
            }

            catch (FormatException)
            {
                throw new Exception("Could not convert time.");
            }

            return convertedTime;
        }

	

        private void ParseDoc()
        {
             XmlDocument xmlDoc;
             XmlNode rootNode;

             try
             {
                 //remove <!DOCTYPE tv SYSTEM "xmltv.dtd"> string, 'cause xmldocument doesn't support it
                 m_Content = m_Content.Remove(44, 32);

                 //load content into xmldoc
                 xmlDoc = new XmlDocument();
                 xmlDoc.LoadXml(m_Content);
                 

                 foreach (XmlNode commNode in xmlDoc.SelectNodes("//comment()"))
                 {
                     commNode.ParentNode.RemoveChild(commNode);
                 }

                 rootNode = xmlDoc.DocumentElement;

                 if ((rootNode == null) ||!rootNode.Name.Equals("tv"))
                 {
                     throw new Exception("TV tag not found. XML file not well formed.");
                 }

                 if (!rootNode.FirstChild.Name.Equals("programme"))
                 {
                    throw new Exception("programme tag not found. XML file not well formed.");
                 }

                 //enough of the checking, let's parse the file already!

                 //create an holder collection for our programmes
                 ProgrammeCollection progColl = new ProgrammeCollection();



                 foreach (XmlNode programmeNodes in rootNode.SelectNodes("programme"))
                 {
                     //define defaults
                     string _title = "unknown title";
                     string _lang = "unknown lang";
                     string _descr = "no description";
                     string _date ="unknown date";
                     string _type ="unknown type";
                     string _channel = "unknown channel";

                     XmlNode nodTitle = programmeNodes.SelectSingleNode("title");
                     XmlNode nodDesc = programmeNodes.SelectSingleNode("desc");
                     XmlNode nodDate = programmeNodes.SelectSingleNode("date");

                     XmlNodeList nodCategories = programmeNodes.SelectNodes("category");
                     if (nodCategories.Count > 0)
                     {
                         foreach (XmlNode nodCategory in nodCategories)
                         {
                             _type += nodCategory.InnerText + ",";
                         }

                     }
                     if (nodTitle != null)
                     {
                         _title = nodTitle.InnerText;
                     }

                     if (nodDesc != null)
                     {
                         _descr = nodDesc.InnerText;
                     }

                     if (nodDate != null)
                     {
                         _date = nodDate.InnerText;
                     }


                     //get parameters
                     _channel = programmeNodes.Attributes.GetNamedItem("channel").Value;
                     DateTime _startTime = ConvertTime(programmeNodes.Attributes.GetNamedItem("start").Value);
                     DateTime _stopTime = ConvertTime(programmeNodes.Attributes.GetNamedItem("stop").Value);          
                     _lang = programmeNodes.SelectSingleNode("title").Attributes.GetNamedItem("lang").Value;
                    
                     //create an programme object with above parameters
                     Programme progXML = new Programme(_channel,
                                                        _date,
                                                        _type,
                                                        _title,
                                                        _lang,
                                                        _startTime,
                                                        _stopTime,
                                                        _descr);

                     progColl.Add(progXML);


                 }

                 parsedProgs = progColl;
             }
             catch (XmlException ex)
             {

                 throw new Exception("Error parsing XML file. Msg: " + ex.Message);
             }


            
        }

     
    }
}
