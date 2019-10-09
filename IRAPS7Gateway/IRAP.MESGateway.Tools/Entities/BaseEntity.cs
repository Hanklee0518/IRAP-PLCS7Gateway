using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAP.MESGateway.Tools.Entities
{
    internal class BaseEntity
    {
        [Browsable(false)]
        public Guid ID { get; private set; } = Guid.NewGuid();
    }

    internal class BaseEntityCollection
    {
        private Dictionary<Guid, BaseEntity> datas =
            new Dictionary<Guid, BaseEntity>();

        public BaseEntity this[int index]
        {
            get
            {
                if (index >=0 && index < datas.Count)
                {
                    return datas.ElementAt(index).Value;
                }
                else
                {
                    return null;
                }
            }
        }
        public BaseEntity this[Guid key]
        {
            get
            {
                datas.TryGetValue(key, out BaseEntity rlt);
                return rlt;
            }
        }
        public int Count
        {
            get { return datas.Count; }
        }

        public void Add(BaseEntity entity)
        {
            if (datas.ContainsKey(entity.ID))
            {
                return;
            }

            datas.Add(entity.ID, entity);
        }

        public void Remove(Guid id)
        {
            datas.Remove(id);
        }
    }
}
