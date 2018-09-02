using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using GHC.Data;
using System.Collections.Generic;

namespace GHC.Adapters
{
    class ServicesAdapter : RecyclerView.Adapter
    {
        public event EventHandler<ServicesAdapterClickEventArgs> ItemClick;
        public event EventHandler<ServicesAdapterClickEventArgs> ItemLongClick;
        List<HealthService> items;

        string language;

        public ServicesAdapter(List<HealthService> data, string language = "en")
        {
            items = data;
            this.language = language;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = LayoutInflater.From(parent.Context).
                   Inflate(Resource.Layout.service_item, parent, false);

            var vh = new ServicesAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as ServicesAdapterViewHolder;

            if (language == "en")
                holder.TextView.Text = items[position].Name;
            else
                holder.TextView.Text = items[position].SwahiliName;
        }

        public override int ItemCount => items.Count;

        void OnClick(ServicesAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(ServicesAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class ServicesAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextView { get; set; }


        public ServicesAdapterViewHolder(View itemView, Action<ServicesAdapterClickEventArgs> clickListener,
                            Action<ServicesAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            TextView = itemView.FindViewById<TextView>(Resource.Id.textView);
            itemView.Click += (sender, e) => clickListener(new ServicesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new ServicesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class ServicesAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}