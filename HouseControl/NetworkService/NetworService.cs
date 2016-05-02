﻿using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Common;
using Facade;

namespace ViewModelBase
{
    public class NetworService : ServiceBase, INetworService
    {
        public string Ping(string address)
        {
            try
            {
                var p = new Ping();
                var reply = p.Send(address, 100);
                return reply.Status.ToString();
            }
            catch (Exception e)
            {

                if (e.InnerException != null)
                    return e.InnerException.Message;
                return e.Message;
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
                res = ((HttpWebResponse) response).StatusDescription;
                var reader = new StreamReader(response.GetResponseStream());
                res = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                res = e.ToString();
                throw e;
            }
            finally
            {
                Use<ILog>().Log(LogCategory.Network,string.Format("url:{0} \r\n response:\r\n {1}", url, res));
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
                var response = request.GetResponseAsync();
                await response;
                res = ((HttpWebResponse)response.Result).StatusDescription;
                var reader = new StreamReader(response.Result.GetResponseStream());
                res = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                res = e.ToString();
                throw e;
            }
            finally
            {
                Use<ILog>().Log(LogCategory.Network, string.Format("url:{0} \r\n response:\r\n {1}", url, res));
            }
            return res;
        }
    }
}