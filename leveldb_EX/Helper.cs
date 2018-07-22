using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LevelDB.Ex
{

    public enum Key_DataType : byte
    {
        ItemData = 0x01,//普通数据，byte[]
        //Map = 0x11,//字典，value=InstanceID  0x11+tabelname+0x00+mapname
        Map_Count = 0x10,//字典的值数量 0x12+InstanceID(固定长度)
        Map_Values,//字典的值,如果写入一个不存在的值，count+1,delete一个存在的值，count-1
        //0x13+InstanceID+Key

        InstanceMax = 0x20,//所有的引用对象池，随机分配，64bit,不会给重复的
                   //一个map是一个Instance，使用这个机制是为了方便DeleteMap或者RenameMap

    }

    /// <summary>
    /// LevelDB只提供了简单的KeyValue操作，和读快照与批次写入这些操作
    /// 缺乏一些数据结构的支持，使用起来还是有一些不方便
    /// </summary>
    public class Helper
    {
        public static LevelDB.DB OpenDB(string path)
        {
            var op = new LevelDB.Options() { CreateIfMissing = true, Compression = LevelDB.CompressionType.SnappyCompression };
            return LevelDB.DB.Open(op, path);
        }
        public static LevelDB.ReadOptions CreateSnapshot(LevelDB.DB db)
        {
            return new LevelDB.ReadOptions() { Snapshot = db.CreateSnapshot() };
        }

        //table不是数据结构，只是给存储的数据加上前缀
        public static Table GetTable(LevelDB.DB db, byte[] tablename)
        {
            if (tablename == null || tablename.Length == 0)
            {
                return new Table(db, new byte[0]);
            }
            if (tablename.Contains((byte)0x00))
                throw new Exception("not a vaild tablename");
            return new Table(db, tablename);
        }
        public static string Hex2Str(byte[] data)
        {
            var outstr = "";
            if (data != null)
            {
                foreach (var b in data)
                {
                    outstr += b.ToString("X02");
                }
            }
            return outstr;
        }
    }
}
