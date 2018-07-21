using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace leveldb_EX
{
    public enum DataType : byte
    {
        Data = 0x01,//普通数据，byte[]
        Map = 0x11,//字典，value=InstanceID  0x11+tabelname+0x00+mapname
        Map_Count = 0x12,//字典的值数量 0x12+InstanceID(固定长度)
        Map_Values = 0x13,//字典的值,如果写入一个不存在的值，count+1,delete一个存在的值，count-1
        //0x13+InstanceID+Key

        Queue = 0x21,//队列

        AllInstanceID = 0xa0,//所有的引用对象池，随机分配，64bit,不会给重复的
        //一个map是一个Instance，使用这个机制是为了方便DeleteMap或者RenameMap

    }
    /// <summary>
    /// LevelDB只提供了简单的KeyValue操作，和读快照与批次写入这些操作
    /// 缺乏一些数据结构的支持，使用起来还是有一些不方便
    /// </summary>
    public class LevelDBEx
    {

        public LevelDBEx(string path)
        {
            var op = new LevelDB.Options() { CreateIfMissing = true, Compression = LevelDB.CompressionType.SnappyCompression };
            this.db = LevelDB.DB.Open(op, path);
        }
        LevelDB.DB db;
        LevelDB.WriteBatch rb;
        #region Batch
        public bool InBatch()
        {
            return db != null;
        }
        public void BeginBatch()
        {
            if (rb != null)
                throw new Exception("already in a batch");
            this.rb = new LevelDB.WriteBatch();
        }
        public void EndBatch()
        {
            if (this.rb == null)
                throw new Exception("not in a batch");
            this.db.Write(this.rb);
            this.rb = null;
        }
        #endregion endbatch

        public LevelDB.ReadOptions CreateSnapshot()
        {
            return new LevelDB.ReadOptions() { Snapshot = this.db.CreateSnapshot() };
        }
        #region Raw操作，比较危险，没事不要用
        public void PutRaw(byte[] key, byte[] data)
        {
            if (rb == null)
            {
                this.db.Put(key, data);
            }
            else
            {
                this.rb.Put(key, data);
            }
        }
        public void DeleteRaw(byte[] key)
        {
            if (rb == null)
            {
                this.db.Delete(key);
            }
            else
            {
                this.rb.Delete(key);
            }
        }
        public byte[] GetRaw(LevelDB.ReadOptions ro, byte[] key)
        {
            return this.db.Get(ro, key);
        }
        #endregion

        //table不是数据结构，只是给存储的数据加上前缀
        /// <summary>
        /// 限制表名中不得含有0，通过0作为终结字符，防止表名数据污染
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public Table GetTable(byte[] tablename)
        {
            if (tablename == null || tablename.Length == 0)
                throw new Exception("not a vaild tablename");
            if (tablename.Contains((byte)0x00))
                throw new Exception("not a vaild tablename");


            return new Table(this, tablename);
        }

    }
}
