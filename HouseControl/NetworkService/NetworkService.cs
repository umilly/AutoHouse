using Common;
using Facade;
using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ViewModelBase
{
    public class NetworkService : ServiceBase, INetworkService
    {
        public T Deserialize<T>(string json)
        {
            var serializer = new DataContractJsonSerializer(typeof (T));
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            try
            {
                return (T) serializer.ReadObject(stream);
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }

        public string Serilize<T>(T obj)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            var stream=new MemoryStream();
            try
            {
                serializer.WriteObject(stream, obj);
                var bytes = stream.ToArray();
                var res = Encoding.UTF8.GetString(bytes);
                return res;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }

        public override void OnContainerSet()
        {
            base.OnContainerSet();
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.MaxServicePointIdleTime = 2000;
        }

        public IPStatus Ping(string address)
        {
            Ping p = null;
            try
            {
                p = new Ping();
                var reply = p.Send(address, 100);
                return reply.Status;
            }
            catch (Exception e)
            {
                Use<ILog>().Log(LogCategory.Network,e);
                return IPStatus.Unknown;
            }
            finally
            {
                p?.Dispose();
            }
        }

        public string SyncRequest(string url)
        {
            string res = null;
            try
            {
                var request = WebRequest.Create(url);
                request.Timeout = 1000;
                request.Credentials = CredentialCache.DefaultCredentials;
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    var reader = new StreamReader(response.GetResponseStream());
                    res = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                var prefix = $"Sync request to {url} Exception:\r\n ";
                Use<ILog>().LogNetException(e, prefix);
                return string.Empty;
            }
            return res;
        }

        public async Task<string> AsyncRequest(string url)
        {
            string res = null;
            try
            {
                var response = (HttpWebResponse) await GetResponseTask(url);
                var stream = response.GetResponseStream();
                var reader = new StreamReader(stream,Encoding.UTF8);
                res = reader.ReadToEnd();
                stream.Dispose();
                reader.Dispose();
                response.Dispose();
                return res;

            }
            catch (Exception e)
            {
                var prefix = $"ASync request to {url} Exception:\r\n ";
                Use<ILog>().LogNetException(e, prefix);
                return string.Empty;
            }
            return res;
        }

        

        private const int Timeout = 1000;
        private const int StepTime = 100;

        private async Task<WebResponse> GetResponseTask(string url)
        {

            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Timeout = Timeout;
            request.ReadWriteTimeout = Timeout;
            request.ContinueTimeout = Timeout;
            request.KeepAlive = false;
            request.Method = "GET";
            request.Accept = @"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.UserAgent =
                @"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36 OPR/42.0.2393.94";
            var res = request.GetResponseAsync();
            request.ProtocolVersion=new Version(1,1);
            var steps = Timeout/StepTime;
            for (int i = 0; i < steps; i++)
            {
                await Task.Delay(StepTime);
                if (res.IsCompleted)
                    return res.Result;
            }
            request.Abort();
            throw new TimeoutException("No Response");
        }
    }
}