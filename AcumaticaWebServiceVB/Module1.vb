Imports AcumaticaWebServiceVB.TEST

Module Module1

    Sub Main()
        CreateCustomer()
    End Sub

    ' HELPER METHOD
    Private Function CreateValue(ByVal newVal As String, ByVal screenField As TEST.Field) As Value
        Return CreateValue(newVal, screenField, False)
    End Function

    ' HELPER METHOD
    Private Function CreateValue(ByVal newVal As String, ByVal screenField As TEST.Field, ByVal addCommit As Boolean) As Value
        Dim theValue As Value = New Value()
        theValue.LinkedCommand = screenField
        theValue.Value = newVal

        If addCommit Then
            theValue.Commit = True
        End If

        Return theValue
    End Function

    Private Sub CreateCustomer()
        Dim context As New TEST.Screen()

        Try
            context.CookieContainer = New System.Net.CookieContainer()
            context.EnableDecompression = True
            context.Timeout = 10000 'set timeout when to terminate connection
            context.Url = "http://localhost/AcumaticaERP/Soap/APITEST.asmx"
            Dim result As LoginResult = context.Login("admin", "123")

            Dim schema As AR303000Content = context.AR303000GetSchema()

            'Assign Values
            Dim customerID As Value = CreateValue("Test2", schema.CustomerSummary.CustomerID)
            Dim customerName As Value = CreateValue("Test Customer", schema.CustomerSummary.CustomerName)
            Dim email As Value = CreateValue("test@email.com", schema.GeneralInfoMainContact.Email)
            Dim addressLine1 As Value = CreateValue("Address 1", schema.GeneralInfoMainAddress.AddressLine1)
            Dim addressLine2 As Value = CreateValue("Address 2", schema.GeneralInfoMainAddress.AddressLine2)
            Dim city As Value = CreateValue("New York", schema.GeneralInfoMainAddress.City)

            'list all values
            Dim commands As Command() = {customerID, customerName, email, addressLine1, addressLine2, city, schema.Actions.Save, schema.CustomerSummary.CustomerID, schema.GeneralInfoFinancialSettings.CustomerClass}

            schema = context.AR303000Submit(commands)(0)

            Console.WriteLine("Created Customer: " + schema.CustomerSummary.CustomerID.Value.ToString())
            Console.WriteLine("Under Customer Class " + schema.GeneralInfoFinancialSettings.CustomerClass.Value.ToString())
            Console.Read()

        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Console.Read()
        Finally
            'Terminate Session
            context.Logout()
        End Try
    End Sub

End Module
