

using System;
using System.Drawing;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Drawing.Design;


namespace daReport
{
	/// <summary>
	/// Summary description for StyledTable.
	/// </summary>
	public class StyledTable : ICustomPaint
	{
		string d = "pedja";
		#region Declarations
		private int cellHeight = 18;
		private DataTable theData;
		private string dataSource;
		private bool mSelectable = true;
		private Color mBorderColor = Color.Black;
		private string mGroupByField = "";
		private StyledTableColumn[] columns = new StyledTableColumn[0];

		private Rectangle theRegion = new Rectangle(0,0,0,0);
		private Font dataFont = new Font("Tahoma",8,FontStyle.Regular);
		private Color mDataFontColor = Color.Black;
		private bool doDrawEmptyRows = false;

		private bool doDrawHeader = true;
		private Font headerFont = new Font("Tahoma",8,FontStyle.Bold);
		private Color mHeaderFontColor = Color.Black;
		private Color mHeaderBackgroundColor = Color.White;
		#endregion

		#region Public Properties

		/// <summary>
		/// Gets/Sets the border color for the StyledTable
		/// </summary>
		/// <remarks>This property sets the border color of the StyledTable object. This can be any color
		/// from the System.Drawing.Color structure
		/// </remarks>
		[Category("Appearance")]
		public Color BorderColor
		{
			get { return this.mBorderColor; }
			set { this.mBorderColor=value; }
		}


		/// <summary>
		/// Returns relative data row height after checking if row will be multi-line
		/// </summary>
		/// <param name="CurrentRow">Current Data Row to calculate height</param>
		/// <param name="g">Current Graphics object</param>
		/// <returns>integer value of relative row height</returns>
		public int CalculateRelativeDataRowHeight(DataRow CurrentRow, Graphics g)
		{
			int relativeDataRowHeight = 0;

			for (int i=0;i<Columns.Length;i++)
			{
				StyledTableColumn CurrentColumn = Columns[i];

				if (CurrentColumn.Visible)
				{
					object theObject = CurrentRow[CurrentColumn.Name];
					string tmpValue = FormatValue(theObject,CurrentRow.Table.Columns[CurrentColumn.Name].DataType, CurrentColumn.FormatMask);
					string theLine = "";
					int drawnRowsCounter = 0;

					bool hasMore = false;
					do
					{
						hasMore = splitString(ref tmpValue,ref theLine,columns[i].Width,g,dataFont);					
						drawnRowsCounter++;
					}
					while ( hasMore );

					if (drawnRowsCounter>relativeDataRowHeight)
						relativeDataRowHeight = drawnRowsCounter;
				}
			}

			return relativeDataRowHeight;
		}


		// calculates relative header height with respect to cell height
		/// <summary>
		/// Returns relative header row height after checking if headers will be multi-line
		/// </summary>
		/// <param name="g">Graphics Object</param>
		/// <returns>integer value of relative Header row height</returns>
		public int CalculateRelativeHeaderHeight(Graphics g)
		{
			if (!doDrawHeader) return 0;

			int headerRelativeHeight = 0;

			ArrayList[] headerLabels = new ArrayList[columns.Length];
			for (int i=0;i<columns.Length;i++)
			{
				if (columns[i].Visible)
				{
					string tmpValue = columns[i].Label;
					string theLine = "";
					headerLabels[i] = new ArrayList();
					bool WillTextWrap = false;
					do
					{
						WillTextWrap = splitString(ref tmpValue,ref theLine,columns[i].Width,g,headerFont);
						headerLabels[i].Add(theLine);
					}
					while(WillTextWrap);

					if (headerLabels[i].Count > headerRelativeHeight)
						headerRelativeHeight = headerLabels[i].Count;
				}
			}
			return headerRelativeHeight;
		}
		
		
		/// <summary>
		/// Gets/Sets the height of every cell in the table
		/// </summary>
		public int CellHeight
		{
			get {return cellHeight;}
			set {cellHeight = value;}
		}

		
		/// <summary>
		/// A collection of <see cref="daReport.StyledTableColumn">daReport.StyledTableColumn</see> columns.
		/// </summary>
		[Category("Data"), Description("Table columns.")]
		public StyledTableColumn[] Columns
		{
			get {return columns;}
			set 
			{
				columns = value;
				if (columns.Length>0)
				{
					int sumOfWidths = 0;
					for (int i=0;i<columns.Length;i++)
					{
						sumOfWidths += columns[i].Width;
					}
					theRegion.Width = sumOfWidths;
				}
			}
		}

		
		/// <summary>
		/// Gets/Sets the current System.Data.DataTable which contains the data for the StyledTable object.
		/// </summary>
		[Browsable(false)]	
		public DataTable Data
		{
			get {return theData;}
			set {theData = value;}
		}


		/// <summary>
		/// Gets/Sets the static data for the StyledTable object.
		/// </summary>
		[Category("Data")]
		[Editor(typeof(editors.StaticTableEditor), typeof(UITypeEditor)), Description("Static table data.")]
		public string[][] StaticData
		{
			get
			{
				if (Data != null) 
				{

					int colNumber = Columns.Length;
					int rowNumber = Data.Rows.Count;

					string[][] tmpData = new string[rowNumber+1][];
					for (int i=0;i<rowNumber+1;i++)
					{
						tmpData[i] = new string[colNumber];
					}
				
					for (int j=0;j<colNumber;j++)
					{
						tmpData[0][j] = Columns[j].Name;
					}

					for (int i=0;i<rowNumber;i++)
					{
						for (int j=0;j<colNumber;j++)
						{
							if (Data.Columns.Contains( Columns[j].Name) )
							{
								tmpData[i+1][j] = Data.Rows[i][j].ToString();
							}
							else
							{
								tmpData[i+1][j] = "";
							}
						}

					}

					return tmpData;
				}
				else
				{
					int colNumber = Columns.Length;
					string[][] tmpData = new string[1][];
					tmpData[0] = new string[colNumber];
					for (int j=0;j<colNumber;j++)
					{
						tmpData[0][j] = Columns[j].Name;
					}
					return tmpData;
				}
			}

			set 
			{
				DataTable dataTable = new DataTable();

				for (int i=0;i<this.Columns.Length;i++)
				{
					dataTable.Columns.Add(new DataColumn(this.Columns[i].Name));
				}

				for (int i=0;i<value.Length;i++)
				{
					string[] theRow = new string[this.Columns.Length];
					for (int j=0;j<value[i].Length;j++)
					{
						theRow[j] = value[i][j];
					}

					dataTable.Rows.Add(theRow);
				}

				this.Data = dataTable;
			}
		}

		/// <summary>
		/// Gets/Sets the data font for the StyledTable
		/// </summary>
		/// <remarks>This property sets the font of the StyledTable data rows. This can be any font
		/// from the System.Drawing.Font structure
		/// </remarks>
		[Category("DataRow Settings")]
		public Font DataFont
		{
			get {return dataFont;}
			set {dataFont = value;}
		}


		/// <summary>
		/// Gets/Sets the data font color for the StyledTable
		/// </summary>
		/// <remarks>This property sets the data row font color of the StyledTable object. This can be any color
		/// from the System.Drawing.Color structure
		/// </remarks>
		[Category("DataRow Settings")]
		public Color DataFontColor
		{
			get { return this.mDataFontColor; }
			set { this.mDataFontColor=value; }
		}

		
		/// <summary>
		/// The name for DataTable which exports data to this styled table
		/// </summary>
		/// <remarks>
		/// Relevant for dynamic contents table. The binding itself is done programmaticly with
		/// <see cref="daReport.DaPrintDocument.AddData">daReport.DaPrintDocument.AddData(DataTable)</see>
		/// method of the DaPrintDocument class.
		/// </remarks>
		[Category("Data"), Description("The name for DataTable which exports data to this styled table. Relevant for dynamic contents table. The binding itself is done programmaticly with AddData(DataTable) method of the DaPrintDocument class.")]
		public string DataSource
		{
			get {return dataSource;}
			set {dataSource = value;}
		}


		/// <summary>
		/// Gets/Sets whether empty rows be drawn if any space in table area remains unused.
		/// </summary>
		[Category("DataRow Settings"), Description("Should empty rows be drawn if any space in table area remains unused.")]
		public bool DrawEmptyRows
		{
			get {return doDrawEmptyRows;}
			set {doDrawEmptyRows = value;}
		}


		/// <summary>
		/// Gets/Sets whether to draw the table header row in the generated report.
		/// </summary>
		/// <remarks>This will affect how many possible rows can be drawn on the table.
		/// See <see cref="daReport.StyledTable.GetPossibleRowNumber">daReport.StyledTable.GetPossibleRowNumber</see></remarks>
		[Category("Header Settings")]
		public bool DrawHeader
		{
			get {return doDrawHeader;}
			set {doDrawHeader = value;}
		}


		/// <summary>
		/// Returns possible number of rows for the current table region
		/// </summary>
		/// <returns>integer value of possible number of rows for current table</returns>
		public int GetPossibleRowNumber()
		{
			return theRegion.Height/cellHeight;
		}


		/// <summary>
		/// Gets/Sets the GroupByField for the data
		/// </summary>
		/// <remarks>This property is only used for dynamic tables. Grouping occurs on the field specified.</remarks>
		[Category("Data")]
		public string GroupByField
		{
			get { return this.mGroupByField; }
			set { this.mGroupByField=value; }
		}


		/// <summary>
		/// Gets/Sets the header background color for the StyledTable
		/// </summary>
		/// <remarks>This property sets the header background color of the StyledTable object. This can be any color
		/// from the System.Drawing.Color structure
		/// </remarks>
		[Category("Header Settings")]
		public Color HeaderBackgroundColor
		{
			get {return mHeaderBackgroundColor;}
			set {mHeaderBackgroundColor = value;}
		}


		/// <summary>
		/// Gets/Sets the header font for the StyledTable
		/// </summary>
		/// <remarks>This property sets the header font of the StyledTable object. This can be any font
		/// from the System.Drawing.Font structure
		/// </remarks>
		[Category("Header Settings")]
		public Font HeaderFont
		{
			get {return headerFont;}
			set {headerFont = value;}
		}


		/// <summary>
		/// Gets/Sets the header font color for the StyledTable
		/// </summary>
		/// <remarks>This property sets the header font color of the StyledTable object. This can be any color
		/// from the System.Drawing.Color structure
		/// </remarks>
		[Category("Header Settings")]
		public Color HeaderFontColor
		{
			get { return this.mHeaderFontColor; }
			set { this.mHeaderFontColor=value; }
		}


		
		
		
		#endregion

		#region Public Overrides

		/// <summary>
		///  Gets or sets the height of the StyledTable
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
		///  Gets or sets the horizontal alignment of the StyledTable
		/// </summary>
		[Category("Layout"), Description("Horizontal alignment in the page, relative to margins. This property overrides element coordinates.")]
		public override ICustomPaint.HorizontalAlignmentTypes HorizontalAlignment
		{
			get {return horizontalAlignment;}
			set 
			{
				horizontalAlignment = value;
				switch (horizontalAlignment)
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
		///  Gets or sets the vertical alignment of the StyledTable
		/// </summary>
		[Category("Layout"), Description("Vertical alignment in the page, relative to margins. This property overrides element coordinates.")]
		public override ICustomPaint.VerticalAlignmentTypes VerticalAlignment
		{
			get {return verticalAlignment;}
			set 
			{
				verticalAlignment = value;
				switch (verticalAlignment)
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
		///  Gets or sets the width of the StyledTable.
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
				

				if (columns.Length>0)
				{
					int sumOfWidths = 0;
					for (int i=0;i<columns.Length;i++)
					{
						sumOfWidths += columns[i].Width;
					}

					if (sumOfWidths<theRegion.Width)
						columns[columns.Length-1].Width += theRegion.Width - sumOfWidths;
					else
					{
						if (sumOfWidths-columns[columns.Length-1].Width < theRegion.Width)
							columns[columns.Length-1].Width -= sumOfWidths - theRegion.Width;
						else
						{
							double ratio = (double)theRegion.Width / sumOfWidths ;
							for (int i=0;i<columns.Length;i++)
							{
								columns[i].Width = (int)(ratio*columns[i].Width);
							}

						}
					}
				}
			}
		}


		/// <summary>
		/// The X coordinate of the left-upper corner of the StyledTable
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
		/// The Y coordinate of the left-upper corner of the StyledTable
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
		/// Gets/Sets whether the StyledTable is selectable in the design pane of the DaReport Designer
		/// </summary>
		/// <remarks>If set to true, then the StyledTable is not selectable in the DaReport Designer application
		/// design pane. It is still selectable in the tree view listing of objects.
		/// </remarks>
		[Category("Data"), Description("Sets whether the StyledTable can be selected in the design pane.")]
		public override bool Selectable
		{
			get { return this.mSelectable; }
			set { this.mSelectable=value; }
		}

		#endregion

		#region Private Properties

		private int drawHeader(Graphics g, Rectangle TargetRegion, int DrawnSoFar)
		{
			int MaxLinesDrawn = 0;
			int leftBorder = theRegion.X;

			ArrayList[] headerLabels = new ArrayList[columns.Length];
			for (int i=0;i<columns.Length;i++)
			{
				if (columns[i].Visible)
				{
					string tmpValue = columns[i].Label;
					string theLine = "";
					headerLabels[i] = new ArrayList();
					bool WillTextWrap = false;
					do
					{
						WillTextWrap = splitString(ref tmpValue,ref theLine,columns[i].Width,g,headerFont);
						headerLabels[i].Add(theLine);
					}
					while(WillTextWrap);

					if (headerLabels[i].Count > MaxLinesDrawn)
						MaxLinesDrawn = headerLabels[i].Count;
				}
			}

			// draw header background
			g.FillRectangle(new SolidBrush(mHeaderBackgroundColor),TargetRegion.X,TargetRegion.Y,TargetRegion.Width,cellHeight*MaxLinesDrawn);	

			for (int i=0;i<columns.Length;i++)
			{
				if (columns[i].Visible)
				{
					ArrayList theLines = headerLabels[i];

					for (int j=0;j<theLines.Count;j++)
					{
						SizeF sf = g.MeasureString(theLines[j].ToString(),headerFont);				
						float yPos = leftBorder+1;

						switch (columns[i].Alignment)
						{
							case StyledTableColumn.AlignmentType.Left:
								yPos = leftBorder + 1;
								break;

							case StyledTableColumn.AlignmentType.Center:
								yPos = leftBorder + ((columns[i].Width / 2) - (sf.Width / 2));
								break;

							case StyledTableColumn.AlignmentType.Right:
								yPos = leftBorder + columns[i].Width - sf.Width;
								break;

							default:
								yPos = leftBorder + 1;
								break;
						}
						g.DrawString(theLines[j].ToString(),headerFont,new SolidBrush(this.mHeaderFontColor),yPos,TargetRegion.Top+cellHeight*j+cellHeight/2-sf.Height/2);
					}
					leftBorder += columns[i].Width;
				}
				
			}

			g.DrawLine(new Pen(this.mBorderColor,1), TargetRegion.X, TargetRegion.Y, TargetRegion.X+TargetRegion.Width, TargetRegion.Y);

			return DrawnSoFar+MaxLinesDrawn;
		}
		
		
		private int drawDataRow(Graphics g, int theRow, int drawnSoFar)
		{
			 
			int numOfRows = CalculateRelativeDataRowHeight(theData.Rows[theRow],g);

			if (theRow > theData.Rows.Count || theRow < 0 || drawnSoFar >= GetPossibleRowNumber() || drawnSoFar+numOfRows > GetPossibleRowNumber()) return drawnSoFar;

			int maxDrawnRows = 0;			
			int	topBorder = theRegion.Top + drawnSoFar*cellHeight;

			g.DrawLine(new Pen(this.mBorderColor,1),theRegion.X,topBorder,theRegion.Right,topBorder);

			int leftBorder = theRegion.X;
			for (int i=0;i<theData.Rows[theRow].ItemArray.Length;i++)
			{
				if (Columns[i].Visible)
				{
					object theObject = theData.Rows[theRow].ItemArray.GetValue(i);				

					// format object as string
					string tmpValue = FormatValue(theObject,theData.Columns[i].DataType, Columns[i].FormatMask);

					string theLine = "";
					int drawnRowsCounter = 0;

					bool hasMore = false;
					do
					{
						hasMore = splitString(ref tmpValue,ref theLine,columns[i].Width,g,dataFont);
						SizeF sf = g.MeasureString(theLine,dataFont);

						float yPos = leftBorder+1;

						switch (columns[i].Alignment)
						{
							case StyledTableColumn.AlignmentType.Left:
								yPos = leftBorder + 1;
								break;

							case StyledTableColumn.AlignmentType.Center:
								yPos = leftBorder + ((columns[i].Width / 2) - (sf.Width / 2));
								break;

							case StyledTableColumn.AlignmentType.Right:
								yPos = leftBorder + columns[i].Width - sf.Width;
								break;

							default:
								yPos = leftBorder + 1;
								break;
						}

						g.DrawString(theLine,dataFont,new SolidBrush(this.mDataFontColor),yPos,topBorder+cellHeight*drawnRowsCounter+cellHeight/2-sf.Height/2);	
						drawnRowsCounter++;
					}
					while ( hasMore );

					leftBorder += columns[i].Width;

					if (drawnRowsCounter>maxDrawnRows)
						maxDrawnRows = drawnRowsCounter;
				}
			}

			return drawnSoFar+maxDrawnRows;
		}

		
		//   formating datatable objects
		//   TODO : improve this function 
		//   for instance : dates with respect to user input locale, etc.
		private string FormatValue(object theObject,Type tip, string FormatMask)
		{
			string theValue = "";
			if ( theObject == DBNull.Value )
			{
				theValue = "";
			}
			else
			{
				try
				{
					if (FormatMask != "")
					{
						theValue = string.Format("{0:"+FormatMask+"}", theObject);
					}
					else
					{
						if (tip == System.Type.GetType("System.DateTime") )
						{
							theValue = ((DateTime)theObject).ToString("dd.MM.yyyy.");
						}
						else if (tip == System.Type.GetType("System.Decimal") )
						{
							theValue = ((Decimal)theObject).ToString("f02");
						}
						else if (tip == System.Type.GetType("System.UInt32") || tip == System.Type.GetType("System.UInt16") || tip == System.Type.GetType("System.UInt64") || tip == System.Type.GetType("System.Int16") || tip == System.Type.GetType("System.Int32") || tip == System.Type.GetType("System.Int64"))
						{
							theValue = theObject.ToString();
						}
						else
						{
							theValue = theObject.ToString();
						}
					}
				}
				catch
				{
					if (tip == System.Type.GetType("System.DateTime") )
					{
						theValue = ((DateTime)theObject).ToString("dd.MM.yyyy.");
					}
					else if (tip == System.Type.GetType("System.Decimal") )
					{
						theValue = ((Decimal)theObject).ToString("f02");
					}
					else if (tip == System.Type.GetType("System.UInt32") || tip == System.Type.GetType("System.UInt16") || tip == System.Type.GetType("System.UInt64") || tip == System.Type.GetType("System.Int16") || tip == System.Type.GetType("System.Int32") || tip == System.Type.GetType("System.Int64"))
					{
						theValue = theObject.ToString();
					}
					else
					{
						theValue = theObject.ToString();
					}
				}
				
			}
			return theValue;
		}

		
		private bool splitString(ref string theSource,ref string theResult,int theWidth,Graphics g,Font theFont)
		{
			int lastBlank = -1;
			string tmpBuffer = "";
			bool hasMore = false;

			for (int i=0;i<theSource.Length;i++)
			{
				if ( theSource[i] == ' ')
					lastBlank = i;
				tmpBuffer += theSource[i];
				SizeF sf = g.MeasureString(tmpBuffer,theFont);
				
				if ( sf.Width > theWidth)
				{
					int breakPoint = lastBlank != -1? lastBlank+1 : (i==0?1:i);
					theResult = theSource.Substring(0,breakPoint);
					theSource = theSource.Substring(breakPoint);
					hasMore = true;
					return hasMore;
				}
			}
			
			theResult = theSource;
			theSource = "";
			return hasMore;
		}


		#endregion

		#region Private Functions

		private void drawEmptyRows(Graphics g,int drawnSoFar)
		{
			int topBorder;
			
			topBorder = theRegion.Top + cellHeight * Math.Min(drawnSoFar,GetPossibleRowNumber() );
			
			for (int j=0;j< GetPossibleRowNumber()- Math.Min(drawnSoFar,GetPossibleRowNumber());j++)
			{

				g.DrawLine(new Pen(Color.Black,1),theRegion.X,topBorder,theRegion.Right,topBorder);

				int leftBorder = theRegion.X;
				for (int i=0;i<theData.Columns.Count;i++)
				{
					SizeF sf = g.MeasureString("",dataFont);
					float yPos = leftBorder+1;

					g.DrawString("",dataFont,new SolidBrush(Color.Black),yPos,topBorder+cellHeight/2-sf.Height/2);
					leftBorder += columns[i].Width;
				}
				topBorder += cellHeight;
			}
		}

		
		private void drawBorders(Graphics g,int drawnSoFar)
		{
			int rectHeight;
			if (theData != null)
			{
				if (doDrawEmptyRows)
					rectHeight = cellHeight * GetPossibleRowNumber() ;
				else
					rectHeight = cellHeight * ( Math.Min(drawnSoFar,GetPossibleRowNumber()) );
			}
			else
				rectHeight = drawnSoFar*cellHeight;

			g.DrawRectangle(new Pen(this.mBorderColor,1),theRegion.X,theRegion.Y,theRegion.Width,rectHeight);

			int leftBorder = theRegion.X;
			for (int i=0;i<columns.Length;i++)
			{
				if (columns[i].Visible)
				{
					g.DrawLine(new Pen(this.mBorderColor,1),leftBorder,theRegion.Y,leftBorder,theRegion.Y+rectHeight);
					leftBorder += columns[i].Width;
				}
			}
		}


		#endregion

		#region ICustomPaint Members

		/// <summary>
		/// Paints the StyledTable
		/// </summary>
		/// <param name="g">The Graphics object to draw</param>
		/// <remarks>Causes the PictureBox to be painted to the screen.</remarks>
		public override void Paint(System.Drawing.Graphics g)
		{
			// TODO:  Add StyledTable.Paint implementation
			
			int RowsDrawnSoFar = 0;

			if (doDrawHeader)
				RowsDrawnSoFar = drawHeader(g, theRegion, RowsDrawnSoFar);

			int GroupByFieldIndex = -1;
			string PreviousGroupByFieldValue = "";
 
			if (this.mGroupByField != "")
			{
				if (theData != null)
				{
					if (theData.Rows.Count > 0)
					{
						GroupByFieldIndex = theData.Columns.IndexOf(this.mGroupByField);
						PreviousGroupByFieldValue = theData.Rows[0][GroupByFieldIndex].ToString();
					}
				}
			}

			if (theData != null)
			{
				for (int i=0;i<theData.Rows.Count;i++)
				{
					DataRow nextRow = theData.Rows[i];

					if (this.mGroupByField != "")
					{
						if (PreviousGroupByFieldValue != nextRow[GroupByFieldIndex].ToString())
						{
							Rectangle temp = new Rectangle(this.X, this.Y+(this.cellHeight*RowsDrawnSoFar), this.Width, this.cellHeight);
							RowsDrawnSoFar = drawHeader(g, temp, RowsDrawnSoFar);

							PreviousGroupByFieldValue = nextRow[GroupByFieldIndex].ToString();
						}
					}

					RowsDrawnSoFar = drawDataRow(g,i,RowsDrawnSoFar);	
				}
				
				if (doDrawEmptyRows)
					drawEmptyRows(g,RowsDrawnSoFar);
			}

			drawBorders(g,RowsDrawnSoFar);
		}


		/// <summary>
		/// Gets the region of the current StyledTable
		/// </summary>
		/// <returns>System.Drawing.Rectangle</returns>
		public override Rectangle GetRegion()
		{
			return theRegion;
		}


		/// <summary>
		/// Clones the structure of the StyledTable, including all properties
		/// </summary>
		/// <returns><see cref="daReport.ChartBox">daReport.ChartBox</see></returns>
		public override object Clone()
		{
			StyledTable tmp = new StyledTable();
			tmp.document = this.document;
			tmp.X = this.X;
			tmp.Y = this.Y;
			tmp.Width = this.Width;
			tmp.Height = this.Height;
			tmp.DataFont = this.DataFont;

			StyledTableColumn[] cols = new StyledTableColumn[this.Columns.Length];
			for (int i=0;i<this.Columns.Length;i++)
				cols[i] = this.Columns[i].Clone() as StyledTableColumn;
			tmp.Columns = cols;

			tmp.Data = this.Data;
			tmp.DataSource = this.DataSource;
			tmp.CellHeight = this.CellHeight;
			tmp.DrawEmptyRows = this.DrawEmptyRows;
			tmp.DrawHeader = this.DrawHeader;
			tmp.HeaderBackgroundColor = this.HeaderBackgroundColor;
			tmp.HeaderFont = this.HeaderFont;
			return tmp;
		}


		#endregion

		#region Creator

		/// <summary>
		/// Initializes a new instance of the StyledTable class.
		/// </summary>
		public StyledTable()
		{
			
		}


		/// <summary>
		/// Initializes a new instance of the StyledTable class.
		/// </summary>
		/// <param name="originX">x-position of the new StyledTable</param>
		/// <param name="originY">y-position of the new StyledTable</param>
		/// <param name="width">Width of the new StyledTable</param>
		/// <param name="height">Height of the new StyledTable</param>
		/// <param name="parent">Parent of the new StyledTable</param>
		public StyledTable(int originX,int originY,int width,int height, DaPrintDocument parent):this()
		{
			document = parent;
			theRegion = new Rectangle(originX,originY,width,height);
		}

		
		#endregion
	}
}