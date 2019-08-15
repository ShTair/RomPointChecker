using RomController;
using ShComp;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RomPointChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            while (true)
            {
                var p = await GetPointAsync();
                var w = await RomWindow.FromPointAsync(p);

                if (w is null) break;

                var r = w.Rectangle;
                var dx = (double)(p.X - r.X) / r.Width;
                var dy = (double)(p.Y - r.Y) / r.Height;

                Console.WriteLine($"({p.X}, {p.Y}) -> ({dx:0.0000}, {dy:0.0000})");
            }

            Console.WriteLine("停止しました。");
            Console.ReadLine();
        }

        private static Task<Point> GetPointAsync()
        {
            var tcs = new TaskCompletionSource<Point>();
            var hooker = new MouseHooker();

            hooker.EventReceived += (status, point) =>
            {
                if (status == 514)
                {
                    tcs.TrySetResult(point);
                    Application.Exit();
                }
            };

            Task.Run(() =>
            {
                hooker.Start();
                Application.Run();
                hooker.Stop();
            });

            return tcs.Task;
        }
    }
}
