Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager

Imports System.IO.Compression
Imports System.IO
Imports System
Imports System.Text
Imports System.Xml
Imports System.Linq


Partial Class Sunat
    Inherits System.Web.UI.Page
    Dim venta As New ClsSunat
    Dim vardia As String = "01"
    Dim fecha As String = "2016-02-" & vardia & ""
    Dim fec As String = String.Empty
    'Dim fecha As String = DateTime.Now.ToString("yyyy-MM-dd")
    'Dim varfecha As String = DateTime.Now.ToString("yyyyMMdd")
    Dim varfecha As String = "201602" & vardia & ""
    Dim tienda As String = "0"
    'Dim idcabecera As String = "B07000001081"
    ' Dim idcabecera As String = "570010014212016"
    Dim idcabecera As String = "0"
    Dim archivo As Object
    Dim objx As Object
    Dim path As String

    Private Sub form1_Load(sender As Object, e As EventArgs) Handles form1.Load
        generartxtsunat()
    End Sub

    Public Sub generartxtsunat()
        Dim DirectoryDay As String = ""
        Dim ServPathDirectory As String = ""

        Dim pathXml As String = String.Empty
        Dim pathZip As String = String.Empty
        Dim filezip As String = String.Empty

        Dim vartotal As Double = 0
        Dim varsub As String = 0
        Dim varigv As String = 0
        Dim numresumen As String = "52"
        Dim contfila As String = "0"
        '  Dim varfec As String = DateTime.Now.ToString("yyyyMMdd")
        ' Dim varfec As String = "20160201"
        ServPathDirectory = "D:\sistema\" + varfecha
        Dim vartienda As String
        Dim dtTienda As DataTable

        Dim directory As New DirectoryInfo(ServPathDirectory)
        Dim name As String = "RC-" & numresumen & ".txt"
        objx = CreateObject("Scripting.FileSystemObject")
        Dim ruta As String = "D:\sistema\" & varfecha & "\"
        path = ruta & name
        archivo = objx.createtextfile(path, True)
        If (directory.Exists) Then
            Dim directories As DirectoryInfo() = directory.GetDirectories()
            '   Invoice.generaresumencabecera(varfecha, numresumen)
            For i As Integer = 0 To directories.Length - 1
                ' Dim directorye As Boolean = Convert.ToBoolean(directories(i).Name(0))

                vartienda = Right((directories(i).Name()), 2)
                '  Dim vartienda As String = vartda.Substring(2, 2)
                dtTienda = venta.serielist(fecha, vartienda)


                If (directories(i).Name(0).Equals("T"c)) Then
                    '  If (directorye.Equals("T02")) Then
                    pathXml = ServPathDirectory + "\" + directories(i).Name + "\xml"
                    pathZip = ServPathDirectory + "\" + directories(i).Name + "\zip"
                    Console.WriteLine(pathXml)
                    Console.WriteLine(pathZip)

                    Dim directorieXml As New DirectoryInfo(pathXml)
                    Dim files As FileInfo() = directorieXml.GetFiles("*.XML")
                    ' For  donde identifica n series por tienda
                    For x As Integer = 0 To dtTienda.Rows.Count - 1
                        ' Fue recorre todos los xmls por local
                        For j As Integer = 0 To files.Length - 1

                            Dim serieT As String = files(j).Name
                            Dim serieTd As String = serieT.Substring(15, 4)
                            If serieTd = dtTienda.Rows(x)(0) Then



                                Console.WriteLine(files(j).FullName)
                                Console.WriteLine(files(j).Name)

                                filezip = files(j).Name
                                Dim local_xmlarchivo As String = filezip
                                Dim value As String = String.Empty

                                Dim xmlDoc As New XmlDocument

                                pathZip = pathXml + "\" + filezip
                                xmlDoc.Load(pathZip)
                                Dim xnodes As XmlNodeList = xmlDoc.GetElementsByTagName("cac:LegalMonetaryTotal")
                                Console.WriteLine(pathZip)
                                If (xnodes(0).InnerText <> String.Empty) Then
                                    value = xnodes(0).InnerText
                                    vartotal += Double.Parse(value)

                                End If
                            End If
                            varsub = (vartotal / 1.18).ToString("###0.00")
                            varigv = ((vartotal / 1.18) * 0.18).ToString("###0.00")
                            Console.WriteLine(vartotal)
                            Console.WriteLine(varsub)
                            Console.WriteLine(varigv)
                        Next



                        Dim varmin As String = venta.numeromin(fecha, dtTienda.Rows(x)(0))
                        Dim varmax As String = venta.numeromax(fecha, dtTienda.Rows(x)(0))
                        contfila += dtTienda.Rows.Count
                        ''Generacion del txt


                        Dim tot As String = vartotal.ToString("###0.00")
                        Dim serie As String = dtTienda.Rows(x)(0)
                        '  If Not dtGuiaDetalle Is Nothing Then


                        archivo.writeLine(contfila.ToString() + "|" + serie.ToString() + "|" + varmin.ToString() + "|" + varmax.ToString() + "|" + tot.ToString() + "|")

                        ' Response.Write(tab)
                        '   Next


                        '  End If
                        '    Response.End()
                        '  doc.Save(Path)


                        vartotal = "0"


                        '   Invoice.generaresumendetalle(contfila, "03", dtTienda.Rows(x)(0), varmin, varmax, vartotal.ToString("###0.00"), varsub, varigv)
                    Next

                End If


            Next
            archivo.Close()
            ' Invoice.generaresumenpie()
        End If
        '  Invoice.FirmaDigital()
        '   Invoice.ZipXmlResumen()

    End Sub

End Class
