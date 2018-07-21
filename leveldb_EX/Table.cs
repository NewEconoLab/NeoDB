using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace leveldb_EX
{
    public class Table
    {
        byte[] prefix;
        leveldb_EX.LevelDBEx dbex;
        static readonly byte[] tagZero = new byte[] { 0x00 };
        static readonly byte[] tagData = new byte[] { (byte)DataType.Data };
        static readonly byte[] tagMap = new byte[] { (byte)DataType.Map };
        static readonly byte[] tagQueue = new byte[] { (byte)DataType.Queue };


        public Table(leveldb_EX.LevelDBEx dbex, byte[] tablename)
        {
            this.dbex = dbex;
            this.prefix = tablename;
        }

        public void PutBytes(byte[] key, byte[] data)
        {
            //0x01(type)+tablename(string)+"0"+data;
            var _key = tagData.Concat(prefix).Concat(tagZero).Concat(key).ToArray();
            this.dbex.PutRaw(_key, data);
        }
        public void DeleteBytes(byte[] key)
        {
            var _key = tagData.Concat(prefix).Concat(tagZero).Concat(key).ToArray();
            this.dbex.DeleteRaw(_key);
        }
        public byte[] GetBytes(LevelDB.ReadOptions ro, byte[] key)
        {
            var _key = tagData.Concat(prefix).Concat(tagZero).Concat(key).ToArray();
            return this.dbex.GetRaw(ro, _key);
        }

        ///// <summary>
        ///// 创建一个字典
        ///// </summary>
        ///// <param name="key"></param>
        public Map NewMap(byte[] key)
        {
            return new Map();
        }
        ///// <summary>
        ///// 删除一个字典
        ///// </summary>
        ///// <param name="key"></param>
        public void DeleteMap(byte[] key)
        {

        }
        ///// <summary>
        ///// 得到一个字典
        ///// </summary>
        ///// <param name="key"></param>
        public Map GetMap(byte[] key)
        {
            return new Map();
        }
    }
}
