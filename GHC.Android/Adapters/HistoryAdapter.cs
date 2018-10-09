using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using GHC.Data;
using Android.Locations;
using GeoCoordinatePortable;

namespace GHC.Adapters
{
    class HistoryAdapter : RecyclerView.Adapter
    {
        public event EventHandler<HistoryAdapterClickEventArgs> ItemClick;
        public event EventHandler<HistoryAdapterClickEventArgs> ItemLongClick;
        RequestVM[] items;

        public HistoryAdapter(RequestVM[] data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.history_item, parent, false);

            var vh = new HistoryAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as HistoryAdapterViewHolder;
            holder.CustomerName.Text = item.ProviderName.ToUpper();
            holder.Service.Text = item.HealthServiceName;
            holder.Date.Text = item.CompletedTime.Value.ToString("dd MMMM, yyyy");
            holder.Price.Text = item.Price.ToString("#,###");
        }

        public override int ItemCount => items.Length;

        void OnClick(HistoryAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(HistoryAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class HistoryAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView CustomerName { get; set; }
        public TextView Price { get; set; }
        public TextView Date { get; set; }
        public TextView Service { get; set; }


        public HistoryAdapterViewHolder(View itemView, Action<HistoryAdapterClickEventArgs> clickListener,
                            Action<HistoryAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            CustomerName = itemView.FindViewById<TextView>(Resource.Id.tvName);
            Price = itemView.FindViewById<TextView>(Resource.Id.tvPrice);
            Date = itemView.FindViewById<TextView>(Resource.Id.tvDate);
            Service = itemView.FindViewById<TextView>(Resource.Id.tvService);
            itemView.Click += (sender, e) => clickListener(new HistoryAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new HistoryAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class HistoryAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}