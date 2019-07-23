using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
using System.Linq;
using System.Text;
//using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Runtime.InteropServices;
namespace Firmado
{
   //<class
   
    public class firma
    {
        public string pathxml {get;set;}
        public string pathcert { get; set; }
        public bool IsSigned;
       // public string local_xmlArchivo { get; set; }
    public firma()
        {
            this.pathcert = "D:\\mIDLL";// Directory.GetCurrentDirectory(); //+   @"\Certificado\";
            this.IsSigned=false;
        }

        public void firmaDigital(string filexml)
        { 
            try
            {
                  String local_xmlArchivo = pathxml + filexml;
            //    String local_xmlArchivo = @"D:\sistema\20151228\xml\20427799973-03-BB11-00095393.xml";
                String local_typoDocumento = string.Empty;
                String local_nombreXML = System.IO.Path.GetFileName(local_xmlArchivo);
                if (local_nombreXML.Substring(12, 2) != "RC")
                {
                    local_typoDocumento = local_nombreXML.Substring(12, 2);
                }
                else
                {
                    local_typoDocumento = "RC";
                }
                this.pathcert = pathcert + @"\Certificado\" + "WGN4NGp3SGVySVQwRnFIQQ==.p12";
                X509Certificate2 MiCertificado = new X509Certificate2(this.pathcert, "PgKk9fukA5bKgxfG");                
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.Load(local_xmlArchivo);
                //RSACryptoServiceProvider y X509Certificate 
                SignedXml signedXml = new SignedXml(xmlDoc);

                signedXml.SigningKey = MiCertificado.PrivateKey;

                KeyInfo KeyInfo = new KeyInfo();

                Reference Reference = new Reference();
                Reference.Uri = "";

                Reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());


                signedXml.AddReference(Reference);

                X509Chain X509Chain = new X509Chain();
                X509Chain.Build(MiCertificado);

                X509ChainElement local_element = X509Chain.ChainElements[0];
                KeyInfoX509Data x509Data = new KeyInfoX509Data(local_element.Certificate);
                String subjectName = local_element.Certificate.Subject;

                x509Data.AddSubjectName(subjectName);
                KeyInfo.AddClause(x509Data);

                signedXml.KeyInfo = KeyInfo;
                signedXml.ComputeSignature();

                XmlElement signature = signedXml.GetXml();
                signature.Prefix = "ds";
                signedXml.ComputeSignature();

                foreach (XmlNode node in signature.SelectNodes("descendant-or-self::*[namespace-uri()='http://www.w3.org/2000/09/xmldsig#']"))
                {
                    if (node.LocalName == "Signature")
                    {
                        XmlAttribute newAttribute = xmlDoc.CreateAttribute("Id");
                        newAttribute.Value = "SignatureSP";
                        node.Attributes.Append(newAttribute);
                        break;
                    }
                }
                String local_xpath = string.Empty;
                XmlNamespaceManager nsMgr;
                nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsMgr.AddNamespace("sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1");
                nsMgr.AddNamespace("ccts", "urn:un:unece:uncefact:documentation:2");
                nsMgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");


                switch (local_typoDocumento)
                {
                    case "01":
                    case "03":
                        //factura / boleta
                        nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");
                        local_xpath = "/tns:Invoice/ext:UBLExtensions/ext:UBLExtension[2]/ext:ExtensionContent";
                        break;
                    case "07":
                        //credit note
                        nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2");
                        local_xpath = "/tns:CreditNote/ext:UBLExtensions/ext:UBLExtension[2]/ext:ExtensionContent";
                        break;
                    case "08":
                        //debit note
                        nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:DebitNote-2");
                        local_xpath = "/tns:DebitNote/ext:UBLExtensions/ext:UBLExtension[2]/ext:ExtensionContent";
                        break;
                    case "RA": //Communicacion de baja 
                        nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:VoidedDocuments-1");
                        local_xpath = "/tns:VoidedDocuments/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent";
                        break;
                    case "RC":  //Resument de diario                                                 
                        nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:SummaryDocuments-1");
                        local_xpath = "/tns:SummaryDocuments/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent";
                        break;
                }
                nsMgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                nsMgr.AddNamespace("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2");
                nsMgr.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
                nsMgr.AddNamespace("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2");
                nsMgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                nsMgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");

                xmlDoc.SelectSingleNode(local_xpath, nsMgr).AppendChild(xmlDoc.ImportNode(signature, true));
                xmlDoc.Save(local_xmlArchivo);
                XmlNodeList nodeList = xmlDoc.GetElementsByTagName("ds:Signature");
                //el nodo <ds:Signature> debe existir unicamente 1 vez
                if (nodeList.Count != 1)
                    throw new Exception("Se produjo un error en la firma del documento");

                signedXml.LoadXml((XmlElement)nodeList[0]);

                //verificacion de la firma generada
                if (signedXml.CheckSignature() == false)
                {
                    this.IsSigned=false;
                }
                else
                    this.IsSigned=true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string Get_HashCode(string filexml)
        {
            String local_xmlArchivo = filexml;
            string value = string.Empty;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(local_xmlArchivo);
            XmlNodeList xnodes = xmlDoc.GetElementsByTagName("DigestValue");
            if (xnodes[0].InnerText != string.Empty)
                value = xnodes[0].InnerText;
            return value;            
        }

        public bool ConvertZipToFile(string path)
        {
            String local_xmlArchivo =  path;
            //D:\William\SunatPrueba\cdr'
            //   string startPath = @"D:\William\SunatPrueba\sign\";
            string startPath = @"D:\sistema\20151228\xml";
            // string startPath = @"C:\Users\cvera\Desktop\certificado\file";
            //20432348114-01-FF11-00095452

            string zipPath = local_xmlArchivo; //+ ".zip";
            zipPath = zipPath.Replace(".xml", ".zip");
            
          //  string extractPath = @"C:\Users\cvera\Desktop\certificado\extract";

            ZipFile.CreateFromDirectory(startPath, zipPath);
            return true;
        }

    }
}
