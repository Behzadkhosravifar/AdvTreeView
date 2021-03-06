﻿Imports Windows.Forms

Public Class TestForm
    Public Sub New()
        InitializeComponent()

        AddHandler advTree.CheckedChanged, AddressOf advTree_CheckedChanged
        AddHandler chkSiblingCheckLimitation.CheckedChanged, AddressOf chkSiblingCheckLimitation_CheckedChanged
        AddHandler numErrorDuration.ValueChanged, AddressOf numErrorDuration_ValueChanged
        AddHandler txtSiblingSelectError.TextChanged, AddressOf txtSiblingSelectError_TextChanged
        AddHandler txtParentSelectError.TextChanged, AddressOf txtParentSelectError_TextChanged

        txtParentSelectError.Text = advTree.ParentNodeSelectError
        txtSiblingSelectError.Text = advTree.SiblingNodeSelectError
        numErrorDuration.Value = 3000
        advTree.CheckNodeValidation = AddressOf NodeValidation
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        Dim treeNode1 = New TreeNode("Node0")
        Dim treeNode2 = New TreeNode("Node5")
        Dim treeNode3 = New TreeNode("Node8")
        Dim treeNode4 = New TreeNode("Node9")
        Dim treeNode5 = New TreeNode("Node10")
        Dim treeNode6 = New TreeNode("Node6")
        treeNode6.AddRangeNodes(New TreeNode() {treeNode3, treeNode4, treeNode5})
        Dim treeNode7 = New TreeNode("Node7")
        Dim treeNode8 = New TreeNode("Node1")
        treeNode8.AddRangeNodes(New TreeNode() {treeNode2, treeNode6, treeNode7})
        Dim treeNode9 = New TreeNode("Node2")
        Dim treeNode10 = New TreeNode("Node11")
        Dim treeNode11 = New TreeNode("Node12")
        Dim treeNode12 = New TreeNode("Node14")
        Dim treeNode13 = New TreeNode("Node19")
        Dim treeNode14 = New TreeNode("Node20")
        Dim treeNode15 = New TreeNode("Node17")
        treeNode15.AddRangeNodes(New TreeNode() {treeNode13, treeNode14})
        Dim treeNode16 = New TreeNode("Node18")
        Dim treeNode17 = New TreeNode("Node15")
        treeNode17.AddRangeNodes(New TreeNode() {treeNode15, treeNode16})
        Dim treeNode18 = New TreeNode("Node16")
        Dim treeNode19 = New TreeNode("Node13")
        treeNode19.AddRangeNodes(New TreeNode() {treeNode12, treeNode17, treeNode18})
        Dim treeNode20 = New TreeNode("Node3")
        treeNode20.AddRangeNodes(New TreeNode() {treeNode10, treeNode11, treeNode19})
        Dim treeNode21 = New TreeNode("Node4")

        treeNode1.Name = "Node0"
        treeNode1.Text = "Node0"
        treeNode2.Name = "Node5"
        treeNode2.Text = "Node5"
        treeNode3.Name = "Node8"
        treeNode3.Text = "Node8"
        treeNode4.Name = "Node9"
        treeNode4.Text = "Node9"
        treeNode5.Name = "Node10"
        treeNode5.Text = "Node10"
        treeNode6.Name = "Node6"
        treeNode6.Text = "Node6"
        treeNode7.Name = "Node7"
        treeNode7.Text = "Node7"
        treeNode8.Name = "Node1"
        treeNode8.Text = "Node1"
        treeNode9.Name = "Node2"
        treeNode9.Text = "Node2"
        treeNode10.Name = "Node11"
        treeNode10.Text = "Node11"
        treeNode11.Name = "Node12"
        treeNode11.Text = "Node12"
        treeNode12.Name = "Node14"
        treeNode12.Text = "Node14"
        treeNode13.Name = "Node19"
        treeNode13.Text = "Node19"
        treeNode14.Name = "Node20"
        treeNode14.Text = "Node20"
        treeNode15.Name = "Node17"
        treeNode15.Text = "Node17"
        treeNode16.Name = "Node18"
        treeNode16.Text = "Node18"
        treeNode17.Name = "Node15"
        treeNode17.Text = "Node15"
        treeNode18.Name = "Node16"
        treeNode18.Text = "Node16"
        treeNode19.Name = "Node13"
        treeNode19.Text = "Node13"
        treeNode20.Name = "Node3"
        treeNode20.Text = "Node3"
        treeNode21.Name = "Node4"
        treeNode21.Text = "Node4"


        advTree.AddRange(New TreeNode() {treeNode1, treeNode8, treeNode9, treeNode20, treeNode21})
    End Sub




    Private Sub advTree_CheckedChanged(e As TreeViewEventArgs)
        e.Node.Text = e.Node.Index.ToString() + e.Node.CheckState().ToString()
    End Sub

    Private Sub chkSiblingCheckLimitation_CheckedChanged(sender As Object, e As EventArgs)
        advTree.SiblingLimitSelection = DirectCast(sender, CheckBox).Checked
    End Sub

    Private Sub txtSiblingSelectError_TextChanged(sender As Object, e As EventArgs)
        advTree.SiblingNodeSelectError = txtSiblingSelectError.Text
    End Sub

    Private Sub txtParentSelectError_TextChanged(sender As Object, e As EventArgs)
        advTree.ParentNodeSelectError = txtParentSelectError.Text
    End Sub

    Private Sub numErrorDuration_ValueChanged(sender As Object, e As EventArgs)
        advTree.NodeErrorDuration = CInt(numErrorDuration.Value)
    End Sub


    Private Function NodeValidation(e As TreeNode) As String
        Return If(e.Text.Contains("0UnChecked"), "This is not valid", Nothing)
    End Function
End Class