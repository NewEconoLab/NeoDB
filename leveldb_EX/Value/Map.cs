using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LevelDB.Ex
{
    /// <summary>
    /// 可以遍历keys的数据结构，未实现完全
    /// </summary>
    public class Map : IItem
    {
        public Value_DataType type => Value_DataType.Map;
        static readonly byte[] tagZero = new byte[] { 0x00 };

        /// <summary>
        /// Map的value 是一个64位的instanceID
        /// </summary>
        public byte[] Value
        {
            get;
            private set;
        }

        public Map()
        {
        }
        public void Init(LevelDB.DB db, LevelDB.ReadOptions ro, byte[] key)
        {
            var instid = db.Get(ro, key);
            if (instid == null || instid.Length == 0)
                throw new Exception("error map in Init");
            this.Value = instid.Skip(1).ToArray();
        }
        public void Put(LevelDB.DB db, byte[] key)
        {
            var snapshot = Helper.CreateSnapshot(db);

            if (this.Value == null)
            {//申请新的实例ID，然后初始化存储Map
                var key_instMax = Table.tagKey_InstanceMax;
                var instid = db.Get(snapshot, key_instMax);
                if (instid == null || instid.Length == 0)
                {
                    instid = BitConverter.GetBytes((UInt64)1);
                }

                this.Value = instid;

                //刷新max
                {
                    UInt64 v = BitConverter.ToUInt64(instid, 0);
                    v++;
                    instid = BitConverter.GetBytes((UInt64)v);
                    db.Put(key_instMax, instid);
                }

                //初始化字典数量
                byte[] key_count = Table.tagKey_MapCount.Concat(this.Value).ToArray();
                db.Put(key_count, BitConverter.GetBytes((UInt64)0));
            }
            else
            {//检查count是否存在，
                byte[] key_count = Table.tagKey_MapCount.Concat(this.Value).ToArray();
                var count = db.Get(snapshot, key_count);
                if (count == null || count.Length == 0)
                    throw new Exception("error map instance.");
            }

            db.Put(key, Table.tagValue_Map.Concat(this.Value).ToArray());
        }
        public void Batch_Put(LevelDB.WriteBatch batch, LevelDB.DB db, byte[] key)
        {
            throw new NotSupportedException();
            batch.Put(key, this.Value);
        }

        public UInt64 Count(LevelDB.DB db, LevelDB.ReadOptions ro)
        {
            byte[] key_count = Table.tagKey_MapCount.Concat(this.Value).ToArray();
            byte[] data = db.Get(ro, key_count);
            if (data == null || data.Length == 0)
                throw new Exception("error map in Count");
            
            return BitConverter.ToUInt64(data,0);
        }
        public void SetItem(LevelDB.DB db, byte[] key, IItem item)
        {
            var _key = Table.tagKey_MapValues.Concat(this.Value).Concat(key).ToArray();
            item.Put(db, _key);
        }
    }
}
