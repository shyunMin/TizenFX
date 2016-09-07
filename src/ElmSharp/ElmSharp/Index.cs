using System;
using System.Collections.Generic;

namespace ElmSharp
{
    public class Index : Layout
    {
        HashSet<IndexItem> _children = new HashSet<IndexItem>();
        Interop.SmartEvent _delayedChanged;

        public Index(EvasObject parent) : base(parent)
        {
            _delayedChanged = new Interop.SmartEvent(this, Handle, "delay,changed");
            _delayedChanged.On += _delayedChanged_On;
        }

        public event EventHandler Changed;

        public bool AutoHide
        {
            get
            {
                return !Interop.Elementary.elm_index_autohide_disabled_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_index_autohide_disabled_set(Handle, !value);
            }
        }

        public bool IsHorizontal
        {
            get
            {
                return Interop.Elementary.elm_index_horizontal_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_index_horizontal_set(Handle, value);
            }
        }

        public bool IndicatorVisible
        {
            get
            {
                return !Interop.Elementary.elm_index_indicator_disabled_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_index_indicator_disabled_set(Handle, !value);
            }
        }

        public bool OmitEnabled
        {
            get
            {
                return Interop.Elementary.elm_index_omit_enabled_get(Handle);
            }
            set
            {
                Interop.Elementary.elm_index_omit_enabled_set(Handle, value);
            }
        }

        public IndexItem SelectedItem
        {
            get
            {
                IntPtr handle = Interop.Elementary.elm_index_selected_item_get(Handle, 0);
                return ItemObject.GetItemByHandle(handle) as IndexItem;
            }
        }

        public IndexItem Append(string label)
        {
            IndexItem item = new IndexItem(label);
            item.Handle = Interop.Elementary.elm_index_item_append(Handle, label, null, (IntPtr)item.Id);
            return item;
        }

        public IndexItem Prepend(string label)
        {
            IndexItem item = new IndexItem(label);
            item.Handle = Interop.Elementary.elm_index_item_prepend(Handle, label, null, (IntPtr)item.Id);
            return item;
        }

        public IndexItem InsertBefore(string label, IndexItem before)
        {
            IndexItem item = new IndexItem(label);
            item.Handle = Interop.Elementary.elm_index_item_insert_before(Handle, before, label, null, (IntPtr)item.Id);
            return item;
        }

        public void Update(int level)
        {
            Interop.Elementary.elm_index_level_go(Handle, level);
        }

        protected override IntPtr CreateHandle(EvasObject parent)
        {
            return Interop.Elementary.elm_index_add(parent);
        }

        void _delayedChanged_On(object sender, EventArgs e)
        {
            SelectedItem?.SendSelected();
            Changed?.Invoke(this, e);
        }

        void AddInternal(IndexItem item)
        {
            _children.Add(item);
            item.Deleted += Item_Deleted;
        }

        void Item_Deleted(object sender, EventArgs e)
        {
            _children.Remove((IndexItem)sender);
        }
    }
}
