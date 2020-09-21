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
using System.ComponentModel;
using System.Drawing.Design;


namespace daReport
{
	/// <summary>
	/// Class representing the StyledTableColumn object.
	/// </summary>
	/// <remarks>The StyledTableColumn is used in the
	/// <see cref="daReport.StyledTable.Columns">daReport.StyledTable.Columns</see> collection
	/// </remarks>
	public class StyledTableColumn : ICloneable
	{
		#region Declarations
		/// <summary>
		/// Enumeration of possible horizontal alignments for the StyledTableColumn text
		/// </summary>
		public enum AlignmentType
		{
			/// <summary>Left Alignment in the coloumn</summary>
			Left = 1,
			/// <summary>Center Alignment in the coloumn</summary>
			Center,
			/// <summary>Right Alignment in the coloumn</summary>
			Right
		};

		private string mName;
		private string mLabel;
		private int mWidth;
		private string mFormatMask = "";
		private AlignmentType mAlignment;
		private bool mVisible = true;
		#endregion

		#region Public Properties

		/// <summary>
		///  Gets/Sets the horizontal alignment of text in the StyledTableColumn
		/// </summary>
		[Description("Horizontal alignment in the column, relative to borders.")]
		public AlignmentType Alignment
		{
			get {return mAlignment;}
			set {mAlignment = value;}
		}


		/// <summary>
		/// Gets/Sets the FormatMask used to format the data being placed into the row
		/// </summary>
		/// <remarks>These can be any standard formatting of strings used in the string.Format method. This
		/// also depends on the data-type being passed in. For DateTime datatypes, you can use a FormatMask of
		/// "yyyy-MM-dd" for example. Or for currency use a "c" (without quotes for both of these examples)</remarks>
		[Editor(typeof(daReport.editors.FormatMaskEditor), typeof(UITypeEditor))]
		public string FormatMask
		{
			get { return this.mFormatMask; }
			set { this.mFormatMask=value; }
		}


		/// <summary>
		/// Gets/Sets the label of the column displayed in the report
		/// </summary>
		public string Label
		{
			get {return mLabel;}
			set {mLabel = value;}
		}


		/// <summary>
		/// Gets/Sets the name of the column
		/// </summary>
		public string Name
		{
			get {return mName;}
			set {mName = value;}
		}


		/// <summary>
		///  Gets or sets a value indicating whether the StyledTableColumn is displayed.
		/// </summary>
		/// <remarks>If set to false, the column will not be displayed in the report.</remarks>
		[Description("Visibility of the StyledTableColumn.")]
		public bool Visible
		{
			get {return mVisible;}
			set {mVisible = value;}
		}


		/// <summary>
		///  Gets or sets the width of the StyledTableColumn.
		/// </summary>
		/// <remarks>This will affect the width of the overall table.</remarks>
		[Description("The width of the element.")]
		public int Width
		{
			get {return mWidth;}
			set {mWidth = value;}
		}


		#endregion
		
		#region ICloneable Members

		/// <summary>
		/// Clones the structure of the StyledTableColumn, including all properties
		/// </summary>
		/// <returns><see cref="daReport.StyledTableColumn">daReport.StyledTableColumn</see></returns>
		public object Clone()
		{
			StyledTableColumn tmp = new StyledTableColumn();
			tmp.Name = this.Name;
			tmp.Label = this.Label;
			tmp.FormatMask = this.FormatMask;
			tmp.Width = this.Width;
			tmp.Alignment = this.Alignment;
			tmp.Visible = this.Visible;
			return tmp;
		}

		
		#endregion

		#region Creator

		/// <summary>
		/// Initializes a new instance of the StyledTableColumn class.
		/// </summary>
		public StyledTableColumn()
		{
			mName = "columnName";
			mLabel = "columnLabel";
			mWidth = 80;
			mAlignment = AlignmentType.Left;
		}


		#endregion
	}
}