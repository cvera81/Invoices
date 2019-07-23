Imports System.IO
Imports System.Net
Imports System.Xml
Imports System.Text
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationSettings
Imports System.Data.OleDb


Public Class Clas_FE
    Dim wService As WS_DFE.billServiceClient
    Dim ds As New DataSet
    Dim cnnSql As New SqlConnection()
    Dim cmdSQL As New SqlCommand
    Dim adaSQL As New SqlDataAdapter
    Dim wResultado As String = ""
    Dim xResultado As New DataTable
    Dim cnnSqlProd As New SqlConnection()

    Sub New()
        wService = New WS_DFE.billServiceClient
        ServicePointManager.UseNagleAlgorithm = True
        ServicePointManager.Expect100Continue = False
        ServicePointManager.CheckCertificateRevocationList = True
        wService.ClientCredentials.CreateSecurityTokenManager()
        wService.ClientCredentials.UserName.UserName = "20427799973CVERA"
        wService.ClientCredentials.UserName.Password = "factu123"
        
    End Sub
    'Public Function getConexion() As SqlConnection
    '    Dim cnn As New SqlConnection(cadena)
    '    Return cnn
    'End Function
    Dim cadena As String = "Data Source=192.168.3.9; Initial Catalog=bd_promotor; User ID=admin; Password=abcABC123*"


    Public Function insertarenviosunat(ByVal transaccion As String, ByVal fechaublxml As String, ByVal ublxml As String, ByVal nombreempaquetado As String,
                                     ByVal empaquetado As String, ByVal fechacreacionemp As String, ByVal fechaenvio As String,
                                      ByVal fecharespuesta As String, ByVal codigorespuesta As String, ByVal hash As String)

        Dim sql As String = ""

        cnnSql.ConnectionString = cadena
        If cnnSql.State = System.Data.ConnectionState.Closed Then
            cnnSql.Open()
        End If
        cmdSQL = cnnSql.CreateCommand

        sql += " insert into CONTROLENVIOSUNAT"
        sql += " ("
        sql += " TRANSACCION,"
        sql += " FECHACREACIONUBLXML,"
        sql += " UBLXML,"
        sql += " NOMBREEMPAQUETADO,"
        sql += " EMPAQUETADO,"
        sql += " FECHACREACIONEMPAQUETADO,"
        sql += " FECHAENVIO,"
        sql += " FECHARESPUESTA,"
        sql += "CODIGORESPUESTA,"
        sql += " HASH)"
        sql += "  VALUES ( "
        sql += "'" & transaccion & "',"
        sql += "'" & fechaublxml & "',"
        sql += "'" & ublxml & "',"
        sql += "'" & nombreempaquetado & "',"
        sql += "" & empaquetado & ","
        sql += "'" & fechacreacionemp & "',"
        sql += "'" & fechaenvio & "',"
        sql += "'" & fecharespuesta & "',"
        sql += "'" & codigorespuesta & "',"
        sql += "'" & hash & "')"
        cmdSQL.CommandText = sql
        cmdSQL.CommandType = CommandType.Text
        adaSQL.SelectCommand = cmdSQL
        adaSQL.SelectCommand = cmdSQL
        '  DataTable dt = ds.Tables[0]

        Dim row As Integer = cmdSQL.ExecuteNonQuery
        Return row

    End Function





    Public Function EnviarDocumento(ByVal pArchivo As String) As String


        'Dim Ruta As String = "C:\Users\usuariosis\Desktop\firmaDigital\xml\"
        ' Dim Ruta As String = "D:\William\SunatPrueba\demos\zip\"
        Dim Ruta As String = "D:\sistema\20160117\resumen\zip\"
        Dim RutaCdr As String = "D:\sistema\20160117\resumen\cdr\"
        'System.IO.Directory.GetCurrentDirectory()
        Dim strRetorno As String = ""
        Dim Filezip As String = pArchivo
        Dim FilePath As String = Ruta & Filezip
        Dim bytearray As Byte() = File.ReadAllBytes(FilePath)
      
        
        
        Try
            wService.Open()
            Dim returnbyte As Byte() = wService.sendBill(Filezip, bytearray)
            wService.Close()
            Dim fs As New FileStream(RutaCdr & "R-" & Filezip, FileMode.Create)
            'Dim fs As New FileStream(RutaCdr & Filezip & ".zip", FileMode.Create)
            fs.Write(returnbyte, 0, returnbyte.Length)
            fs.Close()
            strRetorno = "Archivo Generado con Exito"
        Catch ex As System.ServiceModel.FaultException
            strRetorno = ex.Code.Name
        End Try
        Return strRetorno
    End Function



    Public Function EnviarResumenBaja(ByVal pArchivo As String) As String

        ' Dim Ruta As String = "C:\Users\WILLIAM\Desktop\firmaDigital\xml\"
        '  Dim Ruta As String = "D:\William\SunatPrueba\cdr\"
        Dim Ruta As String = "D:\sistema\20160102\resumen\zip\"
        Dim RutaCdr As String = "C:\Users\WILLIAM\Desktop\firmaDigital\cdr\"
        'System.IO.Directory.GetCurrentDirectory()
        Dim strRetorno As String = ""
        Dim Filezip As String = pArchivo
        Dim FilePath As String = Ruta & Filezip
        Dim bytearray As Byte() = File.ReadAllBytes(FilePath)
     

        Try
            wService.Open()
            Dim ticket As String = wService.sendSummary(Filezip, bytearray)
            wService.Close()
            strRetorno = ticket

        Catch ex As System.ServiceModel.FaultException
            strRetorno = ex.Code.Name
        End Try

        Return strRetorno


    End Function

    Public Function ObtenerEstado(ByVal pticket As String) As String
        ' Dim Ruta As String = System.IO.Directory.GetCurrentDirectory()
        Dim strRetorno As String = ""
      

        Try
            wService.Open()
            Dim returnstring As WS_DFE.statusResponse = wService.getStatus(pticket)
            Dim retorno As String = returnstring.statusCode
            wService.Close()

            'agregado wc
            strRetorno = retorno
        Catch ex As System.ServiceModel.FaultException
            strRetorno = ex.Code.Name
        End Try
        Return strRetorno
    End Function




End Class
