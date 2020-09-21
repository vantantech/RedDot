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
using System.Data;
using System.ComponentModel;
using System.Collections;


namespace daReport
{
	/// <summary>
	/// Class representing the ChartBox object.
	/// </summary>
	/// <remarks>The ChartBox is used in reports where graphical representation of data is used.</remarks>
	public class ChartBox : ICustomPaint 
	{
		#region Declarations
		/// <summary>
		/// Enumeration of possible chart types
		/// </summary>
		public enum ChartType
		{
			/// <summary>Bar Graph</summary>
			Bar = 1,
			/// <summary>Pie Chart</summary>
			Pie
		};

		private Rectangle mRegion = new Rectangle(0,0,0,0);
		private Rectangle mMapArea = new Rectangle(0,0,0,0);
		private Rectangle mLegendArea = new Rectangle(0,0,0,0);
		
		private string mName = "chart0";
		private string mTitle = "Chart mTitle";
		private string mXLabel = "Categories";
		//private string yLabel = "Values";
		private ChartType mChartType = ChartType.Bar;
		private string[] mXCategories = new string[0];
		private ArrayList mYSeries = new ArrayList();
		private string[] mSeriesNames = new string[0];
		private bool mShowLegend = false;

		private Color mBorderColor = Color.Black;
		private Color mMapAreaColor = Color.WhiteSmoke;
		private int mBorderWidth = 1;
		private Font mTitleFont = new Font("Tahoma",8,FontStyle.Bold);
		private Font mLabelFont = new Font("Tahoma",8,FontStyle.Regular);
		private Color[] mSeriesColors = new Color[0];
		private Color[] mPieColors = new Color[0];

		private float mMaxY = 100f;
		private float mMinY = 0f;

		private bool mSelectable = true;
		#endregion

		#region Public Properties

		/// <summary>
		/// Gets/Sets the border color for the ChartBox
		/// </summary>
		/// <remarks>This property sets the border color of the ChartBox object. This can be any color
		/// from the System.Drawing.Color structure
		/// </remarks>
		public Color BorderColor
		{
			get {return mBorderColor;}
			set {mBorderColor = value;}
		}


		/// <summary>
		/// Gets/Sets the border width for the ChartBox
		/// </summary>
		/// <remarks>
		/// BorderWidth of the ChartBox. If this is set to zero, then the border is invisible
		/// </remarks>
		public int BorderWidth
		{
			get {return mBorderWidth;}
			set {mBorderWidth = value;}
		}


		/// <summary>
		/// Gets/Sets the font used for any labels displayed on the ChartReport
		/// </summary>
		public Font LabelFont
		{
			get {return mLabelFont;}
			set {mLabelFont = value;}
		}


		/// <summary>
		/// The background color displayed behind the chart image
		/// </summary>
		/// <remarks>This property sets the background color displayed behind the chart image. This can be any color
		/// from the System.Drawing.Color structure
		/// </remarks>
		[Description("The color for map area within the chart.")]
		public Color MapAreaColor
		{
			get {return mMapAreaColor;}
			set {mMapAreaColor = value;}
		}


		/// <summary>
		/// Gets/Sets the name of the chart.
		/// </summary>
		/// <remarks>This property is used in code when setting other properties programmaticly.</remarks>
		[Description("The name of the chart. This property is used in code when setting other properties programmaticly.")]
		public string Name
		{
			get {return mName;}
			set {mName = value;}
		}


		/// <summary>
		/// Gets/Sets whether to display the ChartBox's legend
		/// </summary>
		[Description("Show legend in chart.")]
		public bool ShowLegend
		{
			get {return mShowLegend;}
			set {mShowLegend = value;}
		}


		/// <summary>
		/// Gets/Sets the title: value used at the top of the chart
		/// </summary>
		[Description("The Title of the chart.")]
		public string Title
		{
			get {return mTitle;}
			set {mTitle = value;}
		}


		


		/// <summary>
		/// Gets/Sets the font used for the title of the ChartReport
		/// </summary>
		public Font TitleFont
		{
			get {return mTitleFont;}
			set {mTitleFont = value;}
		}


		/// <summary>
		/// Gets/Sets the type of the ChartBox
		/// </summary>
		[Description("The type of data presentation.")]
		public ChartType Type
		{
			get {return mChartType;}
			set {mChartType = value;}
		}


		/// <summary>
		/// XCategories is a string array of the categories defined in the chart data.
		/// </summary>
		[Browsable(false)]
		public string[] XCategories
		{
			get {return mXCategories;}
			set {mXCategories = value;}
		}


		/// <summary>
		/// Gets/Sets the value used for labelling the X-axis of the ChartReport
		/// </summary>
		[Description("The X-axis label.")]
		public string XLabel
		{
			get {return mXLabel;}
			set {mXLabel = value;}
		}


		#endregion

		#region Public Overrides

		/// <summary>
		///  Gets or sets the horizontal alignment of the ChartBox
		/// </summary>
		[Description("Horizontal alignment in the page, relative to margins. This property overrides element coordinates.")]
		public override ICustomPaint.HorizontalAlignmentTypes HorizontalAlignment
		{
			get {return horizontalAlignment;}
			set 
			{
				horizontalAlignment = value;
				switch (horizontalAlignment)
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
		/// Clones the structure of the ChartBox, including all properties
		/// </summary>
		/// <returns><see cref="daReport.ChartBox">daReport.ChartBox</see></returns>
		public override object Clone()
		{
			ChartBox tmp = new ChartBox(0,0,0,0,document);
			tmp.X = this.X;
			tmp.Y = this.Y;
			tmp.Width = this.Width;
			tmp.Height = this.Height;
			tmp.BorderWidth = this.BorderWidth;
			tmp.BorderColor = this.BorderColor;
			tmp.Type = this.Type;
			tmp.mTitleFont = this.mTitleFont;
			tmp.mTitle = this.mTitle;
			tmp.ShowLegend = this.ShowLegend;
			tmp.MapAreaColor = this.MapAreaColor;
			tmp.mLabelFont = this.mLabelFont;
			tmp.XLabel = this.XLabel;
			tmp.Name = "chart"+tmp.GetHashCode().ToString();

			return tmp;
		}


		/// <summary>
		/// Gets the region of the current ChartBox
		/// </summary>
		/// <returns>System.Drawing.Rectangle</returns>
		public override Rectangle GetRegion()
		{
			return mRegion;
		}


		/// <summary>
		///  Gets or sets the height of the ChartBox
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
		/// Paints the ChartBox
		/// </summary>
		/// <param name="g">The Graphics object to draw</param>
		/// <remarks>Causes the ChartBox to be painted to the screen.</remarks>
		public override void Paint(Graphics g)
		{
			if ( mChartType == ChartType.Bar)
				drawBarsDiagram(g);
			else if ( mChartType == ChartType.Pie)
				drawPieDiagram(g);
			
			if ( mBorderWidth > 0 )
				g.DrawRectangle( new Pen(mBorderColor,mBorderWidth),mRegion);
			
		}

		
		/// <summary>
		///  Gets or sets the vertical alignment of the ChartBox
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
		///  Gets or sets the width of the ChartBox.
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
		/// The X coordinate of the left-upper corner of the element
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
		/// The Y coordinate of the left-upper corner of the element
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
		/// Gets/Sets whether the ChartBox is selectable in the design pane of the DaReport Designer
		/// </summary>
		/// <remarks>If set to true, then the ChartBox is not selectable in the DaReport Designer application
		/// design pane. It is still selectable in the tree view listing of objects.
		/// </remarks>
		[Description("Sets whether the object can be selected in the design pane.")]
		public override bool Selectable
		{
			get { return this.mSelectable; }
			set { this.mSelectable=value; }
		}
		
		#endregion

		#region Public Functions

		/// <summary>
		/// A serie is used to populate the ChartBox.
		/// </summary>
		/// <param name="serieName">Name of the serie. Displayed in the Legend</param>
		/// <param name="values">A array of Double values</param>
		/// <param name="serieColor">Color of the bar/pie</param>
		/// <remarks>
		/// You can add multiple serie to a chart. This would be useful for comparing values over certain years.
		/// Please see the "Chart Report" example, especially the City Population chart
		/// C# Sample
		/// <code language="c#">
		/// daPrintDocument.AddChartSerie("chart0","Year 1975.",new double[3]{15.9,11.4,11.2},Color.DarkGreen);
		/// </code>
		/// VB.Net Sample
		/// <code language="Visual Basic">
		/// daPrintDocument.AddChartSerie("chart0", "Year 1975.", New Double() {15.9, 11.4, 11.2}, Color.DarkGreen)
		/// </code>
		/// </remarks>
		public void AddSerie(string serieName,double[] values,Color serieColor)
		{
			mYSeries.Add(values);

			Color[] tmp = new Color[mSeriesColors.Length+1];
			Array.Copy(mSeriesColors,0,tmp,0,mSeriesColors.Length);
			tmp[mSeriesColors.Length] = serieColor;
			mSeriesColors = tmp;

			string[] tmp1 = new string[mSeriesNames.Length+1];
			Array.Copy(mSeriesNames,0,tmp1,0,mSeriesNames.Length);
			tmp1[mSeriesNames.Length] = serieName;
			mSeriesNames = tmp1;

		}

		
		#endregion

		#region Private Functions
		
		private void calculateBarsMapArea(Graphics g)
		{
			SizeF sf = g.MeasureString(mTitle,mTitleFont);
			SizeF sfLabel = g.MeasureString(mXLabel,mLabelFont);

			if (mShowLegend)
			{
				int originX = (int) (mRegion.X+mRegion.Width*0.08);
				int originY = (int) (mRegion.Y+sf.Height+10);
				int width = (int) (mRegion.Width*0.68);
				int height = (int) (mRegion.Height-sf.Height-10-2*sfLabel.Height-9);

				mMapArea = new Rectangle(originX,originY,width,height);
			
				originX = (int) (mRegion.X+mRegion.Width*0.78);
				originY = (int) (mRegion.Y+sf.Height+10);
				width = (int) (mRegion.Width*0.20);
				height = (int) (mRegion.Height-sf.Height-10-2*sfLabel.Height-9);

				mLegendArea = new Rectangle(originX,originY,width,height);
			}
			else
			{
				int originX = (int) (mRegion.X+mRegion.Width*0.1);
				int originY = (int) (mRegion.Y+sf.Height+10);
				int width = (int) (mRegion.Width*0.8);
				int height = (int) (mRegion.Height-sf.Height-10-2*sfLabel.Height-9);

				mMapArea = new Rectangle(originX,originY,width,height);
			
			}
		}

		
		private void calculatePieMapArea(Graphics g)
		{
			SizeF sf = g.MeasureString(mTitle,mTitleFont);
			SizeF sfLabel = g.MeasureString(mXLabel,mLabelFont);

			if (mShowLegend)
			{
				int originX = (int) (mRegion.X+mRegion.Width*0.02);
				int originY = (int) (mRegion.Y+sf.Height+12);
				int width = (int) (mRegion.Width*0.72);
				int height = (int) (mRegion.Height-sf.Height-20);

				mMapArea = new Rectangle(originX,originY,width,height);
			
				originX = (int) (mRegion.X+mRegion.Width*0.76);
				originY = (int) (mRegion.Y+sf.Height+12);
				width = (int) (mRegion.Width*0.22);
				height = (int) (mRegion.Height-sf.Height-20);

				mLegendArea = new Rectangle(originX,originY,width,height);
			}
			else
			{
				int originX = (int) (mRegion.X+mRegion.Width*0.02);
				int originY = (int) (mRegion.Y+sf.Height+12);
				int width = (int) (mRegion.Width*0.96);
				int height = (int) (mRegion.Height-sf.Height-20);

				mMapArea = new Rectangle(originX,originY,width,height);
			
			}
		}

		
		private void drawBars(Graphics g)
		{
			float overlappingFactor = 0.2f;
			float fillSliceFactor = 0.9f;

			int theNumberOfCategories = mXCategories.Length;
			if (theNumberOfCategories==0) return;

			int theNumberOfSeries = mYSeries.Count;

			float sliceWidth = (float)mMapArea.Width/theNumberOfCategories;

			float recWidth = fillSliceFactor*sliceWidth/(theNumberOfSeries*(1-overlappingFactor)+overlappingFactor);
			float recOffset = recWidth*(1-overlappingFactor);
			
			
			float zeroPosition = calculateZeroPosition();


			for (int i=0;i<mYSeries.Count;i++)
			{
				double[] theValues = (double[]) mYSeries[i];
	
				for (int j=0;j<theNumberOfCategories;j++)
				{
					double theValue = 0;
					if (j<theValues.Length)
						theValue = theValues[j];
					
					
					float recHeight = calculateBarHeight(theValue);
					
					if (theValue>=0)
					{
						g.FillRectangle(new SolidBrush(mSeriesColors[i]),mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset,zeroPosition-recHeight,recWidth,recHeight);
						g.DrawRectangle(new Pen(Color.Black,1),mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset,zeroPosition-recHeight,recWidth,recHeight);
					}
					else
					{
						g.FillRectangle(new SolidBrush(mSeriesColors[i]),mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset,zeroPosition,recWidth,recHeight);
						g.DrawRectangle(new Pen(Color.Black,1),mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset,zeroPosition,recWidth,recHeight);
					}

				}
			}
		}


		private void drawBarsDiagram(Graphics g)
		{
			drawTitle(g);
			calculateBarsMapArea(g);
			drawBarsMapArea(g);
			drawLabels(g);
			drawBars(g);
			if (mShowLegend)
				drawBarsLegend(g);
		}

		
		private void drawBarsLegend(Graphics g)
		{
			g.DrawRectangle(new Pen(Color.Black,1),mLegendArea);
			
			for (int i=0;i<mYSeries.Count;i++)
			{
				SizeF sf = g.MeasureString(mSeriesNames[i],mLabelFont);
				int yPos = mLegendArea.Y + 8 + i*20;
				g.FillRectangle(new SolidBrush(mSeriesColors[i]),mLegendArea.X+4,yPos,6,6);
				g.DrawString(mSeriesNames[i],mLabelFont,new SolidBrush(Color.Black),mLegendArea.X+11,yPos+3-sf.Height/2);	
			}
		}

		
		private void drawBarsMapArea(Graphics g)
		{
			g.FillRectangle(new SolidBrush(mMapAreaColor),mMapArea);
			g.DrawRectangle(new Pen(Color.Black,1),mMapArea);

			int theNumberOfCategories = mXCategories.Length;
			if (theNumberOfCategories==0) return;

			float sliceWidth = (float)mMapArea.Width/theNumberOfCategories;

			for (int i=0;i<theNumberOfCategories;i++)
			{
				SizeF sf = g.MeasureString(mXCategories[i],mLabelFont);
				g.DrawString(mXCategories[i],mLabelFont,new SolidBrush(Color.Black),mMapArea.X+i*sliceWidth+sliceWidth/2-sf.Width/2,mRegion.Bottom-2*sf.Height-6);

				if (i!= theNumberOfCategories-1)
					g.DrawLine(new Pen(Color.Black,1),mMapArea.X+(i+1)*sliceWidth,mMapArea.Top,mMapArea.X+(i+1)*sliceWidth,mMapArea.Bottom);
			}
		}


		private void drawLabels(Graphics g)
		{
			resolveRange();

			SizeF sf = g.MeasureString(mXLabel,mLabelFont);
			g.DrawString(mXLabel,mLabelFont,new SolidBrush(Color.Black),mMapArea.X+mMapArea.Width/2-sf.Width/2,mRegion.Bottom-sf.Height-3);

			float step = Math.Min(Math.Abs(mMinY),Math.Abs(mMaxY))/2f;

			if (mMaxY!=0 && mMinY!=0)
				step = Math.Min(Math.Abs(mMinY),Math.Abs(mMaxY))/2f;
			else if (mMaxY!=0 && mMinY==0)
				step = Math.Abs(mMaxY)/4f;
			else if (mMaxY==0 && mMinY!=0)
				step = Math.Abs(mMinY)/4f;
			else
				return;

			for ( float i=mMinY;i<=mMaxY;i+=step)
			{
				string numLabel = i.ToString("#,##0.#");
				sf = g.MeasureString(numLabel,mLabelFont);
				float theYPosition = calculatePosition(i);
				g.DrawString(numLabel,mLabelFont,new SolidBrush(Color.Black),mMapArea.Left-2-sf.Width,theYPosition-sf.Height/2);
				if (i!=mMinY && i!=mMaxY)
					g.DrawLine(new Pen(Color.DarkGray,1),mMapArea.Left,theYPosition,mMapArea.Right,theYPosition);
			}
			
		}

		
		private void drawPie(Graphics g)
		{
			if (mYSeries.Count == 0) return;

			int theNumberOfCategories = mXCategories.Length;
			double[] theValues = mYSeries[0] as double[];

			double theSum = 0;
			for (int i=0;i<theNumberOfCategories;i++)
			{
				double x = 0;
				if (i<theValues.Length)
					x = Math.Abs(theValues[i]);

				theSum += x;			
			}

			if (theSum==0) return;

			int pieRecWidth = (int) (Math.Min(mMapArea.Width,mMapArea.Height)*0.82f);
			Rectangle pieRectangle = new Rectangle(mMapArea.X + (mMapArea.Width-pieRecWidth)/2,mMapArea.Y + (mMapArea.Height-pieRecWidth)/2,pieRecWidth,pieRecWidth);
			

			int colorRedStep = (int)((255-mSeriesColors[0].R)/theNumberOfCategories);
			int colorGreenStep = (int)((255-mSeriesColors[0].G)/theNumberOfCategories);
			int colorBlueStep = (int)((255-mSeriesColors[0].B)/theNumberOfCategories);
			mPieColors = new Color[theNumberOfCategories];

			float currentAngle = 0;
			for (int i=0;i<theNumberOfCategories;i++)
			{
				double x = 0;
				if (i<theValues.Length)
					x = Math.Abs(theValues[i]);

				mPieColors[i] = Color.FromArgb( mSeriesColors[0].R+i*colorRedStep,mSeriesColors[0].G+i*colorGreenStep,mSeriesColors[0].B+i*colorBlueStep);				
				float theAngle = (float) (360 * Math.Abs(x) / theSum) ;
				float percentage = (float) (Math.Abs(x) / theSum) ;

				
				g.FillPie(new SolidBrush(mPieColors[i]),pieRectangle,currentAngle,theAngle);
				g.DrawPie(new Pen(Color.Black,1),pieRectangle,currentAngle,theAngle);

				float labelAngle = currentAngle + theAngle/2;
				int labelRadius = pieRecWidth/2 + 1;
				
				string labelString = XCategories[i] + " (" + percentage.ToString("#0.#%") + ")"; 

				float labelX = (float)(pieRectangle.X + pieRectangle.Width/2 + labelRadius*Math.Cos(6.28*labelAngle/360)) ;
				float labelY = (float)(pieRectangle.Y + pieRectangle.Height/2 + labelRadius*Math.Sin(6.28*labelAngle/360)) ;

				SizeF sf = g.MeasureString(labelString,mLabelFont);

				if (labelX < pieRectangle.X + pieRectangle.Width/2)
					labelX = labelX - sf.Width;
				if (labelY < pieRectangle.Y + pieRectangle.Height/2)
					labelY = labelY - sf.Height;
				
				if (percentage>0.01)
					g.DrawString(labelString,mLabelFont,new SolidBrush(Color.Black),labelX,labelY);

				currentAngle += theAngle;
			}
		}

		
		private void drawPieDiagram(Graphics g)
		{
			drawTitle(g);
			calculatePieMapArea(g);
			drawPieMapArea(g);
			drawPie(g);
			if (mShowLegend)
				drawPieLegend(g);
		}


		private void drawPieLegend(Graphics g)
		{
			g.DrawRectangle(new Pen(Color.Black,1),mLegendArea);
		
			if (XCategories.Length==0 || mYSeries.Count == 0) return;

			double[] theValues = mYSeries[0] as double[];
			for (int i=0;i<XCategories.Length;i++)
			{
				double x = 0;
				if (i<theValues.Length)
					x = Math.Abs(theValues[i]);

				string legendText = XCategories[i] + " (" + x.ToString("#0.##") + ")";

				SizeF sf = g.MeasureString(legendText,mLabelFont);
				int yPos = mLegendArea.Y + 8 + i*20;
				g.FillRectangle(new SolidBrush(mPieColors[i]),mLegendArea.X+4,yPos,6,6);
				g.DrawString(legendText,mLabelFont,new SolidBrush(Color.Black),mLegendArea.X+11,yPos+3-sf.Height/2);	
			}
		}

		
		private void drawPieMapArea(Graphics g)
		{
			g.FillRectangle(new SolidBrush(this.mMapAreaColor),mMapArea);
		}

		
		private void drawTitle(Graphics g)
		{
			SizeF sf = g.MeasureString(mTitle,mTitleFont);
			g.DrawString(mTitle,mTitleFont,new SolidBrush(Color.Black),mRegion.X+mRegion.Width/2-sf.Width/2,mRegion.Y+5);
		}

		
		private void resolveRange()
		{
			double maxValue = 0;
			double minValue = 0;

			for (int i=0;i<mYSeries.Count;i++)
			{
				double[] theValues = (double[])mYSeries[i];

				double[] tmp = new double[theValues.Length];
				Array.Copy(theValues,0,tmp,0,theValues.Length);

				Array.Sort(tmp);
				maxValue = Math.Max(maxValue,tmp[tmp.Length-1]);
				minValue = Math.Min(minValue,tmp[0]);
			}

			if (maxValue==0 && minValue==0)
			{
				maxValue = 100;
				minValue = 0;
			}
			else if (maxValue!=0 && minValue==0)
			{
				int theBase = (int)Math.Ceiling(Math.Log10(maxValue));
				mMaxY = (float)Math.Pow(10,theBase);

				if (maxValue<=mMaxY/2) mMaxY=mMaxY/2;
			}
			else
			{
				int theBase = (int)Math.Max( Math.Ceiling(Math.Log10(Math.Abs(maxValue))),Math.Ceiling(Math.Log10(Math.Abs(minValue))) );
				mMaxY = (float)Math.Pow(10,theBase);
				mMinY = (float)Math.Pow(10,theBase)*Math.Sign(minValue);

				if (maxValue<=mMaxY/2) mMaxY=mMaxY/2;
				if (minValue>=mMinY/2) mMinY=mMinY/2;

			}

		}

		
		#endregion

		#region Private Properties

		private float calculateBarHeight(double theValue)
		{
			double ratio = (double)mMapArea.Height/(mMaxY-mMinY);
			return (float) Math.Abs(theValue*ratio);
		}

		
		private float calculatePosition(float x)
		{
			return calculateZeroPosition() - mMapArea.Height*x/(mMaxY-mMinY);
		}


		private float calculateZeroPosition()
		{
			return (float)(mMapArea.Top+mMapArea.Height*mMaxY/(mMaxY-mMinY));
		}

		
		#endregion

		#region Creator

		/// <summary>
		/// Initializes a new instance of the ChartBox class.
		/// </summary>
		/// <param name="x">x-position of the new ChartBox</param>
		/// <param name="y">y-position of the new ChartBox</param>
		/// <param name="width">Width of the new ChartBox</param>
		/// <param name="height">Height of the new ChartBox</param>
		/// <param name="parent">Parent of the new ChartBox</param>
		public ChartBox(int x,int y, int width, int height, DaPrintDocument parent)
		{
			document = parent;
			mRegion = new Rectangle(x,y,width,height);	
		}
		

		#endregion
	}
}
