using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracy.DataModel;

namespace Tracy.DataAccess
{
    public class EntryProvider : AbstractMongoDataProvider<Entry>
    {
        public override string CollectionName
        {
            get
            {
                return "entry";
            }
        }

        public EntryProvider(MongoDB db) : base(db) { }

        public void LinkMediaFile(Entry entry, MediaFile mediaFile, bool willSaveEntry)
        {
            if (entry == null) return;
            entry.MediaFileIds.Add(mediaFile.Id);
            if(!entry.MaxEpisode.HasValue || (mediaFile.Episode.HasValue && entry.MaxEpisode < mediaFile.Episode))
            {
                entry.MaxEpisode = mediaFile.Episode;
            }
            if (willSaveEntry) TracyFacade.Instance.Manager.EntryProvider.Collection.Save(entry);
        }
    }
}
