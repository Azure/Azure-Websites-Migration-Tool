// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.
using System;

namespace AzureAppServiceMigrationAssistant.Helpers
{
    public enum ProtocolType
    {
        Http = 0,
        SniSSL,
        IPSSL,
        Other
    }

    [Serializable]
    public class Binding
    {
        private readonly bool _customHostName;
        private readonly bool _customIP;
        private readonly int _port;
        private readonly ProtocolType _protocolType;
        private string _displayName;
        private const string Asterisk = "*";

        public Binding(string iisBinding, string protocol)
        {
            string[] array = iisBinding.Split(new[] { ':' });
            _customHostName = false;
            _customIP = false;
            _port = 80;
            _protocolType = ProtocolType.Http;

            if (array.Length == 3)
            {
                if (!string.Equals(array[0], Asterisk) && 
                    !string.Equals(array[0], string.Empty))
                {
                    _customIP = true;
                }

                if (!string.Equals(array[2], Asterisk) &&
                    !string.Equals(array[0], string.Empty))
                {
                    _customHostName = true;
                }

                _port = int.Parse(array[1]);
            }
            else if (array.Length == 2)
            {
                if (!string.Equals(array[1], Asterisk))
                {
                    _customHostName = true;
                }

                _port = int.Parse(array[0]);
            }

            if (string.Equals(protocol, "http", StringComparison.OrdinalIgnoreCase))
            {
                _protocolType = ProtocolType.Http;
            }
            else if (string.Equals(protocol, "https", StringComparison.OrdinalIgnoreCase))
            {
                _protocolType = _customHostName ? ProtocolType.IPSSL : ProtocolType.SniSSL;
            }
            else
            {
                _protocolType = ProtocolType.Other;
            }
        }

        public ProtocolType Protocol
        {
            get { return _protocolType; }
        }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName))
                {
                    var customIP = CustomServerIP ? "customIP" : Asterisk;
                    var customHostName = CustomHostName ? "customHostName" : Asterisk;
                    _displayName = string.Format("{0}:{1}:{2}", customIP, Port, customHostName);
                }

                return _displayName;
            }
        }

        public bool CustomHostName
        {
            get { return _customHostName; }
        }

        public bool CustomServerIP
        {
            get { return _customIP; }
        }

        public int Port
        {
            get { return _port; }
        }
    }
}