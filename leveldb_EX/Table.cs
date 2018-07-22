using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LevelDB.Ex
{

    public class Table
    {
        byte[] prefix;
        LevelDB.DB db;
        public static readonly byte[] tagZero = new byte[] { 0x00 };
        public static readonly byte[] tagKey_Item = new byte[] { (byte)Key_DataType.ItemData };
        public static readonly byte[] tagKey_MapCount = new byte[] { (byte)Key_DataType.Map_Count };
        public static readonly byte[] tagKey_MapValues = new byte[] { (byte)Key_DataType.Map_Values };
        public static readonly byte[] tagKey_InstanceMax = new byte[] { (byte)Key_DataType.InstanceMax };

        public static readonly byte[] tagValue_Bytes = new byte[] { (byte)Value_DataType.Bytes };
        public static readonly byte[] tagValue_Map = new byte[] { (byte)Value_DataType.Map };

        public Table(LevelDB.DB db, byte[] tablename)
        {
            this.db = db;
            this.prefix = tablename;
        }
        /// <summary>
        /// delete
        /// </summary>
        /// <param name="key"></param>
        public byte[] CalcKey(byte[] tag, byte[] key)
        {
            return tag.Concat(prefix).Concat(tagZero).Concat(key).ToArray();
        }
        public IItem GetItem(LevelDB.ReadOptions ro, byte[] key)
        {
            var _key = tagKey_Item.Concat(prefix).Concat(tagZero).Concat(key).ToArray();
            var data = this.db.Get(ro, _key);
            if (data == null || data.Length == 0)
                return null;
            if (data[0] == (byte)Value_DataType.Bytes)
            {
                return new Bytes(data.Skip(1).ToArray());
            }
            if (data[0] == (byte)Value_DataType.Map)
            {
                var map = new Map();
                map.Init(this.db, ro, _key);
                return map;
            }

            throw new Exception("unknown datatype.");
        }
        public void DeleteItem(byte[] key)
        {
            var _key = CalcKey(tagKey_Item, key);
            this.db.Delete(_key);
        }
        public void Batch_DeleteItem(LevelDB.WriteBatch batch, byte[] key)
        {
            var _key = CalcKey(tagKey_Item, key);
            batch.Delete(_key);
        }
        public void PutItem(byte[] key, IItem value)
        {
            var _key = CalcKey(tagKey_Item, key);
            value.Put(this.db, _key);
        }
        public void PutItem_Batch(LevelDB.WriteBatch batch, byte[] key, IItem value)
        {
            var _key = CalcKey(tagKey_Item, key);
            value.Batch_Put(batch, this.db, _key);
        }

    }
}
