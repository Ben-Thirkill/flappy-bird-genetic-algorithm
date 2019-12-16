Imports MathNet.Numerics
Public Class PipePair
    Public X As Integer
    Public MidPoint As Integer
    Public Width As Integer
    Public Gap As Integer
    Public Index As Integer

    Sub draw(g, b)
        Dim gapdiv As Integer = Math.Round(Gap / 2)
        Dim widthdiv As Integer = Math.Round(Width / 2)
        g.FillRectangle(Brushes.Green, X - widthdiv, 0, Width, MidPoint - gapdiv)

        g.FillRectangle(Brushes.Green, X - widthdiv, MidPoint + gapdiv, Width, b.Height - MidPoint + Gap)
    End Sub

    Sub move(ByRef list As List(Of PipePair))
        X -= 1
        If X + Width < 0 Then
            list(Index) = Nothing
        End If

    End Sub

    Sub New()
        Width = 32
        Gap = 55
        MidPoint = 0
        X = 350
    End Sub

End Class

Public Class bird
    Dim _x As Integer
    Dim _y As Integer
    Dim _position As Point
    Dim _alive As Boolean
    Dim _chance As Double
    Dim _bestest As Boolean
    Dim _mutationrate As Double = 0.1
    Dim _velocity As Integer = 0
    Dim _fitness As Double = 0
    Dim _jumpTimer As Double = 10
    Public Brain As NN = New NN(New List(Of Integer) From {4, 10, 10, 10, 1})
    Public UserControlled = False

    Property Fitness As Double
        Set(value As Double)
            _fitness = value
        End Set
        Get
            Return _fitness
        End Get
    End Property

    Property Rate As Double
        Set(value As Double)
            _mutationrate = value
        End Set
        Get
            Return _mutationrate
        End Get
    End Property

    Property Best As Boolean
        Set(value As Boolean)
            _bestest = value
        End Set
        Get
            Return _bestest
        End Get
    End Property

    Property Alive As Boolean
        Set(value As Boolean)
            _alive = value
        End Set
        Get
            Return _alive
        End Get
    End Property

    Property X As Integer
        Set(value As Integer)
            _x = value
        End Set
        Get
            Return _x
        End Get
    End Property

    Property Y As Integer
        Set(value As Integer)
            _y = value

        End Set
        Get
            Return _y
        End Get
    End Property

    Property Position As Point
        Set(value As Point)
            _x = value.X
            _y = value.Y
            _position = value
        End Set
        Get
            Return New Point(_x, _y)
        End Get
    End Property

    Property Chance As Double
        Set(value As Double)
            _chance = value
        End Set
        Get
            Return _chance
        End Get
    End Property

    ' Initializing the bitmaps here instead of every frame for a small performance boost.
    Dim flappyImage As Bitmap = Drawing.Image.FromFile(Application.StartupPath & "\flappy.png")
    Dim flappyTilted As Bitmap = Drawing.Image.FromFile(Application.StartupPath & "\flappyJump.png")
    Sub draw(g)
        If _alive Then
            If _velocity <> 0 Then
                If UserControlled Then
                    g.DrawImage(flappyTilted, _x, _y)
                Else
                    g.DrawImage(flappyTilted, _x, _y)
                End If
            Else
                If UserControlled Then
                    g.DrawImage(flappyImage, _x, _y)
                Else
                    g.DrawImage(flappyImage, _x, _y)
                End If
            End If
        End If
    End Sub

    Sub move(g, p, ByRef birdsLiving)
        Dim width = 20
        Dim height = 14
        If _velocity <> 0 Then
            width = 17
            height = 15
        End If

        If (_x + width > g.Width Or _x < 0 Or _y < 0 Or _y + height > g.Height) And _alive Then
            _alive = False
            birdsLiving -= 1
        End If

        For Each item In p
            If Not IsNothing(item) Then
                Dim gapdiv As Integer = Math.Round(item.Gap / 2)

                If (_x + width > item.X And _x < item.X + item.Width And Not (_y > item.MidPoint - gapdiv And _y + height < item.MidPoint + gapdiv)) And _alive Then
                    _alive = False
                    birdsLiving -= 1

                End If
            End If
        Next

        If _alive Then
            Fitness += 1 / 100
            If _velocity > 0 Then
                If _velocity = 1 Then
                    _velocity = -5

                Else
                    _y -= _velocity
                    _velocity -= 1
                End If
            End If

            If _velocity < 0 Then
                If _velocity < -2 Then
                    _velocity += 1
                Else
                    _y += (_velocity + 2)
                    _velocity += 1
                End If
            End If
            If _velocity = 0 Then
                _y += 3
            End If
        End If
    End Sub

    Sub jump()
        If _jumpTimer >= 8 Then
            _velocity = 8

            _jumpTimer = 0
        End If
    End Sub

    Sub think(pipes, g)
        If _jumpTimer < 8 Then
            _jumpTimer += 1
        End If
        If Not UserControlled Then
            Dim closestPipe As PipePair

            Dim distancePipe As Double = 10000

            For Each item In pipes
                If item IsNot Nothing Then
                    If item.X + item.Width - _x < distancePipe Then
                        distancePipe = Math.Abs(item.X - _x)
                        closestPipe = item
                    End If
                End If
            Next

            Dim distancePipeTop As Double = (closestPipe.MidPoint - closestPipe.Gap / 2) - _y
            Dim distancePipeBottom As Double = (closestPipe.MidPoint + closestPipe.Gap / 2) - _y
            If _y > closestPipe.MidPoint - closestPipe.Gap / 2 And _y + 16 < closestPipe.MidPoint + closestPipe.Gap / 2 Then Fitness += 2
            If Brain.FeedForward(LinearAlgebra.Double.Matrix.Build.DenseOfArray({{distancePipe / (g.Width / 2), distancePipeTop / g.Height, distancePipeBottom / g.Height, _velocity / 10}})) > 0.5 Then
                jump()
            End If
        End If

    End Sub

    Function Copy() As bird
        Dim newBird As bird = New bird
        Dim c = 0
        For Each matrix In Brain.weightmatrixes
            matrix.CopyTo(newBird.Brain.weightmatrixes(c))
            c += 1
        Next
        Return newBird
    End Function

    ' Not used, however I might use this later to improve the learning capabilities.
    Function Breed(ByVal breedee As bird) As bird
        Dim newBird As bird = New bird
        For i = 0 To breedee.Brain.weightmatrixes.Count()
            newBird.Brain.weightmatrixes(i) = (breedee.Brain.weightmatrixes(i).Add(Brain.weightmatrixes(i))) / 2
        Next
        Return newBird
    End Function

    Sub mutate()
        For i = 0 To Brain.weightmatrixes.Count - 1
            For a = 0 To Brain.weightmatrixes(i).RowCount() - 1
                For b = 0 To Brain.weightmatrixes(i).ColumnCount() - 1
                    If Rnd() < 0.1 Then
                        Brain.weightmatrixes(i)(a, b) += -4 + Rnd() * 8
                    End If
                Next
            Next
        Next
    End Sub

    Public Sub New()
        _x = 50
        _y = 0
        _position = New Point(_x, _y)
        _alive = True
    End Sub

End Class