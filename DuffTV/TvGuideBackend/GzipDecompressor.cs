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
using ICSharpCode.SharpZipLib.GZip;


namespace DuffTv.TvGuideBackend
{
   public class GzipDecompressor
    {
        public GzipDecompressor()
        {
            //empty constructor
        }

        public string Decompress(string filename)
        {
            StringBuilder result = null;
     
            try
            {
                Stream s = new GZipInputStream(File.OpenRead(filename));

                result = new StringBuilder(8192);
                UTF7Encoding encoding = new UTF7Encoding(true);

                int size = 2048;
                byte[] writeData = new byte[2048];
                while (true)
                {
                    size = s.Read(writeData, 0, size);
                    if (size > 0)
                    {
                       result.Append(encoding.GetString(writeData,0,size)); 
                    }
                    else
                    {
                        break;
                    }
                }
                s.Close();
                           


            } // end try
            catch (GZipException)
            {
                throw new Exception("Error: The file being read contains invalid data.");
            }
            catch (FileNotFoundException)
            {
                throw new Exception("Error:The file specified was not found.");
            }
            catch (ArgumentException)
            {
                throw new Exception("Error: path is a zero-length string, contains only white space, or contains one or more invalid characters");
            }
            catch (PathTooLongException)
            {
                throw new Exception("Error: The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.");
            }
            catch (DirectoryNotFoundException)
            {
                throw new Exception("Error: The specified path is invalid, such as being on an unmapped drive.");
            }
            catch (IOException)
            {
                throw new Exception("Error: An I/O error occurred while opening the file.");
            }
            catch (UnauthorizedAccessException)
            {
                throw new Exception("Error: path specified a file that is read-only, the path is a directory, or caller does not have the required permissions.");
            }

            return result.ToString();
            
        }


    }
}
