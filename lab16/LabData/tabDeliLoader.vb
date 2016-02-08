'
' Created by SharpDevelop.
' User: Angel Salamanca
' Date: 24/12/2015
' Time: 13:21
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System.IO
Imports System.Collections.Generic 

Public Class tabDeliLoader
	
	Private mySep As Char
	Private DS As TablasLab
	Private fileName As String
	Private varList As List(Of String)
	Private sr As StreamReader 
	Private opResult As loadResult 
	Private typeList As Char(,)
	Private strValues As Dictionary(Of String, List(Of String))
	Private intValues As Dictionary(Of String, List(Of Int32))
	Private contValues As Dictionary(Of String, List(Of Double))
	Private readOnly maxTextItems As Int32 = 100
	Private ReadOnly maxIntItems As Int32 = 100
    Private ReadOnly maxContItems As Int32 = 10
    Private strLength As Int32 = 30
    Private guessedRecordLength As Int32

    Public Enum loadResult
		Success
		DuplicateVar
	End Enum
	
	Public SUb New(byref newDS As TablasLab, sep As Char, byref fn As String)
		Me.mySep  =sep
		Me.DS = newDS
		Me.fileName = fn
		
	End Sub
	
	Public Sub Load
		Me.strValues = New Dictionary(Of String, List(Of String))
		Me.intValues = New Dictionary(Of String, List(Of Int32))
		Me.contValues = New Dictionary(Of String, List(Of Double))
		
		Me.sr =New StreamReader(Me.fileName)
		
		If readHeader Then
			If readLines Then
				populateDS
			End If
		End If
		
		Sr.close
		
		
	End Sub
	
	Private Function readHeader() As boolean
		Dim words() As String = Me.getWords()
		Me.varList = New List(Of String)
		
		For Each varName As String In words
			Dim properVarName As String =  MLBase.XmlTools.To2007Name(varName)
			If varList.Contains(properVarName) Then
				Me.opResult  = loadResult.DuplicateVar 
				return False 
			End If
			varList.Add(properVarName )
			
		Next
		Return True
	End Function
	
	Private Function readLines As Boolean

        Dim myInt As Int32, myDouble As Double
        Me.guessedRecordLength = 0

        Me.typeList = Array.CreateInstance(GetType(Char), 10, varList.Count)
        For nl As Int32 = 1 To 10 ' read 10 lines
            Dim words() As String = Me.getWords(True)
            ' go over words
            For nw As Int32 = 0 To Math.Min(words.Length, varList.Count) - 1
                Dim varName As String = Me.varList(nw)
                ' Is it integer?
                If Int32.TryParse(words(nw), NumberStyles.Any, Unica.Entorno.EngineCUlture, myInt) Then
                    typeList(nl - 1, nw) = "N"
                Else
                    If Double.TryParse(words(nw), NumberStyles.Any, Unica.Entorno.EngineCUlture, myDouble) Then
                        typeList(nl - 1, nw) = "C"
                    Else
                        typeList(nl - 1, nw) = "T"
                    End If
                End If
            Next nw
        Next nl
        Me.guessedRecordLength = Me.guessedRecordLength / 10


        ' Decide of var type
        For nv As Int32 = 0 To Me.varlist.Count-1
			Dim varType As String = "N" ' Default
			For nl As Int32 = 0 To Me.typeList.GetLength(0) - 1
				If typeList(nl, nv) = "T" Then
					varType = "T"
					Exit for
				End If
				If typeList(nl, nv) = "C" Then
					varType = "C"
				End If
			Next
			typeList(0, nv) = varType 
			
		Next
		
		Return true
	End Function
	
	Private Function populateDS As Boolean
		
		Dim initialPos As Int32 = 1
		
		For nv As Int32 = 0 To Me.varList.Count - 1
			Dim myLen As Int32
			Select Case Me.typeList(0, nv)
				Case "N"
					myLen = MLStudioLib.Constants.intDefaultLength 
				Case "C"
					myLen = MLStudioLib.Constants.contDefaultLength 
				Case Else
                    myLen = Me.strLength
            End Select
			Dim oldVar As Boolean = DS.NuevaVar(Me.varList(nv), Me.typeList(0, nv), initialPos,	 myLen, MLBase.MLBase.FiltroView.Entrada, False)
			initialPos += myLen
			
			If Not oldVar Then
				Select Case Me.typeList(0, nv)
				Case "N"
					Me.intValues.Add(Me.varList(nv), New List(Of Int32))
				Case "C"
					Me.contValues.Add(Me.varList(nv), New List(Of Double))
				Case "T"
					Me.strValues.Add(Me.varList(nv), New List(Of String))
				End Select
			End if	
			
		Next
		DS.LongitudRegistro = Math.Max(DS.LongitudRegistro, initialPos - 1)  ' In case we have -Old vars that go beyond
		
		DS.CreaSinGrupo()
        DS.CreaGruposBIM()

        DS.CreaSinCat()
        DS.CatSinGrupo()
		
		Return true
	End Function

    Private Function getWords(Optional ByVal addToLength As Boolean = False) As String()
        Dim L As String = Me.sr.ReadLine
        If addToLength Then Me.guessedRecordLength += L.Length

        Return L.Split(mySep)
    End Function

    Public Sub loadData()
		Dim HayCRLF As Boolean = True
        Dim Leido, Campo, L As String, LC() As Char
        Dim Leidos As Integer = 1, Nregs As Integer = 0
        Dim sb As New StringBuilder, varRow As datarow
        
        Dim Il As New InputLab(DS, DS.ArchivoOriginal, False, True, False)
        Il.AbrirEscritura()

        sr.Close
        Me.sr =New StreamReader(Me.fileName)
        sr.ReadLine ' skip header

        Unica.HojaPrincipal.ShowProgressBar(GM("FNP0017"), 1, sr.BaseStream.Length \ Me.guessedRecordLength)

        Do While sr.Peek > -1
            Unica.HojaPrincipal.PasoFinoAdelante()
            Dim words() As String = Me.getWords
            sb.Clear 
            
             Dim Col As Integer
             For I As Int32 = 0 To  varList.Count-1
             	
             	varRow = DS.BuscarVar(Me.varList(I))
             	Dim varName As String = varRow("Nombre")
             	Dim Longitud As Integer = varRow("Longitud")


                If I < words.Length Then
                    Leido = words(I)
                    If Leido.Length > Me.strLength Then Leido = Leido.Substring(0, Me.strLength)
                Else
                    	Leido = ""
					End If
                    Select Case CStr(varRow("Tipo"))
                        Case "T"
                            
                            Campo = Leido.PadRight(Longitud)
                            If Me.strValues.Keys.Contains(varName)
                            	Dim myList As List(Of String) = Me.strValues.Item(varName)
                            	If Not myList.Contains(Campo) AndAlso myList.Count < 1 + maxTextItems  Then
                            		myList.add(New String(Campo))
                            	End If
                            End if 
                        Case "N", "C"
                        	campo = Leido.PadLeft(Longitud)
                        	
                        	If varRow("Tipo") = "N" Then
                        		Dim intValue As Int32
                        		Int32.TryParse(Campo, NumberStyles.Any, Unica.Entorno.EngineCUlture, intValue)
                        		If Me.intValues.Keys.Contains(varName) then
                        			Dim myList As List(Of Int32) = Me.intValues.Item(varName)
                            		If Not myList.Contains(intValue) AndAlso myList.Count < 1 + maxContItems  Then
                            			myList.add(intValue)
                            		End If
                            	End if	
                        	Else
                        		Dim contValue As Double
                        		Double.TryParse(Campo, NumberStyles.Any, Unica.Entorno.EngineCUlture, contValue)
                        		If Me.contValues.Keys.Contains(varName) then
                        			Dim myList As List(Of Double) = Me.contValues.Item(varRow("Nombre"))
                            		If Not myList.Contains(contValue) AndAlso myList.Count < 1 + Me.maxContItems Then
                            			myList.add(contValue)
                            		End If
                            	End if	
                        	End If
                        Case "F"
                        	Throw New Exception 
                    End Select
                    Sb.Append(Campo)
             Next
            If sb.Length < DS.LongitudRegistro Then sb.Append(New String(" ", DS.LongitudRegistro - sb.Length))
            Il.Escribir(System.Text.Encoding.Default.GetBytes(sb.ToString))
        Loop

        sr.Close()

        Unica.RL.RegTrazas("loadData", "All records loaded")

        Unica.HojaPrincipal.HideProgressBar()

		Il.CerrarEscritura()

		Me.autoCatGeneration 

        Unica.HojaPrincipal.ProcessCompleted()
	End Sub
	
	Private Sub autoCatGeneration()
		
		Dim varRow As DataRow 
		
		' String first
		For Each kp As keyvaluepair(Of String , List(Of String)) In Me.strValues 
			varRow = DS.BuscarVar(kp.Key)
			If kp.Value.Count <= Me.maxTextItems Then
				kp.Value.Sort 
				For Each catName As String In kp.Value 
					Dim catRow As DataRow = DS.CreaCat(varRow, catName)
				Next
				
			End If
			
		Next
		
		' Int now 
		For Each kp As keyvaluepair(Of String , List(Of Int32)) In Me.intValues  
			varRow = DS.BuscarVar(kp.Key)
			If kp.Value.Count <= Me.maxIntItems  Then
				kp.Value.Sort 
				For Each catValue As Int32 In kp.Value 
					Dim catRow As DataRow = DS.CreaCat(varRow, catValue.ToString(Unica.Entorno.EngineCUlture))
					catRow("Desde") = catValue
					catRow("Hasta") = catValue
				Next
				
			End If
			
		Next
		
		' And Cont
		For Each kp As keyvaluepair(Of String , List(Of Double)) In Me.contValues  
			varRow = DS.BuscarVar(kp.Key)
			If kp.Value.Count <= Me.maxContItems Then
				kp.Value.Sort 
				For Each catvalue As Double In kp.Value 
					Dim catRow As DataRow = DS.CreaCat(varRow, catValue.ToString(Unica.Entorno.EngineCUlture))
					catRow("Desde") = catValue
					catRow("Hasta") = catValue
				Next
				
			End If
			
		Next
	End Sub
	
End Class