using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace WebServiceProject
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {
        [WebMethod]
        public string AnalyzeXML(XmlDocument aNode)
        {
            double shippedQty = 0;

            try
            {
                // validate xml
                var path = new Uri(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
                XmlSchemaSet schema = new XmlSchemaSet();
                schema.Add("", path + "\\ESSVMI_Sample-TEST.xsd");

                XDocument doc = ToXDocument(aNode);
                doc.Validate(schema, ValidationEventHandler);

                // summary ShippedQty              
                var entries = doc.Descendants().Where(x => x.Name.LocalName == "Detail").ToList();
                foreach (XElement v in entries)
                {
                    var asd = v.Descendants().Where(x => x.Name.LocalName == "ShippedQty").SingleOrDefault().Value;
                    if (asd != "" || asd != null)
                    {
                        //var value = Convert.ToInt32(asd.ToString());
                        shippedQty += Convert.ToDouble(asd.ToString());                           
                    }                        
                }
            }
            catch (Exception ex)
            {
                return "-1";
            }

            return shippedQty.ToString();
        }

        private void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            XmlSeverityType type = XmlSeverityType.Warning;
            if (Enum.TryParse<XmlSeverityType>("Error", out type))
            {
                if (type == XmlSeverityType.Error) throw new Exception(e.Message);
            }
        }

        private XDocument ToXDocument(XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }


    }
}
