﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Windows.Forms
{

    /// <summary>
    /// Provides a tree view control supporting three state checkboxes.
    /// </summary>
    public class AdvTreeView : TreeView
    {
        #region Events

        /// <summary>
        /// Validate node must checked or not ? and get not check cause message
        /// </summary>
        /// <param name="e">Current checked Node</param>
        /// <returns>Why must not check error message. if must checked and not any error then return null</returns>
        public delegate string NodeValidator(TreeNode e);

        public delegate void CheckedChangedHandler(TreeViewEventArgs e);
        public event CheckedChangedHandler CheckedChanged;
        protected virtual void OnCheckedChanged(TreeViewEventArgs e)
        {
            CheckedChangedHandler handler = CheckedChanged;
            if (handler != null) handler(e);
        }

        public delegate void NodeAddedHandler(TreeViewEventArgs e);
        public event NodeAddedHandler NodeAdded;
        protected virtual void OnNodeAdded(TreeViewEventArgs e)
        {
            e.Node.Checked = e.Node.Checked;
            NodeAddedHandler handler = NodeAdded;
            if (handler != null) handler(e);
        }

        internal void PerformNodeAdded(TreeNode node)
        {
            OnNodeAdded(new TreeViewEventArgs(node));
        }

        #endregion

        #region Fields

        private readonly ImageList _ilStateImages;
        private readonly List<string> _errorNodes;
        private bool _checkBoxesVisible;
        private bool _preventCheckEvent;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of this control.
        /// </summary>
        public AdvTreeView()
        {
            _errorNodes = new List<string>();
            NodeErrorDuration = 3000;
            ErrorForeColor = Color.Crimson;
            ParentNodeSelectError = @"Parent not selectable class!";
            SiblingNodeSelectError = @"The ({0}) is a selected sibling node was found!";

            _ilStateImages = new ImageList();											// first we create our state image
            var cbsState = CheckBoxState.UncheckedNormal;

            for (int i = 0; i <= 2; i++)
            {												// let's iterate each three state
                var bmpCheckBox = new Bitmap(16, 16);
                var gfxCheckBox = Graphics.FromImage(bmpCheckBox);
                switch (i)
                {															// it...
                    case 0: cbsState = CheckBoxState.UncheckedNormal; break;
                    case 1: cbsState = CheckBoxState.CheckedNormal; break;
                    case 2: cbsState = CheckBoxState.MixedNormal; break;
                }
                CheckBoxRenderer.DrawCheckBox(gfxCheckBox, new Point(2, 2), cbsState);	// ...rendering the checkbox and...
                gfxCheckBox.Save();
                _ilStateImages.Images.Add(bmpCheckBox);									// ...adding to sate image list.

                CheckBoxesThreeState = true;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets to display
        /// checkboxes in the tree
        /// view.
        /// </summary>
        [Category("Appearance")]
        [Description("Sets tree view to display checkboxes or not.")]
        [DefaultValue(false)]
        public new bool CheckBoxes
        {
            get { return _checkBoxesVisible; }
            set
            {
                _checkBoxesVisible = value;
                base.CheckBoxes = _checkBoxesVisible;
                this.StateImageList = _checkBoxesVisible ? _ilStateImages : null;
            }
        }

        [Browsable(false)]
        public new ImageList StateImageList
        {
            get { return base.StateImageList; }
            set { base.StateImageList = value; }
        }

        /// <summary>
        /// Gets or sets to support three state in the checkboxes or not.
        /// </summary>
        [Category("Appearance"), Description("Sets tree view to use three state checkboxes or not."), DefaultValue(true)]
        public bool CheckBoxesThreeState { get; set; }

        /// <summary>
        /// Gets or sets to no support multi sibling checks.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets to no support multi sibling checks."), DefaultValue(false)]
        public bool SiblingLimitSelection { get; set; }

        /// <summary>
        /// Gets or sets Parent select error message.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets Parent select error message.")]
        public string ParentNodeSelectError { get; set; }

        /// <summary>
        /// Gets or sets Sibling select error message.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets Sibling select error message.")]
        public string SiblingNodeSelectError { get; set; }

        /// <summary>
        /// Gets or sets select error duration per millisecond.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets select error duration per millisecond.")]
        public int NodeErrorDuration { get; set; }

        /// <summary>
        /// Gets or sets select error ForeColor.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets select error ForeColor.")]
        public Color ErrorForeColor { get; set; }

        /// <summary>
        /// TreeNode validator for define selected node must checked or not ? and get not check cause message
        /// </summary>
        /// <value>
        /// The check node validation.
        /// </value>
        [Browsable(false)]
        public NodeValidator CheckNodeValidation { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Refreshes this control.
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();

            if (!CheckBoxes)												// nothing to do here if
                return;														// checkboxes are hidden.

            base.CheckBoxes = false;										// hide normal checkboxes...

            var stNodes = new Stack<TreeNode>(this.Nodes.Count);
            foreach (TreeNode tnCurrent in this.Nodes)						// push each root node.
                stNodes.Push(tnCurrent);

            while (stNodes.Count > 0)
            {										                        // let's pop node from stack,
                var tnStacked = stNodes.Pop();
                if (tnStacked.StateImageIndex == -1)						// index if not already done
                    tnStacked.StateImageIndex = tnStacked.Checked ? 1 : 0;	// and push each child to stack
                for (int i = 0; i < tnStacked.Nodes.Count; i++)				// too until there are no
                    stNodes.Push(tnStacked.Nodes[i]);						// nodes left on stack.
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            Refresh();
        }

        protected override void OnAfterExpand(TreeViewEventArgs e)
        {
            base.OnAfterExpand(e);

            foreach (TreeNode tnCurrent in e.Node.Nodes)					// set tree state image
                if (tnCurrent.StateImageIndex == -1)						// to each child node...
                    tnCurrent.StateImageIndex = tnCurrent.Checked ? 1 : 0;
        }

        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            base.OnAfterCheck(e);

            if (_preventCheckEvent)
                return;

            OnNodeMouseClick(new TreeNodeMouseClickEventArgs(e.Node, MouseButtons.None, 0, 0, 0));
        }

        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            base.OnNodeMouseClick(e);

            try
            {
                _preventCheckEvent = true;

                if (!e.Node.Checked && SiblingLimitSelection) // not checked show before clicking mode and is not current state
                {
                    if (e.Node.GetNodeCount(false) > 0) // is nested node ?
                    {
                        e.Node.Checked = false;
                        SetError(e.Node, ParentNodeSelectError);
                        return;
                    }

                    TreeNode sibling;
                    if ((sibling = e.Node.GetFirstCheckedSiblingNode()) != null) // check sibling selections
                    {
                        e.Node.Checked = false;
                        SetError(e.Node, SiblingNodeSelectError, sibling.Text);
                        return;
                    }

                    if (CheckNodeValidation != null)
                    {
                        string errorMsg = CheckNodeValidation(e.Node);
                        if (errorMsg != null)
                        {
                            e.Node.Checked = false;
                            SetError(e.Node, errorMsg);
                            return;
                        }
                    }
                }

                int iSpacing = ImageList == null ? 0 : 18;
                if ((e.X > e.Node.Bounds.Left - iSpacing || // *not* used by the state
                     e.X < e.Node.Bounds.Left - (iSpacing + 16)) && // image we can leave here.
                    e.Button != MouseButtons.None)
                {
                    return;
                }

                var tnBuffer = e.Node;
                if (e.Button == MouseButtons.Left) // flip its check state.
                    tnBuffer.Checked = !tnBuffer.Checked;

                tnBuffer.StateImageIndex = tnBuffer.Checked
                    ? // set state image index
                    1 : tnBuffer.StateImageIndex; // correctly.

                OnAfterCheck(new TreeViewEventArgs(tnBuffer, TreeViewAction.ByMouse));

                var stNodes = new Stack<TreeNode>(tnBuffer.Nodes.Count);
                stNodes.Push(tnBuffer); // push buffered node first.
                do
                {
                    // let's pop node from stack,
                    tnBuffer = stNodes.Pop(); // inherit buffered node's
                    tnBuffer.Checked = e.Node.Checked; // check state and push
                    tnBuffer.StateImageIndex = e.Node.Checked ? 1 : 0;
                    OnCheckedChanged(new TreeViewEventArgs(tnBuffer));

                    for (int i = 0; i < tnBuffer.Nodes.Count; i++) // each child on the stack
                        stNodes.Push(tnBuffer.Nodes[i]); // until there is no node
                } while (stNodes.Count > 0); // left.

                var bMixedState = false;
                tnBuffer = e.Node; // re-buffer clicked node.
                while (tnBuffer.Parent != null)
                {
                    // while we get a parent we
                    foreach (TreeNode tnChild in tnBuffer.Parent.Nodes) // determine mixed check states
                        bMixedState |= (tnChild.Checked != tnBuffer.Checked | // and convert current check
                                        tnChild.StateImageIndex == 2); // state to state image index.
                    var iIndex = (int)Convert.ToUInt32(tnBuffer.Checked);
                    tnBuffer.Parent.Checked = bMixedState || (iIndex > 0); // state image in dependency
                    if (bMixedState) // of mixed state.
                        tnBuffer.Parent.StateImageIndex = CheckBoxesThreeState ? 2 : 1;
                    else
                        tnBuffer.Parent.StateImageIndex = iIndex;
                    tnBuffer = tnBuffer.Parent; // finally buffer parent and

                    OnCheckedChanged(new TreeViewEventArgs(tnBuffer));
                } // loop here.

                // set this node StateImageIndex to 0 if not checked
                if (!e.Node.Checked) e.Node.StateImageIndex = 0;
                // raise checked changed event
                OnCheckedChanged(new TreeViewEventArgs(e.Node));
            }
            finally
            {
                _preventCheckEvent = false;
            }
        }

        protected async void SetError(TreeNode node, string errorText, params object[] errorParams)
        {
            lock (node)
            {
                if (_errorNodes.Contains(node.GetUniqueValue())) return;
                _errorNodes.Add(node.GetUniqueValue());
            }

            try
            {
                var tBuffer = node.Text;
                var cBuffer = node.ForeColor;

                node.ForeColor = ErrorForeColor;
                node.Text = String.Format("{1}    ({0})", String.Format(errorText, errorParams), node.Text);

                await Task.Delay(NodeErrorDuration);

                if (!this.IsHandleCreated) return;

                node.ForeColor = cBuffer;
                node.Text = tBuffer;
            }
            finally
            {
                _errorNodes.Remove(node.GetUniqueValue());
            }
        }

        public TreeNode Add(TreeNode node)
        {
            this.Nodes.Add(node);
            OnNodeAdded(new TreeViewEventArgs(node));
            return node;
        }

        public void AddRange(TreeNode[] nodeArray)
        {
            foreach (TreeNode node in nodeArray)
            {
                this.Nodes.Add(node);
                OnNodeAdded(new TreeViewEventArgs(node));
            }
        }

        #endregion
    }


    public static class AdvTreeViewExtensions
    {
        public static CheckBoxState CheckState(this TreeNode node)
        {
            switch (node.StateImageIndex)
            {
                case 0: return CheckBoxState.UncheckedNormal;
                case 1: return CheckBoxState.CheckedNormal;
                case 2: return CheckBoxState.MixedNormal;
                default:
                    return CheckBoxState.UncheckedNormal;
            }
        }

        public static TreeNode GetFirstCheckedSiblingNode(this TreeNode node)
        {
            while (node.Parent != null && node.Parent.GetNodeCount(false) > 1) // have sibling node except self?
            {
                foreach (TreeNode sibling in node.Parent.Nodes)
                {
                    if (sibling.Index != node.Index && sibling.CheckState() != CheckBoxState.UncheckedNormal)
                    // is sibling node not me and checked or mixed?
                    {
                        return sibling;
                    }
                }

                node = node.Parent; // check next time, this node parent sibling nodes
            }

            return null;
        }

        public static List<TreeNode> GetCheckedSiblingsNode(this TreeNode node)
        {
            var siblings = new List<TreeNode>();

            while (node.Parent != null && node.Parent.GetNodeCount(false) > 1) // have sibling node except self?
            {
                foreach (TreeNode sibling in node.Parent.Nodes)
                {
                    if (sibling.Index != node.Index && sibling.CheckState() != CheckBoxState.UncheckedNormal)
                    // is sibling node not me and checked or mixed?
                    {
                        siblings.Add(sibling);
                    }
                }

                node = node.Parent; // check next time, this node parent sibling nodes
            }

            return siblings;
        }

        internal static string GetUniqueValue(this TreeNode node)
        {
            string key = "";

            while (node != null)
            {
                key += string.Format(@"\{0}", node.Index);
                node = node.Parent;
            }

            return key;
        }

        public static TreeNode AddNode(this TreeNode node, TreeNode newNode)
        {
            var tree = (AdvTreeView)node.TreeView;
            node.Nodes.Add(newNode);
            if (tree != null) tree.PerformNodeAdded(newNode);
            return newNode;
        }
        public static void AddRangeNodes(this TreeNode node, TreeNode[] newNodes)
        {
            var tree = (AdvTreeView)node.TreeView;

            foreach (TreeNode newNode in newNodes)
            {
                node.Nodes.Add(newNode);
                if (tree != null) tree.PerformNodeAdded(newNode);
            }
        }
    }
}