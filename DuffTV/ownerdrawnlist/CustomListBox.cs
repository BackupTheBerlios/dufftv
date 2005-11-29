using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using OpenNETCF.Windows.Forms;

namespace OwnerDrawnListFWProject
{
	/// <summary>
	/// Summary description for CustomListBox.
	/// </summary>
	public class CustomListBox : OwnerDrawnList
	{
		const int DRAW_OFFSET  = 4;

		private ImageList imageList = null;
		private bool wrapText = false;
		ImageAttributes imageAttr = new ImageAttributes();

		public bool WrapText
		{
			get
			{
				return wrapText;
			}
			set
			{
				wrapText = value;
			}
		}

		public CustomListBox(int itemHeight)
		{
			this.ShowScrollbar = true;
			this.ForeColor = Color.Black;
			//Set the item's height
			Graphics g = this.CreateGraphics();
			if (wrapText)
			  this.ItemHeight = 2 * Math.Max((int)(g.MeasureString("A", this.Font).Height), this.ItemHeight) + 2;
			else
			  this.ItemHeight = Math.Max((int)(g.MeasureString("A", this.Font).Height), this.ItemHeight) + 4;

          if (itemHeight > 0)
          {
              this.ItemHeight = itemHeight;
          }
			g.Dispose();
		}

		public ImageList ImageList
		{
			get
			{
				return imageList;
			}
			set
			{
				imageList = value;
			}
		}

		protected override void OnDrawItem(object sender, DrawItemEventArgs e)
		{
			Brush textBrush; //Brush for the text
   
			Rectangle rc = e.Bounds;
			rc.X += DRAW_OFFSET;
			//Check if the item is selected
			if (e.State == DrawItemState.Selected)
			{
				//Highlighted
				e.DrawBackground();
				textBrush = new SolidBrush(SystemColors.HighlightText);
			}
			else
			{
				//Change the background for every even item
				if ((e.Index % 2) == 0)
				{
					e.DrawBackground(Color.Thistle);
				}
				else
				{
					
					e.DrawBackground(this.BackColor);
				}
								
				textBrush = new SolidBrush(this.ForeColor);
			}
			
			//Get the ListItem
			ListItem item = (ListItem)this.Items[e.Index];
//			//Check if the item has a image
			if (item.ImageIndex > -1)
			{
				Image img = imageList.Images[item.ImageIndex];
				if (img != null)
				{
					imageAttr = new ImageAttributes();
					//Set the transparency key
					imageAttr.SetColorKey(BackgroundImageColor(img), BackgroundImageColor(img));
					//imageAttr.SetColorKey(Color.White, Color.White);
					//Image's rectangle
					Rectangle imgRect = new Rectangle(2, rc.Y+1, img.Width, img.Height);
					//Draw the image
					e.Graphics.DrawImage(img, imgRect, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imageAttr); 
					//Shift the text to the right
					rc.X+=img.Width + 2;
				}
			}
			
			//Draw item's text
			e.Graphics.DrawString(item.Text, e.Font, textBrush,  rc);
			//Draw the line
			e.Graphics.DrawLine(new Pen(Color.Navy), 0, e.Bounds.Bottom, e.Bounds.Width, e.Bounds.Bottom);
			//Call the base's OnDrawEvent	
			base.OnDrawItem (sender, e);
		}

		private Color BackgroundImageColor(Image image)
		{
			Bitmap bmp = new Bitmap(image);
			Color ret = bmp.GetPixel(0, 0);
			return ret;
		}
	}
	//ListItem class
	public class ListItem
	{
		private string text = "";
		private int imageIndex = -1;

		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}

		public int ImageIndex
		{
			get
			{
				return imageIndex;
			}
			set
			{
				imageIndex = value;
			}
		}
	}
}
