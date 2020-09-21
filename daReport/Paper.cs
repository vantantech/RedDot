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
using System.Collections;


namespace daReport
{
	/// <summary>
	/// Class representing the Paper object.
	/// </summary>
	/// <remarks>Contains static methods related to Paper objects.</remarks>
	public class Paper
	{
		#region Declarations
		/// <summary>
		/// Enumeration of possible paper type selections
		/// </summary>
		public enum Type
		{
			/// <summary>A4 paper (210x297mm)</summary>
			A4=1,
			/// <summary>A5 paper (148x210mm</summary>
			A5,
			/// <summary>Letter Paper (8.5x11 in)</summary>
			Letter,
			/// <summary>Legal Paper (8.4x14 in)</summary>
			Legal,
			/// <summary>Executive paper (184x266mm)</summary>
			Executive,
			/// <summary>Envelope Monarch paper (98x190mm)</summary>
			EnvelopeMonarch,
			/// <summary>EnvelopeC5 paper (162x229mm</summary>
			EnvelopeC5,
			/// <summary>Envelope Paper (110x220 mm)</summary>
			EnvelopeDL
		};		
		#endregion

		#region Public Properties

		/// <summary>
		/// Returns width an height of paper type
		/// </summary>
		/// <param name="PaperType">PaperType: <see cref="daReport.Paper.Type">Type</see> of paper</param>
		/// <returns>Returns 2-element array of width and height of the paper type</returns>
		public static int[] GetPaperSize(Paper.Type PaperType)
		{
			Hashtable types = new Hashtable();
			types.Add(Type.A4, new int[2]{827,1169});
			types.Add(Type.A5, new int[2]{583,827});
			types.Add(Type.Letter, new int[2]{850,1100});
			types.Add(Type.Legal, new int[2]{850,1400});
			types.Add(Type.Executive, new int[2]{725,1050});
			types.Add(Type.EnvelopeMonarch, new int[2]{387,750});
			types.Add(Type.EnvelopeC5, new int[2]{638,902});
			types.Add(Type.EnvelopeDL, new int[2]{433,866});

			if (types.Contains(PaperType))
				return (int[])types[PaperType];
			else
				return (int[])types[Type.A4];
		}

		/// <summary>
		/// Returns Paper.Type based on string
		/// </summary>
		/// <param name="PaperType">PaperType: string name of a paper type</param>
		/// <returns>Returns <see cref="daReport.Paper.Type">Type</see> of paper</returns>
		/// <remarks>If the string is not a valid paper type, then A4 is passed back as the default
		/// paper type</remarks>
		public static Type GetType(string PaperType)
		{
			switch (PaperType)
			{
				case "A4" : return Type.A4;
				case "A5" : return Type.A5;
				case "Letter" : return Type.Letter;
				case "Legal" : return Type.Legal;
				case "Executive" : return Type.Executive;
				case "EnvelopeMonarch" : return Type.EnvelopeMonarch;
				case "EnvelopeC5" : return Type.EnvelopeC5;
				case "EnvelopeDL" : return Type.EnvelopeDL;
				default : return Type.A4;
			}
			
		}


		#endregion
	}
}
