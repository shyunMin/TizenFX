using System;
using System.Collections.Generic;

namespace ElmSharp
{
    public enum ToolbarSelectionMode
    {
        Default = 0,
        Always,
        None,
        DisplayOnly,
    }

    public enum ToolbarShrinkMode
    {
        None = 0,
        Hide,
        Scroll,
        Menu,
        Expand
    }

    public class ToolbarItemEventArgs : EventArgs {
        public ToolbarItem Item { get; private set; }

        internal static ToolbarItemEventArgs CreateFromSmartEvent(IntPtr data, IntPtr obj, IntPtr info)
        {
            ToolbarItem item = ItemObject.GetItemByHandle(info) as ToolbarItem;
            return new ToolbarItemEventArgs() { Item = item };
        }
    }

    public class Toolbar : Widget
    {
        Interop.SmartEvent<ToolbarItemEventArgs> _clicked;
        Interop.SmartEvent<ToolbarItemEventArgs> _selected;
        Interop.SmartEvent<ToolbarItemEventArgs> _longpressed;
        public Toolbar(EvasObject parent) : base(parent)
        {
            _selected = new Interop.SmartEvent<ToolbarItemEventArgs>(this, Handle, "selected", ToolbarItemEventArgs.CreateFromSmartEvent);
            _selected.On += (s, e) =>
            {
                if (e.Item != null)
                {
                    Selected?.Invoke(this, e);
                    e.Item.SendSelected();
                }
            };
            _longpressed = new Interop.SmartEvent<ToolbarItemEventArgs>(this, Handle, "longpressed", ToolbarItemEventArgs.CreateFromSmartEvent);
            _longpressed.On += (s, e) =>
            {
                e.Item?.SendLongPressed();
            };
            _clicked = new Interop.SmartEvent<ToolbarItemEventArgs>(this, Handle, "clicked", ToolbarItemEventArgs.CreateFromSmartEvent);
            _clicked.On += (s, e) =>
            {
                e.Item?.SendClicked();
            };
        }

        public event EventHandler<ToolbarItemEventArgs> Selected;

        public bool Homogeneous
        {
            get
            {
                return Interop.Elementary.elm_toolbar_homogeneous_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_toolbar_homogeneous_set(Handle, value);
            }
        }

        public ToolbarSelectionMode SelectionMode
        {
            get
            {
                return (ToolbarSelectionMode)Interop.Elementary.elm_toolbar_select_mode_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_toolbar_select_mode_set(Handle, (int)value);
            }
        }

        public ToolbarShrinkMode ShrinkMode
        {
            get
            {
                return (ToolbarShrinkMode)Interop.Elementary.elm_toolbar_shrink_mode_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_toolbar_shrink_mode_set(Handle, (int)value);
            }
        }

        public double ItemAlignment
        {
            get
            {
                return Interop.Elementary.elm_toolbar_align_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_toolbar_align_set(Handle, value);
            }
        }

        public bool TransverseExpansion
        {
            get
            {
                return Interop.Elementary.elm_toolbar_transverse_expanded_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_toolbar_transverse_expanded_set(Handle, value);
            }
        }

        public ToolbarItem Append(string label)
        {
            return Append(label, null);
        }
        public ToolbarItem Append(string label, string icon)
        {
            ToolbarItem item = new ToolbarItem(label, icon);
            item.Handle = Interop.Elementary.elm_toolbar_item_append(Handle, icon, label, null, (IntPtr)item.Id);
            return item;
        }

        public ToolbarItem Prepend(string label)
        {
            return Prepend(label, null);
        }

        public ToolbarItem Prepend(string label, string icon)
        {
            ToolbarItem item = new ToolbarItem(label, icon);
            item.Handle = Interop.Elementary.elm_toolbar_item_prepend(Handle, icon, label, null, (IntPtr)item.Id);
            return item;
        }

        public ToolbarItem InsertBefore(ToolbarItem before, string label)
        {
            return InsertBefore(before, label);
        }

        public ToolbarItem InsertBefore(ToolbarItem before, string label, string icon)
        {
            ToolbarItem item = new ToolbarItem(label, icon);
            item.Handle = Interop.Elementary.elm_toolbar_item_insert_before(Handle, before, icon, label, null, (IntPtr)item.Id);
            return item;
        }

        public ToolbarItem SelectedItem
        {
            get
            {
                IntPtr handle = Interop.Elementary.elm_toolbar_selected_item_get(Handle);
                return ItemObject.GetItemByHandle(handle) as ToolbarItem;
            }
        }

        protected override IntPtr CreateHandle(EvasObject parent)
        {
            return Interop.Elementary.elm_toolbar_add(parent);
        }
    }
}
