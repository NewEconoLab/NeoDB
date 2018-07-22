using System;
using System.Collections.Generic;
using System.Text;

namespace LevelDB.Ex
{
    public interface IItem
    {
        Value_DataType type
        {
            get;
        }
        byte[] Value
        {
            get;
        }
        void Put(LevelDB.DB db,byte[] key);
        void Batch_Put(LevelDB.WriteBatch batch, LevelDB.DB db, byte[] key);
    }
}
