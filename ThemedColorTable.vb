Public Class ThemedColorTable : Inherits ProfessionalColorTable


    Public Overrides ReadOnly Property ToolStripBorder As Color
        Get
            Return Color.Gray
        End Get
    End Property

    Public Overrides ReadOnly Property ToolStripDropDownBackground As Color
        Get
            Return Color.Black
        End Get
    End Property

    Public Overrides ReadOnly Property ToolStripGradientBegin As Color
        Get
            Return Color.Black
        End Get
    End Property

    Public Overrides ReadOnly Property ToolStripGradientEnd As Color
        Get
            Return Color.Black
        End Get
    End Property

    Public Overrides ReadOnly Property ToolStripGradientMiddle As Color
        Get
            Return Color.Black
        End Get
    End Property

    Public Overrides ReadOnly Property ImageMarginGradientBegin As Color
        Get
            Return Color.Black
        End Get
    End Property
    Public Overrides ReadOnly Property ImageMarginGradientMiddle As Color
        Get
            Return Color.Black
        End Get
    End Property

    Public Overrides ReadOnly Property ImageMarginGradientEnd As Color
        Get
            Return Color.Black
        End Get
    End Property

    Public Overrides ReadOnly Property MenuItemSelected As Color
        Get
            Return Color.DarkBlue
        End Get
    End Property

End Class
