Imports System.Numerics
Public Class Form1

    ' Global variables, not really preferable but this is only a proof of concept project.
    Dim birdsLiving As Integer = 0
    Dim birds As List(Of bird) = New List(Of bird)
    Dim pipes As List(Of PipePair) = New List(Of PipePair)

    Sub initializeGame(b)
        pointsLabel.Text = 0
        birds = b

        pipes = New List(Of PipePair)
        For i = 0 To 1
            Dim newPipe As PipePair = New PipePair With {
            .MidPoint = 105 + Rnd() * (graphicBox.Height - 210),
            .X = graphicBox.Width + i + (graphicBox.Width / 2) * i,
            .Index = i
        }
            pipes.Add(newPipe)
        Next
    End Sub

    Sub logicFrame()
        For Each item In birds
            item.move(graphicBox, pipes, birdsLiving)
            item.think(pipes, graphicBox)
        Next

        For i = 0 To pipes.Count() - 1
            Dim item = pipes(i)
            If Not IsNothing(item) Then


                item.move(pipes)
                Dim Halfsies As Integer = 50
                If item.X + 10 > Halfsies And item.X + 10 < Halfsies + 2 Then
                    pointsLabel.Text = Convert.ToString(Convert.ToInt16(pointsLabel.Text) + 1)
                End If
            Else

                Dim newPipe As PipePair = New PipePair With {
                .MidPoint = 105 + Rnd() * (graphicBox.Height - 210),
                .Gap = 55,
                .X = graphicBox.Width,
                .Index = i}
                pipes(i) = newPipe
            End If
        Next
    End Sub

    Sub drawFrame()
        Dim frame As New Bitmap(graphicBox.Width, graphicBox.Height)
        Using g As Graphics = Graphics.FromImage(frame)
            For Each item In birds


                item.draw(g)
            Next

            For i = 0 To pipes.Count() - 1
                Dim item = pipes(i)
                If Not IsNothing(item) Then

                    item.draw(g, graphicBox)
                End If

            Next
        End Using

        If graphicBox.Image IsNot Nothing Then graphicBox.Image.Dispose()
        graphicBox.Image = frame
    End Sub

    Sub createNewGeneration()

        Dim newGeneration As List(Of bird) = New List(Of bird)
        Dim mostFitBird As bird = New bird With {
            .Fitness = 0
        }

        'Evaluate Chances and generate most fit bird'
        Dim overallFitness = 0
        For Each item In birds

            If item.Fitness > mostFitBird.Fitness Then
                mostFitBird = item.Copy()
            End If
            overallFitness += item.Fitness
        Next
        For Each item In birds
            item.Chance = item.Fitness / overallFitness
            item.Fitness = 0
        Next
        'Chances Evaluated and most fit bird generated'

        'Best bird added to new generation'
        mostFitBird.Best = True
        newGeneration.Add(mostFitBird)

        birdsLiving += 1

        'Create the rest of the new generation' 
        For i = 1 To birds.Count() - 1

            Dim overallChance = 0.0
            Dim chosenBird = mostFitBird.Copy()
            Dim choice = Rnd()
            For Each item In birds
                overallChance += item.Chance
                If choice <= overallChance And item.UserControlled = False Then
                    chosenBird = item.Copy()
                    Exit For
                End If
            Next

            chosenBird.mutate()
            birdsLiving += 1
            newGeneration.Add(chosenBird.Copy())
        Next

        initializeGame(newGeneration)
        genLabel.Text = Convert.ToString(Convert.ToInt16(genLabel.Text) + 1)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Randomize()
        MyBase.FormBorderStyle = Windows.Forms.FormBorderStyle.None

        Dim newBirds As List(Of bird) = New List(Of bird)
        For i = 0 To 100
            Dim bir As bird = New bird
            If i = 1 Then bir.UserControlled = True
            newBirds.Add(bir)
            birdsLiving += 1
        Next

        initializeGame(newBirds)

        For i = 1 To 1
            If birdsLiving = 0 Then
                initializeGame(birds)
                logicFrame()
                'drawFrame()
            Else
                logicFrame()
                ' drawFrame()
            End If
        Next
    End Sub

    Private Sub graphicBox_Paint(sender As Object, e As EventArgs) Handles graphicBox.Paint
        If birdsLiving = 0 Then
            createNewGeneration()
            logicFrame()
            drawFrame()
        Else
            logicFrame()
            drawFrame()
        End If
    End Sub

    Private Sub graphicBox_Click(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown, graphicBox.Click
        If e.KeyCode = Keys.Space Then
            For Each item In birds
                If item.UserControlled Then
                    item.jump()
                End If
            Next
        End If
    End Sub

    Dim clicked = False
    Dim changePoint As Point = New Point
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Application.Exit()
    End Sub

    Private Sub Panel1_Up(sender As Object, e As EventArgs) Handles Panel1.MouseUp
        clicked = False
    End Sub

    Private Sub Panel1_Move(sender As Object, e As MouseEventArgs) Handles Panel1.MouseMove
        If clicked = True Then
            MyBase.Location = New Point(MyBase.Location.X + e.X - changePoint.X, MyBase.Location.Y + e.Y - changePoint.Y)
        End If
    End Sub

    Private Sub Panel1_Down(sender As Object, e As MouseEventArgs) Handles Panel1.MouseDown
        clicked = True
        changePoint = New Point(e.X, e.Y)
    End Sub
End Class
