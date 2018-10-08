using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using GHC.Data;
using Android.Locations;
using GeoCoordinatePortable;

namespace GHC.Adapters
{
    class RequestsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<RequestsAdapterClickEventArgs> ItemClick;
        public event EventHandler<RequestsAdapterClickEventArgs> ItemLongClick;
        RequestVM[] items;
        public Location Location;

        public RequestsAdapter(RequestVM[] data, Location location)
        {
            items = data;
            this.Location = location;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.service_request_item, parent, false);

            var vh = new RequestsAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as RequestsAdapterViewHolder;
            holder.CustomerName.Text = item.CustomerName.ToUpper();
            holder.Service.Text = item.HealthServiceName;

            if (Location != null)
            {
                double distance = CalculateDistance(Location.Latitude, Location.Longitude, item.Latitude, item.Longitude);
                if (distance < 1000)
                    holder.CustomerDistance.Text = $"{distance.ToString("#,###.#")}m";
                else
                {
                    string km = (distance / 1000).ToString("#,###.#");
                    holder.CustomerDistance.Text = $"{km}km";
                }
            }
        }

        public override int ItemCount => items.Length;

        void OnClick(RequestsAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(RequestsAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

        double CalculateDistance(double sLatitude, double sLongitude, double eLatitude, double eLongitude)
        {
            var sCoord = new GeoCoordinate(sLatitude, sLongitude);
            var eCoord = new GeoCoordinate(eLatitude, eLongitude);

            return sCoord.GetDistanceTo(eCoord);
        }

    }

    public class RequestsAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView CustomerName { get; set; }
        public TextView CustomerPhone { get; set; }
        public TextView CustomerDistance { get; set; }
        public TextView Service { get; set; }


        public RequestsAdapterViewHolder(View itemView, Action<RequestsAdapterClickEventArgs> clickListener,
                            Action<RequestsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            CustomerName = itemView.FindViewById<TextView>(Resource.Id.tvName);
            CustomerDistance = itemView.FindViewById<TextView>(Resource.Id.tvDistance);
            Service = itemView.FindViewById<TextView>(Resource.Id.tvService);
            itemView.Click += (sender, e) => clickListener(new RequestsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new RequestsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class RequestsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}