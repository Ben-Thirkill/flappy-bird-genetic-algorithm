Imports MathNet.Numerics
Public Class NN
    Private neurons As List(Of Integer) = New List(Of Integer)
    Private neuronmatrixes As List(Of LinearAlgebra.Double.Matrix) = New List(Of LinearAlgebra.Double.Matrix)
    Public weightmatrixes As List(Of LinearAlgebra.Double.Matrix) = New List(Of LinearAlgebra.Double.Matrix)
    Private errors As List(Of LinearAlgebra.Double.Matrix) = New List(Of LinearAlgebra.Double.Matrix)
    Private deltas As List(Of LinearAlgebra.Double.Matrix) = New List(Of LinearAlgebra.Double.Matrix)

    Function sigmoid(ByVal x As Double) As Double
        Return 1 / (1 + Math.Exp(-x))
    End Function

    Function derivsigmoid(ByVal x As Double) As Double
        Return x * (1 - x)
    End Function

    Function sig(ByVal mat As LinearAlgebra.Double.Matrix)
        Dim matrix As LinearAlgebra.Double.Matrix = LinearAlgebra.Double.Matrix.Build.DenseOfMatrix(mat)
        For _x = 0 To matrix.RowCount() - 1
            For _y = 0 To matrix.ColumnCount() - 1
                matrix(_x, _y) = sigmoid(matrix(_x, _y))
            Next
        Next
        Return matrix
    End Function

    Function divsig(ByVal mat As LinearAlgebra.Double.Matrix)
        Dim matrix As LinearAlgebra.Double.Matrix = LinearAlgebra.Double.Matrix.Build.DenseOfMatrix(mat)

        For _x = 0 To matrix.RowCount() - 1
            For _y = 0 To matrix.ColumnCount() - 1
                matrix(_x, _y) = derivsigmoid(matrix(_x, _y))
            Next
        Next
        Return matrix
    End Function

    Function FeedForward(input)
        neuronmatrixes(0) = input
        For i = 1 To neuronmatrixes.Count - 1
            neuronmatrixes(i) = (sig(neuronmatrixes(i - 1) * weightmatrixes(i - 1))) ' use dot
        Next
        'neuronmatrixes(neuronmatrixes.Count() - 1) = sig(neuronmatrixes(neuronmatrixes.Count() - 1) * weightmatrixes(weightmatrixes.Count() - 1)) ' use dot
        Dim o = neuronmatrixes(neuronmatrixes.Count() - 1)
        If o.ColumnCount() = 1 And o.RowCount() = 1 Then
            Return o(0, 0)
        Else
            Return o
        End If
    End Function

    Sub New(layers As List(Of Integer))
        For Each item In layers
            neurons.Add(item)
        Next
        For i = 1 To neurons.Count() - 1
            Dim w As LinearAlgebra.Double.Matrix = -1 + 2 * LinearAlgebra.Double.Matrix.Build.Random(neurons(i - 1), neurons(i))

            weightmatrixes.Add(w)
        Next
        For i = 0 To neurons.Count() - 1
            neuronmatrixes.Add(LinearAlgebra.Double.Matrix.Build.DenseOfArray({{0, 0}, {0, 1}, {1, 0}, {1, 1}}))
        Next
    End Sub
End Class