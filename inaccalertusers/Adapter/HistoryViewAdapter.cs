using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using inaccalertusers.Datamodels;
using System;
using System.Collections.Generic;

namespace inaccalertusers.Adapter
{
    class HistoryViewAdapter : RecyclerView.Adapter
    {
        public event EventHandler<HistoryViewAdapterClickEventArgs> ItemClick;
        public event EventHandler<HistoryViewAdapterClickEventArgs> ItemLongClick;
        List <HistoryDataModel> items;

        public HistoryViewAdapter(List<HistoryDataModel> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ListViewDataTemplate,parent,false);
            

            var vh = new HistoryViewAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            //var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as HistoryViewAdapterViewHolder;

            holder.nametext.Text = items[position].myname;
            holder.volunteernametext.Text = items[position].volunteername;
            holder.addresslocationtext.Text = items[position].addresslocation;
            holder.datetext.Text = items[position].date;
            holder.timetext.Text = items[position].time;
            //holder.TextView.Text = items[position];
        }

        public override int ItemCount => items.Count;

        void OnClick(HistoryViewAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(HistoryViewAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class HistoryViewAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public TextView nametext { get; set; }
        public TextView volunteernametext { get; set; }
        public TextView addresslocationtext { get; set; }
        public TextView datetext { get; set; }
        public TextView timetext { get; set; }

        public HistoryViewAdapterViewHolder(View itemView, Action<HistoryViewAdapterClickEventArgs> clickListener,
                            Action<HistoryViewAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //TextView = v;
            nametext = (TextView)itemView.FindViewById(Resource.Id.usernametext);
            volunteernametext = (TextView)itemView.FindViewById(Resource.Id.volunteernametext);
            addresslocationtext = (TextView)itemView.FindViewById(Resource.Id.addresstext);
            datetext = (TextView)itemView.FindViewById(Resource.Id.datetitle);
            timetext = (TextView)itemView.FindViewById(Resource.Id.timetext);

            itemView.Click += (sender, e) => clickListener(new HistoryViewAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new HistoryViewAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class HistoryViewAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}