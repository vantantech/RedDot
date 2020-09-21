

using System;
using System.Drawing;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;


namespace daReport
{
	/// <summary>
	/// Class representing the TextField object.
	/// </summary>
	/// <remarks>The TextField is used in reports where graphical representation of data is used.</remarks>
	public class TextField : ICustomPaint
	{
		#region Declarations
		private string theText = "Sample text";
		private Font theFont = new Font("Tahoma",10);
		private Color foregroundColor = Color.Black;
		private Color backgroundColor = Color.Transparent;
		private Color borderColor = Color.Black;
		private int borderWidth = 0;
		private int innerPadding = 2;
		private int lineSpacing = 3;
		private bool mSelectable = true;

		/// <summary>
		/// Enumeration of possible horizontal alignments for the TextField text
		/// </summary>
		public enum TextAlignmentType
		{
			/// <summary>Text is aligned to the left of the TextField</summary>
			Left = 1,
			/// <summary>Text is aligned to the center of the TextField</summary>
			Center,
			/// <summary>Text is aligned to the right of the TextField</summary>
			Right,
			/// <summary>Text is aligned to both sides of the TextField</summary>
			Justified,
			/// <summary>The text is not aligned and will be displayed normally</summary>
			None
		};
		
		/// <summary>
		/// Enumeration of possible horizontal alignments for the TextField text
		/// </summary>
		public enum TextVerticalAlignmentType
		{
			/// <summary>Text is aligned to the top of the TextField</summary>
			Top = 1,
			/// <summary>Text is aligned to the middle of the TextField</summary>
			Middle,
			/// <summary>Text is aligned to the bottom of the TextField</summary>
			Bottom,
			/// <summary>The text is not aligned and will be displayed normally</summary>
			None
		};
		
		private TextAlignmentType textAlignment = TextAlignmentType.Left;
		private TextVerticalAlignmentType textVerticalAlignment = TextVerticalAlignmentType.Top;
		
		private Rectangle theRegion = new Rectangle(0,0,0,0);
		private ArrayList theLines;
		#endregion

		#region Public Properties

		/// <summary>
		/// Gets/Sets the background color for the TextField
		/// </summary>
		/// <remarks>This property sets the background color of the TextField object. This can be any color
		/// from the System.Drawing.Color structure
		/// </remarks>
		[Category("Appearance")]
		public Color BackgroundColor
		{
			get {return backgroundColor;}
			set {backgroundColor = value;}
		}


		/// <summary>
		/// Gets/Sets the border color for the TextField
		/// </summary>
		/// <remarks>This property sets the border color of the TextField object. This can be any color
		/// from the System.Drawing.Color structure
		/// </remarks>
		[Category("Appearance")]
		public Color BorderColor
		{
			get {return borderColor;}
			set {borderColor = value;}
		}


		/// <summary>
		/// Gets/Sets the border width for the TextField
		/// </summary>
		/// <remarks>
		/// BorderWidth of the TextField. If this is set to zero, then the border is invisible
		[Category("Appearance")]
		public int BorderWidth
		{
			get {return borderWidth;}
			set {borderWidth = value;}
		}


		/// <summary>
		/// Gets/Sets the font used in the TextField object
		/// </summary>
		[Category("Appearance")]
		public Font Font
		{
			get {return theFont;}
			set {theFont = value;}
		}


		/// <summary>
		/// Gets/Sets the foreground color for the TextField
		/// </summary>
		/// <remarks>This property sets the foreground color of the TextField object. This can be any color
		/// from the System.Drawing.Color structure
		/// </remarks>
		[Category("Appearance")]
		public Color ForegroundColor
		{
			get {return foregroundColor;}
			set {foregroundColor = value;}
		}


		


		/// <summary>
		/// Gets/Sets text to be displayed in the TextField object.
		/// </summary>
		[Category("Data")]
		[Editor(typeof(editors.PlainTextEditor), typeof(UITypeEditor)), Description("Text content of the Text Field.")]
		public string Text
		{
			get {return theText;}
			set {theText = value;}
		}
		

		/// <summary>
		///  Gets or sets the horizontal alignment of text in the ChartBox
		/// </summary>
		[Category("Appearance"), Description("Horizontal alignment of text in the TextField object, relative to borders.")]
		public TextAlignmentType TextAlignment
		{
			get {return textAlignment;}
			set {textAlignment = value;}
		}


		/// <summary>
		///  Gets or sets the vertical alignment of text in the ChartBox
		/// </summary>
		[Category("Appearance"), Description("Horizontal vertical of text in the TextField object, relative to borders.")]
		public TextVerticalAlignmentType TextVerticalAlignment
		{
			get {return textVerticalAlignment;}
			set {textVerticalAlignment = value;}
		}


		#endregion
		
		#region Public Overrides

		/// <summary>
		///  Gets or sets the horizontal alignment of the TextField
		/// </summary>
		[Category("Layout"), Description("Horizontal alignment in the page, relative to margins. This property overrides element coordinates.")]
		public override ICustomPaint.HorizontalAlignmentTypes HorizontalAlignment
		{
			get {return this.horizontalAlignment;}
			set 
			{
				this.horizontalAlignment = value;
				switch (this.horizontalAlignment)
				{
					case ICustomPaint.HorizontalAlignmentTypes.Center:
						theRegion.X = (document.DefaultPageSettings.Bounds.Width-document.Margins.Right+document.Margins.Left)/2 - Width/2;
						break;

					case ICustomPaint.HorizontalAlignmentTypes.Right:
						theRegion.X = document.DefaultPageSettings.Bounds.Right - document.Margins.Right - Width;
						break;

					case ICustomPaint.HorizontalAlignmentTypes.Left:
						theRegion.X = document.Margins.Left;
						break;
				}
			}
		}


		/// <summary>
		///  Gets or sets the height of the TextField
		/// </summary>
		[Category("Layout"), Description("The height of the element.")]
		public override int Height
		{
			get {return theRegion.Height;}
			set
			{
				if (this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.None || this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.Top)
					theRegion.Height = value;
				else if (this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.Bottom)
				{
					theRegion.Y = theRegion.Y - value + theRegion.Height;
					theRegion.Height = value;									
				}
				else if (this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.Middle)
				{
					theRegion.Y = theRegion.Y - value/2 + theRegion.Height/2;
					theRegion.Height = value;									
				}
			}
		}


		/// <summary>
		///  Gets or sets the vertical alignment of the TextField
		/// </summary>
		[Category("Layout"), Description("Vertical alignment in the page, relative to margins. This property overrides element coordinates.")]
		public override ICustomPaint.VerticalAlignmentTypes VerticalAlignment
		{
			get {return this.verticalAlignment;}
			set 
			{
				this.verticalAlignment = value;
				switch (this.verticalAlignment)
				{
					case ICustomPaint.VerticalAlignmentTypes.Middle:
						theRegion.Y = (document.DefaultPageSettings.Bounds.Height-document.Margins.Bottom+document.Margins.Top)/2 - Height/2;
						break;

					case ICustomPaint.VerticalAlignmentTypes.Bottom:
						theRegion.Y = document.DefaultPageSettings.Bounds.Bottom - document.Margins.Bottom - Height;
						break;

					case ICustomPaint.VerticalAlignmentTypes.Top:
						theRegion.Y = document.Margins.Top;
						break;
				}
			}
		}


		/// <summary>
		///  Gets or sets the width of the TextField.
		/// </summary>
		[Category("Layout"), Description("The width of the element.")]
		public override int Width
		{
			get {return theRegion.Width;}
			set
			{
				if (this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.None || this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.Left)
					theRegion.Width = value;
				else if (this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.Right)
				{
					theRegion.X = theRegion.X - value + theRegion.Width;
					theRegion.Width = value;									
				}
				else if (this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.Center)
				{
					theRegion.X = theRegion.X - value/2 + theRegion.Width/2;
					theRegion.Width = value;									
				}
			}
		}


		/// <summary>
		/// The X coordinate of the left-upper corner of the TextField
		/// </summary>
		[Category("Layout"), Description("The X coordinate of the left-upper corner of the element.")]
		public override int X
		{
			get {return theRegion.X;}
			set 
			{
				if (this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.None)
					theRegion.X = value;
				else if (this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.Right)
				{
					theRegion.Width = theRegion.Width + theRegion.X - value;
					theRegion.X = value;					
				}
				else if (this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.Center)
				{
					theRegion.Width = theRegion.Width + 2*(theRegion.X - value);
					theRegion.X = value;					
				}

			}
		}

		
		/// <summary>
		/// The Y coordinate of the left-upper corner of the TextField
		/// </summary>
		[Category("Layout"), Description("The Y coordinate of the left-upper corner of the element.")]
		public override int Y
		{
			get {return theRegion.Y;}
			set 
			{
				if (this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.None)
					theRegion.Y = value;
				else if (this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.Bottom)
				{
					theRegion.Height = theRegion.Height + theRegion.Y - value;
					theRegion.Y = value;					
				}
				else if (this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.Middle)
				{
					theRegion.Height = theRegion.Height + 2*(theRegion.Y - value);
					theRegion.Y = value;					
				}
			}
		}

		/// <summary>
		/// Gets/Sets whether the TextField is selectable in the design pane of the DaReport Designer
		/// </summary>
		/// <remarks>If set to true, then the TextField is not selectable in the DaReport Designer application
		/// design pane. It is still selectable in the tree view listing of objects.
		/// </remarks>
		[Description("Sets whether the object can be selected in the design pane.")]
		public override bool Selectable
		{
			get { return this.mSelectable; }
			set { this.mSelectable=value; }
		}
		
		#endregion

		#region Private Functions
		
		private void drawJustified(Graphics g,string text,int yPos,bool isLast,bool isIndented)
		{
			
			string txt = text.Replace("\t","     ");
			float indentSize = g.MeasureString("mmm",theFont).Width;

			float xPos = theRegion.X + innerPadding + (isIndented?indentSize:0);

			string[] theWords = txt.Split(new char[]{' '});

			float[] wordsWidths = new float[theWords.Length];

			float totalWordsWidth = 0;
			for (int i=0;i<theWords.Length;i++)
			{
				SizeF sf = g.MeasureString(theWords[i],theFont);
				wordsWidths[i] = sf.Width;
				totalWordsWidth += sf.Width;
			}

			float theOffset = 0;
			if (theWords.Length>1)
				theOffset = (theRegion.Width - 2*innerPadding - totalWordsWidth - (isIndented?indentSize:0) ) / (theWords.Length-1);

			if (isLast)
			{
				g.DrawString(txt,theFont,new SolidBrush(foregroundColor),xPos,yPos);	
			}
			else
			{
				for (int i=0;i<theWords.Length;i++)
				{

					if (i==0)
					{
						xPos = theRegion.X + innerPadding + (isIndented?indentSize:0);
					}
					else if(i==theWords.Length-1)
					{
						xPos = theRegion.Right - innerPadding - g.MeasureString(theWords[i],theFont).Width;
					}					
				
					g.DrawString(theWords[i],theFont,new SolidBrush(foregroundColor),xPos,yPos);							
					
					xPos += wordsWidths[i]+ theOffset;
				}
			}

			/*
			CharacterRange[] characterRanges = new CharacterRange[theWords.Length];

			int pos = 0;
			for (int i=0;i<theWords.Length;i++)
			{
				characterRanges[i] = new CharacterRange(txt.IndexOf(theWords[i],pos),theWords[i].Length);
				pos += theWords[i].Length;
				while ( pos+1 < txt.Length && txt.Substring(pos+1,1).Equals(" ") )
					pos++;
			}

			Region[] stringRegions = new Region[theWords.Length];


			StringFormat stringFormat = new StringFormat();
			stringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces ;
			stringFormat.SetMeasurableCharacterRanges(characterRanges);

			
			SizeF sf = g.MeasureString(txt,theFont);

			float theOffset = 0;
			if (isIndented)
			{
				theOffset = (theRegion.Width - 2*innerPadding - indentSize - sf.Width) / theWords.Length;
				stringRegions = g.MeasureCharacterRanges(txt,theFont,new RectangleF(theRegion.X+innerPadding+indentSize,yPos,sf.Width,sf.Height),stringFormat);
			}
			else
			{
				theOffset = (theRegion.Width - 2*innerPadding - sf.Width) / theWords.Length;
				//theOffset = (theRegion.Width - 2*innerPadding - sf.Width) / (theWords.Length-1);
				stringRegions = g.MeasureCharacterRanges(txt,theFont,new RectangleF(theRegion.X+innerPadding,yPos,sf.Width,sf.Height),stringFormat);
				//stringRegions = g.MeasureCharacterRanges(txt,theFont,new RectangleF(theRegion.X+innerPadding,yPos,theRegion.Width-2*innerPadding,sf.Height),stringFormat);
			}

			if (isLast)
			{
				g.DrawString(txt,theFont,new SolidBrush(foregroundColor),xPos,yPos,stringFormat);	
			}
			else
			{
				for (int i=0;i<theWords.Length;i++)
				{

					RectangleF rect = stringRegions[i].GetBounds(g);

					if (i==0)
					{
						//xPos = theRegion.X + innerPadding;
					}
					else if(i==theWords.Length-1)
					{
						xPos = theRegion.Right - innerPadding - g.MeasureString(theWords[i],theFont).Width;
					}
					else
					{					
						xPos = rect.X + i*theOffset;	
					}
				
					g.DrawString(theWords[i],theFont,new SolidBrush(foregroundColor),xPos,rect.Y,stringFormat);							
					
				}
			}
			
			*/
		}


		private void drawSimpleString(Graphics g,string txt,int yPos,bool isLast)
		{
			int xPos = 0;

			SizeF sf = g.MeasureString(txt,theFont);

			switch (TextAlignment)
			{
				case TextAlignmentType.Center:
					xPos = theRegion.X + theRegion.Width/2 - (int)sf.Width/2;
					g.DrawString(txt,theFont,new SolidBrush(foregroundColor),new Point(xPos,yPos));	
					break;

				case TextAlignmentType.Right:
					xPos = theRegion.Right - innerPadding - (int)sf.Width;
					g.DrawString(txt,theFont,new SolidBrush(foregroundColor),new Point(xPos,yPos));	
					break;

				case TextAlignmentType.Justified:
					if (txt.StartsWith("\t"))
					{
						drawJustified(g,txt.Trim(),yPos,isLast,true);
					}
					else
					{
						drawJustified(g,txt.TrimEnd(new char[]{' '}),yPos,isLast,false);	
					}
					break;

				default:
					xPos = theRegion.X + innerPadding;
					g.DrawString(txt,theFont,new SolidBrush(foregroundColor),new Point(xPos,yPos));	
					break;

			}			
		}

		
		private void drawText(Graphics g)
		{

			// check for general exceptions
			if (theText.Length == 0 ) return;
			theLines.Clear();		

			string line = "";
			int lastBlank = -1;
			int current = -1;

			int yPosition = theRegion.Top + innerPadding;
			SizeF sf = g.MeasureString(line,theFont);

			while ( yPosition+sf.Height <= (theRegion.Bottom - innerPadding) && theText.Length > ++current )
			{
				string nextChar = theText.Substring(current,1);

				line += nextChar;
				sf = g.MeasureString(line,theFont);

				if (sf.Width > theRegion.Width - 2*innerPadding)
				{
					if (lastBlank == -1)
					{
						line = line.Substring(0,line.Length-1);
						current--;
					}
					else
					{
						line = line.Substring(0,line.Length-(current-lastBlank));
						current = lastBlank;

					}
					
					theLines.Add(line);
					yPosition += (int)sf.Height + lineSpacing;
					line = "";

					
					lastBlank = -1;
				}
				else if (current == theText.Length-1)
				{
					theLines.Add(line);
				}
				else
				{
					if (nextChar.Equals(" "))
						lastBlank = current;
				}
			}
		
			startPainting(g);

		}

		
		private void startPainting(Graphics g)
		{
			int lineHeight = (int)g.MeasureString("This is dummy text",theFont).Height;
			int yPos = 0;

			switch (textVerticalAlignment)
			{
				case TextVerticalAlignmentType.Bottom:
					yPos = theRegion.Bottom - innerPadding - theLines.Count*lineHeight - (theLines.Count-1)*lineSpacing; 
					break;

				case TextVerticalAlignmentType.Middle:
					yPos = theRegion.Top + theRegion.Height/2 - ( theLines.Count*lineHeight + (theLines.Count-1)*lineSpacing )/2; 
					break;

				default:
					yPos = theRegion.Top + innerPadding;
					break;
			}

			for (int i=0;i<theLines.Count;i++)
			{				
				drawSimpleString(g,theLines[i].ToString(),yPos, i==theLines.Count-1 );
				yPos = yPos + lineHeight + lineSpacing;
			}
		}

		
		#endregion

		#region Creator

		/// <summary>
		/// Initializes a new instance of the TextField class.
		/// </summary>
		public TextField()
		{
			theLines = new ArrayList();
		}


		/// <summary>
		/// Initializes a new instance of the TextField class.
		/// </summary>
		/// <param name="originX">x-position of the new TextField</param>
		/// <param name="originY">y-position of the new TextField</param>
		/// <param name="width">Width of the new TextField</param>
		/// <param name="height">Height of the new TextField</param>
		/// <param name="parent">Parent of the new TextField</param>
		public TextField(int originX,int originY,int width,int height, DaPrintDocument parent)
		{
			document = parent;
			theText = "";
			theRegion = new Rectangle(originX,originY,width,height);
			theLines = new ArrayList();
		}

		
		#endregion

		#region ICustomPaint Members

		/// <summary>
		/// Clones the structure of the TextField, including all properties
		/// </summary>
		/// <returns><see cref="daReport.TextField">daReport.TextField</see></returns>
		public override object Clone()
		{
			TextField tmp = new TextField();
			tmp.document = this.document;
			tmp.Text = this.Text;
			tmp.X = this.X;
			tmp.Y = this.Y;
			tmp.Width = this.Width;
			tmp.Height = this.Height;
			tmp.BorderWidth = this.BorderWidth;
			tmp.BorderColor = this.BorderColor;
			tmp.BackgroundColor = this.BackgroundColor;
			tmp.Font = this.Font;
			tmp.TextAlignment = this.TextAlignment;
			tmp.TextVerticalAlignment = this.TextVerticalAlignment;
			tmp.ForegroundColor = this.ForegroundColor;

			return tmp;
		}


		/// <summary>
		/// Gets the region of the current TextField
		/// </summary>
		/// <returns>System.Drawing.Rectangle</returns>
		public override Rectangle GetRegion()
		{
			return theRegion;
		}


		/// <summary>
		/// Paints the TextField
		/// </summary>
		/// <param name="g">The Graphics object to draw</param>
		/// <remarks>Causes the ChartBox to be painted to the screen.</remarks>
		public override void Paint(System.Drawing.Graphics g)
		{

			if ( backgroundColor != Color.Transparent )
			{
				g.FillRectangle(new SolidBrush(backgroundColor),theRegion);
			}

			if ( this.borderWidth > 0 )
			{
				g.DrawRectangle(new Pen(this.borderColor,borderWidth),theRegion);
			}

			drawText(g);
		}


		#endregion
	}
}
