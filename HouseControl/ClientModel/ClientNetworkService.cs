namespace ClientModel
{
    public class ClientNetworkService
    {
        //public IPStatus Ping(string address)
        //{
        //    Ping p = null;
        //    try
        //    {
        //        p = new Ping();
        //        var reply = p.Send(address, 100);
        //        return reply.Status;
        //    }
        //    catch (Exception e)
        //    {
        //        Use<ILog>().Log(LogCategory.Network, "Ping: " + e.Message);
        //        return IPStatus.Unknown;
        //    }
        //    finally
        //    {
        //        p?.Dispose();
        //    }
        //}

        //public string SyncRequest(string url)
        //{
        //    string res = null;
        //    try
        //    {
        //        var request = WebRequest.Create(url);
        //        request.Credentials = CredentialCache.DefaultCredentials;
        //        using (var response = (HttpWebResponse)request.GetResponse())
        //        {
        //            var reader = new StreamReader(response.GetResponseStream());
        //            res = reader.ReadToEnd();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Use<ILog>().Log(LogCategory.Network, $"Sync request to {url}:\r\n {e.Message}");
        //        return string.Empty;
        //    }
        //    return res;
        //}

        //public async Task<string> AsyncRequest(string url)
        //{
        //    string res = null;
        //    try
        //    {
        //        var response = (HttpWebResponse)await GetResponseTask(url);
        //        var reader = new StreamReader(response.GetResponseStream());
        //        res = reader.ReadToEnd();
        //        response.Dispose();
        //    }
        //    catch (Exception e)
        //    {
        //        Use<ILog>().Log(LogCategory.Network, $"async request to url:{url} \r\n {e.Message}");
        //        return string.Empty;
        //    }
        //    return res;
        //}
        //private const int Timeout = 1000;
        //private const int StepTime = 100;

        //private async Task<WebResponse> GetResponseTask(string url)
        //{

        //    var request = (HttpWebRequest)WebRequest.Create(url);
        //    request.Credentials = CredentialCache.DefaultCredentials;
        //    request.Timeout = Timeout;
        //    request.ReadWriteTimeout = Timeout;
        //    request.ContinueTimeout = Timeout;
        //    request.KeepAlive = false;
        //    var res = request.GetResponseAsync();
        //    var steps = Timeout / StepTime;
        //    for (int i = 0; i < steps; i++)
        //    {
        //        await Task.Delay(StepTime);
        //        if (res.IsCompleted)
        //            return res.Result;
        //    }
        //    request.Abort();
        //    throw new TimeoutException("No Response");
        //}
    }
}
