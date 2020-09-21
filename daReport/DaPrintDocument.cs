

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Data;
using System.Xml;
using System.Windows.Forms;
using System.IO;


namespace daReport
{
	#region Event Delegate Declarations

	/// <summary>
	/// Represents the method that will handle the MarginsChanged event.
	/// </summary>
	/// <param name="sender">The source of the event. </param>
	/// <remarks>
	/// When you create a MarginsChangedHandler delegate, you identify the method that will handle the event.
	/// To associate the event with your event handler, add an instance of the delegate to the event. The event
	/// handler is called whenever the event occurs, until you remove the delegate.
	/// Note: The declaration of your event handler must have the same parameters as the
	/// MarginsChangedHandler delegate declaration.
	/// </remarks>
	public delegate void MarginsChangedHandler(object sender);

	/// <summary>
	/// Represents the method that will handle the PageSizeChanged event.
	/// </summary>
	/// <param name="sender">The source of the event. </param>
	/// <remarks>
	/// When you create a PageSizeChangedHandler delegate, you identify the method that will handle the event.
	/// To associate the event with your event handler, add an instance of the delegate to the event. The event
	/// handler is called whenever the event occurs, until you remove the delegate.
	/// Note: The declaration of your event handler must have the same parameters as the
	/// PageSizeChangedHandler delegate declaration.
	/// </remarks>
	public delegate void PageSizeChangedHandler(object sender);

	/// <summary>
	/// Represents the method that will handle the PageLayoutChanged event.
	/// </summary>
	/// <param name="sender">The source of the event. </param>
	/// <remarks>
	/// When you create a PageLayoutChangedHandler delegate, you identify the method that will handle the event.
	/// To associate the event with your event handler, add an instance of the delegate to the event. The event
	/// handler is called whenever the event occurs, until you remove the delegate.
	/// Note: The declaration of your event handler must have the same parameters as the
	/// PageLayoutChangedHandler delegate declaration.
	/// </remarks>
	public delegate void PageLayoutChangedHandler(object sender);

	#endregion

	/// <summary>
	/// Class representing the DaPrintDocument object.
	/// </summary>
	/// <remarks>The DaPrintDocument parses the XML template files and produces the report. The designer
	/// application also uses it to parse all the document objects and place them on the designer pane</remarks>
	public class DaPrintDocument : System.Drawing.Printing.PrintDocument
	{
		#region Declarations
		private System.ComponentModel.Container components = null;

		private License designLicense;

		/// <summary>
		/// Event declaration for the delegate MarginsChangedHandler
		/// </summary>
		public event MarginsChangedHandler OnMarginsChanged;
		
		/// <summary>
		/// Event declaration for the delegate PageSizeChangedHandler
		/// </summary>
		public event PageSizeChangedHandler OnPageSizeChanged;
		
		/// <summary>
		/// Event declaration for the delegate PageLayoutChangedHandler
		/// </summary>
		public event PageLayoutChangedHandler OnPageLayoutChanged;
		private bool designMode = false;
		private Paper.Type paperType = Paper.Type.A4;
		
		private string mDocRoot = "";
		private XmlDocument xmlDoc;
		private bool isXmlOK = true;
		private string theErrorMessage;
		private XmlNodeList xmlStaticElements ;
		private XmlNodeList xmlDynamicElements;

		private ArrayList declaredParameters;
		private Hashtable parameterValues;
		private Hashtable systemValues;
		private Hashtable theTables;
		private Hashtable rowsPrintedSoFar;
		private Hashtable theColumns;
		private Hashtable theCharts;

		private ICustomPaint[] staticObjects;
		private ICustomPaint[] dynamicObjects;

		/// <summary>
		/// Specifies the layout of the report.
		/// </summary>
		public enum LayoutType
		{
			/// <summary>Portrait Layout</summary>
			Portrait=0,
			/// <summary>Landscape Layout</summary>
			Landscape
		};
		#endregion

		#region Public Methods

		/// <summary>
		/// A serie is used to populate the ChartBox.
		/// </summary>
		/// <param name="chartName">The name of the chart to add the serie to</param>
		/// <param name="serieName">Name of the serie. Displayed in the Legend</param>
		/// <param name="Values">A array of Double values</param>
		/// <param name="serieColor">Color of the bar/pie</param>
		/// <remarks>
		/// This method is used to a serie to a specific chart in the current DaPrintDocument object.
		/// See <see cref="daReport.ChartBox.AddSerie">daReport.ChartBox.AddSerie</see> for an example
		/// of using the method</remarks>
		public void AddChartSerie(string chartName,string serieName,double[] Values,Color serieColor)
		{
			if ( !theCharts.Contains(chartName) ) return;

			int theIndex = (int)theCharts[chartName];
			ChartBox chartBox = staticObjects[theIndex] as ChartBox;
			chartBox.AddSerie(serieName,Values,serieColor);

		}


		/// <summary>
		/// Adds a DataTable to the collection
		/// </summary>
		/// <remarks>The DataTable.Name must match the dataSource property in the report definition for a
		/// dynamic table</remarks>
		/// <param name="newTable">System.Data.DataTable: source data for a dynamic table</param>
		public void AddData(DataTable newTable)
		{
			try
			{
				if (theTables.Contains(newTable.TableName))
				{
					theTables.Remove(newTable.TableName);
					rowsPrintedSoFar.Remove(newTable.TableName);
				}

				theTables.Add(newTable.TableName,newTable);
				rowsPrintedSoFar.Add(newTable.TableName,0);
			}
			catch (Exception){}
		}


		/// <summary>
		/// Function to load the Dynamic Document objects in the XML template file
		/// </summary>
		/// <param name="designMode">Specifies if docuemtn is being loaded into designer</param>
		public void InitDynamicObjects(bool designMode)
		{
			if ( xmlDynamicElements == null )
			{
				dynamicObjects = new ICustomPaint[0];
				return;
			}

			dynamicObjects = new ICustomPaint[xmlDynamicElements.Count];

			for (int i=0;i<xmlDynamicElements.Count;i++)
			{
				switch( xmlDynamicElements[i].Name )
				{
					case "textField":
						dynamicObjects[i] = resolveTextField(xmlDynamicElements[i],designMode);
						break;


					case "table":
						dynamicObjects[i] = resolveTable(xmlDynamicElements[i]);
						break;

					default:
						dynamicObjects[i] = null;
						break;
				}
			}
		}


		/// <summary>
		/// Function to load the Static Document objects in the XML template file
		/// </summary>
		/// <param name="designMode">Specifies if docuemtn is being loaded into designer</param>
		public void InitStaticObjects(bool designMode)
		{
			if ( xmlStaticElements == null )
			{
				staticObjects = new ICustomPaint[0];
				return;
			}

			staticObjects = new ICustomPaint[xmlStaticElements.Count];

			for (int i=0;i<xmlStaticElements.Count;i++)
			{
				switch( xmlStaticElements[i].Name )
				{
					case "textField":
						staticObjects[i] = resolveTextField(xmlStaticElements[i],designMode);
						break;

					case "pictureBox":
						staticObjects[i] = resolvePictureBox(xmlStaticElements[i]);
						break;

					case "chartBox":
						staticObjects[i] = resolveChartBox(xmlStaticElements[i],i);
						break;

					case "table":
						staticObjects[i] = resolveTable(xmlStaticElements[i]);
						break;

					default:
						staticObjects[i] = null;
						break;
				}
			}
		}


		/// <summary>
		/// Public function to reapply alignments to all static and dynamic document objects.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <remarks>This function is called when margins or page layout has been changed, and the alignment
		/// of the objects needs to be reset.</remarks>
		public void RepeatAlignments(object sender)
		{
			for (int i=0;i<this.staticObjects.Length;i++)
			{
				staticObjects[i].HorizontalAlignment = staticObjects[i].HorizontalAlignment;
				staticObjects[i].VerticalAlignment = staticObjects[i].VerticalAlignment;
			}

			for (int i=0;i<this.dynamicObjects.Length;i++)
			{
				dynamicObjects[i].HorizontalAlignment = dynamicObjects[i].HorizontalAlignment;
				dynamicObjects[i].VerticalAlignment = dynamicObjects[i].VerticalAlignment;
			}
		}


		/// <summary>
		/// Function to load a Hashtable of parameter values
		/// </summary>
		/// <param name="parameters">Hashtable of parameter values</param>
		/// <remarks>
		/// c#
		/// <code language="c#">
		/// // fill in with some parameters
		/// // (parameter names are case sensitive)
		/// Hashtable parameters = new Hashtable();
		/// parameters.Add("author","Predrag Dukanac");
		/// daPrintDocument.SetParameters(parameters);
		/// </code>
		/// vb.net
		/// <code language="Visual Basic">
		/// ' fill in with some parameters
		/// ' (parameter names are case sensitive)
		/// Dim parameters as Hashtable = new Hashtable()
		/// parameters.Add("author","Predrag Dukanac")
		/// daPrintDocument.SetParameters(parameters)
		/// </code>
		/// </remarks>
		public void SetParameters(Hashtable parameters)
		{
			parameterValues = parameters;
		}


		/// <summary>
		/// Function to load XML template from a file
		/// </summary>
		/// <param name="FileName">File location of XML template file</param>
		public void setXML(string FileName)
		{
			xmlDoc = new XmlDocument();
			xmlDoc.Load(FileName);
			DocRoot = (new FileInfo(FileName)).DirectoryName;
			setXML();
		}


		/// <summary>
		/// Function to load XML template from a file
		/// </summary>
		/// <param name="XmlDoc">XMLDoc variable.</param>
		/// <param name="XMLDocRoot">XMLDocument Root</param>
		/// <remarks>
		/// This function can be used to load an XML template file, make some changes to the 
		/// XML and then pass through. The XMLDocRoot is used as a starting point for
		/// PictureBox image locations
		/// </remarks>
		public void setXML(System.Xml.XmlDocument XmlDoc, string XMLDocRoot)
		{
			this.xmlDoc = xmlDoc;
			DocRoot = XMLDocRoot;
			setXML();
		}


		/// <summary>
		/// Sets the categories for the specified chart
		/// </summary>
		/// <param name="chartName"><see cref="daReport.ChartBox">daReport.ChartBox</see> to set categories for</param>
		/// <param name="theCategories">string array of categories</param>
		/// <remarks>
		/// This sets grouping for the data being passed into the ChartBox.
		/// c#
		/// <code language="c#">
		/// daPrintDocument.SetChartCategories("chart0",new string[] {"New York","Shangai","Mexico City"});
		/// </code>
		/// vb.net
		/// <code language="c#">
		/// daPrintDocument.SetChartCategories("chart0",new string() {"New York","Shangai","Mexico City"})
		/// </code>
		/// </remarks>
		public void SetChartCategories(string chartName,string[] theCategories)
		{
			if ( !theCharts.Contains(chartName) ) return;

			int theIndex = (int)theCharts[chartName];
			ChartBox chartBox = staticObjects[theIndex] as ChartBox;
			chartBox.XCategories = theCategories;	
		}


		#endregion

		#region Public Properties

		/// <summary>
		/// Gets/Sets the DocRoot for the current XML document
		/// </summary>
		/// <remarks>This is used to parse for locations of referenced images.</remarks>
		public string DocRoot
		{
			get { return this.mDocRoot; }
			set 
			{
				this.mDocRoot=value;
				if (staticObjects != null)
				{
					for (int i=0;i<staticObjects.Length;i++)
					{
						if (staticObjects[i] is PictureBox)
							((PictureBox)staticObjects[i]).SetDocumentRoot(mDocRoot);


					}
				}
			}
		}


		/// <summary>
		/// Gets/Sets the 
		/// <see cref="daReport.DaPrintDocument.LayoutType">daReport.DaPrintDocument.LayoutType</see>
		/// for the current DaPrintDocument
		/// </summary>
		public LayoutType Layout
		{
			get
			{
				if ( this.DefaultPageSettings.Landscape )
					return LayoutType.Landscape;
				else
					return LayoutType.Portrait;
			}

			set
			{
				if ( value == LayoutType.Landscape )
					this.DefaultPageSettings.Landscape = true;
				else
					this.DefaultPageSettings.Landscape = false;

				if(OnPageLayoutChanged!=null)
					OnPageLayoutChanged(this);
			}
		}


		/// <summary>
		/// Gets/Sets the margins for this page.
		/// </summary>
		/// <remarks>When handling the PrintDocument.PrintPage event, you can use this property along with the
		/// Bounds property to calculate the printing area for the page.</remarks>
		public Margins Margins
		{
			get {return this.DefaultPageSettings.Margins;}
			set 
			{
				this.DefaultPageSettings.Margins = value;
				if(OnMarginsChanged!=null)
					OnMarginsChanged(this);
			}
		}


		/// <summary>
		/// Gets/Sets the 
		/// <see cref="daReport.Paper.Type">daReport.Paper.Type</see>
		/// for the current DaPrintDocument
		/// </summary>
		public Paper.Type PaperType
		{
			get{ return paperType;}
			set
			{
				paperType = value;
				int[] size  = Paper.GetPaperSize(paperType);
				this.DefaultPageSettings.PaperSize = new PaperSize("",size[0],size[1]);
				if(OnPageSizeChanged!=null)
					OnPageSizeChanged(this);
			}

		}


		/// <summary>
		/// A string collection of parameters for the current DaPrintDocument object.
		/// </summary>
		/// <remarks>You will use the
		/// <see cref="daReport.DaPrintDocument.SetParameters">daReport.DaPrintDocument.SetParameters</see>
		/// function to set the values of parameters.</remarks>
		public string[] Parameters
		{
			get
			{
				string[] tmp = new string[declaredParameters.Count];
				for (int i=0;i<declaredParameters.Count;i++)
				{
					tmp[i] = declaredParameters[i].ToString();
				}
				return tmp;
			}

			set
			{
				declaredParameters = new ArrayList(value);
			}

		}	


		#endregion

		#region Private Event Handlers

		private void DaPrintDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			if (isXmlOK)
			{
				systemValues["pageNumber"] = (int)systemValues["pageNumber"]+1 ;
				Graphics g = e.Graphics;

				if ( (int)systemValues["totalPages"] == 0 )
					systemValues["totalPages"] = calculateNumberOfPages(g) ;

				bool morePages = updateDynamicContent(g);
				
				for (int i =0;i<staticObjects.Length;i++)
				{
					if (staticObjects[i] != null)
						staticObjects[i].Paint(g);
				}

				for (int i =0;i<dynamicObjects.Length;i++)
				{
					if (dynamicObjects[i] != null)
						dynamicObjects[i].Paint(g);
				}

				e.HasMorePages = morePages;
			}
			else
			{
				string theMessage = "Xml file structure is incorrect !\r\n\r\n";

				e.Graphics.DrawString(theMessage+theErrorMessage,new Font("Tahoma",10),new SolidBrush(Color.Black),e.MarginBounds);
				e.HasMorePages = false;
			}
		}


		private void DaPrintDocument_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			if (!designMode)
				PrepareStaticObjects();
			
			IDictionaryEnumerator en = theTables.GetEnumerator();
			while (en.MoveNext())
			{
				rowsPrintedSoFar[en.Key] = 0;
			}
			
			// reset system variables on begin print
			systemValues["pageNumber"] = 0 ;
			systemValues["totalPages"] = 0 ;
		}


		#endregion

		#region Private Functions

		private void getChartNames()
		{
			if (xmlStaticElements==null) return;

			for (int i=0;i<xmlStaticElements.Count;i++)
			{
				if (xmlStaticElements[i].Name == "chartBox")
				{
					if (xmlStaticElements[i].Attributes["name"] != null)
					{
						theCharts.Add(xmlStaticElements[i].Attributes["name"].Value,i);
					}
				}
			}
		}


		private void initSystemParameters()
		{
			systemValues = new Hashtable();
			systemValues.Add("pageNumber",0);
			systemValues.Add("totalPages",0);
			systemValues.Add("systemDate",DateTime.Now.ToString("dd/MM/yyyy"));
			systemValues.Add("systemTime",DateTime.Now.ToString("HH:mm:ss"));
		}


		private void PrepareStaticObjects()
		{
			for (int i=0;i<staticObjects.Length;i++)
			{
				if (staticObjects[i] is TextField)
				{
					TextField txtField = staticObjects[i] as TextField;
					txtField.Text = resolveParameterValues(txtField.Text);
				}
			}
		}		


		private void resolveContents(XmlNode theNode)
		{
			XmlNodeList temp = theNode.ChildNodes;
			for (int i=0;i<temp.Count;i++)
			{
				switch (temp[i].Name)
				{
					case "dynamicContent" :
						xmlDynamicElements = temp[i].ChildNodes;
						break;

					case "staticContent" :
						xmlStaticElements = temp[i].ChildNodes;
						break;
				}
			}
		}
		
		
		private void resolveLayout(XmlNode theNode)
		{
			
			if (theNode.Attributes["size"] != null)
			{
				string theSize = theNode.Attributes["size"].Value;
			}			
			
			if (theNode.Attributes["layout"] != null)
			{
				string theOrientation = theNode.Attributes["layout"].Value;

				if ( theOrientation == "Landscape" )
					this.DefaultPageSettings.Landscape = true;
				else
					this.DefaultPageSettings.Landscape = false;
			}
		}
		

		private void resolveMargins(XmlNode theNode)
		{
			try
			{
				this.Margins.Top = Convert.ToInt32 (theNode.Attributes["top"].Value);
				this.Margins.Left = Convert.ToInt32 (theNode.Attributes["left"].Value);
				this.Margins.Bottom = Convert.ToInt32 (theNode.Attributes["bottom"].Value);
				this.Margins.Right = Convert.ToInt32 (theNode.Attributes["right"].Value);
			}
			catch(Exception){}
		}

		
		private void resolvePaperSize(XmlNode theNode)
		{			
			if (theNode.Attributes["papersize"] != null)
			{
				this.PaperType = Paper.GetType(theNode.Attributes["papersize"].Value);
			}					
		}


		private void resolveParameterDeclaration(XmlNode theNode)
		{
			XmlNodeList childNodes = theNode.ChildNodes;

			for (int i=0;i<childNodes.Count;i++)
			{
				try
				{
					string parameterName = childNodes[i].Attributes["name"].Value;
					if (!declaredParameters.Contains(parameterName))
					{
						declaredParameters.Add(parameterName);
					}
				}
				catch(Exception){}
			}
		}


		private void setXML()
		{
			try
			{
				XmlNode root = xmlDoc.FirstChild;
				XmlNodeList temp = root.ChildNodes;
				
				resolvePaperSize(root);
				resolveLayout(root);

				for (int i=0;i<temp.Count;i++)
				{
					switch (temp[i].Name)
					{
						case "margins" :
							resolveMargins(temp[i]);
							break;

						case "parameters" :
							resolveParameterDeclaration(temp[i]);
							break;

						case "content" :
							resolveContents(temp[i]);
							break;
					}
				}

				InitStaticObjects(true);
				InitDynamicObjects(true);

				this.OnMarginsChanged += new MarginsChangedHandler(RepeatAlignments);
				this.OnPageLayoutChanged += new PageLayoutChangedHandler(RepeatAlignments);
			}
			catch(FileNotFoundException)
			{
				//MessageBox.Show("No such file "+filename);
				
			}
			catch(XmlException e)
			{
				isXmlOK = false;
				Exception tmp = e;
				theErrorMessage = "";
				while (tmp != null)
				{
					theErrorMessage += tmp.Message + "\r\n";
					tmp = tmp.InnerException;
				}
			}

		}

		
		#endregion

		#region Private Properties

		private int adjustHorizontalFill(string theFill)
		{
			int width = Math.Max(0,DefaultPageSettings.Bounds.Right - DefaultPageSettings.Bounds.Left - DefaultPageSettings.Margins.Left - DefaultPageSettings.Margins.Right);
			try
			{
				int pctFill = Convert.ToInt32(theFill);
				if (pctFill>100)
				{
					pctFill = 100;
				}
				else if (pctFill<0)
				{
					pctFill = 0;
				}
				else
				{
					width = pctFill*width / 100;
				}

			}
			catch(Exception)
			{}

			return width;
		}


		private int adjustVerticalFill(string theFill)
		{
			int height = Math.Max(0,DefaultPageSettings.Bounds.Bottom - DefaultPageSettings.Bounds.Top - DefaultPageSettings.Margins.Top - DefaultPageSettings.Margins.Bottom);
			try
			{
				int pctFill = Convert.ToInt32(theFill);
				if (pctFill>100)
				{
					pctFill = 100;
				}
				else if (pctFill<0)
				{
					pctFill = 0;
				}
				else
				{
					height = pctFill*height / 100;
				}

			}
			catch(Exception)
			{}

			return height;
		}


		private int calculateNumberOfPages(Graphics g)
		{
			int result = 1; 

			for (int i=0;i<dynamicObjects.Length;i++)
			{
				if ( dynamicObjects[i] != null && dynamicObjects[i] is StyledTable)
				{
					StyledTable tempTable = (StyledTable)dynamicObjects[i];
					if ( tempTable.DataSource != null && theTables.Contains(tempTable.DataSource) )
					{

						string theTableName = tempTable.DataSource;
						DataTable podaci = (DataTable)theTables[theTableName];

						bool hasMore = false;
						int counted = 0;
						int numPages = 0;

						do
						{
							int relativeHeaderHeight = tempTable.CalculateRelativeHeaderHeight(g);
							int rowsForPrint = 0;
							int relativeHeight = 0;
							int relativeDataRowHeight = 0;

							// find out how many succeeding rows will fit into table area
							do
							{
								if (podaci.Rows.Count <= counted + rowsForPrint )
									break;

								DataRow nextRow = podaci.Rows[counted + rowsForPrint];
								relativeDataRowHeight = tempTable.CalculateRelativeDataRowHeight(nextRow,g); 
								if (relativeHeaderHeight+relativeHeight+relativeDataRowHeight<=tempTable.GetPossibleRowNumber())
								{
									relativeHeight += relativeDataRowHeight;
									rowsForPrint++;
								}
							}
							while (relativeHeaderHeight+relativeHeight+relativeDataRowHeight<=tempTable.GetPossibleRowNumber());
																				
							counted += rowsForPrint;
							numPages++;
						
							hasMore = podaci.Rows.Count > counted;
							
						}
						while (hasMore);
						
						result = Math.Max(result,numPages);
					}
				}
			}
			return result;
		}


		private DataTable createSubtable(DataTable masterTable,StyledTable styledTable,int start,int length)
		{
			DataTable currentData = new DataTable();

			if ( styledTable.Columns.Length > 0 )
			{				
				for (int i=0;i<styledTable.Columns.Length;i++)
				{
					if ( masterTable.Columns.Contains(styledTable.Columns[i].Name) )
					{
						int ord = masterTable.Columns.IndexOf(styledTable.Columns[i].Name);
						currentData.Columns.Add(new DataColumn(masterTable.Columns[ord].ColumnName,masterTable.Columns[ord].DataType));
					}
					else
						throw new Exception("No such column "+ styledTable.Columns[i].Name.ToString());
				}


				for (int i=start;i<start+length;i++)
				{
					object[] newRow = new object[currentData.Columns.Count];
					for (int j=0;j<currentData.Columns.Count;j++)
					{
						int ord = masterTable.Columns.IndexOf(currentData.Columns[j].ColumnName);
						newRow[j] = masterTable.Rows[i].ItemArray[ord];
					}
					currentData.Rows.Add(newRow);
				}
				
			}
			else
			{

				for (int i=0;i<masterTable.Columns.Count;i++)
				{
					currentData.Columns.Add(new DataColumn(masterTable.Columns[i].ColumnName,masterTable.Columns[i].DataType));
				}

				for (int i=start;i<start+length;i++)
				{
					currentData.Rows.Add(masterTable.Rows[i].ItemArray);
				}
			}
			return currentData;
		}


		private StyledTableColumn[] createColumns(DataTable masterTable)
		{

			StyledTableColumn[] cols = new StyledTableColumn[masterTable.Columns.Count];
			for (int i=0;i<masterTable.Columns.Count;i++)
			{
				cols[i] = new StyledTableColumn();
				cols[i].Name = masterTable.Columns[i].ColumnName;
				cols[i].Label = masterTable.Columns[i].ColumnName;
			}
			return cols;
		}


		private ChartBox resolveChartBox(XmlNode theNode,int theIndex)
		{
			int x = Convert.ToInt32( theNode.Attributes["x"].Value );
			int y = Convert.ToInt32( theNode.Attributes["y"].Value );
			int width = Convert.ToInt32( theNode.Attributes["width"].Value );
			int height = Convert.ToInt32( theNode.Attributes["height"].Value );

			ChartBox chartBox = new ChartBox(x,y,width,height,this);
			chartBox.Name = theNode.Attributes["name"]==null ? "chart0" : theNode.Attributes["name"].Value ;
			
			if (theNode.Attributes["horAlignment"] != null)
				chartBox.HorizontalAlignment = resolveHorizontalAlignment(theNode.Attributes["horAlignment"].Value);

			if (theNode.Attributes["verAlignment"] != null)
				chartBox.VerticalAlignment = resolveVerticalAlignment(theNode.Attributes["verAlignment"].Value);

			if (theNode.Attributes["Selectable"] != null)
				chartBox.Selectable = Convert.ToBoolean(theNode.Attributes["Selectable"].Value);

			string theType = theNode.Attributes["type"]==null ? "Bars" : theNode.Attributes["type"].Value;
			switch (theType)
			{

				case "Pie":
					chartBox.Type = ChartBox.ChartType.Pie;
					break;

				default :
					chartBox.Type = ChartBox.ChartType.Bar;
					break;
			}

			XmlNodeList childNodes = theNode.ChildNodes;

			for (int i=0;i<childNodes.Count;i++)
			{
				switch (childNodes[i].Name)
				{
					
					case "title":
						try
						{							
							chartBox.Title = childNodes[i].InnerText ;
						}
						catch (Exception){}
						break;

					case "xLabel":
						try
						{							
							chartBox.XLabel = childNodes[i].InnerText ;
						}
						catch (Exception){}
						break;

					case "labelFont":
						try
						{							
							chartBox.LabelFont = resolveFont( childNodes[i] );
						}
						catch (Exception){}
						break;

					case "titleFont":
						try
						{							
							chartBox.TitleFont = resolveFont( childNodes[i] );
						}
						catch (Exception){}
						break;

					case "mapAreaColor":
						try
						{							
							chartBox.MapAreaColor = Color.FromName(childNodes[i].InnerText) ;
						}
						catch (Exception){}
						break;

					case "showLegend":
						try
						{							
							chartBox.ShowLegend = Convert.ToBoolean(childNodes[i].InnerText) ;
						}
						catch (Exception){}
						break;

					case "border":
						try
						{
							chartBox.BorderColor = Color.FromName( childNodes[i].Attributes["color"].Value );
							chartBox.BorderWidth = childNodes[i].Attributes["width"]==null ? 0 : Convert.ToInt32( childNodes[i].Attributes["width"].Value );
						}
						catch (Exception){}
						break;

				}

			}

			if (!theCharts.Contains(chartBox.Name))
				theCharts.Add(chartBox.Name,theIndex);
			
			return chartBox;	
		}


		private StyledTableColumn[] resolveColumns(XmlNode theNode)
		{
			XmlNodeList columnNodes = theNode.ChildNodes;

			StyledTableColumn[] result = new StyledTableColumn[columnNodes.Count];
		
			for (int i=0;i<columnNodes.Count;i++)
			{
				result[i] = new StyledTableColumn();
				
				try
				{
					result[i].Name =  columnNodes[i].Attributes["name"].Value ;
				}
				catch (Exception){}

				try
				{
					result[i].Label =  columnNodes[i].Attributes["label"].Value ;
				}
				catch (Exception){}

				try
				{
					result[i].FormatMask =  columnNodes[i].Attributes["FormatMask"].Value;
				}
				catch (Exception){}

				try
				{
					result[i].Visible =  Convert.ToBoolean(columnNodes[i].Attributes["Visible"].Value);
				}
				catch (Exception){}

				try
				{
					result[i].Width =  Convert.ToInt32(columnNodes[i].Attributes["width"].Value) ;
				}
				catch (Exception){}

				try
				{
					string align =  columnNodes[i].Attributes["align"].Value ;
					switch (align)
					{
						case "Center":
							result[i].Alignment = StyledTableColumn.AlignmentType.Center;
							break;
						case "Right":
							result[i].Alignment = StyledTableColumn.AlignmentType.Right;
							break;
						default:
							result[i].Alignment = StyledTableColumn.AlignmentType.Left;
							break;
					}
				}
				catch (Exception)
				{
					result[i].Alignment = StyledTableColumn.AlignmentType.Left;
				}

				
			}
			return result;
		}


		private Font resolveFont(XmlNode theNode)
		{
			try
			{
				Font theFont;
				string fntName = theNode.Attributes["family"]==null ? "Arial" : theNode.Attributes["family"].Value;
				int fntSize = theNode.Attributes["size"]==null ? 10 : Convert.ToInt32( theNode.Attributes["size"].Value );
				string fntStyle = theNode.Attributes["style"]==null ? "Regular" : theNode.Attributes["style"].Value;

				switch (fntStyle)
				{
					case "Bold Italic":
						theFont = new Font(fntName,fntSize,FontStyle.Bold | FontStyle.Italic);
						break;

					case "Bold":
						theFont = new Font(fntName,fntSize,FontStyle.Bold);
						break;

					case "Italic":
						theFont = new Font(fntName,fntSize,FontStyle.Italic);
						break;

					default :
						theFont = new Font(fntName,fntSize,FontStyle.Regular);
						break;
				}

				return theFont;
										
			}
			catch (Exception)
			{
				return new Font("Arial",8,FontStyle.Regular);
			}
		}


		private ICustomPaint.HorizontalAlignmentTypes resolveHorizontalAlignment(string theAlignment)
		{
			if (theAlignment == "Center")
				return ICustomPaint.HorizontalAlignmentTypes.Center;
			else if (theAlignment == "Left")
				return ICustomPaint.HorizontalAlignmentTypes.Left;
			else if (theAlignment == "Right")
				return ICustomPaint.HorizontalAlignmentTypes.Right;
			else
				return ICustomPaint.HorizontalAlignmentTypes.None;						
		}


		private string resolveParameterValues(string input)
		{
			string buffer = "";
			int pos = -1;
			int oldPos = 0;

			while( (pos=input.IndexOf("$P",oldPos)) != -1 )
			{

				buffer += input.Substring(oldPos,pos-oldPos);
				if ( input.Substring(pos+2,1).Equals("{") && input.IndexOf("}",pos+2) != -1 )
				{
					string parameterName = input.Substring(pos+3,input.IndexOf("}",pos+2)-pos-3).Trim();


					if ( systemValues.ContainsKey(parameterName))
					{
						buffer += systemValues[parameterName].ToString();
					}
					else if ( declaredParameters.Contains(parameterName) && parameterValues.ContainsKey(parameterName))
					{
						buffer += parameterValues[parameterName].ToString();
					}
					oldPos = input.IndexOf("}",pos+2) + 1;
				}
				else
				{				
					oldPos = pos+2;
				}
			}

			buffer += input.Substring(oldPos);

			return buffer;
		}


		private PictureBox resolvePictureBox(XmlNode theNode)
		{
			int x = Convert.ToInt32( theNode.Attributes["x"].Value );
			int y = Convert.ToInt32( theNode.Attributes["y"].Value );
			int width = Convert.ToInt32( theNode.Attributes["width"].Value );
			int height = Convert.ToInt32( theNode.Attributes["height"].Value );
			
			PictureBox pictureBox = new PictureBox(x,y,width,height,this);			

			if (theNode.Attributes["horAlignment"] != null)
				pictureBox.HorizontalAlignment = resolveHorizontalAlignment(theNode.Attributes["horAlignment"].Value);

			if (theNode.Attributes["verAlignment"] != null)
				pictureBox.VerticalAlignment = resolveVerticalAlignment(theNode.Attributes["verAlignment"].Value);

			if (theNode.Attributes["Selectable"] != null)
				pictureBox.Selectable = Convert.ToBoolean(theNode.Attributes["Selectable"].Value);

			bool doStretch = false;
			try
			{
				doStretch = theNode.Attributes["stretch"]==null ? true : Convert.ToBoolean(theNode.Attributes["stretch"].Value);
			}
			catch (Exception){}
			pictureBox.Stretch = doStretch;

			XmlNodeList childNodes = theNode.ChildNodes;

			for (int i=0;i<childNodes.Count;i++)
			{
				switch (childNodes[i].Name)
				{
					case "file":
						
						pictureBox.SetDocumentRoot(mDocRoot);
						pictureBox.ImageFile = childNodes[i].InnerText;
						break;

					case "border":
						try
						{
							pictureBox.BorderColor = Color.FromName( childNodes[i].Attributes["color"].Value );
							pictureBox.BorderWidth = childNodes[i].Attributes["width"]==null ? 0 : Convert.ToInt32( childNodes[i].Attributes["width"].Value );
						}
						catch (Exception){}
						break;

				}

			}
			return pictureBox;
		}


		private DataTable resolveStaticTableData(XmlNode theNode,StyledTableColumn[] staticColumns)
		{
			DataTable dataTable = new DataTable();

			XmlNodeList recordNodes = theNode.ChildNodes;

			int recordCount = recordNodes.Count;
			if (recordCount > 0)
			{
				//int columnCount = recordNodes[0].ChildNodes.Count;
				int columnCount = staticColumns.Length;
				for (int i=0;i<columnCount;i++)
					dataTable.Columns.Add(new DataColumn(staticColumns[i].Name));

				for (int i=0;i<recordNodes.Count;i++)
				{
					XmlNodeList fieldNodes = recordNodes[i].ChildNodes;
					string[] theRow = new string[columnCount];
					for (int j=0;j<columnCount;j++)
					{
						try
						{
							theRow[j] = resolveParameterValues(fieldNodes[j].InnerText);
						}
						catch (Exception)
						{
							theRow[j] = "";
						}
					}

					dataTable.Rows.Add(theRow);
				}
			}
			return dataTable;
		}


		private StyledTable resolveTable(XmlNode theNode)
		{
			int x = Convert.ToInt32( theNode.Attributes["x"].Value );
			int y = Convert.ToInt32( theNode.Attributes["y"].Value );
			int width = Convert.ToInt32( theNode.Attributes["width"].Value );
			int height = Convert.ToInt32( theNode.Attributes["height"].Value );

			StyledTable styledTable = new StyledTable(x,y,width,height,this);

			if (theNode.Attributes["horAlignment"] != null)
				styledTable.HorizontalAlignment = resolveHorizontalAlignment(theNode.Attributes["horAlignment"].Value);

			if (theNode.Attributes["verAlignment"] != null)
				styledTable.VerticalAlignment = resolveVerticalAlignment(theNode.Attributes["verAlignment"].Value);
			
			if (theNode.Attributes["borderColor"] != null)
				styledTable.BorderColor = Color.FromName(theNode.Attributes["borderColor"].Value);

			if (theNode.Attributes["Selectable"] != null)
				styledTable.Selectable = Convert.ToBoolean(theNode.Attributes["Selectable"].Value);

			if (theNode.Attributes["GroupByField"] != null)
				styledTable.GroupByField = theNode.Attributes["GroupByField"].Value;

			bool hasDataSource = false;
			if ( theNode.Attributes["dataSource"] != null )
			{
				styledTable.DataSource = theNode.Attributes["dataSource"].Value;
				hasDataSource = true;
			}
			
			try
			{
				styledTable.DrawHeader =  theNode.Attributes["showHeader"]==null ? true : Convert.ToBoolean(theNode.Attributes["showHeader"].Value);				
			}
			catch (Exception){}

			try
			{
				styledTable.DrawEmptyRows =  theNode.Attributes["drawEmptyRows"]==null ? false : Convert.ToBoolean(theNode.Attributes["drawEmptyRows"].Value);				
			}
			catch (Exception){}

			try
			{
				styledTable.CellHeight =  theNode.Attributes["cellHeight"]==null ? 18 : Convert.ToInt32(theNode.Attributes["cellHeight"].Value);				
			}
			catch (Exception){}
			

			string[] columnLabels = new string[0];
			XmlNodeList childNodes = theNode.ChildNodes;

			for (int i=0;i<childNodes.Count;i++)
			{
				switch (childNodes[i].Name)
				{
					case "columns":
						styledTable.Columns = this.resolveColumns(childNodes[i]);						
						break;

					case "header":
						try
						{
							styledTable.HeaderBackgroundColor =  Color.FromName( childNodes[i].Attributes["headerColor"].Value);
						}
						catch (Exception){}

						try
						{
							styledTable.HeaderFontColor =  Color.FromName( childNodes[i].Attributes["headerFontColor"].Value);
						}
						catch (Exception){}

						XmlNodeList headerNodes = childNodes[i].ChildNodes;
						for (int j=0;j<headerNodes.Count;j++)
						{
							switch (headerNodes[j].Name)
							{
								case "font":
									styledTable.HeaderFont = resolveFont(headerNodes[j]);
									break;
							}
						}						
						break;

					case "dataRows":
						try
						{
							styledTable.DataFontColor =  Color.FromName(childNodes[i].Attributes["dataFontColor"].Value);
						}
						catch (Exception){}

						XmlNodeList dataNodes = childNodes[i].ChildNodes;
						for (int j=0;j<dataNodes.Count;j++)
						{
							switch (dataNodes[j].Name)
							{
								case "font":
									styledTable.DataFont = resolveFont(dataNodes[j]);
									break;
							}
						}						
						break;

					case "font":
						styledTable.DataFont = resolveFont(childNodes[i]);
						break;


					case "data":
						if (!hasDataSource)
							styledTable.Data = resolveStaticTableData(childNodes[i],styledTable.Columns);
						break;
				}
			}

			if (styledTable.Columns.Length == 0)
			{
				if (hasDataSource && theTables.Contains(styledTable.DataSource) )
					styledTable.Columns = createColumns ((DataTable)theTables[styledTable.DataSource]);
				else
				{
					StyledTableColumn[] kolone = new StyledTableColumn[1];
					kolone[0] = new StyledTableColumn();
					kolone[0].Label = "Wrong dataSource name";
					styledTable.Columns = kolone;
				}
			}

			return styledTable;
		}


		private TextField resolveTextField(XmlNode theNode,bool designMode)
		{
			int x = Convert.ToInt32( theNode.Attributes["x"].Value );
			int y = Convert.ToInt32( theNode.Attributes["y"].Value );
			int width = Convert.ToInt32( theNode.Attributes["width"].Value );
			int height = Convert.ToInt32( theNode.Attributes["height"].Value );

			TextField textField = new TextField(x,y,width,height,this);

			if (theNode.Attributes["horAlignment"] != null)
				textField.HorizontalAlignment = resolveHorizontalAlignment(theNode.Attributes["horAlignment"].Value);

			if (theNode.Attributes["verAlignment"] != null)
				textField.VerticalAlignment = resolveVerticalAlignment(theNode.Attributes["verAlignment"].Value);

			if (theNode.Attributes["Selectable"] != null)
				textField.Selectable = Convert.ToBoolean(theNode.Attributes["Selectable"].Value);

			XmlNodeList childNodes = theNode.ChildNodes;

			for (int i=0;i<childNodes.Count;i++)
			{
				switch (childNodes[i].Name)
				{
					case "text":
						textField.Text = designMode? childNodes[i].InnerText : resolveParameterValues(childNodes[i].InnerText);

						string alignmentType = childNodes[i].Attributes["horAlignment"]==null ? "Left" : childNodes[i].Attributes["horAlignment"].Value;
					switch (alignmentType)
					{
						case "Center":
							textField.TextAlignment = TextField.TextAlignmentType.Center;
							break;

						case "Right":
							textField.TextAlignment = TextField.TextAlignmentType.Right;
							break;

						case "Justified":
							textField.TextAlignment = TextField.TextAlignmentType.Justified;
							break;

						default :
							textField.TextAlignment = TextField.TextAlignmentType.Left;
							break;
					}

						if ( childNodes[i].Attributes["verAlignment"]!= null )
						{
							switch (childNodes[i].Attributes["verAlignment"].Value)
							{
								case "Middle":
									textField.TextVerticalAlignment = TextField.TextVerticalAlignmentType.Middle;
									break;

								case "Bottom":
									textField.TextVerticalAlignment = TextField.TextVerticalAlignmentType.Bottom;
									break;
							}
						}
						break;

					case "font":
						textField.Font = resolveFont( childNodes[i] );
						break;

					case "foregroundColor":
						try
						{
							textField.ForegroundColor = Color.FromName( childNodes[i].Attributes["color"].Value );
						}
						catch (Exception){}
						break;

					case "backgroundColor":
						try
						{
							textField.BackgroundColor = Color.FromName( childNodes[i].Attributes["color"].Value );
						}
						catch (Exception){}
						break;

					case "border":
						try
						{
							textField.BorderColor = Color.FromName( childNodes[i].Attributes["color"].Value );
							textField.BorderWidth = childNodes[i].Attributes["width"]==null ? 0 : Convert.ToInt32( childNodes[i].Attributes["width"].Value );
						}
						catch (Exception){}
						break;

				}
			}

			return textField;
		}


		private ICustomPaint.VerticalAlignmentTypes resolveVerticalAlignment(string theAlignment)
		{			
			if (theAlignment == "Middle")
				return ICustomPaint.VerticalAlignmentTypes.Middle;
			else if (theAlignment == "Top")
				return ICustomPaint.VerticalAlignmentTypes.Top;
			else if (theAlignment == "Bottom")
				return ICustomPaint.VerticalAlignmentTypes.Bottom;
			else
				return ICustomPaint.VerticalAlignmentTypes.None;						
		}


		private DataTable SetGroupByOnDataTable(DataTable Table, string GroupByValue)
		{
			DataTable tempSourceTable = Table.Copy();
			DataTable tempTargetTable = Table.Copy();
			DataRow[] FilteredRows;
			Hashtable GroupBysDone = new Hashtable();

			tempTargetTable.Rows.Clear();

			foreach (DataRow CurrentTableRow in tempSourceTable.Rows)
			{
				if (! GroupBysDone.Contains(CurrentTableRow[GroupByValue].ToString()))
				{
					FilteredRows = tempSourceTable.Select(GroupByValue+"='"+CurrentTableRow[GroupByValue].ToString()+"'");
					FilteredRows= (DataRow[])FilteredRows.Clone();

					foreach (DataRow CurrentFilteredRow in FilteredRows)
					{
						tempTargetTable.ImportRow(CurrentFilteredRow);
					}

					GroupBysDone.Add(CurrentTableRow[GroupByValue].ToString(), "");
				}
			}

			return tempTargetTable;
		}


		private bool updateDynamicContent(Graphics g)
		{
			bool printMore = false;

			for (int i=0;i<dynamicObjects.Length;i++)
			{
				if ( dynamicObjects[i] != null )
				{
					if ( dynamicObjects[i] is TextField )
					{
						string theText = "";
						for (int j=0;j<xmlDynamicElements[i].ChildNodes.Count;j++)
						{
							if (xmlDynamicElements[i].ChildNodes[j].Name.Equals("text") )
								theText = xmlDynamicElements[i].ChildNodes[j].InnerText;
						}

						((TextField)dynamicObjects[i]).Text = resolveParameterValues(theText);
					}
					else if ( dynamicObjects[i] is StyledTable)
					{
						StyledTable tempTable = (StyledTable)dynamicObjects[i];
						if ( tempTable.DataSource != null && theTables.Contains(tempTable.DataSource) )
						{
							string theTableName = tempTable.DataSource;
							DataTable podaci = (DataTable)theTables[theTableName];

							if (tempTable.GroupByField != "")
								podaci = this.SetGroupByOnDataTable(podaci, tempTable.GroupByField);

							try
							{
								int relativeHeaderHeight = tempTable.CalculateRelativeHeaderHeight(g);
								int rowsForPrint = 0;
								int relativeHeight = 0;
								int relativeDataRowHeight = 0;

								// find out how many succeeding rows will fit into table area
								//taking into account grouping because that will inject another header row
								do
								{
									if (podaci.Rows.Count <= (int)rowsPrintedSoFar[theTableName] + rowsForPrint )
										break;

									DataRow nextRow = podaci.Rows[(int)rowsPrintedSoFar[theTableName] + rowsForPrint];

									relativeDataRowHeight = tempTable.CalculateRelativeDataRowHeight(nextRow,g); 
									if (relativeHeaderHeight+relativeHeight+relativeDataRowHeight<=tempTable.GetPossibleRowNumber())
									{
										relativeHeight += relativeDataRowHeight;
										rowsForPrint++;
									}
								}
								while (relativeHeaderHeight+relativeHeight+relativeDataRowHeight<=tempTable.GetPossibleRowNumber());
					
								// create subtable for printing
								tempTable.Data = createSubtable(podaci,tempTable,(int)rowsPrintedSoFar[theTableName],rowsForPrint);
								rowsPrintedSoFar[theTableName] = rowsForPrint + (int)rowsPrintedSoFar[theTableName];

								// if there are more rows, go on with printing
								if ( podaci.Rows.Count > (int)rowsPrintedSoFar[theTableName])
									printMore = true;
							}
							catch (Exception e)
							{
								// print exception text in table header
								printMore = false;
								StyledTableColumn[] kolone = new StyledTableColumn[1];
								kolone[0] = new StyledTableColumn();
								kolone[0].Label = e.Message;
								tempTable.Columns = kolone;
							}
						}
					}
				}
			}
			return printMore;
		}


		#endregion

		#region ICustomPaint Declarations

		/// <summary>
		/// Gets a collection of <see cref="daReport.ICustomPaint">daReport.ICustomPaint</see> objects
		/// which display dynamic data. This is only the <see cref="daReport.StyledTable">daReport.StyledTable</see>
		/// object at the moment.
		/// </summary>
		[Browsable(false)]
		public ICustomPaint[] DynamicObjects
		{
			get {return this.dynamicObjects;}
		}
		

		/// <summary>
		/// Gets a collection of <see cref="daReport.ICustomPaint">daReport.ICustomPaint</see> objects
		/// which only display static data such as TextField, PictureBox, etc
		/// </summary>
		[Browsable(false)]
		public ICustomPaint[] StaticObjects
		{
			get {return this.staticObjects;}
		}

		
		#endregion

		#region Creators and Destructor

		/// <summary>
		/// Initializes a new instance of the DaPrintDocument class.
		/// </summary>
		public DaPrintDocument()
		{
			//designLicense = LicenseManager.Validate(typeof(DaPrintDocument),this);

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			
			int[] size  = Paper.GetPaperSize(paperType);
			DefaultPageSettings.PaperSize = new PaperSize("",size[0],size[1]);
			DefaultPageSettings.Margins = new Margins(50,50,50,50);
			DefaultPageSettings.Landscape = false;
			declaredParameters = new ArrayList();
			initSystemParameters();
			parameterValues = new Hashtable();
			theTables = new Hashtable();
			theColumns = new Hashtable();
			theCharts = new Hashtable();
			rowsPrintedSoFar = new Hashtable();

		}	


		/// <summary>
		/// Initializes a new instance of the DaPrintDocument class.
		/// </summary>
		public DaPrintDocument(bool theMode):this() 
		{			
			designMode = theMode;
		}


		/// <summary>
		/// Initializes a new instance of the DaPrintDocument class.
		/// </summary>
		public DaPrintDocument(Hashtable parameters):this()
		{
			parameterValues = parameters;
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (staticObjects != null)
				{
					for (int i =0;i<staticObjects.Length;i++)
					{
						if ( staticObjects[i] is PictureBox )
							((PictureBox)staticObjects[i]).Dispose();
					}
				}

				if (designLicense != null)
				{
					designLicense.Dispose();
					designLicense = null;
				}

				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		
		/// <summary>
		/// Gives DaPrintDocument the opportunity to finalize any child resources
		/// </summary>
		~DaPrintDocument()
		{
			Dispose();
		}

		
		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// DaPrintDocument
			// 
			this.BeginPrint += new System.Drawing.Printing.PrintEventHandler(this.DaPrintDocument_BeginPrint);
			this.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.DaPrintDocument_PrintPage);

		}
		#endregion

		#endregion
	}
}