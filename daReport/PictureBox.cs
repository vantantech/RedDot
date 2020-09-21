	/*
This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General
Public License as published by the Free Software Foundation; either version 2.1 of the License, or (at your option)
any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, write to
the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

using System;
using System.Drawing;
using System.IO;
using System.ComponentModel;


namespace daReport
{
	/// <summary>
	/// Class representing the PictureBox object.
	/// </summary>
	/// <remarks>The PictureBox is used in reports where images need to be displayed.
	/// Compay logo, background image, etc. A background image can be combined with the
	/// <see cref="daReport.PictureBox.Selectable">daReport.PictureBox.Selectable</see> property
	/// so it can't be selected in the Designer Application.
	/// </remarks>
	public class PictureBox : ICustomPaint
	{
		#region Declarations
		private Bitmap mImage;
		private Rectangle mRegion;
		private bool mDoStretch = false;
		private Color mBorderColor = Color.Black;
		private int mBorderWidth = 0;
		private string mFilename;
		private string mDocRoot = "";
		private bool mSelectable = true;
		#endregion

		#region Public Functions

		/// <summary>
		/// Used to set root location of image file
		/// </summary>
		/// <remarks>This is the root directory that the image is located in. Defaults to location of
		/// xml template location</remarks>
		/// <param name="RootLocation">string RootLocation</param>
		public void SetDocumentRoot(string RootLocation)
		{
			mDocRoot = RootLocation;
			if (mFilename != null)
				Load(mFilename);
		}

		
		#endregion

		#region Public Properties

		/// <summary>
		/// Gets/Sets the border color for the PictureBox
		/// </summary>
		/// <remarks>This property sets the border color of the PictureBox object. This can be any color
		/// from the System.Drawing.Color structure
		/// </remarks>
		public Color BorderColor
		{
			get {return mBorderColor;}
			set {mBorderColor = value;}
		}


		/// <summary>
		/// Gets/Sets the border width for the PictureBox
		/// </summary>
		/// <remarks>
		/// BorderWidth of the PictureBox. If this is set to zero, then the border is invisible
		/// </remarks>
		public int BorderWidth
		{
			get {return mBorderWidth;}
			set {mBorderWidth = value;}
		}


		/// <summary>
		/// Location of image file relative to xml template file.
		/// </summary>
		/// <remarks>For new documents defaults to working directory of Designer.</remarks>
		[Description("Image file location relative to .xml file. For new documents defaults to working directory of Designer.")]
		public string ImageFile
		{
			get {return mFilename;}
			set
			{
				Load(value);

			}
		}


		

		/// <summary>
		/// Stretch the image to the container size.
		/// </summary>
		/// <remarks>
		/// If set to true, the image to the container size, otherwise will be displayed as it's physical size.
		/// </remarks>
		[Description("Stretch the image to the container size.")]
		public bool Stretch
		{
			get {return mDoStretch;}
			set {mDoStretch = value;}
		}


		#endregion

		#region Public Overrides

		/// <summary>
		/// The X coordinate of the left-upper corner of the PictureBox
		/// </summary>
		[Description("The X coordinate of the left-upper corner of the element.")]
		public override int X
		{
			get {return mRegion.X;}
			set 
			{
				if (this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.None)
					mRegion.X = value;
				else if (this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.Right)
				{
					mRegion.Width = mRegion.Width + mRegion.X - value;
					mRegion.X = value;					
				}
				else if (this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.Center)
				{
					mRegion.Width = mRegion.Width + 2*(mRegion.X - value);
					mRegion.X = value;					
				}

			}
		}


		/// <summary>
		/// The Y coordinate of the left-upper corner of the PictureBox
		/// </summary>
		[Description("The Y coordinate of the left-upper corner of the element.")]
		public override int Y
		{
			get {return mRegion.Y;}
			set 
			{
				if (this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.None)
					mRegion.Y = value;
				else if (this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.Bottom)
				{
					mRegion.Height = mRegion.Height + mRegion.Y - value;
					mRegion.Y = value;					
				}
				else if (this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.Middle)
				{
					mRegion.Height = mRegion.Height + 2*(mRegion.Y - value);
					mRegion.Y = value;					
				}
			}
		}


		/// <summary>
		///  Gets or sets the width of the PictureBox.
		/// </summary>
		[Description("The width of the element.")]
		public override int Width
		{
			get {return mRegion.Width;}
			set
			{
				if (this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.None || this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.Left)
					mRegion.Width = value;
				else if (this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.Right)
				{
					mRegion.X = mRegion.X - value + mRegion.Width;
					mRegion.Width = value;									
				}
				else if (this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.Center)
				{
					mRegion.X = mRegion.X - value/2 + mRegion.Width/2;
					mRegion.Width = value;									
				}
			}
		}


		/// <summary>
		///  Gets or sets the height of the PictureBox
		/// </summary>
		[Description("The height of the element.")]
		public override int Height
		{
			get {return mRegion.Height;}
			set
			{
				if (this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.None || this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.Top)
					mRegion.Height = value;
				else if (this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.Bottom)
				{
					mRegion.Y = mRegion.Y - value + mRegion.Height;
					mRegion.Height = value;									
				}
				else if (this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.Middle)
				{
					mRegion.Y = mRegion.Y - value/2 + mRegion.Height/2;
					mRegion.Height = value;									
				}
			}
		}


		/// <summary>
		///  Gets or sets the horizontal alignment of the PictureBox
		/// </summary>
		[Description("Horizontal alignment in the page, relative to margins. This property overrides element coordinates.")]
		public override ICustomPaint.HorizontalAlignmentTypes HorizontalAlignment
		{
			get {return this.horizontalAlignment;}
			set 
			{
				this.horizontalAlignment = value;
				switch (this.horizontalAlignment)
				{
					case ICustomPaint.HorizontalAlignmentTypes.Center:
						mRegion.X = (document.DefaultPageSettings.Bounds.Width-document.Margins.Right+document.Margins.Left)/2 - Width/2;
						break;

					case ICustomPaint.HorizontalAlignmentTypes.Right:
						mRegion.X = document.DefaultPageSettings.Bounds.Right - document.Margins.Right - Width;
						break;

					case ICustomPaint.HorizontalAlignmentTypes.Left:
						mRegion.X = document.Margins.Left;
						break;
				}
			}
		}


		/// <summary>
		///  Gets or sets the vertical alignment of the PictureBox
		/// </summary>
		[Description("Vertical alignment in the page, relative to margins. This property overrides element coordinates.")]
		public override ICustomPaint.VerticalAlignmentTypes VerticalAlignment
		{
			get {return this.verticalAlignment;}
			set 
			{
				this.verticalAlignment = value;
				switch (this.verticalAlignment)
				{
					case ICustomPaint.VerticalAlignmentTypes.Middle:
						mRegion.Y = (document.DefaultPageSettings.Bounds.Height-document.Margins.Bottom+document.Margins.Top)/2 - Height/2;
						break;

					case ICustomPaint.VerticalAlignmentTypes.Bottom:
						mRegion.Y = document.DefaultPageSettings.Bounds.Bottom - document.Margins.Bottom - Height;
						break;

					case ICustomPaint.VerticalAlignmentTypes.Top:
						mRegion.Y = document.Margins.Top;
						break;
				}
			}
		}

		/// <summary>
		/// Gets/Sets whether the PictureBox is selectable in the design pane of the DaReport Designer
		/// </summary>
		/// <remarks>If set to true, then the PictureBox is not selectable in the DaReport Designer application
		/// design pane. It is still selectable in the tree view listing of objects.
		/// </remarks>
		[Description("Sets whether the PictureBox can be selected in the design pane.")]
		public override bool Selectable
		{
			get { return this.mSelectable; }
			set { this.mSelectable=value; }
		}


		#endregion

		#region Private Functions

		private void Load(string filename)
		{
			try
			{
				string imageFile = mDocRoot;
				if (filename.StartsWith(Path.DirectorySeparatorChar.ToString()))
					imageFile += filename;
				else
					imageFile += Path.DirectorySeparatorChar.ToString() + filename;

				mImage = new Bitmap(imageFile);
				mFilename = filename;
				
			}
			catch(Exception)
			{
				mImage = null;
				mFilename = filename;
			}
		}


		#endregion

		#region Creator and Destructor

		/// <summary>
		/// Initializes a new instance of the PictureBox class.
		/// </summary>
		/// <param name="x">x-position of the new PictureBox</param>
		/// <param name="y">y-position of the new PictureBox</param>
		/// <param name="width">Width of the new PictureBox</param>
		/// <param name="height">Height of the new PictureBox</param>
		/// <param name="parent">Parent of the new PictureBox</param>
		public PictureBox(int x,int y, int width, int height, DaPrintDocument parent)
		{
			document = parent;
			mRegion = new Rectangle(x,y,width,height);
		}


		/// <summary>
		/// Releases all resources used by this PictureBox object.
		/// </summary>
		public void Dispose()
		{
			if (mImage!=null)
				mImage.Dispose();
		}


		#endregion

		#region ICustomPaint Members

		/// <summary>
		/// Paints the PictureBox
		/// </summary>
		/// <param name="g">The Graphics object to draw</param>
		/// <remarks>Causes the PictureBox to be painted to the screen.</remarks>
		public override void Paint(Graphics g)
		{
			
			if (mImage != null)
			{
				if (mDoStretch)
				{
					g.DrawImage(mImage,mRegion,0,0,mImage.Width,mImage.Height,GraphicsUnit.Pixel);
				}
				else
				{
					g.DrawImage(mImage,mRegion,0,0,mRegion.Width,mRegion.Height,GraphicsUnit.Pixel);
				}

				if ( mBorderWidth > 0 )
				{
					g.DrawRectangle( new Pen(mBorderColor,mBorderWidth),mRegion);
				}
			}
		}


		/// <summary>
		/// Gets the region of the current PictureBox
		/// </summary>
		/// <returns>System.Drawing.Rectangle</returns>
		public override Rectangle GetRegion()
		{
			return mRegion;
		}


		/// <summary>
		/// Clones the structure of the PictureBox, including all properties
		/// </summary>
		/// <returns><see cref="daReport.ChartBox">daReport.ChartBox</see></returns>
		public override object Clone()
		{
			PictureBox tmp = new PictureBox(0,0,0,0,document);
			tmp.X = this.X;
			tmp.Y = this.Y;
			tmp.Width = this.Width;
			tmp.Height = this.Height;
			tmp.BorderWidth = this.BorderWidth;
			tmp.BorderColor = this.BorderColor;
			tmp.Stretch = this.Stretch;
			tmp.ImageFile = this.ImageFile;
			return tmp;
		}


		/*
		*** not sure this is used
		public void AdjustPosition(Rectangle page){}
		*/
		

		#endregion
	}
}
