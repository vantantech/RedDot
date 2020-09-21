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


namespace daReport
{
	/// <summary>
	/// Class representing the ICustomPaint object.
	/// </summary>
	/// <remarks>
	/// ICustomPaint is the base for all document types and contains some properties and methods
	/// common to all document types.
	/// </remarks>
	public abstract class ICustomPaint : ICloneable
	{
		#region Declarations
		// the parent document of element
		// this reference is mandatory due to handling margins 
		/// <summary>
		/// The <see cref="daReport.DaPrintDocument">DaReport.PrintDocument</see> object
		/// </summary>
		protected DaPrintDocument document = null;

		/// <summary>
		/// Enumeration of possible horizontal alignments for the PrintDocument
		/// </summary>
		public enum HorizontalAlignmentTypes
		{
			/// <summary>No alignment. Manual placement</summary>
			None=0,
			/// <summary>Left Alignment on the page</summary>
			Left,
			/// <summary>Canter Alignment on the page</summary>
			Center,
			/// <summary>Right Alignment on the page</summary>
			Right
		};

		/// <summary>
		/// Enumeration of possible vertical alignments for the PrintDocument
		/// </summary>
		public enum VerticalAlignmentTypes
		{
			/// <summary>No alignment. Manual placement</summary>
			None=0,
			/// <summary>Aligns to the top the page</summary>
			Top,
			/// <summary>Aligns to the middle the page</summary>
			Middle,
			/// <summary>Aligns to the bottom the page</summary>
			Bottom
		};
		
		/// <summary>
		/// Horizontal alignment of the object compared to the page
		/// </summary>
		protected HorizontalAlignmentTypes horizontalAlignment = HorizontalAlignmentTypes.None;

		/// <summary>
		/// Vertical alignment of the object compared to the page
		/// </summary>
		protected VerticalAlignmentTypes verticalAlignment = VerticalAlignmentTypes.None;

		#endregion

		#region Public Abstract Declarations

		/// <summary>
		/// When implemented by a class, the object is painted to the screen.
		/// </summary>
		public abstract void Paint(Graphics g);
		
		/// <summary>
		/// When implemented by a class, gets or sets the region the object is contained in.
		/// </summary>
		public abstract Rectangle GetRegion();

		/// <summary>
		/// When implemented by a class, gets or sets the horizontal alignment of the object
		/// </summary>
		public abstract ICustomPaint.HorizontalAlignmentTypes HorizontalAlignment
		{
			get;
			set; 		
		}


		/// <summary>
		/// When implemented by a class, gets or sets the vertical alignment of the object
		/// </summary>
		public abstract ICustomPaint.VerticalAlignmentTypes VerticalAlignment
		{
			get;
			set; 		
		}

		/// <summary>
		/// When implemented by a class, gets or sets whether the object is selectable in the Designer pane
		/// </summary>
		public abstract bool Selectable
		{
			get;
			set; 		
		}


		/// <summary>
		/// When implemented by a class, gets or sets the x-location of the object
		/// </summary>
		public abstract int X
		{
			get;
			set;
		}

		
		/// <summary>
		/// When implemented by a class, gets or sets the y-location of the object
		/// </summary>
		public abstract int Y
		{
			get;
			set;
		}

		
		/// <summary>
		/// When implemented by a class, gets or sets the width of the object
		/// </summary>
		public abstract int Width
		{
			get;
			set;
		}


		/// <summary>
		/// When implemented by a class, gets or sets the height of the object
		/// </summary>
		public abstract int Height
		{
			get;
			set;
		}


		#endregion

		#region ICloneable Members

		// needed for duplicateXXX() functions in design mode
		// the clone object MUST BE deep copy of the original element
		/// <summary>
		/// When implemented by a class, creates a deep copy of the object.
		/// </summary>
		/// <returns></returns>
		public abstract object Clone();		

		#endregion
	}
}
