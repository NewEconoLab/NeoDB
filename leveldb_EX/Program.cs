using System;

namespace leveldb_EX
{
    class Program
    {
        static string Hex2Str(byte[] data)
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
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            LevelDBEx dbex = new LevelDBEx("c:\\testdb");


            while (true)
            {
                var read = dbex.CreateSnapshot();

                var data = new byte[16];
                Random r = new Random();
                r.NextBytes(data);

                var key = new byte[] { 0x01, 0x02 };

                var table = dbex.GetTable(new byte[] { 0x03 });
                var table2 = dbex.GetTable(new byte[] { 0x04 });

                dbex.BeginBatch();
                table.PutBytes(key, data);
                r.NextBytes(data);
                table2.PutBytes(key, data);
                dbex.EndBatch();

                dbex.DeleteRaw(new byte[] { 0x88 });
                var read2 = dbex.CreateSnapshot();

                var get1 = table.GetBytes(read2, key);
                var get2 = table2.GetBytes(read2, key);
                Console.WriteLine("get1=" + Hex2Str(get1));
                Console.WriteLine("get2=" + Hex2Str(get2));
                Console.ReadLine();
            }
        }

    }
}
