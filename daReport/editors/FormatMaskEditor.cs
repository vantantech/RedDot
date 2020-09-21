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
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;


namespace daReport.editors
{
	/// <summary>
	/// FormatMaskEditor: provides editing interface for the FormatMask property
	/// </summary>
	public class FormatMaskEditor : UITypeEditor
	{
		#region Creator

		/// <summary>
		/// Initializes a new instance of the FormatMaskEditor class.
		/// </summary>
		public FormatMaskEditor()
		{
		}


		#endregion

		#region Public Overrides

		/// <summary>
		/// Edits the value of the specified object using the editor style indicated by GetEditStyle.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that can be used to gain additional context information.</param>
		/// <param name="provider">An IServiceProvider that this editor can use to obtain services. </param>
		/// <param name="value">The object to edit. </param>
		/// <returns>The new value of the object.</returns>
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			//return base.EditValue (context, provider, value);
			// Attempts to obtain an IWindowsFormsEditorService.
			IWindowsFormsEditorService edSvc  = ((IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService)));
				//CType(provider.GetService(GetType(IWindowsFormsEditorService)), IWindowsFormsEditorService)
			if (edSvc == null)
				return null;

			//Displays a StringInputDialog Form to get a user-adjustable 
			//string value.
			FormatMaskEditorDialog f = new FormatMaskEditorDialog();
			f.FormatMask = value.ToString();
			if (edSvc.ShowDialog(f) == DialogResult.OK)
			{
				return f.FormatMask;
			}

			//If OK was not pressed, return the original value
			return value;
		}


		/// <summary>
		/// Gets the editor style used by the EditValue method.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that can be used to gain additional context information. </param>
		/// <returns>UITypeEditorEditStyle.Modal indicating the editor will be a modal form </returns>
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			//return base.GetEditStyle (context);
			return UITypeEditorEditStyle.Modal;
		}


		#endregion
	}
}
