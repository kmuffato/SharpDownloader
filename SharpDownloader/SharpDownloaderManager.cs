using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpDownloader.Extensions;
namespace SharpDownloader
{
    public class SharpDownloaderManager:IList<Downloader>
    {
        SharpDownloaderSettings SharpDownloaderSettings { get; set; }

        List<Downloader> InternalList { get; set; }
        List<Task> InternalTaskList { get; set; }


        public SharpDownloaderManager(SharpDownloaderSettings Settings)
        {
            this.SharpDownloaderSettings = Settings;
            this.InternalList = new List<Downloader>();
            this.InternalTaskList = new List<Task>();
        }


        public void StartDownloading()
        {
            if (Count < 0)
                throw new Exception("Can not start downloading when no class provided");
            InternalList.ForEach(   x => 
                                    {
                                        x.NumberOfProcessors = SharpDownloaderSettings.NumberOfProcessors;
                                        
                                    });
            ConsoleExtensions.tableWidth = SharpDownloaderSettings.TableCellWidth;

            foreach (var item in InternalList)
            {
                Task TaskItem = item.StartDownloadAsync();
                InternalTaskList.Add(TaskItem);
            }

            while (InternalTaskList.Any(x => x.IsCompleted != true))
            {
                Console.Clear();
                ConsoleExtensions.PrintLine();
                ConsoleExtensions.PrintRow(Downloader.ReportingHeader);
                ConsoleExtensions.PrintLine();
                
                for (int i = 0; i < InternalList.Count; i++)
                {
                    ConsoleExtensions.PrintRow(InternalList[i].ReportValues);
                    
                    ConsoleExtensions.PrintLine();

                }
                
                Thread.Sleep(SharpDownloaderSettings.RefreshTime);
            }

            InternalTaskList.ForEach(x =>
            {
                if (x.IsFaulted)
                {
                    Console.WriteLine($"Task:{x.Id} is faulted : InnerException =>{x.Exception}");
                }
            });
            Console.WriteLine("Process Finished");
            Console.ReadLine();


        }



        public Downloader this[int index] { get => InternalList?[index]; set => InternalList[index] = value; }
        
        public int Count => InternalList.Count;

        public bool IsReadOnly => false;
        
        public void Add(Downloader item)
        {
            if (!InternalList.Contains(item)) { InternalList.Add(item); }
        }

        public void Clear()
        {
            InternalList.Clear();
        }

        public bool Contains(Downloader item)
        {
            return InternalList.Contains(item);
        }

        public void CopyTo(Downloader[] array, int arrayIndex)
        {
            array = InternalList.Skip(arrayIndex).ToArray();
        }

        public IEnumerator<Downloader> GetEnumerator()
        {
           return InternalList.GetEnumerator();
        }

        public int IndexOf(Downloader item)
        {
            return InternalList.IndexOf(item);
        }

        public void Insert(int index, Downloader item)
        {
            InternalList.Insert(index, item);
        }

        public bool Remove(Downloader item)
        {
            return InternalList.Remove(item);
        }

        public void RemoveAt(int index)
        {
            InternalList.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InternalList.GetEnumerator();
        }
    }
}
