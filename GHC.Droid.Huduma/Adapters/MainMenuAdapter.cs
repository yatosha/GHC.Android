using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Square.Picasso;

namespace GHC.Adapters
{
    class MainMenuAdapter : RecyclerView.Adapter
    {
        public event EventHandler<MainMenuAdapterClickEventArgs> ItemClick;
        public event EventHandler<MainMenuAdapterClickEventArgs> ItemLongClick;
        string[] items;

        public MainMenuAdapter(string[] data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = LayoutInflater.From(parent.Context).
                   Inflate(Resource.Layout.main_menu_item, parent, false);

            var vh = new MainMenuAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as MainMenuAdapterViewHolder;
            holder.TextView.Text = items[position];

            if (position == 0)
                Picasso.With(holder.ImageView.Context).Load(Resource.Drawable.history).Into(holder.ImageView);
            else
                Picasso.With(holder.ImageView.Context).Load(Resource.Drawable.settings).Into(holder.ImageView);
        }

        public override int ItemCount => items.Length;

        void OnClick(MainMenuAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(MainMenuAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class MainMenuAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextView { get; set; }
        public ImageView ImageView { get; set; }


        public MainMenuAdapterViewHolder(View itemView, Action<MainMenuAdapterClickEventArgs> clickListener,
                            Action<MainMenuAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            TextView = itemView.FindViewById<TextView>(Resource.Id.textView);
            ImageView = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            itemView.Click += (sender, e) => clickListener(new MainMenuAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new MainMenuAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class MainMenuAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}