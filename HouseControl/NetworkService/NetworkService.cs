﻿using Common;
using Facade;
using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace ViewModelBase
{
    public class NetworkService : ServiceBase, INetworkService
    {
        public IPStatus Ping(string address)
        {
            try
            {
                var p = new Ping();
                var reply = p.Send(address, 100);
                return reply.Status;
            }
            catch (Exception e)
            {
                Use<ILog>().Log(LogCategory.Network, e.ToString());
                //return e.InnerException != null ? e.InnerException.Message : e.Message;
                return IPStatus.Unknown;
            }
        }

        public string SyncRequest(string url)
        {
            string res = null;
            try
            {
                var request = WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;
                var response = request.GetResponse();
                //res = ((HttpWebResponse)response).StatusDescription;
                var reader = new StreamReader(response.GetResponseStream());
                res = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                Use<ILog>().Log(LogCategory.Network, e.ToString());
                return string.Empty;
            }
            return res;
        }

        public async Task<string> AsyncRequest(string url)
        {
            string res = null;
            try
            {
                var request = WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Timeout = 1000;
                var response = request.GetResponseAsync();
                await response;
                res = ((HttpWebResponse)response.Result).StatusDescription;
                var reader = new StreamReader(response.Result.GetResponseStream());
                res = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                res = e.ToString();
                return string.Empty;
            }
            finally
            {
                Use<ILog>().Log(LogCategory.Network, $"url:{url} \r\n response:\r\n {res}");
            }
            return res;
        }
    }
}