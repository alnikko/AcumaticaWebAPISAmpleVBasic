Imports AcumaticaWebServiceVB.TEST

Module Module1

    Sub Main()
        'CreateCustomer()
        CreateSaleOrder()
    End Sub

    ' HELPER METHOD
    ''' <summary>
    ''' Acts as a insert new Value like in C#
    ''' </summary>
    ''' <param name="newVal"></param>
    ''' <param name="screenField"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
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
        Dim success As Boolean = False
        Try
            context.CookieContainer = New System.Net.CookieContainer()
            context.EnableDecompression = True
            context.Timeout = 10000 'set timeout when to terminate connection
            context.Url = "http://localhost/AcumaticaQAT/Soap/APITEST.asmx"
            Dim result As LoginResult = context.Login("admin", "123")
            success = True
            Dim schema As AR303000Content = context.AR303000GetSchema()

            'Assign Values
            Dim customerID As Value = CreateValue("Test2", schema.CustomerSummary.CustomerID)
            Dim customerName As Value = CreateValue("Test Customer", schema.CustomerSummary.CustomerName)
            Dim email As Value = CreateValue("test@email.com", schema.GeneralInfoMainContact.Email)
            Dim addressLine1 As Value = CreateValue("Address 1", schema.GeneralInfoMainAddress.AddressLine1)
            Dim addressLine2 As Value = CreateValue("Address 2", schema.GeneralInfoMainAddress.AddressLine2)
            Dim city As Value = CreateValue("New York", schema.GeneralInfoMainAddress.City)

            'list all values
            Dim commands As Command() = {
                customerID,
                customerName,
                email,
                addressLine1,
                addressLine2,
                city,
                schema.Actions.Save,
                schema.CustomerSummary.CustomerID,
                schema.GeneralInfoFinancialSettings.CustomerClass ' return value
            }

            schema = context.AR303000Submit(commands)(0)

            Console.WriteLine("Created Customer: " + schema.CustomerSummary.CustomerID.Value.ToString())
            Console.WriteLine("Under Customer Class " + schema.GeneralInfoFinancialSettings.CustomerClass.Value.ToString())
            Console.Read()

        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Console.Read()
        Finally
            If success Then
                'Terminate Session
                context.Logout()
            End If
        End Try
    End Sub


    Private Sub CreateSaleOrder()
        Dim context As New TEST.Screen()
        Dim success As Boolean = False

        Try
            context.CookieContainer = New System.Net.CookieContainer()
            context.EnableDecompression = True
            context.Url = "http://localhost/AcumaticaQAT/Soap/APITEST.asmx"
            Dim result As LoginResult = context.Login("admin", "123")

            success = True

            Dim schema As SO301000Content = context.SO301000GetSchema()
            Dim commands As New List(Of TEST.Command)()

            'Assign Values
            commands.Add(CreateValue("IN", schema.OrderSummary.OrderType))
            commands.Add(CreateValue("<NEW>", schema.OrderSummary.OrderNbr))
            commands.Add(CreateValue("TEST2", schema.OrderSummary.Customer))
            commands.Add(CreateValue("Test Sales Order", schema.OrderSummary.Description))
            commands.Add(CreateValue("OPTIONAL", schema.OrderSummary.CustomerOrder))

            'add new item on document details tab of sales order screen

            'first item
            commands.Add(schema.DocumentDetails.ServiceCommands.NewRow)
            commands.Add(CreateValue("AALEGO500", schema.DocumentDetails.InventoryID))
            commands.Add(CreateValue("4", schema.DocumentDetails.Quantity))
            commands.Add(CreateValue("EA", schema.DocumentDetails.UOM))

            'second item
            commands.Add(schema.DocumentDetails.ServiceCommands.NewRow)
            commands.Add(CreateValue("CONGRILL", schema.DocumentDetails.InventoryID))
            commands.Add(CreateValue("2", schema.DocumentDetails.Quantity))
            commands.Add(CreateValue("EA", schema.DocumentDetails.UOM))

            'Save Action
            commands.Add(schema.Actions.Save)

            'Fetch Data that is generated
            commands.Add(schema.OrderSummary.OrderType)
            commands.Add(schema.OrderSummary.OrderNbr)
            commands.Add(schema.OrderSummary.OrderedQty)
            commands.Add(schema.OrderSummary.OrderTotal)


            Dim schemaResult As SO301000Content() = context.SO301000Submit(commands.ToArray())


            Console.WriteLine("Order Type: " + schemaResult(0).OrderSummary.OrderType.Value.ToString())
            Console.WriteLine("Order Nbr: " + schemaResult(0).OrderSummary.OrderNbr.Value.ToString())
            Console.WriteLine("Ordered Qty: " + schemaResult(0).OrderSummary.OrderedQty.Value.ToString())
            Console.WriteLine("Order Total: " + schemaResult(0).OrderSummary.OrderTotal.Value.ToString())
            Console.Read()

        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Console.Read()
        Finally
            If success Then
                context.Logout()
            End If
        End Try
    End Sub

End Module
