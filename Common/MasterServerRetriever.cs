﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace LunaCommon
{
    /// <summary>
    /// This class retrieves the ips of master servers that are stored in:
    /// http://raw.githubusercontent.com/DaggerES/LunaMultiPlayer/master/MasterServersList
    /// </summary>
    public static class MasterServerRetriever
    {
        public static string MasterServersListShortUrl => "https://goo.gl/NJqZbc";
        public static string MasterServersListUrl => "https://raw.githubusercontent.com/DaggerES/LunaMultiPlayer/master/MasterServersList";

        /// <summary>
        /// Download the master server list from the MasterServersListUrl and return the ones that are correctly written
        /// We should add a ping check aswell...
        /// </summary>
        /// <returns></returns>
        public static string[] RetrieveWorkingMasterServersEndpoints()
        {
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            var parsedServers = new List<IPEndPoint>();
            using (var client = new WebClient())
            using (var stream = client.OpenRead(MasterServersListUrl))
            {
                if (stream == null)
                {
                    throw new Exception($"Cannot open the master servers list file from: {MasterServersListUrl}");
                }
                using (var reader = new StreamReader(stream))
                {
                    var content = reader.ReadToEnd();
                    var servers = content
                        .Trim()
                        .Split('\n')
                        .Where(s => !s.StartsWith("#") && s.Contains(":"))
                        .ToArray();

                    foreach (var server in servers)
                    {
                        var ipPort = server.Split(':');
                        IPAddress ip;
                        ushort port;
                        if (!IPAddress.TryParse(ipPort[0], out ip))
                        {
                            var hostEntry = Dns.GetHostEntry(ipPort[0]);
                            if (hostEntry.AddressList.Length > 0)
                            {
                                ip = hostEntry.AddressList[0];
                            }
                        }

                        if (ip != null && ushort.TryParse(ipPort[1], out port))
                        {
                            parsedServers.Add(new IPEndPoint(ip, port));
                        }
                    }
                }
            }

            return parsedServers.Select(s => s.Address.ToString() + ":" + s.Port).ToArray();
        }

        /// <summary>
        /// This callback is added because MONO does not contain any certificate authority and in fails when connecting to github.
        /// https://stackoverflow.com/questions/4926676/mono-webrequest-fails-with-https
        /// </summary>
        /// <returns></returns>
        public static bool MyRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (var i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }
    }
}
