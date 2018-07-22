using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LevelDB;

namespace LevelDB.Ex
{
    public class Bytes : IItem
    {
        public Value_DataType type => Value_DataType.Bytes;

        public byte[] Value
        {
            get;
            private set;
        }
        public Bytes(byte[] value)
        {
            this.Value = value;
        }

        public void Put(LevelDB.DB db, byte[] key)
        {
            var _value = Table.tagValue_Bytes.Concat(this.Value).ToArray();
            db.Put(key, _value);
        }
        public void Batch_Put(LevelDB.WriteBatch batch, LevelDB.DB db, byte[] key)
        {
            var _value = Table.tagValue_Bytes.Concat(this.Value).ToArray();
            batch.Put(key, _value);
        }
    }
}
