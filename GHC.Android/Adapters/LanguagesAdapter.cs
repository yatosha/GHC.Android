using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Square.Picasso;

namespace GHC
{
    class LanguagesAdapter : RecyclerView.Adapter
    {
        public event EventHandler<LanguagesAdapterClickEventArgs> ItemClick;
        public event EventHandler<LanguagesAdapterClickEventArgs> ItemLongClick;
        string[] items;

        public LanguagesAdapter(string[] data)
        {
            items = data;
        }

        public int SelectedPosition = -1;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = LayoutInflater.From(parent.Context). Inflate(Resource.Layout.language_item, parent, false);
            //var id = Resource.Layout.__YOUR_ITEM_HERE;
            //itemView 

            var vh = new LanguagesAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as LanguagesAdapterViewHolder;
            holder.LanguageName.Text = items[position];

            if (position == 0)
                Picasso.With(holder.Flag.Context).Load(Resource.Drawable.tanzania_flag).Into(holder.Flag);
            else
                Picasso.With(holder.Flag.Context).Load(Resource.Drawable.uk_flag).Into(holder.Flag);

            if (SelectedPosition >= 0)
            {
                if (position == SelectedPosition)
                {
                    holder.ItemView.SetBackgroundColor(new Android.Graphics.Color(169, 228, 252));
                }
                else
                    holder.ItemView.SetBackgroundColor(new Android.Graphics.Color(255, 255, 255));
            }
        }

        public override int ItemCount => items.Length;

        void OnClick(LanguagesAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(LanguagesAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class LanguagesAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView LanguageName { get; set; }
        public ImageView Flag { get; set; }


        public LanguagesAdapterViewHolder(View itemView, Action<LanguagesAdapterClickEventArgs> clickListener,
                            Action<LanguagesAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            LanguageName = itemView.FindViewById<TextView>(Resource.Id.languageName);
            Flag = itemView.FindViewById<ImageView>(Resource.Id.flag);

            itemView.Click += (sender, e) => clickListener(new LanguagesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new LanguagesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class LanguagesAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}